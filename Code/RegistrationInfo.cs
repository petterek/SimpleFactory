using System;
using System.Linq;
using System.Reflection;

namespace SimpleFactory.Contract
{
    public class RegistrationInfo
    {
        public Type Key { get; set; }
        public Type Type { get; set; }


        public FactoryData  Factory { get; set; }
        public ConstructorData Constructor { get; set; }

        public void Singleton() { LifeCycle = LifeTimeEnum.Singleton; }
        public void Transient() { LifeCycle = LifeTimeEnum.Transient; }
        public void Scoped() { LifeCycle = LifeTimeEnum.Scoped; }


        public object SingletonInstance { get; set; }

        public LifeTimeEnum LifeCycle { get; set; }

        public Type RegisteredWith => Key;

        public Type ImplementedBy { get => Type; set => Type = value; }

        public bool IsImplementedByFactory => Factory != null;
    }

    public class FactoryData
    {
        public FactoryData(MethodInfo method, object target)
        {
            Method = method;
            Target = target;
            Parameters = method.GetParameters().Select(p=>p.ParameterType).ToArray();
        }

        public Type[] Parameters { get; }
        public MethodInfo Method { get; }
        public object Target { get; }

    }

    public class ConstructorData
    {
        public ConstructorData(ConstructorInfo data)
        {
            Method = data;
            Params = Method.GetParameters().Select(p=>p.ParameterType).ToArray() ;
        }
        public ConstructorInfo Method { get; }
        public Type[] Params { get; }
    }
}
