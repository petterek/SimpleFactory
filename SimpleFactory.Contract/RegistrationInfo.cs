using System;
using System.Reflection;

namespace SimpleFactory.Contract
{
    public interface IRegistrationInfo
    {

        Type RegisteredWith { get; }
        Type ImplementedBy { get; set; }
        bool IsImplementedByFactory { get; }

        void AsSingleton();
        void PerGraph();
        void Transient();
             
    }
}
