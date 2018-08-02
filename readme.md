
# SimpleFactory
Self developed DI container. 

Has all the major features you need for DI. Can be used when developing thins outside of asp .net. 

It has a contract, that you should reference, use the contract in your project not the implementation, that way u will not have the to update the
nuget packages when features is added or bugs is fixed. 

## Usage

### Basic
```
IContainer container = new SimpleFactory.Container();
container.Register<Class1>();
            
Assert.IsNotNull(container.CreateInstance<Class1>());

```

### With factory
```
IContainer container = new Container();
            
container.Register<IClass2, Class2>();
container.Register<Class1>(()=>  new Class1() { CheckThisValue=10}); //This is a factory/delegate
container.Register<Class3>();

Class3 inst = null;

Assert.DoesNotThrow(() => inst = container.CreateInstance<Class3>());
                        
Assert.AreEqual(10, ((Class1)inst.dep1).CheckThisValue);
```
