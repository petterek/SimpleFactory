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
        internal Type Type;
        public Func<Dictionary<Type, Object>, object> Factory;
        internal ConstructorInfo Constructor;
        internal ParameterInfo[] ConstructorParams;
        public void AsSingleton() { Singleton = true; }
        internal bool Singleton;
        internal object Instance;
    }



    public class Container
    {

        private Dictionary<string, Func<Dictionary<Type, Object>, object>> creatorFunctions = new Dictionary<string, Func<Dictionary<Type, object>, object>>();
        private Dictionary<Type, RegistrationInfo> Registered = new Dictionary<Type, RegistrationInfo>();

        private MethodInfo creatorMethodInfo;
        private MethodInfo containsKey;
        private MethodInfo containerMi;
        private MemberInfo RegistrationFactory;
        private Dictionary<Type, MethodInfo> GenericTypeCache = new Dictionary<Type, MethodInfo>();

        public Container()
        {
            creatorMethodInfo = typeof(Container).GetMethod(nameof(CreateInstance), BindingFlags.NonPublic | BindingFlags.Instance);
            containsKey = typeof(Dictionary<Type, object>).GetMethod("ContainsKey");
            containerMi = typeof(Container).GetMethod(nameof(CreateInstance), new Type[] { typeof(object[]) });
            RegistrationFactory = typeof(RegistrationInfo).GetField("Factory");
        }


        public RegistrationInfo Register<TType>()
        {
            return Register<TType, TType>();
        }

        public RegistrationInfo Register<TInterface, TImplementedBy>() where TImplementedBy : TInterface
        {
            Type type = typeof(TImplementedBy);

            var constructor = type.GetConstructors();
            if (constructor.Length == 0)
            {
                throw new ArgumentOutOfRangeException($"Unable to construct type {type.FullName}, no constructor found!");
            }
            if (constructor.Length > 1)
            {
                throw new TooManyConstructorsException(type);
            }

            RegistrationInfo registrationInfo = new RegistrationInfo { Type = type };
            registrationInfo.Constructor = constructor[0];
            registrationInfo.ConstructorParams = registrationInfo.Constructor.GetParameters();



            Registered[typeof(TInterface)] = registrationInfo;
            return registrationInfo;
        }

        public RegistrationInfo Register<TType>(Func<TType> factory)
        {
            var registrationInfo = new RegistrationInfo
            {
                Type = typeof(TType),
                Factory = p => factory()
            };

            Registered[typeof(TType)] = registrationInfo;
            return registrationInfo;
        }
        public RegistrationInfo Register<TType, TParam1>(Func<TParam1, TType> factory)
        {
            RegistrationInfo registrationInfo = new RegistrationInfo
            {
                Type = typeof(TType),
                Factory = prov => factory(GetParam<TParam1>(prov))
            };
            Registered[typeof(TType)] = registrationInfo;
            return registrationInfo;
        }
        public RegistrationInfo Register<TType, TParam1, TParam2>(Func<TParam1, TParam2, TType> factory)
        {
            RegistrationInfo registrationInfo = new RegistrationInfo
            {
                Type = typeof(TType),
                Factory = prov => factory(GetParam<TParam1>(prov), GetParam<TParam2>(prov))
            };
            Registered[typeof(TType)] = registrationInfo;
            return registrationInfo;
        }
        public RegistrationInfo Register<TType, TParam1, TParam2, TParam3>(Func<TParam1, TParam2, TParam3, TType> factory)
        {
            RegistrationInfo registrationInfo = new RegistrationInfo
            {
                Type = typeof(TType),
                Factory = prov => factory(GetParam<TParam1>(prov), GetParam<TParam2>(prov), GetParam<TParam3>(prov))
            };
            Registered[typeof(TType)] = registrationInfo;
            return registrationInfo;
        }
        public RegistrationInfo Register<TType, TParam1, TParam2, TParam3, TParam4>(Func<TParam1, TParam2, TParam3, TParam4, TType> factory)
        {
            RegistrationInfo registrationInfo = new RegistrationInfo
            {
                Type = typeof(TType),
                Factory = prov => factory(GetParam<TParam1>(prov), GetParam<TParam2>(prov), GetParam<TParam3>(prov), GetParam<TParam4>(prov))
            };
            Registered[typeof(TType)] = registrationInfo;
            return registrationInfo;
        }
        public RegistrationInfo Register<TType, TParam1, TParam2, TParam3, TParam4, TParam5>(Func<TParam1, TParam2, TParam3, TParam4, TParam5, TType> factory)
        {
            RegistrationInfo registrationInfo = new RegistrationInfo
            {
                Type = typeof(TType),
                Factory = prov => factory(GetParam<TParam1>(prov), GetParam<TParam2>(prov), GetParam<TParam3>(prov), GetParam<TParam4>(prov), GetParam<TParam5>(prov))
            };
            Registered[typeof(TType)] = registrationInfo;
            return registrationInfo;
        }
        public RegistrationInfo Register<TType, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>(Func<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TType> factory)
        {
            RegistrationInfo registrationInfo = new RegistrationInfo
            {
                Type = typeof(TType),
                Factory = prov => factory(GetParam<TParam1>(prov), GetParam<TParam2>(prov), GetParam<TParam3>(prov), GetParam<TParam4>(prov), GetParam<TParam5>(prov), GetParam<TParam6>(prov))
            };
            Registered[typeof(TType)] = registrationInfo;
            return registrationInfo;
        }
        public RegistrationInfo Register<TType, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7>(Func<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TType> factory)
        {
            RegistrationInfo registrationInfo = new RegistrationInfo
            {
                Type = typeof(TType),
                Factory = prov => factory(GetParam<TParam1>(prov), GetParam<TParam2>(prov), GetParam<TParam3>(prov), GetParam<TParam4>(prov), GetParam<TParam5>(prov), GetParam<TParam6>(prov), GetParam<TParam7>(prov))
            };
            Registered[typeof(TType)] = registrationInfo;
            return registrationInfo;
        }
        private T GetParam<T>(Dictionary<Type, object> prov)
        {
            return (T)CreateInstance(typeof(T), prov);
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

            return (TToCreate)CreateInstance(typeof(TToCreate), providedTypes);

        }

        private Func<Dictionary<Type, object>, object> BuildLambda(Type type, Stack<Type> list)
        {

            var providedTypes = Expression.Parameter(typeof(Dictionary<Type, object>));

            Expression body = CreateBuilders(type, list, providedTypes);

            return Expression.Lambda<Func<Dictionary<Type, object>, object>>(body, providedTypes).Compile();

        }


        private Expression CreateBuilders(Type type, Stack<Type> list, ParameterExpression providedTypes)
        {
            if (list.Contains(type)) throw new CircularDependencyDetected();
            list.Push(type);

            if (!Registered.ContainsKey(type))
            {
                throw new MissingRegistrationException(type);
            }
            RegistrationInfo registrationInfo = Registered[type];

            if (registrationInfo.Factory != null)
            {
                ConstantExpression target = Expression.Constant(Registered[type]);
                MemberExpression memberExpression = Expression.MakeMemberAccess(target, RegistrationFactory);

                list.Pop();
                return Expression.Convert(Expression.Invoke(memberExpression, providedTypes), type);
            }

            var local = Expression.Variable(type, "result");

            var ifEx = Expression.IfThenElse(
                Expression.Call(providedTypes, containsKey, Expression.Constant(type)),
                Expression.Assign(local, Expression.Convert(Expression.Property(providedTypes, "Item", Expression.Constant(type)), type)),
                Expression.Assign(local, Expression.New(registrationInfo.Constructor, registrationInfo.ConstructorParams.Select(p => CreateBuilders(p.ParameterType, list, providedTypes))))
                );
            list.Pop();
            return Expression.Block(new ParameterExpression[] { local }, new Expression[] { ifEx, local });

        }



        private object CreateInstance(Type toCreate, Dictionary<Type, Object> providedTypes)
        {
            if (providedTypes.ContainsKey(toCreate)) return providedTypes[toCreate];

            var key = $"{toCreate.FullName}-{string.Join("-", providedTypes.Select(e => e.Key.FullName).ToArray())}"; //Just the list of all involved types.. To ensure unique function for each
            if (!creatorFunctions.ContainsKey(key))
            {
                lock (creatorFunctions)
                {
                    if (!creatorFunctions.ContainsKey(key))
                    {
                        creatorFunctions[key] = BuildLambda(toCreate, new Stack<Type>());
                    }
                }
            }

            RegistrationInfo registrationInfo = Registered[toCreate];

            if (registrationInfo.Singleton && registrationInfo.Instance != null)
            {
                return registrationInfo.Instance;
            }

            var ret = creatorFunctions[key](providedTypes);
            if (registrationInfo.Singleton) registrationInfo.Instance = ret;

            return ret;
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

    }
}
