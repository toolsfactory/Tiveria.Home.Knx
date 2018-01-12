using System;
using Tiveria.Knx.IP.ServiceTypes;
using Tiveria.Knx.IP.Structures;
using Tiveria.Knx.IP.Utils;
using Tiveria.Common.Extensions;

namespace Tiveria.Knx.IP
{
    /*
     * +-7-+-6-+-5-+-4-+-3-+-2-+-1-+-0-+-7-+-6-+-5-+-4-+-3-+-2-+-1-+-0-+
     * | Header Length                 | Protocol Version              |
     * | (1 Octet)                     | (1 Octet)                     |
     * +-7-+-6-+-5-+-4-+-3-+-2-+-1-+-0-+-7-+-6-+-5-+-4-+-3-+-2-+-1-+-0-+
     * | Service Type Identifier                                       |
     * | (2 Octet)                                                     |
     * +-7-+-6-+-5-+-4-+-3-+-2-+-1-+-0-+-7-+-6-+-5-+-4-+-3-+-2-+-1-+-0-+
     * | Total Length                                                  |
     * | (2 Octet)                                                     |
     * +-7-+-6-+-5-+-4-+-3-+-2-+-1-+-0-+-7-+-6-+-5-+-4-+-3-+-2-+-1-+-0-+
     */
    public class KnxNetIPFrame
    {
        #region private fields
        private byte[] _body;
        private Header _header;
        #endregion

        #region public properties
        public byte[] Body { get => _body; }
        public Header Header { get => _header; }
        #endregion

        #region Constructors
        public KnxNetIPFrame(ServiceTypeIdentifier servicetypeidentifier, byte[] body)
        {
            if (body == null)
                throw new ArgumentNullException("body is null");
            if (body.Length < 1)
                throw new ArgumentException("body too small");
            if (body.Length > 26)
                throw new ArgumentException("body too big");

            _header = new Header(servicetypeidentifier, (ushort)body.Length);
            _body = body;
        }

        public KnxNetIPFrame(Header header, byte[] body)
        {
            if (body == null)
                throw new ArgumentNullException("body is null");
            if (body.Length != (header.TotalLength - header.Size))
                throw new ArgumentException("body size doesn't fit to size specified in header");

            _header = header;
            _body = body;
        }

        public KnxNetIPFrame(ServiceTypeIdentifier servicetypeidentifier, IServiceType serviceType)
        {
            if (serviceType == null)
                throw new ArgumentNullException("serviceType is null");

            _body = serviceType.ToBytes();
            _header = new Header(servicetypeidentifier, (ushort)_body.Length);
        }
        #endregion

        #region Static Parsing
        public static KnxNetIPFrame Parse(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException("data is empty");
            if (data.Length < 6)
                throw new ArgumentException("data too small");

            var header = Header.FromBuffer(ref data, 0);
            var body = new byte[header.TotalLength - header.Size];

            data.Slice(body, header.Size, 0, body.Length);
            return new KnxNetIPFrame(header, body);
        }
        #endregion

        public byte[] ToBytes()
        {
            var data = new byte[_header.TotalLength];
            _header.WriteToByteArray(ref data, 0);
            _body.CopyTo(data, _header.Size);
            return data;
        }

        private IServiceType GetServiceTypeFromBody()
        {
            if (_body == null || _body.Length == 0)
                return null;
            switch (Header.ServiceTypeIdentifier)
            {
//                case ServiceTypeIdentifier.CONNECT_REQUEST:
//                    return TunnelingConnectionRequest.Parse(ref _body, 0);
                default:
                    return null;
            }
        }
    }
}
