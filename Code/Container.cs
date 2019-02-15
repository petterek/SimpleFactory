using SimpleFactory.Contract;
using SimpleFactory.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SimpleFactory
{
    public class Container :  IDisposable
    {
        private Dictionary<string, Func<Dictionary<Type, Object>, object>> creatorFunctions = new Dictionary<string, Func<Dictionary<Type, object>, object>>();

        public Dictionary<Type, RegistrationInfo> Registered { get; } = new Dictionary<Type, RegistrationInfo>();

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

        public bool IsRegistered(Type type)
        {
            return Registered.ContainsKey(type);
        }

        public RegistrationInfo Register(Type t)
        {
            return Register(t, t);
        }


        public RegistrationInfo Register(Type t, Func<object> factory)
        {

            RegistrationInfo registrationInfo = CreateReginfo(t, factory.Method, factory.Target);
            registrationInfo.LifeCycle = defaultLifeTimeEnum;
            Registered[t] = registrationInfo;
            return registrationInfo;
        }

        public RegistrationInfo Register(Type identifierType, Type instanceType)
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

            RegistrationInfo registrationInfo = new RegistrationInfo { Type = instanceType, Key = identifierType };
            registrationInfo.Constructor = new ConstructorData( constructor[0]);
            registrationInfo.LifeCycle = defaultLifeTimeEnum;
            Registered[identifierType] = registrationInfo;

            return registrationInfo;
        }

        public RegistrationInfo Register<TType>()
        {
            if (typeof(TType).IsInterface) throw new ArgumentException("Type cannot be an Interface");
            return Register<TType, TType>();
        }

        public RegistrationInfo Register<TInterface>(Type implementedBy)
        {
            if (!typeof(TInterface).IsAssignableFrom(implementedBy))
            {
                throw new ArgumentException($"Parameter implementedBy must implement type {typeof(TInterface).FullName}");
            }
            Type identifierType = typeof(TInterface);
            return Register(identifierType, implementedBy);
        }

        public RegistrationInfo Register<TInterface, TImplementedBy>() where TImplementedBy : TInterface
        {
            Type identifierType = typeof(TInterface);
            Type instanceType = typeof(TImplementedBy);
            return Register(identifierType, instanceType);

        }

        private static RegistrationInfo CreateReginfo<TType>(MethodInfo mi, object target)
        {
            var registrationInfo = new RegistrationInfo
            {
                Key = typeof(TType),
                Factory = new FactoryData(mi, target)
            };
            return registrationInfo;
        }
        private static RegistrationInfo CreateReginfo(Type forType, MethodInfo mi, object target)
        {
            var registrationInfo = new RegistrationInfo
            {
                Key = forType,
                Factory = new FactoryData(mi,mi.IsStatic?null:target)
            };
            return registrationInfo;
        }

        public RegistrationInfo Register<TType>(Func<TType> factory)
        {
            RegistrationInfo registrationInfo = CreateReginfo<TType>(factory.Method, factory.Target);
            registrationInfo.LifeCycle = defaultLifeTimeEnum;
            Registered[typeof(TType)] = registrationInfo;
            return registrationInfo;
        }



        public RegistrationInfo Register<TType, TParam1>(Func<TParam1, TType> factory)
        {
            RegistrationInfo registrationInfo = CreateReginfo<TType>(factory.Method, factory.Target);
            registrationInfo.LifeCycle = defaultLifeTimeEnum;
            Registered[typeof(TType)] = registrationInfo;
            return registrationInfo;
        }

        public RegistrationInfo Register<TType, TParam1, TParam2>(Func<TParam1, TParam2, TType> factory)
        {
            RegistrationInfo registrationInfo = CreateReginfo<TType>(factory.Method, factory.Target);
            registrationInfo.LifeCycle = defaultLifeTimeEnum;
            Registered[typeof(TType)] = registrationInfo;
            return registrationInfo;
        }

        public RegistrationInfo Register<TType, TParam1, TParam2, TParam3>(Func<TParam1, TParam2, TParam3, TType> factory)
        {
            RegistrationInfo registrationInfo = CreateReginfo<TType>(factory.Method, factory.Target);
            registrationInfo.LifeCycle = defaultLifeTimeEnum;
            Registered[typeof(TType)] = registrationInfo;
            return registrationInfo;
        }

        public RegistrationInfo Register<TType, TParam1, TParam2, TParam3, TParam4>(Func<TParam1, TParam2, TParam3, TParam4, TType> factory)
        {
            RegistrationInfo registrationInfo = CreateReginfo<TType>(factory.Method, factory.Target);
            registrationInfo.LifeCycle = defaultLifeTimeEnum;
            Registered[typeof(TType)] = registrationInfo;
            return registrationInfo;
        }

        public RegistrationInfo Register<TType, TParam1, TParam2, TParam3, TParam4, TParam5>(Func<TParam1, TParam2, TParam3, TParam4, TParam5, TType> factory)
        {
            RegistrationInfo registrationInfo = CreateReginfo<TType>(factory.Method, factory.Target);
            registrationInfo.LifeCycle = defaultLifeTimeEnum;
            Registered[typeof(TType)] = registrationInfo;
            return registrationInfo;
        }

        public RegistrationInfo Register<TType, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>(Func<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TType> factory)
        {
            RegistrationInfo registrationInfo = CreateReginfo<TType>(factory.Method, factory.Target);
            registrationInfo.LifeCycle = defaultLifeTimeEnum;
            Registered[typeof(TType)] = registrationInfo;
            return registrationInfo;
        }

        public RegistrationInfo Register<TType, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7>(Func<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TType> factory)
        {
            RegistrationInfo registrationInfo = CreateReginfo<TType>(factory.Method, factory.Target);
            registrationInfo.LifeCycle = defaultLifeTimeEnum;
            Registered[typeof(TType)] = registrationInfo;
            return registrationInfo;
        }

        public IList<RegistrationInfo> Items()
        {
            return Registered.Select(el => (RegistrationInfo)el.Value).ToList();
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
                var basetype = type;

                while (basetype != null)
                {
                    foreach (var item in providedTypes)
                    {
                        if (basetype.IsAssignableFrom(item.Key))
                        {
                            returnEx = Expression.Convert(Expression.Property(providedTypesParam, "Item", Expression.Constant(item.Key)), type);
                            break;
                        }
                    }
                    if (returnEx != null) break;
                    basetype = basetype.BaseType;
                }

                if (returnEx == null) { throw new MissingRegistrationException(type); }

                //Is provided
            }
            else
            {
                RegistrationInfo registrationInfo = (RegistrationInfo)Registered[type];
                if (mustBeSingleton & registrationInfo.LifeCycle != LifeTimeEnum.Singleton) throw new Exceptions.UnAllowedConstruct();

                if (registrationInfo.Factory != null) //The type is registered with factory not type..
                {
                    var paramList = new List<Expression>();
                    ConstantExpression target = Expression.Constant(registrationInfo.Factory.Target);

                    switch (registrationInfo.LifeCycle)
                    {
                        


                        case LifeTimeEnum.Singleton:
                            if (registrationInfo.SingletonInstance == null)
                            {
                                foreach (var param in registrationInfo.Factory.Parameters)
                                {
                                    var ex = CreateBuilders(param, parameters, assign, list, providedTypesParam, providedTypes, true);
                                    paramList.Add(Expression.Convert(ex, param));
                                }
                                var body = Expression.Call(target, registrationInfo.Factory.Method, paramList);
                                registrationInfo.SingletonInstance = Expression.Lambda<Func<Dictionary<Type, object>, object>>(body, providedTypesParam).Compile().Invoke(new Dictionary<Type, object>());
                            }
                            returnEx = Expression.Constant(registrationInfo.SingletonInstance);
                            break;

                        case LifeTimeEnum.Scoped:

                            ParameterExpression theVar;
                            if (!parameters.ContainsKey(type))
                            {
                                theVar = Expression.Variable(type);
                                parameters[type] = theVar;
                                foreach (var param in registrationInfo.Factory.Parameters)
                                {
                                    var ex = CreateBuilders(param, parameters, assign, list, providedTypesParam, providedTypes, false);
                                    paramList.Add(Expression.Convert(ex, param));
                                }
                                assign.Add(Expression.Assign(theVar, Expression.Convert(Expression.Call(target, registrationInfo.Factory.Method, paramList), theVar.Type)));
                            }
                            else
                            {
                                theVar = parameters[type];
                            }
                            returnEx = theVar;
                            break;


                        case LifeTimeEnum.Transient:

                            foreach (var p in registrationInfo.Factory.Parameters)
                            {
                                var ex = CreateBuilders(p, parameters, assign, list, providedTypesParam, providedTypes, false);
                                paramList.Add(Expression.Convert(ex, p));
                            }
                            if (registrationInfo.Factory.Method.IsStatic)
                            {

                                returnEx = Expression.Call(null, registrationInfo.Factory.Method, paramList);
                            }
                            else
                            {
                                returnEx = Expression.Call(target, registrationInfo.Factory.Method, paramList);

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
                            if (registrationInfo.SingletonInstance == null)
                            {
                                foreach (var param in registrationInfo.Constructor.Params)
                                {
                                    var ex = CreateBuilders(param, parameters, assign, list, providedTypesParam, providedTypes, true);
                                    paramList.Add(Expression.Convert(ex, param));
                                }
                                var body = Expression.New(registrationInfo.Constructor.Method, paramList);
                                registrationInfo.SingletonInstance = Expression.Lambda<Func<Dictionary<Type, object>, object>>(body, providedTypesParam).Compile().Invoke(new Dictionary<Type, object>());
                            }

                            returnEx = Expression.Constant(registrationInfo.SingletonInstance);
                            break;

                        case LifeTimeEnum.Scoped:
                            ParameterExpression theVar;
                            if (!parameters.ContainsKey(type))
                            {
                                theVar = Expression.Variable(type);
                                parameters[type] = theVar;

                                foreach (var param in registrationInfo.Constructor.Params)
                                {
                                    var ex = CreateBuilders(param, parameters, assign, list, providedTypesParam, providedTypes, false);
                                    paramList.Add(Expression.Convert(ex, param));
                                }
                                assign.Add(Expression.Assign(theVar, Expression.New(registrationInfo.Constructor.Method, paramList)));
                            }
                            else
                            {
                                theVar = parameters[type];
                            }
                            returnEx = theVar;
                            break;

                        case LifeTimeEnum.Transient:
                            foreach (var param in registrationInfo.Constructor.Params)
                            {
                                var ex = CreateBuilders(param, parameters, assign, list, providedTypesParam, providedTypes, false);
                                paramList.Add(Expression.Convert(ex, param));
                            }

                            returnEx = Expression.New(registrationInfo.Constructor.Method, paramList);
                            break;
                    }
                }
            }

            list.Pop();

            return returnEx;
        }

        private object CreateInstance(Type toCreate, Dictionary<Type, Object> providedTypes)
        {
            //Short circut.. 
            if (providedTypes.ContainsKey(toCreate)) return providedTypes[toCreate];

            if (!Registered.ContainsKey(toCreate)) throw new MissingRegistrationException(toCreate);

            RegistrationInfo registrationInfo =(RegistrationInfo) Registered[toCreate];

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

            if (registrationInfo.LifeCycle == LifeTimeEnum.Singleton && registrationInfo.SingletonInstance != null)
            {
                return registrationInfo.SingletonInstance;
            }

            var ret = creatorFunctions[key](providedTypes);
            if (registrationInfo.LifeCycle == LifeTimeEnum.Singleton) registrationInfo.SingletonInstance = ret;

            return ret;
        }

        public object GetService(Type serviceType)
        {
            return CreateAnonymousInstance(serviceType);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    foreach (var i in Items())
                    {
                        if (i.LifeCycle == LifeTimeEnum.Singleton)
                        {
                            (CreateAnonymousInstance(i.RegisteredWith) as IDisposable)?.Dispose();
                        }
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Container() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion


    }



}