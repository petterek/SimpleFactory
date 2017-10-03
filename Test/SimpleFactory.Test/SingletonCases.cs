using NUnit.Framework;

namespace SimpleFactory.Test
{
    [TestFixture]
    public class SingletonCases
    {
        [Test]
        public void SingletonCreatedFromLambda()
        {
            var c = new Container();
            c.Register<AdderWithId>(() => new AdderWithId(1337)).AsSingleton();

            c.CreateInstance<AdderWithId>().Add();
            c.CreateInstance<AdderWithId>().Add();

            Assert.AreEqual(2, c.CreateInstance<AdderWithId>().Sum);
            Assert.AreEqual(1337+2, c.CreateInstance<AdderWithId>().Id);
        }

        [Test]
        public void NonSingletonCreatedFromLambda()
        {
            var c = new Container();
            c.Register<AdderWithId>(() => new AdderWithId(1337));

            c.CreateInstance<AdderWithId>().Add();
            c.CreateInstance<AdderWithId>().Add();

            Assert.AreNotEqual(2, c.CreateInstance<AdderWithId>().Sum);
            Assert.AreEqual(1337 + 0, c.CreateInstance<AdderWithId>().Id);
        }
    }
    public class AdderWithId
    {
        public int Id { get; set; }

        public AdderWithId(int id)
        {
            Id = id;
        }
        public void Add()
        {
            Sum += 1;
            Id += 1;
        }
        public int Sum;
    }
}
