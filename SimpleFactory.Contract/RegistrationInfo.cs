using System;
using System.Reflection;

namespace SimpleFactory.Contract
{
    public interface IRegistrationInfo
    {
     
        void AsSingleton();
        void PerGraph();
        void Transient();
             
    }
}
