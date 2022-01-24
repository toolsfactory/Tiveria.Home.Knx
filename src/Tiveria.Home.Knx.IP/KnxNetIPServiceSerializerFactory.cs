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
using Tiveria.Home.Knx.IP.Services.Serializers;

namespace Tiveria.Home.Knx.IP
{
    public class KnxNetIPServiceSerializerFactory
    {
        #region Static Singleton
        private static KnxNetIPServiceSerializerFactory? _Instance = null;
        private static readonly object lockobj = new object();

        public static KnxNetIPServiceSerializerFactory Instance
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
                                _Instance = new KnxNetIPServiceSerializerFactory();
                            }
                        }
                    }
                    return _Instance;
                }
            }
        }
        #endregion

        #region static initialization
        static KnxNetIPServiceSerializerFactory()
        {
            Instance.Initialize();
        }
        #endregion

        private Dictionary<ServiceTypeIdentifier, Type> _serializersByServiceTypeIdentifier = new();
        private Dictionary<Type, Type> _serializersByServiceType = new();

        private KnxNetIPServiceSerializerFactory()
        {
        }

        public void Initialize()
        {
            _serializersByServiceTypeIdentifier.Clear();
            _serializersByServiceType.Clear();
            Register<ConnectionRequestServiceSerializer>();
            Register<ConnectionResponseServiceSerializer>();
            Register<ConnectionStateRequestServiceSerializer>();
            Register<ConnectionStateResponseServiceSerializer>();
            Register<DescriptionRequestServiceSerializer>();
            Register<DescriptionResponseServiceSerializer>();
            Register<DeviceConfigurationAcknowledgeServiceSerializer>();
            Register<DeviceConfigurationRequestServiceSerializer>();
            Register<DisconnectRequestServiceSerializer>();
            Register<DisconnectResponseServiceSerializer>();
            Register<ExtendedSearchRequestServiceSerializer>();
            Register<RoutingBusyServiceSerializer>();
            Register<RoutingIndicationServiceSerializer>();
            Register<RoutingLostMessageServiceSerializer>();
            Register<SearchRequestServiceSerializer>();
            Register<SearchResponseServiceSerializer>();
            Register<TunnelingAcknowledgeServiceSerializer>();
            Register<TunnelingRequestServiceSerializer>();
        }

        public void Register<T>() where T : IKnxNetIPServiceSerializer, new()
        {
            var serializer = new T();
            _serializersByServiceTypeIdentifier.Remove(serializer.ServiceTypeIdentifier);
            _serializersByServiceType.Remove(serializer.ServiceType);
            _serializersByServiceTypeIdentifier.Add(serializer.ServiceTypeIdentifier, typeof(T));
            _serializersByServiceType.Add(serializer.ServiceType, typeof(T));
        }

        public IKnxNetIPServiceSerializer Create(ServiceTypeIdentifier service)
        {
            if (_serializersByServiceTypeIdentifier.TryGetValue(service, out var parserType))
                return (IKnxNetIPServiceSerializer) Activator.CreateInstance(parserType)!;

            return new RawServiceSerializer();
        }

        public IKnxNetIPServiceSerializer<T> Create<T>() where T : class, IKnxNetIPService
        {
            if (_serializersByServiceType.TryGetValue(typeof(T), out var serializerType))
                return (IKnxNetIPServiceSerializer<T>)Activator.CreateInstance(serializerType)!;

            throw new InvalidCastException();
        }
    }
}
