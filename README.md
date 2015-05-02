# PotionMessenger
C# compile-time safe messaging/event system.

# How To Use
```C#
public struct SomeMessage
{
  public int num;
}

public class Example : MessageReceiver<SomeMessage>
{
  public void Func()
  {
    MessageBus msgbus = new MessageBus();
      
    // Listen to messages of type
    this.Subscribe<SomeMessage>( msgbus );
      
    // Send messages of type
    SomeMessage msg = new SomeMessage();
    msg.num = 5;

    msgbus.Route( msg );
  }

  public void HandleMessage( SomeMessage  )
  {
    Console.WriteLine( t.num );
  }
}
```
