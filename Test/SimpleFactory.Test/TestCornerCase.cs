﻿using NUnit.Framework;
using SimpleFactory.Contract;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleFactory.Test
{


    [TestFixture]
    public class TestCornerCase
    {
        [Test]
        public void CorneCase1()
        {
            var w = new Wrapper();
            var ret = new SomeDep();

            w.Register<ISomeService, SomeService>();
            w.Register<SomeDep>(e => ret);

            var res = w.Create<ISomeService>(new object[] { new object() });

            Assert.IsNotNull(res);
        }

        [Test]
        public void RegisterSingletonIsWorking()
        {
            var c = new Container();
            c.Register<Adder>().Singleton();
            c.CreateInstance<Adder>().Add();
            c.CreateInstance<Adder>().Add();

            Assert.AreEqual(2, c.CreateInstance<Adder>().Sum);
        }

        [Test]
        public void TestDependencyToSameType()
        {
            var c = new Container();
            c.Register<A1>();
            c.Register<A2>();
            c.Register<RepoMock>();
            c.CreateInstance<A1>();
        }

        [Test]
        public void InjectionOnSubTypesIsworking()
        {
            var c = new Container();
            c.Register<A1>();
            c.Register<A2>();

            c.CreateInstance<A2>(new RepoMock());
        }

        [Test]
        public void LifcycleWorksAsExpetced()
        {
            var c = new SimpleFactory.Container();

            c.Register<Dep1>().Scoped();
            c.Register<Dep2>();
            c.Register<Dep3>();
            c.Register<Dep4>();
            c.Register<Dep5>();

            var d5 = c.CreateInstance<Dep5>();

            Assert.AreNotEqual(d5.dep3.dep2, d5.dep4.dep2);
            Assert.AreEqual(d5.dep3.dep2.dep1, d5.dep4.dep2.dep1);
        }

        [Test]
        public void LifcycleWorksAsExpetcedOnManyLevels()
        {
            var c = new SimpleFactory.Container();

            c.Register<Dep1>(() => new Dep1()).Scoped();
            c.Register<Dep2>().Scoped();
            c.Register<Dep3>().Scoped();
            c.Register<Dep4>();
            c.Register<Dep5>();

            var d5 = c.CreateInstance<Dep5>();

            Assert.AreEqual(d5.dep3.dep2, d5.dep4.dep2);
            Assert.AreEqual(d5.dep3.dep2.dep1, d5.dep4.dep2.dep1);
        }

        [Test]
        public void SingletonIsReallySingleton()
        {
            var c = new SimpleFactory.Container();

            c.Register<Dep1>().Singleton();
            c.Register<Dep2>();
            c.Register<Dep3>();
            c.Register<Dep4>();
            c.Register<Dep5>();

            var d5 = c.CreateInstance<Dep5>();

            var d1 = c.CreateInstance<Dep1>();
            var d1_2 = c.CreateInstance<Dep1>();
            //Assert.AreEqual(d5.dep3.dep2, d5.dep4.dep2);
            Assert.AreEqual(d5.dep3.dep2.dep1, d5.dep4.dep2.dep1);
            Assert.AreEqual(d1, d1_2);
        }

        [Test]
        public void Asd()
        {
            var c = new SimpleFactory.Container();

            c.Register<Dep1>();
            c.Register<Dep2>().Scoped();
            c.Register<Dep3>();
            c.Register<Dep4, Dep2, Dep3>((d2, d3) => new Dep4(d2));
            c.Register<Dep5>();

            var d5 = c.CreateInstance<Dep5>();

            Assert.AreEqual(d5.dep4.dep2, d5.dep3.dep2);
        }

        [Test]
        public void SetDefaultLifeTime()
        {
            var c = new SimpleFactory.Container(LifeTimeEnum.Scoped);

            c.Register<Dep1>();
            c.Register<Dep2>();
            c.Register<Dep3>();
            c.Register<Dep4, Dep2, Dep3>(
                (d2, d3) => new Dep4(d2)
            );
            c.Register<Dep5>();
            var d5 = c.CreateInstance<Dep5>();

            Assert.AreEqual(d5.dep4.dep2, d5.dep3.dep2);
        }

        [Test]
        public void PerGraphLifetimeWorksWithFactoryMethod()
        {
            var c = new SimpleFactory.Container(LifeTimeEnum.Scoped);

            c.Register<Dep1>(() => new Dep1());
            c.Register<Dep2>();

            var res = c.CreateInstance<Dep2>();

            Assert.IsNotNull(res.dep1);
        }

        [Test]
        public void UsingStaticMethodAsFactoryDoesNotThrow()
        {
            var container = new Container();

            container.Register<Dep1>(FactoryHolder.Factory);

            var dep = container.CreateInstance<Dep1>();

            Assert.IsNotNull(dep);
        }

        [Test]
        public void SendInParametrThatIsnotInUser()
        {
            var c = new Container();

            c.Register<Dummy>();
            c.Register<DummyNotInuse>();
            c.Register<Dep1>();

            var dummy = c.CreateInstance<Dummy>(new DummyNotInuse());

            Assert.IsInstanceOf<Dummy>(dummy);
        }

        [Test]
        public void PerGraphLifetimeWorksWithFactoryMethodAndTypeReg()
        {
            var c = new SimpleFactory.Container(LifeTimeEnum.Scoped);

            c.Register(typeof(Dep1), () => new Dep1());
            c.Register<Dep2>();

            var res = c.CreateInstance<Dep2>();

            Assert.IsNotNull(res.dep1);
        }


        [Test] public void ParameterFromFactoryDoesNotThrow()
        {
            var c = new SimpleFactory.Container(LifeTimeEnum.Transient);

            c.Register(typeof(Dep1), () => new Dep1());
            c.Register<Dummy>();


            Assert.DoesNotThrow(() => c.CreateInstance<Dummy>());

        }

     


    }

    public class Dummy
    {
        public Dummy(Dep1 dep)
        {
        }
    }

    public class DummyNotInuse
    {
    }

    public static class FactoryHolder
    {
        public static Dep1 Factory()
        {
            return new Dep1();
        }
    }

    public class Dep1
    {
        public int Value;
    }

    public class Dep2
    {
        public readonly Dep1 dep1;

        public Dep2(Dep1 dep1)
        {
            this.dep1 = dep1;
        }
    }

    public class Dep3
    {
        public readonly Dep2 dep2;

        public Dep3(Dep2 dep2)
        {
            this.dep2 = dep2;
        }
    }

    public class Dep4
    {
        public readonly Dep2 dep2;

        public Dep4(Dep2 dep2)
        {
            this.dep2 = dep2;
        }
    }

    public class Dep5
    {
        public readonly Dep4 dep4;
        public readonly Dep3 dep3;

        public Dep5(Dep3 dep3, Dep4 dep4)
        {
            this.dep3 = dep3;
            this.dep4 = dep4;
        }
    }

    public class RepoMock
    {
    }

    public class A1
    {
        private readonly RepoMock r;
        private readonly A2 a2;

        public A1(RepoMock r, A2 a2)
        {
            this.a2 = a2;
            this.r = r;
        }
    }

    public class A2
    {
        private readonly RepoMock r;

        public A2(RepoMock r)
        {
            this.r = r;
        }
    }

    public class Adder
    {
        public void Add()
        {
            Sum += 1;
        }

        public int Sum;
    }

    public class Result
    {
        public int Value = 1;
    }

    public class Param
    {
    }

    public interface ISomeService
    {
        Result Handle(Param p);
    }

    public class SomeService : ISomeService
    {
        private readonly SomeDep dep;

        public SomeService(SomeDep dep)
        {
            this.dep = dep;
        }

        public Result Handle(Param p)
        {
            return new Result { Value = dep.SomeStrangeValue };
        }
    }

    public class SomeDep
    {
        public int SomeStrangeValue;

        public SomeDep()
        {
            SomeStrangeValue = 100;
        }
    }

    public class Wrapper
    {
        private Container di;

        public Wrapper()
        {
            this.di = new Container();
        }

        public void Register<TInt, TInst>() where TInst : TInt
        {
            di.Register<TInt, TInst>();
        }

        public void Register<T>(Func<object, T> func)
        {
            di.Register<T, object>(e => func(e));
        }

        public T Create<T>(object[] provided)
        {
            return di.CreateInstance<T>(provided.ToDictionary(e => e.GetType()));
        }
    }
}