**Attention!** This is the result of my experiments with C# delegates, if you plan to use it - do it carefully!

## Prolog
As mentioned above, when creating an application in Unity3d, I came across the fact that it is necessary to transfer data from one object to another.
Moreover, this was complicated by the use of multitsen. My project involves creating a lot of scenes and I really did not want to manually implement dependencies.
Methods like `FindObjectsOfType` were also not suitable for me, as they take up too many resources to search for objects. I wanted to simplify data transfer between scene objects as much as possible without compromising performance.

As I delved deeper into the various architectural patterns, I came across **ECS** and decided that I had found a silver bullet that would kill all my monsters. After trying out the most popular frameworks in this area, I chose [Actors](https://github.com/PixeyeHQ/actors.unity). Its authors are very friendly guys, they even allowed me to make some changes regarding the loading of scenes from **AssetBundle**. 

But as is often the case with frameworks, it provided me with huge opportunities, most of which I did not intend to use inside my project. Basically I used a structure called **Signals**. The author himself described it something like this: _"it sends a message literally into the void, and if there is a recipient for it, it will receive it in any part of the application"_. This more than covered the entire range of my tasks to ensure interaction between objects in scenes. Having realized what I really needed, I began to experiment with the entities known to me at that time in C#.

## Design
The block diagram shows the simplest way to transmit a event from a sender to a receiver:

![Structural diagram](https://mofrison.ru/assets/images/posts/2021-03-17-csharp-global-events/structural-diagram.gif)

The **handler** subscribes to a certain type of **event**. **Generator** creates an event, passes data to it, and sends it to all subscribed handlers. The handler receives this event, extracts data from it, and processes it. The handler can break the connection by unsubscribing from the event. Thus, the generator and the handler may not know anything about each other, and the event serves as an interface for passing data between them.

## Implementation
I have created a base class for all events that accepts derived classes as a type. Inside it, a static variable is encapsulated in the form of a delegate that stores methods that accept an object derived from the `GlobalEvent<T>` type as a prameter. This variable is used to implement the event subscription mechanism. It defines the following methods:
Method name                     | Implemented function
--------------------------------|-------------------------
[Contains](#contains)			| Checks if this handler is subscribed to this event.
[Subscribe](#subscribe)			| The method for adding handlers.
[Unsubscribe](#unsubscribe)		| The method for removes handler.
[Handle](#handle)				| Protected method for handling global event.
[AddHendler](#addhendler)		| Private method to add handlers if not already added.
[RemoveHendler](#removehendler)	| Private method for remove a method if it has been added.

> **Attention!** *With great power comes great responsibility:* since method references are stored in a static variable, make sure that the objects are unsubscribed from the event in a timely manner, otherwise the garbage collector will not be able to delete them and there will be a memory leak.

Even though the variable is declared in the base class, a delegate is created for each derived class. Thanks to this, the recipient receives only messages of the type to which he subscribed. The `Subscribe(Hendler hendlers)` method is used to add subscriber delegates, and `Unsubscribe(Hendler hendlers)` - to delete them.

In addition, when subscribing to an event, the handler is compared with previously subscribed ones. If this delegate has been previously signed an exception is thrown.

### Contains
Checks if this handler is subscribed to this event. It takes an event handler as a parameter. Returns `true` if the handler was already subscribed to this event.
```csharp
private static bool Contains(Hendler hendler)
{
    return hashs.Contains(hendler.GetHashCode());
}
```

### Subscribe
The method for adding handlers. Takes as one parameter a reference to a method or a delegate with one parameter derived from GlobalEvent<T>.
```csharp	
public static void Subscribe(Hendler hendler)
{
    for (int i = 0; i < hendler.GetInvocationList().Length; i++)
    {
        AddHendler((Hendler)(hendler.GetInvocationList()[i]));
    }
}
```

### Unsubscribe
The method for removes handler. Takes as one parameter a reference to a method or a delegate with one parameter derived from GlobalEvent<T>.
```csharp
public static void Unsubscribe(Hendler hendler)
{
    for (int i = 0; i < hendler.GetInvocationList().Length; i++)
    {
        RemoveHendler((Hendler)(hendler.GetInvocationList()[i]));
    }
}
```

### Handle
Protected method for handling global event. Takes as one parameter a reference to event.
```csharp
        protected static void Handle(T globalEvent)
        {
            try
            {
                hendlers.Invoke(globalEvent);
            }
            catch (System.NullReferenceException e)
            {
                throw new Exception(e.Message);
            }
        }
```

### AddHendler
Private method for adding handlers if it hasn't already been added. Takes as one parameter a reference to a method or a delegate with one parameter derived from GlobalEvent<T>.
```csharp
private static void AddHendler(Hendler hendler)
{
    var hash = hendler.GetHashCode();
    if (!hashs.Contains(hash))
    {
        hendlers += hendler;
        hashs.Add(hash);
    }
    else throw new Exception($"The {hendler.Method} has already been added in {typeof(T)} hendlers");
}
```

### RemoveHendler
Private method for Remove a method if it has been added. Takes as one parameter a reference to a method or a delegate with one parameter derived from GlobalEvent<T>.
```csharp
private static void RemoveHendler(Hendler hendler)
{
    var hash = hendler.GetHashCode();
    if (hashs.Contains(hash))
    {
        hendlers -= hendler;
        hashs.Remove(hash);
    }
    else throw new Exception($"The {hendler.Method} has not been added in {typeof(T)} hendlers");
}
```

### Simple event example
To implement an event, it is enough to inherit from the `GlobalEvent<T>` type, passing the type of the derived class into it, and add a method to fire this event:
```csharp
public class Message : GlobalEvent<Message>
{
    public string Text { get; }

    private Message(string text) => Text = text;

	public static void Send(string text) => Handle(new Message(text));
}
```
In this implementation, the `Message` event encapsulates a string of text that it will receive when it is created and then send to the recipients.
To send a message, use the static `Send(string text)` method. All that is required of him is to create an instance of the `Message` class and call the `Handle(Message message)` method, passing this object as a parameter.

### Sending and handling an event
In order to handle the above event, you need to implement a method that takes this event as a parameter:
```csharp
private void Receive(Message message)
{
    System.Console.WriteLine(message.Text);
}
```
Let's subscribe this method to the event:
```csharp
Message.Subscribe(Receive);
```
After that, to send the above message, a simple line is enough:
```csharp
Message.Send("Hello World!");
```
Don't forget to unsubscribe as soon as the receipt of this event is no longer relevant for us:
```csharp
Message.Unsubscribe(Receive);
```

## Conclusion
You can also use global events in your project by adding [this repository](https://github.com/mofrison/CSharp-GlobalEvent) as a [submodule](https://git-scm.com/book/en/v2/Git-Tools-Submodules):

	git submodule add https://github.com/mofrison/CSharp-GlobalEvent

The main advantage of this approach is that I can create an event in one part of the program and process it in another, without direct interaction between the handler and the event emitter.

But there are also many disadvantages:
* The use of static variables with all the "leaking" consequences
* Global, public access to all events does not contribute to security
* The need to control the lifetime of handlers

Therefore, it is important that the handler subscribes to the event before its creation and is unsubscribed as soon as it is no longer needed or it becomes disable, and the data passed to the event when it is created is immutable.

In any case, it is up to you to use this method or not. Thanks for your attention :)


