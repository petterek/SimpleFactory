using SimpleFactory.Contract;
using SimpleFactory.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SimpleFactory
{
    public class Container : IContainer
    {
        private Dictionary<string, Func<Dictionary<Type, Object>, object>> creatorFunctions = new Dictionary<string, Func<Dictionary<Type, object>, object>>();
        private Dictionary<Type, RegistrationInfo> Registered = new Dictionary<Type, RegistrationInfo>();

        private MethodInfo creatorMethodInfo;
        private MethodInfo containsKey;
        private MethodInfo containerMi;
        private MemberInfo RegistrationFactory;
        private Dictionary<Type, MethodInfo> GenericTypeCache = new Dictionary<Type, MethodInfo>();
        private readonly LifeTimeEnum defaultLifeTimeEnum;

        public Container()
        {
            creatorMethodInfo = typeof(Container).GetMethod(nameof(CreateInstance), BindingFlags.NonPublic | BindingFlags.Instance);
            containsKey = typeof(Dictionary<Type, object>).GetMethod("ContainsKey");
            containerMi = typeof(Container).GetMethod(nameof(CreateInstance), new Type[] { typeof(object[]) });
            RegistrationFactory = typeof(RegistrationInfo).GetField("Factory");
            defaultLifeTimeEnum = LifeTimeEnum.Transient;
        }

        public Container(LifeTimeEnum defaultLifeTimeEnum) : this()
        {
            this.defaultLifeTimeEnum = defaultLifeTimeEnum;
        }

        public bool IsRegistered<TInterface>()
        {
            return Registered.ContainsKey(typeof(TInterface));
        }

        public IRegistrationInfo Register(Type t)
        {
            return Register(t, t);
        }

        public IRegistrationInfo Register(Type identifierType, Type instanceType)
        {
            var constructor = instanceType.GetConstructors();
            if (constructor.Length == 0)
            {
                throw new ArgumentOutOfRangeException($"Unable to construct type {instanceType.FullName}, no constructor found!");
            }
            if (constructor.Length > 1)
            {
                throw new TooManyConstructorsException(instanceType);
            }

            RegistrationInfo registrationInfo = new RegistrationInfo { Type = instanceType };
            registrationInfo.Constructor = constructor[0];
            registrationInfo.ConstructorParams = registrationInfo.Constructor.GetParameters();

            registrationInfo.LifeCycle = defaultLifeTimeEnum;
            Registered[identifierType] = registrationInfo;

            return registrationInfo;
        }

        public IRegistrationInfo Register<TType>()
        {
            if (typeof(TType).IsInterface) throw new ArgumentException("Type cannot be an Interface");
            return Register<TType, TType>();
        }

        public IRegistrationInfo Register<TInterface>(Type implementedBy)
        {
            if (!typeof(TInterface).IsAssignableFrom(implementedBy))
            {
                throw new ArgumentException($"Parameter implementedBy must implement type {typeof(TInterface).FullName}");
            }
            Type identifierType = typeof(TInterface);
            return Register(identifierType, implementedBy);
        }

        public IRegistrationInfo Register<TInterface, TImplementedBy>() where TImplementedBy : TInterface
        {
            Type identifierType = typeof(TInterface);
            Type instanceType = typeof(TImplementedBy);
            return Register(identifierType, instanceType);
        }

        public IRegistrationInfo Register<TType>(Func<TType> factory)
        {
            var registrationInfo = new RegistrationInfo
            {
                Type = typeof(TType),
                Factory = true,
                FactoryInfo = factory.GetMethodInfo(),
                FactoryTarget = factory.Target
            };
            registrationInfo.LifeCycle = defaultLifeTimeEnum;
            Registered[typeof(TType)] = registrationInfo;
            return registrationInfo;
        }

        public IRegistrationInfo Register<TType, TParam1>(Func<TParam1, TType> factory)
        {
            RegistrationInfo registrationInfo = new RegistrationInfo
            {
                Type = typeof(TType),
                FactoryInfo = factory.GetMethodInfo(),
                FactoryTarget = factory.Target,
                Factory = true
            };
            registrationInfo.LifeCycle = defaultLifeTimeEnum;
            Registered[typeof(TType)] = registrationInfo;
            return registrationInfo;
        }

        public IRegistrationInfo Register<TType, TParam1, TParam2>(Func<TParam1, TParam2, TType> factory)
        {
            RegistrationInfo registrationInfo = new RegistrationInfo
            {
                Type = typeof(TType),
                FactoryInfo = factory.GetMethodInfo(),
                FactoryTarget = factory.Target,
                Factory = true
            };
            registrationInfo.LifeCycle = defaultLifeTimeEnum;
            Registered[typeof(TType)] = registrationInfo;
            return registrationInfo;
        }

        public IRegistrationInfo Register<TType, TParam1, TParam2, TParam3>(Func<TParam1, TParam2, TParam3, TType> factory)
        {
            RegistrationInfo registrationInfo = new RegistrationInfo
            {
                Type = typeof(TType),
                FactoryInfo = factory.GetMethodInfo(),
                FactoryTarget = factory.Target,
                Factory = true
            };
            registrationInfo.LifeCycle = defaultLifeTimeEnum;
            Registered[typeof(TType)] = registrationInfo;
            return registrationInfo;
        }

        public IRegistrationInfo Register<TType, TParam1, TParam2, TParam3, TParam4>(Func<TParam1, TParam2, TParam3, TParam4, TType> factory)
        {
            RegistrationInfo registrationInfo = new RegistrationInfo
            {
                Type = typeof(TType),
                FactoryInfo = factory.GetMethodInfo(),
                FactoryTarget = factory.Target,
                Factory = true
            };
            registrationInfo.LifeCycle = defaultLifeTimeEnum;
            Registered[typeof(TType)] = registrationInfo;
            return registrationInfo;
        }

        public IRegistrationInfo Register<TType, TParam1, TParam2, TParam3, TParam4, TParam5>(Func<TParam1, TParam2, TParam3, TParam4, TParam5, TType> factory)
        {
            RegistrationInfo registrationInfo = new RegistrationInfo
            {
                Type = typeof(TType),
                FactoryInfo = factory.GetMethodInfo(),
                FactoryTarget = factory.Target,
                Factory = true
            };
            registrationInfo.LifeCycle = defaultLifeTimeEnum;
            Registered[typeof(TType)] = registrationInfo;
            return registrationInfo;
        }

        public IRegistrationInfo Register<TType, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>(Func<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TType> factory)
        {
            RegistrationInfo registrationInfo = new RegistrationInfo
            {
                Type = typeof(TType),
                FactoryInfo = factory.GetMethodInfo(),
                FactoryTarget = factory.Target,
                Factory = true
            };
            registrationInfo.LifeCycle = defaultLifeTimeEnum;
            Registered[typeof(TType)] = registrationInfo;
            return registrationInfo;
        }

        public IRegistrationInfo Register<TType, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7>(Func<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TType> factory)
        {
            RegistrationInfo registrationInfo = new RegistrationInfo
            {
                Type = typeof(TType),
                FactoryInfo = factory.GetMethodInfo(),
                FactoryTarget = factory.Target,
                Factory = true
            };
            registrationInfo.LifeCycle = defaultLifeTimeEnum;
            Registered[typeof(TType)] = registrationInfo;
            return registrationInfo;
        }

        public IList<IRegistrationInfo> Items()
        {
            return Registered.Select(el => (IRegistrationInfo)el.Value).ToList();
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

        private Func<Dictionary<Type, object>, object> BuildLambda(Type type, Stack<Type> list, Dictionary<Type, Object> providedTypes)
        {
            var providedTypesParam = Expression.Parameter(typeof(Dictionary<Type, object>));
            var parameters = new Dictionary<Type, ParameterExpression>();
            var assign = new List<Expression>();
            Expression body = CreateBuilders(type, parameters, assign, list, providedTypesParam, providedTypes, false);

            var paramEx = new List<ParameterExpression>();
            foreach (var v in parameters.Values)
            {
                paramEx.Add(v);
            }

            assign.Add(body);

            BlockExpression body1 = Expression.Block(paramEx, assign);

            return Expression.Lambda<Func<Dictionary<Type, object>, object>>(body1, providedTypesParam).Compile();
        }

        private Expression CreateBuilders(Type type,
                                            Dictionary<Type, ParameterExpression> parameters,
                                            List<Expression> assign,
                                            Stack<Type> list,
                                            ParameterExpression providedTypesParam,
                                            Dictionary<Type, Object> providedTypes,
                                            bool mustBeSingleton)
        {
            if (list.Contains(type)) throw new CircularDependencyDetected();
            list.Push(type);

            Expression returnEx = null;

            if (providedTypes.ContainsKey(type)) //If you add a provided type, this will "win"
            {
                returnEx = Expression.Convert(Expression.Property(providedTypesParam, "Item", Expression.Constant(type)), type);
            }
            else if (!Registered.ContainsKey(type)) //The requested type is not provided nor registered, we must loop trough the list of provided to see if we can find a super that match
            {
                foreach (var item in providedTypes)
                {
                    var basetype = type;
                    while (basetype != null)
                    {
                        if (basetype.IsAssignableFrom(item.Key))
                        {
                            returnEx = Expression.Convert(Expression.Property(providedTypesParam, "Item", Expression.Constant(item.Key)), type);
                            break;
                        }
                        if (returnEx != null) break;

                        basetype = type.BaseType;
                    }
                }

                if (returnEx == null) { throw new MissingRegistrationException(type); }

                //Is provided
            }
            else
            {
                RegistrationInfo registrationInfo = Registered[type];
                if (mustBeSingleton & registrationInfo.LifeCycle != LifeTimeEnum.Singleton) throw new Exceptions.UnAllowedConstruct();

                if (registrationInfo.Factory) //The type is registered with factory not type..
                {
                    var paramList = new List<Expression>();
                    ConstantExpression target = Expression.Constant(Registered[type].FactoryTarget);

                    switch (registrationInfo.LifeCycle)
                    {
                        case LifeTimeEnum.Singleton:
                            if (registrationInfo.Instance == null)
                            {
                                foreach (var p in registrationInfo.FactoryInfo.GetParameters())
                                {
                                    paramList.Add(CreateBuilders(p.ParameterType, parameters, assign, list, providedTypesParam, providedTypes, true));
                                }
                                var body = Expression.Call(target, registrationInfo.FactoryInfo, paramList);
                                registrationInfo.Instance = Expression.Lambda<Func<Dictionary<Type, object>, object>>(body, providedTypesParam).Compile().Invoke(new Dictionary<Type, object>());
                            }
                            returnEx = Expression.Constant(registrationInfo.Instance);
                            break;

                        case LifeTimeEnum.PerGraph:

                            ParameterExpression theVar;
                            if (!parameters.ContainsKey(type))
                            {
                                theVar = Expression.Variable(type);
                                parameters[type] = theVar;
                                foreach (var p in registrationInfo.FactoryInfo.GetParameters())
                                {
                                    paramList.Add(CreateBuilders(p.ParameterType, parameters, assign, list, providedTypesParam, providedTypes, false));
                                }
                                assign.Add(Expression.Assign(theVar, Expression.Call(target, registrationInfo.FactoryInfo, paramList)));
                            }
                            else
                            {
                                theVar = parameters[type];
                            }
                            returnEx = theVar;
                            break;

                        case LifeTimeEnum.Transient:

                            foreach (var p in registrationInfo.FactoryInfo.GetParameters())
                            {
                                paramList.Add(CreateBuilders(p.ParameterType, parameters, assign, list, providedTypesParam, providedTypes, false));
                            }
                            if (registrationInfo.FactoryInfo.IsStatic)
                            {
                                returnEx = Expression.Call(null, registrationInfo.FactoryInfo, paramList);
                            }
                            else
                            {
                                returnEx = Expression.Call(target, registrationInfo.FactoryInfo, paramList);
                            }

                            break;
                    }
                }
                else
                {
                    var paramList = new List<Expression>();
                    switch (registrationInfo.LifeCycle)
                    {
                        case LifeTimeEnum.Singleton:
                            if (registrationInfo.Instance == null)
                            {
                                foreach (var param in registrationInfo.ConstructorParams)
                                {
                                    paramList.Add(CreateBuilders(param.ParameterType, parameters, assign, list, providedTypesParam, providedTypes, true));
                                }
                                var body = Expression.New(registrationInfo.Constructor, paramList);
                                registrationInfo.Instance = Expression.Lambda<Func<Dictionary<Type, object>, object>>(body, providedTypesParam).Compile().Invoke(new Dictionary<Type, object>());
                            }

                            returnEx = Expression.Constant(registrationInfo.Instance);
                            break;

                        case LifeTimeEnum.PerGraph:
                            ParameterExpression theVar;
                            if (!parameters.ContainsKey(type))
                            {
                                theVar = Expression.Variable(type);
                                parameters[type] = theVar;

                                foreach (var param in registrationInfo.ConstructorParams)
                                {
                                    paramList.Add(CreateBuilders(param.ParameterType, parameters, assign, list, providedTypesParam, providedTypes, false));
                                }
                                assign.Add(Expression.Assign(theVar, Expression.New(registrationInfo.Constructor, paramList)));
                            }
                            else
                            {
                                theVar = parameters[type];
                            }
                            returnEx = theVar;
                            break;

                        case LifeTimeEnum.Transient:
                            foreach (var param in registrationInfo.ConstructorParams)
                            {
                                paramList.Add(CreateBuilders(param.ParameterType, parameters, assign, list, providedTypesParam, providedTypes, false));
                            }

                            returnEx = Expression.New(registrationInfo.Constructor, paramList);
                            break;
                    }
                }
            }

            list.Pop();

            return returnEx;
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
                        creatorFunctions[key] = BuildLambda(toCreate, new Stack<Type>(), providedTypes);
                    }
                }
            }

            RegistrationInfo registrationInfo = Registered[toCreate];

            if (registrationInfo.LifeCycle == LifeTimeEnum.Singleton && registrationInfo.Instance != null)
            {
                return registrationInfo.Instance;
            }

            var ret = creatorFunctions[key](providedTypes);
            if (registrationInfo.LifeCycle == LifeTimeEnum.Singleton) registrationInfo.Instance = ret;

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