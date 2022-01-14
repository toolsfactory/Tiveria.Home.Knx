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

using Tiveria.Home.Knx.IP.Enums;
using Tiveria.Home.Knx.IP.Frames.Serializers;

namespace Tiveria.Home.Knx.IP
{
    public class KnxNetIPFrameSerializerFactory
    {
        #region Static Singleton
        private static KnxNetIPFrameSerializerFactory? _Instance = null;
        private static readonly object lockobj = new object();

        public static KnxNetIPFrameSerializerFactory Instance
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
                                _Instance = new KnxNetIPFrameSerializerFactory();
                            }
                        }
                    }
                    return _Instance;
                }
            }
        }
        #endregion

        private Dictionary<ServiceTypeIdentifier, Type> _serializersByServiceTypeIdentifier = new();
        private Dictionary<Type, Type> _serializersByFrameType = new();

        private KnxNetIPFrameSerializerFactory()
        {
        }

        public void Initialize()
        {
            _serializersByServiceTypeIdentifier.Clear();
            _serializersByFrameType.Clear();
            Register<ConnectionRequestFrameSerializer>();
            Register<ConnectionResponseFrameSerializer>();
            Register<ConnectionStateRequestFrameSerializer>();
            Register<ConnectionStateResponseFrameSerializer>();
            Register<DescriptionRequestFrameSerializer>();
            Register<DescriptionResponseFrameSerializer>();
            Register<DeviceConfigurationAcknowledgeFrameSerializer>();
            Register<DeviceConfigurationRequestFrameSerializer>();
            Register<DisconnectRequestFrameSerializer>();
            Register<DisconnectResponseFrameSerializer>();
            Register<ExtendedSearchRequestFrameSerializer>();
            Register<RoutingBusyFrameSerializer>();
            Register<RoutingIndicationFrameSerializer>();
            Register<RoutingLostMessageFrameSerializer>();
            Register<SearchRequestFrameSerializer>();
            Register<SearchResponseFrameSerializer>();
            Register<TunnelingAcknowledgeFrameSerializer>();
            Register<TunnelingRequestFrameSerializer>();
        }

        public void Register<T>() where T : IKnxNetIPFrameSerializer, new()
        {
            var serializer = new T();
            _serializersByServiceTypeIdentifier.Remove(serializer.ServiceTypeIdentifier);
            _serializersByFrameType.Remove(serializer.FrameType);
            _serializersByServiceTypeIdentifier.Add(serializer.ServiceTypeIdentifier, typeof(T));
            _serializersByFrameType.Add(serializer.FrameType, typeof(T));
        }

        public IKnxNetIPFrameSerializer Create(ServiceTypeIdentifier service)
        {
            if (_serializersByServiceTypeIdentifier.TryGetValue(service, out var parserType))
                return (IKnxNetIPFrameSerializer) Activator.CreateInstance(parserType)!;

            return new RawFrameSerializer();
        }

        public IKnxNetIPFrameSerializer<T> Create<T>() where T : class, IKnxNetIPFrame
        {
            if (_serializersByFrameType.TryGetValue(typeof(T), out var serializerType))
                return (IKnxNetIPFrameSerializer<T>)Activator.CreateInstance(serializerType)!;

            throw new InvalidCastException();
        }
    }
}
