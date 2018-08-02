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

        IRegistrationInfo Register<TType>();

        IRegistrationInfo Register<TInterface>(Type implementedBy);

        IRegistrationInfo Register<TInterface>(Func<TInterface> factory);

        IRegistrationInfo Register<TInterface, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7>(Func<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TInterface> factory);

        IRegistrationInfo Register<TInterface, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>(Func<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TInterface> factory);

        IRegistrationInfo Register<TInterface, TParam1, TParam2, TParam3, TParam4, TParam5>(Func<TParam1, TParam2, TParam3, TParam4, TParam5, TInterface> factory);

        IRegistrationInfo Register<TInterface, TParam1, TParam2, TParam3, TParam4>(Func<TParam1, TParam2, TParam3, TParam4, TInterface> factory);

        IRegistrationInfo Register<TInterface, TParam1, TParam2, TParam3>(Func<TParam1, TParam2, TParam3, TInterface> factory);

        IRegistrationInfo Register<TInterface, TParam1, TParam2>(Func<TParam1, TParam2, TInterface> factory);

        IRegistrationInfo Register<TInterface, TParam1>(Func<TParam1, TInterface> factory);

        void ResolveFields(object loader, params object[] providedInstances);
    }
}