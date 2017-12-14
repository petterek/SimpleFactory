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
