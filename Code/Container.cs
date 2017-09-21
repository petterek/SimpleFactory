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
        public ConstructorInfo Constructor;
        public ParameterInfo[] ConstructorParams;
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



        public void Register<TType>(Func<TType> factory)
        {
            Registered[typeof(TType)] = new RegistrationInfo
            {
                Type = typeof(TType),
                Factory = p => factory()
            };
        }
        public void Register<TType, TParam1>(Func<TParam1, TType> factory)
        {
            Registered[typeof(TType)] = new RegistrationInfo
            {
                Type = typeof(TType),
                Factory = prov => factory(GetParam<TParam1>(prov))
            };
        }
        public void Register<TType, TParam1, TParam2>(Func<TParam1, TParam2, TType> factory)
        {
            Registered[typeof(TType)] = new RegistrationInfo
            {
                Type = typeof(TType),
                Factory = prov => factory(GetParam<TParam1>(prov), GetParam<TParam2>(prov))
            };
        }
        public void Register<TType, TParam1, TParam2, TParam3>(Func<TParam1, TParam2, TParam3, TType> factory)
        {
            Registered[typeof(TType)] = new RegistrationInfo
            {
                Type = typeof(TType),
                Factory = prov => factory(GetParam<TParam1>(prov), GetParam<TParam2>(prov), GetParam<TParam3>(prov))
            };
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

            var key = $"{typeof(TToCreate).FullName}-{string.Join("-", providedTypes.Select(e => e.Key.FullName).ToArray())}"; //Just the list of all involved types.. To ensure unique function for each
            if (!creatorFunctions.ContainsKey(key))
            {
                lock (creatorFunctions)
                {
                    if (!creatorFunctions.ContainsKey(key))
                    {
                        creatorFunctions[key] = BuildLambda(typeof(TToCreate), new List<Type>());
                    }
                }
            }

            return (TToCreate)creatorFunctions[key](providedTypes);
        }

        private Func<Dictionary<Type, object>, object> BuildLambda(Type type, List<Type> list)
        {

            var providedTypes = Expression.Parameter(typeof(Dictionary<Type, object>));

            Expression body = CreateBuilders(type, list, providedTypes);

            return Expression.Lambda<Func<Dictionary<Type, object>, object>>(body, providedTypes).Compile();

        }


        private Expression CreateBuilders(Type type, List<Type> list, ParameterExpression providedTypes)
        {
            if (list.Contains(type)) throw new Exceptions.CircularDependencyDetected();
            list.Add(type);

            RegistrationInfo registrationInfo = Registered[type];

            if (registrationInfo.Factory != null)
            {
                ConstantExpression target = Expression.Constant(Registered[type]);
                MemberExpression memberExpression = Expression.MakeMemberAccess(target, RegistrationFactory);

                return Expression.Convert(Expression.Invoke(memberExpression, providedTypes), type);
            }

            var local = Expression.Variable(type, "result");

            var ifEx = Expression.IfThenElse(
                Expression.Call(providedTypes, containsKey, Expression.Constant(type)),
                Expression.Assign(local, Expression.Convert(Expression.Property(providedTypes, "Item", Expression.Constant(type)), type)),
                Expression.Assign(local, Expression.New(registrationInfo.Constructor, registrationInfo.ConstructorParams.Select(p => CreateBuilders(p.ParameterType, list, providedTypes))))
                );

            return Expression.Block(new ParameterExpression[] { local }, new Expression[] { ifEx, local });

        }



        private object CreateInstance(Type toCreate, Dictionary<Type, Object> providedTypes)
        {
            if (providedTypes.ContainsKey(toCreate))
            {
                return providedTypes[toCreate];
            }
            if (!Registered.ContainsKey(toCreate))
            {
                throw new MissingRegistrationException(toCreate);
            }
            return Registered[toCreate].Factory(providedTypes);
        }

        //Func<Dictionary<Type, Object>, object> BuildLambda(Type toCreate)
        //{


        //    ConstructorInfo constructorInfo = constructor[0];

        //    var paramList = new List<Expression>();
        //    var refProvide = Expression.Parameter(typeof(Dictionary<Type, object>));

        //    foreach (var item in constructorInfo.GetParameters())
        //    {
        //        var typeParameter = Expression.Constant(item.ParameterType);

        //        var ifTest = Expression.Call(refProvide, containsKey, typeParameter);

        //        var getValueFromDic = Expression.Property(refProvide, "Item", typeParameter);

        //        var getValueFromFunc = Expression.Call(Expression.Constant(this), creatorMethodInfo, typeParameter, refProvide);

        //        paramList.Add(
        //            Expression.Convert(
        //                Expression.Condition(ifTest, getValueFromDic, getValueFromFunc
        //            ), item.ParameterType));
        //    }


        //    var ctor = Expression.New(constructorInfo, paramList);


        //    return Expression.Lambda<Func<Dictionary<Type, Object>, object>>(ctor, refProvide).Compile();
        //}

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
