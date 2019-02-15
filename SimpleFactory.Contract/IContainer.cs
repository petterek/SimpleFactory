using System;
using System.Collections.Generic;


namespace SimpleFactory.Contract
{
    public interface IContainer : IServiceRegistry,IServiceProvider
    {
                        
    }
    
    public interface IServiceRegistry
    {
        Dictionary<Type, RegistrationInfo> Registered { get; }
        
        bool IsRegistered(Type type);

        RegistrationInfo Register(Type t);

        RegistrationInfo Register(Type t, Func<Object> factory);

        RegistrationInfo Register(Type identifierType, Type instanceType);

        RegistrationInfo Register<TInterface, TImplementedBy>() where TImplementedBy : TInterface;

        RegistrationInfo Register<TType>();

        RegistrationInfo Register<TInterface>(Type implementedBy);

        RegistrationInfo Register<TInterface>(Func<TInterface> factory);

        RegistrationInfo Register<TInterface, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7>(Func<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TInterface> factory);

        RegistrationInfo Register<TInterface, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>(Func<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TInterface> factory);

        RegistrationInfo Register<TInterface, TParam1, TParam2, TParam3, TParam4, TParam5>(Func<TParam1, TParam2, TParam3, TParam4, TParam5, TInterface> factory);

        RegistrationInfo Register<TInterface, TParam1, TParam2, TParam3, TParam4>(Func<TParam1, TParam2, TParam3, TParam4, TInterface> factory);

        RegistrationInfo Register<TInterface, TParam1, TParam2, TParam3>(Func<TParam1, TParam2, TParam3, TInterface> factory);

        RegistrationInfo Register<TInterface, TParam1, TParam2>(Func<TParam1, TParam2, TInterface> factory);

        RegistrationInfo Register<TInterface, TParam1>(Func<TParam1, TInterface> factory);

    }
}