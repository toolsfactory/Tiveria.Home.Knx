/*
    Tiveria.Home.Knx - a .Net Core base KNX library
    Copyright (c) 2018-2022 M. Geissler

    This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published by
    the Free Software Foundation; either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
    GNU Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

    Linking this library statically or dynamically with other modules is
    making a combined work based on this library. Thus, the terms and
    conditions of the GNU Lesser General Public License cover the whole
    combination.
*/


namespace Tiveria.Home.Knx.Datapoint
{
    public static class DatapointTypesList
    {
        private static readonly Dictionary<string, IDatapointType> _list = new ();


        public static void AddOrReplace(IDatapointType dptype)
        {
            if (_list.ContainsKey(dptype.Id))
                _list.Remove(dptype.Id);
            _list.Add(dptype.Id, dptype);
        }

        public static IDatapointType? GetTypeById(string id)
        {
            return _list.GetValueOrDefault(id);
        }

        static DatapointTypesList()
        {
            _list.Clear();
            DPType1.Init();
            DPType10.Init();
            DPType11.Init();
            DPType12.Init();
            DPType13.Init();
            DPType14.Init();
            DPType16.Init();
            DPType17.Init();
            DPType18.Init();
            DPType19.Init();
            DPType2.Init();
            DPType20.Init();
            DPType232.Init();
            DPType28.Init();
            DPType3.Init();
            DPType4.Init();
            DPType5.Init();
            DPType7.Init();
            DPType8.Init();
            DPType9.Init();
        }
    }
}