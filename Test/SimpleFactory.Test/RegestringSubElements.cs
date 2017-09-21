using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleFactory.Test
{

 [TestFixture]
    class RegestringSubElements
    {
        
        [Test] public void CircularDependenciesIsDetected()
        {
            var container = new SimpleFactory.Container();
            container.Register<Icor, Level1>();

            Assert.Throws<SimpleFactory.Exceptions.CircularDependencyDetected>(()=> container.CreateInstance<Icor>());
            
            //Assert.IsAssignableFrom<Level2>(((Level1)res).next);

        }

        [Test]
        public void UseSubContainerToRegisterDependency()
        {
           //Abandoned            
           //Use more specific interfaces instead.. 
        }
        

        public interface Icor
        {

        }

        public interface ICorTimer : Icor
        {

        }

        public class Level2 : Icor
        {

        }

        public class Level1 : ICorTimer
        {
            public readonly Icor next;

            public Level1(Icor next)
            {
                this.next = next;
            }
        }
    }


}
