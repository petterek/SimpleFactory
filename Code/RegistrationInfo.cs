using System;
using System.Reflection;

namespace SimpleFactory.Contract
{
    public class RegistrationInfo : IRegistrationInfo
    {
        internal Type Key;
        internal Type Type;
        
        public bool Factory = false;
        internal MethodInfo FactoryInfo;
        internal object FactoryTarget;
        internal ConstructorInfo Constructor;
        internal ParameterInfo[] ConstructorParams;
        public void AsSingleton() { LifeCycle = LifeTimeEnum.Singleton; }
        public void PerGraph() { LifeCycle = LifeTimeEnum.PerGraph; }
        public void Transient() { LifeCycle = LifeTimeEnum.Transient; }

        internal object Instance;

        public LifeTimeEnum LifeCycle { get;  set; }

        public Type RegisteredWith => Key;

        public Type ImplementedBy { get =>Type; set => Type = value;}

        public bool IsImplementedByFactory => Factory;
    }
}
