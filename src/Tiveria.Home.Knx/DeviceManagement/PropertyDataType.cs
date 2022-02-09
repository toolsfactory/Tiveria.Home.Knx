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

using Tiveria.Home.Knx.Datapoint;

namespace Tiveria.Home.Knx.DeviceManagement
{
    public static class PropertyDataType
    {
        public const byte Control            = 0x00;
        public const byte Char               = 0x01;
        public const byte UnsignedChar       = 0x02;
        public const byte Short              = 0x03;
        public const byte UShort             = 0x04;
        public const byte KNXFloat           = 0x05;
        public const byte Date               = 0x06;
        public const byte Time               = 0x07;
        public const byte Long               = 0x08;
        public const byte ULong              = 0x09;
        public const byte Float              = 0x0a;
        public const byte Double             = 0x0b;
        public const byte CharBlock          = 0x0c;
        public const byte PollGroupSetting   = 0x0d;
        public const byte ShortCharBlock     = 0x0e;
        public const byte DateTime           = 0x0f;
        public const byte VariableLength     = 0x10;
        public const byte Generic01          = 0x11;
        public const byte Generic02          = 0x12;
        public const byte Generic03          = 0x13;
        public const byte Generic04          = 0x14;
        public const byte Generic05          = 0x15;
        public const byte Generic06          = 0x16;
        public const byte Generic07          = 0x17;
        public const byte Generic08          = 0x18;
        public const byte Generic09          = 0x19;
        public const byte Generic10          = 0x1a;
        public const byte Generic11          = 0x1b;
        public const byte Generic12          = 0x1c;
        public const byte Generic13          = 0x1d;
        public const byte Generic14          = 0x1e;
        public const byte Generic15          = 0x1f;
        public const byte Generic16          = 0x20;
        public const byte Generic17          = 0x21;
        public const byte Generic18          = 0x22;
        public const byte Generic19          = 0x23;
        public const byte Generic20          = 0x24;
        public const byte Utf8               = 0x2f;
        public const byte Version            = 0x30;
        public const byte AlarmInfoO         = 0x31;
        public const byte BinaryInformation  = 0x32;
        public const byte Bitset8            = 0x33;
        public const byte Bitset16           = 0x34;
        public const byte Enum8              = 0x35;
        public const byte Scaling            = 0x36;
        public const byte NE_VL              = 0x3c;
        public const byte NE_FL              = 0x3d;
        public const byte Function           = 0x3e;
        public const byte Escape             = 0x3f;    
   
        static PropertyDataType()
        {
            Register(Control, "Control", "");
            Register(Char, "Char", "6.010");
            Register(UnsignedChar, "UnsignedChar", "5.010");
            Register(Short, "Short", "8.001");
            Register(UShort, "UShort", "7.001");
            Register(KNXFloat, "KNXFloat", "9.002");
            Register(Date, "Date", "11.001");
            Register(Time, "Time", "10.001");
            Register(Long, "Long", "13.001");
            Register(ULong, "ULong", "12.001");
            Register(Float, "Float", "14.005");
            Register(Double, "Double", "");
            Register(CharBlock, "CharBlock", "24.001");
            Register(PollGroupSetting, "PollGroupSetting", "");
            Register(ShortCharBlock, "ShortCharBlock", "24.001");
            Register(DateTime, "DateTime", "19.001");
            Register(VariableLength, "VariableLength", "24.001");
            Register(Generic01, "Generic01", "");
            Register(Generic02, "Generic02", "");
            Register(Generic03, "Generic03", "");
            Register(Generic04, "Generic04", "");
            Register(Generic05, "Generic05", "");
            Register(Generic06, "Generic06", "");
            Register(Generic07, "Generic07", "");
            Register(Generic08, "Generic08", "");
            Register(Generic09, "Generic09", "");
            Register(Generic10, "Generic10", "");
            Register(Generic11, "Generic11", "");
            Register(Generic12, "Generic12", "");
            Register(Generic13, "Generic13", "");
            Register(Generic14, "Generic14", "");
            Register(Generic15, "Generic15", "");
            Register(Generic16, "Generic16", "");
            Register(Generic17, "Generic17", "");
            Register(Generic18, "Generic18", "");
            Register(Generic19, "Generic19", "");
            Register(Generic20, "Generic20", "");
            Register(Utf8, "Utf8", "");
            Register(Version, "Version", "217.001");
            Register(AlarmInfoO, "AlarmInfoO", "219.001");
            Register(BinaryInformation, "BinaryInformation", "1.002");
            Register(Bitset8, "Bitset8", "21.001");
            Register(Bitset16, "Bitset16", "22.100");
            Register(Enum8, "Enum8", "20.1000");
            Register(Scaling, "Scaling", "5.001");
            Register(NE_VL, "NE_VL", "");
            Register(NE_FL, "NE_FL", "");
            Register(Function, "Function", "");
            Register(Escape, "Escape", "");
    }

    private static Dictionary<byte, PropertyDataTypeDetails> _list = new(); 
    
        public static void Register(byte Id, string Name, string? DatapointTypeId)
        {
            var dptype = String.IsNullOrEmpty(DatapointTypeId) ? null : DatapointTypesList.GetTypeById(DatapointTypeId);
            _list.Add(Id, new PropertyDataTypeDetails(Id, Name, dptype));
        }
    }

    public record PropertyDataTypeDetails(byte Id, string Name, IDatapointType? DatapointType);
}

