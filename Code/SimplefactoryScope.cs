using SimpleFactory.Contract;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleFactory
{
       
    public class SimplefactoryProvider : IServiceProvider, IDisposable
    {
        public SimplefactoryProvider(Container container)
        {
            this.container = container;
        }

        readonly IServiceProvider parent;
               

        private Dictionary<Type, Object> ScopedObjects = new Dictionary<Type, object>();

        readonly Container container;

        public object GetService(Type serviceType)
        {

            if (!container.IsRegistered(serviceType))
            {
                throw new Exceptions.MissingRegistrationException(serviceType);
            }


            return InternalGetService(serviceType, new Stack<Type>());
        }


        private object InternalGetService(Type type, Stack<Type> graph)
        {

            if (graph.Contains(type)) throw new Exceptions.CircularDependencyDetected();

            if (ScopedObjects.ContainsKey(type))
            {
                return ScopedObjects[type];
            }


            //Need to keep this to avoid circular deps
            var regValue = container.Registered[type];
            if (regValue.LifeCycle == LifeTimeEnum.Singleton & regValue.SingletonInstance != null) return regValue.SingletonInstance;


            graph.Push(type);
            object instance;
            if (regValue.IsImplementedByFactory)
            {
                instance = InvokeFactory(regValue, graph);
            }
            else
            {
                instance = InvokeConstructor(regValue, graph);
            }


            switch (regValue.LifeCycle)
            {
                case LifeTimeEnum.Scoped:
                    ScopedObjects.Add(type, instance);
                    break;
                case LifeTimeEnum.Singleton:
                    regValue.SingletonInstance = instance;
                    break;
                default:
                    break;
            }

            graph.Pop();
            return instance;
        }

        private object InvokeFactory(RegistrationInfo registrationInfo, Stack<Type> graph)
        {
            return registrationInfo.Factory.Method.Invoke(registrationInfo.Factory.Target, registrationInfo.Factory.Parameters.Select(param => InternalGetService(param, graph)).ToArray());
        }

        private object InvokeConstructor(RegistrationInfo registrationInfo, Stack<Type> graph)
        {
            return registrationInfo.Constructor.Method.Invoke(registrationInfo.Constructor.Params.Select(e => InternalGetService(e, graph)).ToArray());
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    foreach (var el in ScopedObjects.Values.OfType<IDisposable>())
                    {
                        el.Dispose();
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~SimplefactoryScope() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }



        #endregion
    }
}