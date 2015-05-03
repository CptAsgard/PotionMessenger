#define MESSAGEBUS_THROWEXCEPTIONS // Comment this if you'd like to recieve no exceptions from the messagebus.

#if MESSAGEBUS_THROWEXCEPTIONS
#define MESSAGEBUS_REQUIRELISTENER // Comment this if you don't want listeners to be required for sending messages.
#endif

using System;
using System.Collections;
using System.Collections.Generic;

/**
 * @author      Frank van Wattingen <f.vanwattingen@gmail.com>
 * @version     1.0
 * @since       02-05-2015
 * 
 * For usage examples please see: https://github.com/CptAsgard/PotionMessenger
 */
public class MessageBus
{
    private Dictionary<Type, List<MessageReceiverBase>> subscribers;

    public MessageBus()
    {
        subscribers = new Dictionary<Type, List<MessageReceiverBase>>();
    }

    public void AddSubscriber<Message>( MessageReceiver<Message> receiver ) where Message : struct
    {
        Type type = typeof( Message );

        if( !subscribers.ContainsKey( type ) )
            subscribers[type] = new List<MessageReceiverBase>();

        if( !SubscriptionExists( type, receiver ) )
            subscribers[type].Add( receiver );
    }

    public void RemoveSubscriber<Message>( MessageReceiver<Message> receiver ) where Message : struct
    {
        Type type = typeof( Message );

        if( !subscribers.ContainsKey( type ) )
        {
#if MESSAGEBUS_THROWEXCEPTIONS
            throw new ArgumentException( string.Format( "Removing subscriber, but the message type \"{0}\" isn't registered." ), type.ToString() );
#else
                return;
#endif
        }

        var list = subscribers[type];
        bool existed = false;

        foreach( var s in list )
        {
            if( s == receiver )
            {
                list.Remove( s );
                existed = true;

                if( list.Count == 0 )
                    subscribers.Remove( type );

                return;
            }
        }

#if MESSAGEBUS_THROWEXCEPTIONS
        if( !existed )
        {
            throw new ArgumentException( string.Format( "Removing subscriber, but the supplied receiver isn't subscribed to message type \"{0}\".", type.ToString() ) );
        }
#endif
    }

    public void Route<Message>( Message mess ) where Message : struct
    {
        List<MessageReceiverBase> list;
        if( !subscribers.TryGetValue( typeof( Message ), out list ) )
#if MESSAGEBUS_REQUIRELISTENER
            throw new ArgumentException( string.Format( "Attempting to send message of type \"{0}\", but no listener for this type has been found. Make sure this.Subscribe<{0}>(MessageBus) has been called, or that all listeners to this message haven't been unsubscribed.", typeof( Message ).ToString() ) );
#else
                return;
#endif

        foreach( MessageReceiverBase b in list )
        {
            ( b as MessageReceiver<Message> ).HandleMessage( mess );
        }
    }

    private bool SubscriptionExists( Type type, MessageReceiverBase receiver )
    {
        List<MessageReceiverBase> receivers;

        if( !subscribers.TryGetValue( type, out receivers ) ) return false;

        bool exists = false;

        foreach( var sub in receivers )
        {
            if( sub == receiver )
            {
                exists = true;
                break;
            }
        }

        return exists;
    }
}

public static class MsgRegist
{
    public delegate void Del<T>( T msg );

    public static void Subscribe<MessageType>( this MessageReceiver<MessageType> caller, MessageBus bus ) where MessageType : struct
    {
        bus.AddSubscriber<MessageType>( caller );
    }

    public static void Unsubscribe<MessageType>( this MessageReceiver<MessageType> caller, MessageBus bus ) where MessageType : struct
    {
        bus.RemoveSubscriber<MessageType>( caller );
    }
}

public interface MessageReceiverBase { };

public interface MessageReceiver<T> : MessageReceiverBase
{
    void HandleMessage( T message );
}
