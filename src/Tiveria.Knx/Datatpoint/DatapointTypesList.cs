using System.Collections.Generic;

namespace Tiveria.Knx.Datapoint
{
    public class DatapointTypesList
    {
        private readonly Dictionary<string, IDatapointType> _list = new Dictionary<string, IDatapointType>();

        public void InitializeDefaults()
        {
            AddOrReplace(DPType8bitUnsigned.DPT_ANGLE);
            AddOrReplace(DPType8bitUnsigned.DPT_SCALING);
            AddOrReplace(DPType8bitUnsigned.DPT_PERCENT_U8);
            AddOrReplace(DPType8bitUnsigned.DPT_SCALING);
            AddOrReplace(DPType8bitUnsigned.DPT_TARIFF);
            AddOrReplace(DPType8bitUnsigned.DPT_VALUE_1_UCOUNT);
        }

        public void AddOrReplace(IDatapointType dptype)
        {
            if(_list.ContainsKey(dptype.Id))
                _list.Remove(dptype.Id):
            _list.Add(dptype.Id, dptype);
        }

        public IDatapointType GetTypeById(string id)
        {
            return _list.GetValueOrDefault(id);
        }
    }
}
