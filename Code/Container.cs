using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using SimpleFactory.Exceptions;

namespace SimpleFactory
{
    public class RegistrationInfo
    {
        public Type Type;
        public Func<Dictionary<Type, Object>, object> Factory;
    }

    public class Container
    {
        private Dictionary<Type, RegistrationInfo> Registered = new Dictionary<Type, RegistrationInfo>();

        private MethodInfo creatorMethodInfo;
        private MethodInfo containsKey;
        private MethodInfo containerMi;
        private Dictionary<Type, MethodInfo> GenericTypeCache = new Dictionary<Type, MethodInfo>();

        public Container()
        {
            creatorMethodInfo = typeof(Container).GetMethod(nameof(CreateInstance), BindingFlags.NonPublic | BindingFlags.Instance);
            containsKey = typeof(Dictionary<Type, object>).GetMethod("ContainsKey");
            containerMi = typeof(Container).GetMethod(nameof(CreateInstance), new Type[] { typeof(object[]) });
        }

        public void Register<TInterface, TImplementedBy>() where TImplementedBy : TInterface
        {
            Registered[typeof(TInterface)] = new RegistrationInfo { Type = typeof(TInterface), Factory = BuildLambda(typeof(TImplementedBy)) };
        }
        public void Register<TType>()
        {
            Registered[typeof(TType)] = new RegistrationInfo { Type = typeof(TType), Factory = BuildLambda(typeof(TType)) };
        }

        public void Register<TType>(Func<ProvidedInstances, TType> factory)
        {
            Registered[typeof(TType)] = new RegistrationInfo { Type = typeof(TType), Factory = prov => factory(new ProvidedInstances(prov)) };
        }


        public IEnumerable<RegistrationInfo> Items()
        {
            foreach (var item in Registered)
            {
                yield return item.Value;
            }
        }

        public TToCreate CreateInstance<TToCreate>()
        {
            return CreateInstance<TToCreate>(new Dictionary<Type, object>());
        }
        public object CreateAnonymousInstance(Type toCreate)
        {
            return CreateInstance(toCreate, new Dictionary<Type, object>());
        }
        public object CreateAnonymousInstance(Type toCreate, params object[] providedTypes)
        {
            return CreateInstance(toCreate, providedTypes.ToDictionary(o => o.GetType()));
        }

        public TToCreate CreateInstance<TToCreate>(params object[] providedTypes)
        {
            return CreateInstance<TToCreate>(providedTypes.ToDictionary(o => o.GetType()));
        }

        public TToCreate CreateInstance<TToCreate>(Dictionary<Type, Object> providedTypes)
        {
            if (providedTypes == null)
                throw new ArgumentNullException(nameof(providedTypes));

            if (typeof(TToCreate).IsValueType)
            {
                return default(TToCreate);
            }

            if (!Registered.ContainsKey(typeof(TToCreate)))
            {
                throw new MissingRegistrationException(typeof(TToCreate));
            }

            return (TToCreate)Registered[typeof(TToCreate)].Factory(providedTypes);
        }

        private object CreateInstance(Type toCreate, Dictionary<Type, Object> providedTypes)
        {
            if (!Registered.ContainsKey(toCreate))
            {
                throw new MissingRegistrationException(toCreate);
            }
            return Registered[toCreate].Factory(providedTypes);
        }

        Func<Dictionary<Type, Object>, object> BuildLambda(Type toCreate)
        {

            var constructor = toCreate.GetConstructors();
            if (constructor.Length == 0)
            {
                throw new ArgumentOutOfRangeException($"Unable to construct type {toCreate}, no constructor found!");
            }
                if (constructor.Length > 1)
            {
                throw new TooManyConstructorsException();
            }

            ConstructorInfo constructorInfo = constructor[0];

            var paramList = new List<Expression>();
            var refProvide = Expression.Parameter(typeof(Dictionary<Type, object>));

            foreach (var item in constructorInfo.GetParameters())
            {
                var typeParameter = Expression.Constant(item.ParameterType);

                var ifTest = Expression.Call(refProvide, containsKey, typeParameter);

                var getValueFromDic = Expression.Property(refProvide, "Item", typeParameter);

                var getValueFromFunc = Expression.Call(Expression.Constant(this), creatorMethodInfo, typeParameter, refProvide);

                paramList.Add(
                    Expression.Convert(
                        Expression.Condition(ifTest, getValueFromDic, getValueFromFunc
                    ), item.ParameterType));
            }

            
            var ctor = Expression.New(constructorInfo, paramList);
            

            return Expression.Lambda<Func<Dictionary<Type, Object>, object>>(ctor, refProvide).Compile();
        }

        private MethodInfo GetGenericMethod(Type t)
        {
            if (!GenericTypeCache.ContainsKey(t))
            {
                lock (GenericTypeCache)
                {
                    if (!GenericTypeCache.ContainsKey(t))
                    {
                        GenericTypeCache[t] = containerMi.MakeGenericMethod(t);
                    }
                }
            }
            return GenericTypeCache[t];
        }

        public void ResolveFields(object loader, params object[] providedInstances)
        {
            foreach (var fi in loader.GetType().GetFields())
            {
                fi.SetValue(loader, GetGenericMethod(fi.FieldType).Invoke(this, new object[] { providedInstances }));
            }

        }


        public class ProvidedInstances
        {
            readonly Dictionary<Type, object> prov;

            public ProvidedInstances(params object[] input)
            {
                prov =  input.ToDictionary(o => o.GetType());
            }

            public ProvidedInstances(Dictionary<Type, object> prov)
            {
                this.prov = prov;
            }

            public bool HasInstanceOf<T>()
            {
                return prov.ContainsKey(typeof(T));
            }

            public T GetInstanc<T>()
            {
                return (T)prov[typeof(T)];
            }

        }


    }
}
