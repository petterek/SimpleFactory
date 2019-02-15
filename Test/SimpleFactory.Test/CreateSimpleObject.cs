using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace SimpleFactory.Test
{
    [TestFixture]
    public class CreateSimpleObject
    {
        [Test]
        public void CreateObjectWitEmptyConstructor()
        {
            var container = new Container();
            container.Register<SimpleOne>();

            var test = container.CreateInstance<SimpleOne>();

            Assert.IsInstanceOf<SimpleOne>(test);
        }

        [Test]
        public void CreatObjectWithParamInConstructor()
        {
            var container = new Container();

            container.Register<SimpleOne>();
            container.Register<NotSoSimple>();

            var test = container.CreateInstance<NotSoSimple>();

            Assert.IsInstanceOf<NotSoSimple>(test);
            Assert.AreEqual(1, test.value);
        }

        [Test]
        public void CreatObjectWithParamInConstructorAndInjected()
        {
            var container = new Container();

            container.Register<SimpleOne>();
            container.Register<NotSoSimple>();
            container.Register<WithInjected>();

            var test = container.CreateInstance<WithInjected>(new Dictionary<Type, object> { { typeof(NotSoSimple), new NotSoSimple(new SimpleOne()) { value = 2 } } });

            Assert.IsInstanceOf<WithInjected>(test);
            Assert.AreEqual(2, test.p.value);

            var test2 = container.CreateInstance<WithInjected>(new Dictionary<Type, object> { { typeof(NotSoSimple), new NotSoSimple(new SimpleOne()) { value = 4 } } });
            Assert.AreEqual(4, test2.p.value);
        }

        [Test]
        public void CreateFromInterface()
        {
            var container = new Container();
            container.Register<ISimpleInterface, SimpleOne>();
            var test = container.CreateInstance<ISimpleInterface>();
            Assert.IsInstanceOf<SimpleOne>(test);
        }

        [Test]
        public void RegisterWithFactory()
        {
            var container = new Container();
            container.Register<ISimpleInterface>(() => new SimpleOne());
            var test = container.CreateInstance<ISimpleInterface>();
            Assert.IsInstanceOf<SimpleOne>(test);
        }

        
        [Test]
        public void GetInjectionWhenSuperTypeIsRegisterd()
        {
            var container = new Container();
            container.Register<WithSuperClassNeeds>();

            Assert.DoesNotThrow(() => container.CreateInstance<WithSuperClassNeeds>(new SimpleTwo()));
        }
    }

    public class ClassWithFields
    {
        public ISimpleInterface Simple;
    }

    public interface ISimpleInterface
    { }

    public class SimpleOne : ISimpleInterface
    { }

    public class NotSoSimple
    {
        private readonly SimpleOne p;

        public int value = 1;

        public NotSoSimple(SimpleOne p)
        {
            this.p = p;
        }
    }

    public class WithInjected
    {
        public readonly NotSoSimple p;

        public WithInjected(NotSoSimple p)
        {
            this.p = p;
        }
    }

    public class SimpleTwo : SimpleOne
    {
    }

    public class WithSuperClassNeeds
    {
        private readonly SimpleOne simple;

        public WithSuperClassNeeds(SimpleOne simple)
        {
            this.simple = simple;
        }
    }
}