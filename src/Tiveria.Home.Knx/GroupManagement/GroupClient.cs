using System.Net;
using Tiveria.Home.Knx.Cemi;
using Tiveria.Home.Knx.Datapoint;
using Tiveria.Home.Knx.Exceptions;
using Tiveria.Home.Knx.Primitives;

namespace Tiveria.Home.Knx.GroupManagement
{
    public class GroupClient<T> : IGroupClient
    {
        private readonly IKnxClient _client;
        private readonly GroupAddress _address;
        private readonly DPType<T> _translator;
        private readonly ManualResetEventSlim _answerEvent = new ManualResetEventSlim(false);
        private byte[]? _data;

        public GroupClient(IKnxClient client, GroupAddress address, DPType<T> translator)
        {
            _client = client;
            _address = address;
            _translator = translator;
            _client.CemiReceived += _client_CemiReceived;
        }

        private void _client_CemiReceived(object sender, CemiReceivedArgs e)
        {
            var cemi = e.Message as CemiLData;
            if (cemi == null || cemi.DestinationAddress != _address)
                return;
            if (cemi.Apdu.ApduType == ApduType.GroupValue_Response)
            {
                _data = cemi.Apdu.Data != null ? (byte[])cemi.Apdu.Data.Clone() : Array.Empty<byte>();
                _answerEvent.Set();
            }
        }

        public Task WriteValueAsync(T value)
        { 
            var data = _translator.Encode(value);
            var apdu = new Apdu(Cemi.ApduType.GroupValue_Write, data);
            var ctrl1 = new ControlField1();
            var ctrl2 = new ControlField2(groupAddress: true);
            var cemi = new CemiLData(Cemi.MessageCode.LDATA_REQ, _address, ctrl1, ctrl2, apdu);
            return _client.SendCemiAsync(cemi);
        }

        public async Task<T> ReadValueAsync()
        {
            var apdu = new Apdu(Cemi.ApduType.GroupValue_Read);
            var ctrl1 = new ControlField1();
            var ctrl2 = new ControlField2(groupAddress: true);
            var cemi = new CemiLData(Cemi.MessageCode.LDATA_REQ, _address, ctrl1, ctrl2, apdu);
            _answerEvent.Reset();
            await _client.SendCemiAsync(cemi);
            if (!_answerEvent.Wait(2000))
                throw new KnxTimeoutException();
            return _translator.Decode(_data);
        }
    }
}
