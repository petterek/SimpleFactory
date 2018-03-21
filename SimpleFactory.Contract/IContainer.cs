using System;
using System.Collections.Generic;

namespace SimpleFactory.Contract
{
    public interface IContainer
    {
        object CreateAnonymousInstance(Type toCreate);
        object CreateAnonymousInstance(Type toCreate, params object[] providedTypes);
        TToCreate CreateInstance<TToCreate>();
        TToCreate CreateInstance<TToCreate>(Dictionary<Type, object> providedTypes);
        TToCreate CreateInstance<TToCreate>(params object[] providedTypes);
        IEnumerable<IRegistrationInfo> Items();
        IRegistrationInfo Register(Type t);
        IRegistrationInfo Register(Type identifierType, Type instanceType);
        IRegistrationInfo Register<TInterface, TImplementedBy>() where TImplementedBy : TInterface;
        IRegistrationInfo Register<TType, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7>(Func<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TType> factory);
        IRegistrationInfo Register<TType, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>(Func<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TType> factory);
        IRegistrationInfo Register<TType, TParam1, TParam2, TParam3, TParam4, TParam5>(Func<TParam1, TParam2, TParam3, TParam4, TParam5, TType> factory);
        IRegistrationInfo Register<TType, TParam1, TParam2, TParam3, TParam4>(Func<TParam1, TParam2, TParam3, TParam4, TType> factory);
        IRegistrationInfo Register<TType, TParam1, TParam2, TParam3>(Func<TParam1, TParam2, TParam3, TType> factory);
        IRegistrationInfo Register<TType, TParam1, TParam2>(Func<TParam1, TParam2, TType> factory);
        IRegistrationInfo Register<TType, TParam1>(Func<TParam1, TType> factory);
        IRegistrationInfo Register<TType>();
        IRegistrationInfo Register<TType>(Func<TType> factory);
        void ResolveFields(object loader, params object[] providedInstances);
    }
}