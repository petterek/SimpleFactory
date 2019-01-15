using NUnit.Framework;
using SimpleFactory.Contract;
using System;

namespace SimpleFactory.Test.Scope
{
    [TestFixture]
    public class TestServiceProvider
    {

        [Test]
        public void RegisterAndCreateTypeThroughInterface()
        {
            IServiceRegistry c = new SimpleFactory.Container();

            c.Register<ITestScope, ImplementTestScope>();

            var sp = new SimplefactoryProvider(c);

            var instance = sp.GetService(typeof(ITestScope));

            Assert.IsInstanceOf<ITestScope>(instance);

        }

        [Test]
        public void RegisterAndCreateType()
        {
            IServiceRegistry c = new SimpleFactory.Container();

            c.Register<ImplementTestScope>();

            var sp = new SimplefactoryProvider(c);

            var instance = sp.GetService(typeof(ImplementTestScope));

            Assert.IsInstanceOf<ITestScope>(instance);

        }



        [Test]
        public void CheckScoped()
        {
            IServiceRegistry c = new SimpleFactory.Container();

            c.Register<ShouldBeScoped>().Scoped();

            ShouldBeScoped instance;
            using (var sp = new SimplefactoryProvider(c))
            {
                instance = sp.GetService<ShouldBeScoped>();
                Assert.IsInstanceOf<ShouldBeScoped>(instance);

                instance.value = 10;

                Assert.AreEqual(10, sp.GetService<ShouldBeScoped>().value);
            }

            Assert.IsTrue(instance.IsDisposed);


        }

        [Test]
        public void CircularDependenciesThrowsException()
        {
            IServiceRegistry c = new SimpleFactory.Container();

            c.Register<CircularDependency>();

            using (var sp = new SimplefactoryProvider(c))
            {
                Assert.Throws<Exceptions.CircularDependencyDetected>(() => sp.GetService<CircularDependency>());
            }
        }

        [Test]
        public void RegisteringAndCreatingThroughFactoryWorks()
        {
            IServiceRegistry c = new SimpleFactory.Container();

            c.Register<Level1>(() => new Level1());

            using (var sp = new SimplefactoryProvider(c))
            {
                Assert.IsNotNull(sp.CreateInstance<Level1>());
            }
        }

        [Test]
        public void FactoryWithParams()
        {
            IServiceRegistry c = new SimpleFactory.Container();

            c.Register<Level2>();
            c.Register<Level1,Level2>((lvl2) => new Level1());


            using (var sp = new SimplefactoryProvider(c))
            {
                Assert.IsNotNull(sp.CreateInstance<Level1>());
            }
        }

        [Test]
        public void SingletonIsPreservedBetweenScopes()
        {
            IServiceRegistry c = new SimpleFactory.Container();

            c.Register<IAmSingleton>().Singleton();
            c.Register<Level1>();

            using (var sp = new SimplefactoryProvider(c))
            {
                var instance = sp.CreateInstance<IAmSingleton>();
                instance.Value = 10;
            }

            using (var sp = new SimplefactoryProvider(c))
            {
                Assert.AreEqual(10,sp.CreateInstance<IAmSingleton>().Value);
            }
        }


        [Test]
        public void ScopeDemo()
        {
            IServiceRegistry c = new SimpleFactory.Container();

            
            c.Register<Level2>().Scoped();
            c.Register<NeedsLevel2>().Scoped();

            var main = new SimplefactoryProvider(c);
            var in2 = main.GetService<Level2>();

            in2.Value = 10;

            using (var sp = new SimplefactoryProvider(c))
            {
                var instance = sp.GetService<NeedsLevel2>();
                instance.level2.Value = 20;

                Assert.AreEqual(10, in2.Value);
            }
                        
        }





    }
    
    public class IAmSingleton
    {
        public int Value = 0;
    }

    public class Level1
    {

    }

    public class Level2
    {
        public int Value = 0;
    }

    public class CircularDependency
    {
        public CircularDependency(CircularDependency circular)
        {

        }

    }
    public class NeedsLevel2
    {
        public readonly Level2 level2;

        public NeedsLevel2(Level2 level2)
        {
            this.level2 = level2;
        }
    }


    public class ShouldBeScoped : IDisposable
    {
        public int value = 0;

        public bool IsDisposed => disposedValue;


        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls



        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~ShouldBeScoped() {
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

    public interface ITestScope
    {

    }

    public class ImplementTestScope : ITestScope
    {

    }

}