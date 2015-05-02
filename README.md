# PotionMessenger
C# compile-time safe messaging/event system.

# How To Use

You can listen to a single message in the following manner.
```C#
public struct SomeMessage
{
    public int num;
}

public class Example : MessageReceiver<SomeMessage>
{
    public Example()
    {
        MessageBus msgbus = new MessageBus();
      
        // Listen to messages of type
        this.Subscribe<SomeMessage>( msgbus );
      
        // Send messages of type
        SomeMessage msg = new SomeMessage();
        msg.num = 5;

        msgbus.Route( msg );
    }

    public void HandleMessage( SomeMessage msg )
    {
        Console.WriteLine( msg.num );
    }
}
```

If you're using Unity, you should use Awake() instead of the constructor to start listening to messages.

If you'd like to listen to multiple messages, you should implement the MessageReceiver interface multiple times. Once for each message type. 

```C#
public struct SomeMessage
{
    public int num;
}

public struct AnotherMessage
{
    public string s;
}

public class Example : MessageReceiver<SomeMessage>, MessageReceiver<AnotherMessage>
{
    public void Init( MessageBus msgbus )
    {
        // Listen to messages of type
        this.Subscribe<SomeMessage>( msgbus );
        this.Subscribe<AnotherMessage>( msgbus );
    }

    public void HandleMessage( SomeMessage msg )
    {
        Console.WriteLine( msg.num );
    }
  
    public void HandleMessage( AnotherMessage msg )
    {
        Console.WriteLine( s.num );
    }
}
```

#Compile time vs runtime safety

The strength of this implementation is in its compile-time safety. Having messages be structs provides multiple upsides.

First of all, an object is not only the key to trigger a message, but also contains the data for the message. You don't have to seperate the message and the data. The message *is* the data.

Second of all, because an object has a struct name, there's no chance you'll listen to a message that doesn't exist. In traditional messaging solutions, you register a callback to a string. However, it's extremely easy to make a typo in a string.

#Clean-up

If your object is done listening to a certain type of message, or the object is ready to do clean-up, make sure to call RemoveSubscriber()! If you don't, this can cause memory leaks.

In Unity, this means calling RemoveSubscriber() in OnDestroy()

### Generic C# Example

``` C#
~Example()
{
    this.Unsubscribe<TestMessage>( msgbus );
}
```

### Unity Example

``` C#
void OnDisable()
{
    this.Unsubscribe<TestMessage>( msgbus );
}
```
