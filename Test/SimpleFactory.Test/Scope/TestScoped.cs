using NUnit.Framework;
using SimpleFactory.Contract;
using System;

namespace SimpleFactory.Test.Scope
{
    [TestFixture]
    public class TestScoped
    {

        [Test] public void UseRegistry ()
        {
            Container registry = new SimpleFactory.Container();

            registry.Register<NeedsMyClass>();
            registry.Register<MyClass>();


            //IServiceCreator creator = new SimplefactoryProvider(registry);


            //var a = creator.CreateInstance<NeedsMyClass>();




        }

    }

    public class MyClass
    {

    }

    public class NeedsMyClass : IDisposable
    {
        readonly MyClass dep;

        public NeedsMyClass(MyClass dep)
        {
            this.dep = dep;
        }

        public void Dispose()
        {
            
        }
    }

}