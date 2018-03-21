using System;
using System.Reflection;

namespace SimpleFactory.Contract
{
    public class RegistrationInfo : Contract.IRegistrationInfo
    {
        internal Type Type;
        //public Func<Dictionary<Type, Object>, object> Factory;
        public bool Factory = false;
        internal MethodInfo FactoryInfo;
        internal Object FactoryTarget;
        internal ConstructorInfo Constructor;
        internal ParameterInfo[] ConstructorParams;
        public void AsSingleton() { LifeCycle = LifeTimeEnum.Singleton; }
        public void PerGraph() { LifeCycle = LifeTimeEnum.PerGraph; }
        public void Transient() { LifeCycle = LifeTimeEnum.Transient; }

        internal object Instance;

        public LifeTimeEnum LifeCycle { get;  set; }
    }
}
