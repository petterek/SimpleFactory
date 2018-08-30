using NUnit.Framework;
using SimpleFactory.Contract;
using System;

namespace SimpleFactory.Test
{
    [TestFixture]
    internal class ClassFactoryDemo
    {
        [Test]
        public void BasicUse()
        {
            var container = new SimpleFactory.Container();
            container.Register<Class1>();

            Assert.IsNotNull(container.CreateInstance<Class1>());
        }

        [Test]
        public void InjectBasicWithOutRegistrationThrowsError()
        {
            var container = new Container();
            container.Register<Class2>();
            Assert.Throws<SimpleFactory.Exceptions.MissingRegistrationException>(
                () => container.CreateInstance<Class2>()
                );
        }

      

        [Test]
        public void InjectBasicWithRegistration()
        {
            var container = new Container();
            container.Register<Class1>();
            container.Register<Class2>();
            Assert.DoesNotThrow(() => container.CreateInstance<Class2>());
        }

        [Test]
        public void RegisterInterfaceForClass()
        {
            var container = new Container();
            container.Register<IClass2, Class2>();
            container.Register<Class1>();

            IClass2 inst = container.CreateInstance<IClass2>();
            Assert.IsAssignableFrom<Class2>(inst);
        }

        [Test]
        public void RegisterInterfaceForClassForInjection()
        {
            var container = new Container();
            container.Register<IClass2, Class2>();
            container.Register<Class1>();
            container.Register<Class3>();

            Assert.DoesNotThrow(() => container.CreateInstance<Class3>());
        }

        [Test]
        public void RegisteringTypeWithFactoryMethod()
        {
            IContainer container = new Container();

            container.Register<IClass2, Class2>();
            container.Register<Class1>(() => new Class1() { CheckThisValue = 10 });
            container.Register<Class3>();

            Class3 inst = null;
            Assert.DoesNotThrow(() => inst = container.CreateInstance<Class3>());

            Assert.AreEqual(10, ((Class1)inst.dep1).CheckThisValue);
        }

        [Test]
        public void InjectingValuesTroughCrate()
        {
            var container = new Container();
            container.Register<ClassThatNeedsClass3>();

            ClassThatNeedsClass3 inst = null;
            Assert.Throws<Exceptions.MissingRegistrationException>(() => inst = container.CreateInstance<ClassThatNeedsClass3>());

            Assert.DoesNotThrow(() =>  container.CreateInstance<ClassThatNeedsClass3>(new Class3(new Class1(), new Class2(new Class1()))));
            Assert.DoesNotThrow(() =>  container.CreateInstance<Class3>(new Class2(new Class1()), new Class1()));
        }

        [Test]
        public void RegestringNonCompatiableTypeThrowsException()
        {
            var container = new Container();

            Assert.Throws<ArgumentException>(() => container.Register<IClass1>(typeof(object)));
            Assert.DoesNotThrow(() => container.Register<object>(typeof(object)));
        }

        [Test]
        public void IsRegiteredReturnsTrue()
        {
            var container = new Container();
            container.Register<IClass1, Class1>();

            Assert.IsFalse(container.IsRegistered<IClass2>());
            Assert.IsTrue(container.IsRegistered<IClass1>());
        }

        public interface IClass1 { }

        public interface IClass2 { }

        public class Class1 : IClass1
        {
            public int CheckThisValue;

            public Class1()
            {
            }
        }

        public class Class2 : IClass2
        {
            private readonly Class1 inject;

            public Class2(Class1 inject)
            {
                this.inject = inject;
            }
        }

        public class Class3
        {
            public readonly IClass1 dep1;
            public readonly IClass2 dep2;

            public Class3(Class1 dep1, IClass2 dep2)
            {
                this.dep2 = dep2;
                this.dep1 = dep1;
            }
        }

        public class ClassThatNeedsClass3
        {
            private readonly Class3 dep;

            public ClassThatNeedsClass3(Class3 dep)
            {
                this.dep = dep;
            }
        }
    }
}