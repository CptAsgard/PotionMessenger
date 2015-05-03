# PotionMessenger
C# compile-time safe messaging/event system. 
The code is under the Unlicense, so feel free to use however you like without requiring credit!

# How To Use

You can listen to a single message in the following manner.
```C#
public struct SomeMessage
{
    public int num;
}

public class Example : MessageReceiver<SomeMessage>
{
    public Example( MessageBus msgbus )
    {
        // Listen to messages of type
        this.Subscribe<SomeMessage>( msgbus );
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
    public Example( MessageBus msgbus )
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
        Console.WriteLine( msg.num );
    }
}
```

To send a message all you have to do is:

```C#
// Send messages of type
SomeMessage msg = new SomeMessage();
msg.num = 5;

msgbus.Route( msg );
```

#Compile time safety vs runtime exceptions

The strength of this messenger is in its compile-time safety. Compile-time safety means that errors in your code are raised when you're compiling the code, not when the code is running. This is the result of using messages as structs. Having messages be structs provides multiple upsides.

First of all, an object is not only the key to trigger a message, but also contains the data for the message. You don't have to seperate the message and the data. The message *is* the data.

Second of all, because an object has a struct name, there's no chance you'll listen to a message that doesn't exist. In traditional messaging solutions, you register a callback to a string. However, it's extremely easy to make a typo in a string.

#Clean-up

If your object is done listening to a certain type of message, or the object is ready to do clean-up, make sure to call RemoveSubscriber()! If you don't, this can cause memory leaks.

In Unity, this means calling Unsubscribe in OnDestroy()

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
