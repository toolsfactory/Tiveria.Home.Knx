using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Kaitai
{
/*
meta:
  id: cemi
  file-extension: cemi
  application: KNX CEMI Frame
  license: CC0-1.0
  encoding: UTF-8
  endian: le
seq:
  - id: messagecode
    type: u1
  - id: additional_info_length
    type: u1
  - id: additional_info
    size: additional_info_length
  - id: control_field_1
    type: ctrl1
  - id: control_field_2
    type: ctrl2
  - id: source_address_high
    type: u1
  - id: source_address_low
    type: u1
  - id: dest_address_high
    type: u1
  - id: dest_address_low
    type: u1
  - id: npdu_length
    type: u1
  - id: npdu
    size: npdu_length - 1
  - id: other
    size: 2
    
types:
  ctrl1:
    seq:
      - id: frametype
        type: b1
      - id: reserved
        type: b1
      - id: repeat
        type: b1
      - id: system_broadcast
        type: b1
      - id: priority
        type: b2
      - id: acknowledge_request
        type: b1
      - id: confirm_flag
        type: b1
  ctrl2:
    seq:
      - id: destination_address_type
        type: b1
      - id: hop_count
        type: b3
      - id: extended_frame_format
        type: b4
 */
    public partial class Cemi : KaitaiStruct
    {
        public static Cemi FromFile(string fileName)
        {
            return new Cemi(new KaitaiStream(fileName));
        }

        public Cemi(KaitaiStream io, KaitaiStruct parent = null, Cemi root = null) : base(io)
        {
            m_parent = parent;
            m_root = root ?? this;
            _parse();
        }

        private void _parse()
        {
            _messagecode = m_io.ReadU1();
            _additionalInfoLength = m_io.ReadU1();
            _additionalInfo = m_io.ReadBytes(AdditionalInfoLength);
            _controlField1 = new Ctrl1(m_io, this, m_root);
            _controlField2 = new Ctrl2(m_io, this, m_root);
            _sourceAddressHigh = m_io.ReadU1();
            _sourceAddressLow = m_io.ReadU1();
            _destAddressHigh = m_io.ReadU1();
            _destAddressLow = m_io.ReadU1();
            _npduLength = m_io.ReadU1();
            _npdu = m_io.ReadBytes((NpduLength - 1));
            _other = m_io.ReadU2le();
        }
        public partial class Ctrl1 : KaitaiStruct
        {
            public static Ctrl1 FromFile(string fileName)
            {
                return new Ctrl1(new KaitaiStream(fileName));
            }

            public Ctrl1(KaitaiStream io, Cemi parent = null, Cemi root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
                _frametype = m_io.ReadBitsInt(1) != 0;
                _repeat = m_io.ReadBitsInt(1) != 0;
                _systemBroadcast = m_io.ReadBitsInt(1) != 0;
                _priority = m_io.ReadBitsInt(2);
                _acknowledgeRequest = m_io.ReadBitsInt(1) != 0;
                _confirmFlag = m_io.ReadBitsInt(1) != 0;
            }
            private bool _frametype;
            private bool _repeat;
            private bool _systemBroadcast;
            private ulong _priority;
            private bool _acknowledgeRequest;
            private bool _confirmFlag;
            private Cemi m_root;
            private Cemi m_parent;
            public bool Frametype { get { return _frametype; } }
            public bool Repeat { get { return _repeat; } }
            public bool SystemBroadcast { get { return _systemBroadcast; } }
            public ulong Priority { get { return _priority; } }
            public bool AcknowledgeRequest { get { return _acknowledgeRequest; } }
            public bool ConfirmFlag { get { return _confirmFlag; } }
            public Cemi M_Root { get { return m_root; } }
            public Cemi M_Parent { get { return m_parent; } }
        }
        public partial class Ctrl2 : KaitaiStruct
        {
            public static Ctrl2 FromFile(string fileName)
            {
                return new Ctrl2(new KaitaiStream(fileName));
            }

            public Ctrl2(KaitaiStream io, Cemi parent = null, Cemi root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
                _destinationAddressType = m_io.ReadBitsInt(1) != 0;
                _hopCount = m_io.ReadBitsInt(3);
                _extendedFrameFormat = m_io.ReadBitsInt(4);
            }
            private bool _destinationAddressType;
            private ulong _hopCount;
            private ulong _extendedFrameFormat;
            private Cemi m_root;
            private Cemi m_parent;
            public bool DestinationAddressType { get { return _destinationAddressType; } }
            public ulong HopCount { get { return _hopCount; } }
            public ulong ExtendedFrameFormat { get { return _extendedFrameFormat; } }
            public Cemi M_Root { get { return m_root; } }
            public Cemi M_Parent { get { return m_parent; } }
        }
        private byte _messagecode;
        private byte _additionalInfoLength;
        private byte[] _additionalInfo;
        private Ctrl1 _controlField1;
        private Ctrl2 _controlField2;
        private byte _sourceAddressHigh;
        private byte _sourceAddressLow;
        private byte _destAddressHigh;
        private byte _destAddressLow;
        private byte _npduLength;
        private byte[] _npdu;
        private ushort _other;
        private Cemi m_root;
        private KaitaiStruct m_parent;
        public byte Messagecode { get { return _messagecode; } }
        public byte AdditionalInfoLength { get { return _additionalInfoLength; } }
        public byte[] AdditionalInfo { get { return _additionalInfo; } }
        public Ctrl1 ControlField1 { get { return _controlField1; } }
        public Ctrl2 ControlField2 { get { return _controlField2; } }
        public byte SourceAddressHigh { get { return _sourceAddressHigh; } }
        public byte SourceAddressLow { get { return _sourceAddressLow; } }
        public byte DestAddressHigh { get { return _destAddressHigh; } }
        public byte DestAddressLow { get { return _destAddressLow; } }
        public byte NpduLength { get { return _npduLength; } }
        public byte[] Npdu { get { return _npdu; } }
        public ushort Other { get { return _other; } }
        public Cemi M_Root { get { return m_root; } }
        public KaitaiStruct M_Parent { get { return m_parent; } }
    }
}