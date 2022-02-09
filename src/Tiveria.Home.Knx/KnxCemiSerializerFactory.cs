/*
    Tiveria.Home.Knx - a .Net Core base KNX library
    Copyright (c) 2018 M. Geissler

    This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published by
    the Free Software Foundation; either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

    Linking this library statically or dynamically with other modules is
    making a combined work based on this library. Thus, the terms and
    conditions of the GNU General Public License cover the whole
    combination.
*/

using Tiveria.Home.Knx.Cemi;
using Tiveria.Home.Knx.Cemi.Serializers;

namespace Tiveria.Home.Knx
{
    /// <summary>
    /// Factory class used to access all registered serializers for sepcific cemi messages
    /// </summary>
    public class KnxCemiSerializerFactory
    {
        #region Static Singleton
        private static KnxCemiSerializerFactory? _Instance = null;
        private static readonly object lockobj = new object();

        /// <summary>
        /// Property exposing the single instance of this factory
        /// </summary>
        public static KnxCemiSerializerFactory Instance
        {
            get
            {
                {
                    if (_Instance == null)
                    {
                        lock (lockobj)
                        {
                            if (_Instance == null)
                            {
                                _Instance = new KnxCemiSerializerFactory();
                            }
                        }
                    }
                    return _Instance;
                }
            }
        }
        #endregion

        #region Static Constructor
        KnxCemiSerializerFactory()
        {
            _serializersByMessageCode.Clear();
            Register<CemiLDataSerializer>(MessageCode.LDATA_CON);
            Register<CemiLDataSerializer>(MessageCode.LDATA_IND);
            Register<CemiLDataSerializer>(MessageCode.LDATA_REQ);
        }
        #endregion Static Constructor

        #region Public Methods
        /// <summary>
        /// Registers a new <see cref="IKnxCemiSerializer"/> based type to de/serialize Cemi messages with a specific <see cref="MessageCode"/>
        /// </summary>
        /// <typeparam name="T">The type to register</typeparam>
        /// <param name="messageCode">The <see cref="MessageCode"/> to register the serializer for</param>
        public void Register<T>(MessageCode messageCode) where T : IKnxCemiSerializer, new()
        {
            _serializersByMessageCode.Remove(messageCode);
            _serializersByMessageCode.Add(messageCode, typeof(T));
        }

        /// <summary>
        /// Creates a new instance of a <see cref="IKnxCemiSerializer"/> based on the provided <see cref="MessageCode"/>
        /// </summary>
        /// <param name="messageCode">The messagecode</param>
        /// <returns>A new instance of an <see cref="IKnxCemiSerializer"/></returns>
        public IKnxCemiSerializer Create(MessageCode messageCode)
        {
            if (_serializersByMessageCode.TryGetValue(messageCode, out var parserType))
                return (IKnxCemiSerializer)Activator.CreateInstance(parserType)!;

            return new CemiRawSerializer();
        }
        #endregion Public Methods

        private Dictionary<MessageCode, Type> _serializersByMessageCode = new();
    }
}
