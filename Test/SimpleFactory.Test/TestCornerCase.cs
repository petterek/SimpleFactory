using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleFactory.Test
{
    [TestFixture]public class TestCornerCase
    {

        [Test] public void CorneCase1 ()
        {
            var w = new Wrapper();
            var ret = new SomeDep();

            w.Register<ISomeService, SomeService>();
            w.Register<SomeDep>(e => ret);

            var res = w.Create<ISomeService>(new object[] { new object()});

            Assert.IsNotNull(res);
        }

        [Test] public void RegisterSingletonIsWorking()
        {
            var c = new Container();
            c.Register<Adder>().AsSingleton();
            c.CreateInstance<Adder>().Add();
            c.CreateInstance<Adder>().Add();

            Assert.AreEqual(2, c.CreateInstance<Adder>().Sum);

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
        readonly SomeDep dep;

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

        public void Register<TInt, TInst>() where TInst:TInt
        {
            di.Register<TInt, TInst>();
        }
        public void Register<T>(Func<object,T> func)
        {
            di.Register<T,object>(e => func(e));
        }

        public T Create<T>(object[] provided)
        {
            return di.CreateInstance<T>(provided.ToDictionary(e => e.GetType()));
        }

    }



}
