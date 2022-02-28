using Tiveria.Home.Knx.ObjectServer.Services.Serializers;

namespace Tiveria.Home.Knx.ObjectServer
{
    public class ObjectServerServiceSerializerFactory
    { 
        #region Static Singleton
        public static ObjectServerServiceSerializerFactory Instance { get; } = new ObjectServerServiceSerializerFactory();
        #endregion

        #region static initialization
        static ObjectServerServiceSerializerFactory()
        {
            Instance.Initialize();
        }
        #endregion

        private Dictionary<ushort, Type> _serializersByServiceIdentifier = new();

        private ObjectServerServiceSerializerFactory()
        {
        }

        private void Initialize()
        {
            _serializersByServiceIdentifier.Clear();
        }

        public void Register<T>() where T : IObjectServerServiceSerializer, new()
        {
            var serializer = new T();
            var serviceId = (ushort) ((serializer.MainService << 8) + serializer.SubService);
            _serializersByServiceIdentifier.Remove(serviceId);
            _serializersByServiceIdentifier.Add(serviceId, typeof(T));
        }

        public IObjectServerServiceSerializer Create(byte mainService, byte subService)
        {
            var serviceId = (ushort) ((mainService << 8) + subService);
            if (_serializersByServiceIdentifier.TryGetValue(serviceId, out var parserType))
                return (IObjectServerServiceSerializer) Activator.CreateInstance(parserType)!;

            throw new NotSupportedException();
        }
    }

}
