# SimpleFactory
A very simple POC to make a DI container. 
It is fully working and actually in production, so you can use it if you want to :D

Codebase is not to large, so reading through it is not a monumental task, and understanding it is also doable I hope. 

```csharp

Container c = new SimpleFactory.Container();

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


```
