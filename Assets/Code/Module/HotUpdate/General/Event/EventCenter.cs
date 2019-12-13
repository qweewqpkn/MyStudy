using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    static internal class EventCenter
    {
        static public Dictionary<int, Delegate> _eventTable = new Dictionary<int, Delegate>();

        static public List<int> _permanentMessages = new List<int>();


        static public void MarkAsPermanent(int eventType)
        {
            _permanentMessages.Add(eventType);
        }


        static public void Cleanup()
        {
            List<int> messagesToRemove = new List<int>();

            foreach (KeyValuePair<int, Delegate> pair in _eventTable)
            {
                bool wasFound = false;

                foreach (int message in _permanentMessages)
                {
                    if (pair.Key == message)
                    {
                        wasFound = true;
                        break;
                    }
                }

                if (!wasFound)
                    messagesToRemove.Add(pair.Key);
            }

            foreach (int message in messagesToRemove)
            {
                _eventTable.Remove(message);
            }
        }

        static public void OnListenerAdding(int eventType, Delegate listenerBeingAdded)
        {
            if (!_eventTable.ContainsKey(eventType))
            {
                _eventTable.Add(eventType, null);
            }

            Delegate d = _eventTable[eventType];
            if (d != null && d.GetType() != listenerBeingAdded.GetType())
            {
                throw new ListenerException(string.Format("Attempting to add listener with inconsistent signature for event type {0}. Current listeners have type {1} and listener being added has type {2}", eventType, d.GetType().Name, listenerBeingAdded.GetType().Name));
            }
        }

        static public void OnListenerRemoving(int eventType, Delegate listenerBeingRemoved)
        {
            if (_eventTable.ContainsKey(eventType))
            {
                Delegate d = _eventTable[eventType];

                if (d == null)
                {
                    throw new ListenerException(string.Format("Attempting to remove listener with for event type \"{0}\" but current listener is null.", eventType));
                }
                else if (d.GetType() != listenerBeingRemoved.GetType())
                {
                    throw new ListenerException(string.Format("Attempting to remove listener with inconsistent signature for event type {0}. Current listeners have type {1} and listener being removed has type {2}", eventType, d.GetType().Name, listenerBeingRemoved.GetType().Name));
                }
            }
            else
            {
                throw new ListenerException(string.Format("Attempting to remove listener for type \"{0}\" but Messenger doesn't know about this event type.", eventType));
            }
        }

        static public void OnListenerRemoved(int eventType)
        {
            if (_eventTable[eventType] == null)
            {
                _eventTable.Remove(eventType);
            }
        }

        static public BroadcastException CreateBroadcastSignatureException(int eventType)
        {
            return new BroadcastException(string.Format("Broadcasting message \"{0}\" but listeners have a different signature than the broadcaster.", eventType));
        }

        public class BroadcastException : Exception
        {
            public BroadcastException(string msg)
                : base(msg)
            {
            }
        }

        public class ListenerException : Exception
        {
            public ListenerException(string msg)
                : base(msg)
            {
            }
        }

        //No parameters
        static public void AddListener(int eventType, Callback handler)
        {
            OnListenerAdding(eventType, handler);
            _eventTable[eventType] = (Callback)_eventTable[eventType] + handler;
        }

        //Single parameter
        static public void AddListener<T>(int eventType, Callback<T> handler)
        {
            OnListenerAdding(eventType, handler);
            _eventTable[eventType] = (Callback<T>)_eventTable[eventType] + handler;
        }

        //Two parameters
        static public void AddListener<T, U>(int eventType, Callback<T, U> handler)
        {
            OnListenerAdding(eventType, handler);
            _eventTable[eventType] = (Callback<T, U>)_eventTable[eventType] + handler;
        }

        //Three parameters
        static public void AddListener<T, U, V>(int eventType, Callback<T, U, V> handler)
        {
            OnListenerAdding(eventType, handler);
            _eventTable[eventType] = (Callback<T, U, V>)_eventTable[eventType] + handler;
        }

        //Four parameters
        static public void AddListener<T, U, V, X>(int eventType, Callback<T, U, V, X> handler)
        {
            OnListenerAdding(eventType, handler);
            _eventTable[eventType] = (Callback<T, U, V, X>)_eventTable[eventType] + handler;
        }


        //No parameters
        static public void RemoveListener(int eventType, Callback handler)
        {
            OnListenerRemoving(eventType, handler);
            _eventTable[eventType] = (Callback)_eventTable[eventType] - handler;
            OnListenerRemoved(eventType);
        }

        //Single parameter
        static public void RemoveListener<T>(int eventType, Callback<T> handler)
        {
            OnListenerRemoving(eventType, handler);
            _eventTable[eventType] = (Callback<T>)_eventTable[eventType] - handler;
            OnListenerRemoved(eventType);
        }

        //Two parameters
        static public void RemoveListener<T, U>(int eventType, Callback<T, U> handler)
        {
            OnListenerRemoving(eventType, handler);
            _eventTable[eventType] = (Callback<T, U>)_eventTable[eventType] - handler;
            OnListenerRemoved(eventType);
        }

        //Three parameters
        static public void RemoveListener<T, U, V>(int eventType, Callback<T, U, V> handler)
        {
            OnListenerRemoving(eventType, handler);
            _eventTable[eventType] = (Callback<T, U, V>)_eventTable[eventType] - handler;
            OnListenerRemoved(eventType);
        }

        //Four parameters
        static public void RemoveListener<T, U, V, X>(int eventType, Callback<T, U, V, X> handler)
        {
            OnListenerRemoving(eventType, handler);
            _eventTable[eventType] = (Callback<T, U, V, X>)_eventTable[eventType] - handler;
            OnListenerRemoved(eventType);
        }

        //No parameters
        static public void Broadcast(int eventType)
        {
            Delegate d;
            if (_eventTable.TryGetValue(eventType, out d))
            {
                Callback callback = d as Callback;

                if (callback != null)
                {
                    callback();
                }
                else
                {
                    throw CreateBroadcastSignatureException(eventType);
                }
            }
        }

        static public void SendEvent(Event evt)
        {
            Broadcast<Event>(evt.GetEventId(), evt);
        }

        //Single parameter
        static public void Broadcast<T>(int eventType, T arg1)
        {
            Delegate d;
            if (_eventTable.TryGetValue(eventType, out d))
            {
                Callback<T> callback = d as Callback<T>;

                if (callback != null)
                {
                    callback(arg1);
                }
                else
                {
                    throw CreateBroadcastSignatureException(eventType);
                }
            }
        }

        //Two parameters
        static public void Broadcast<T, U>(int eventType, T arg1, U arg2)
        {
            Delegate d;
            if (_eventTable.TryGetValue(eventType, out d))
            {
                Callback<T, U> callback = d as Callback<T, U>;

                if (callback != null)
                {
                    callback(arg1, arg2);
                }
                else
                {
                    throw CreateBroadcastSignatureException(eventType);
                }
            }
        }

        //Three parameters
        static public void Broadcast<T, U, V>(int eventType, T arg1, U arg2, V arg3)
        {
            Delegate d;
            if (_eventTable.TryGetValue(eventType, out d))
            {
                Callback<T, U, V> callback = d as Callback<T, U, V>;

                if (callback != null)
                {
                    callback(arg1, arg2, arg3);
                }
                else
                {
                    throw CreateBroadcastSignatureException(eventType);
                }
            }
        }

        //Four parameters
        static public void Broadcast<T, U, V, X>(int eventType, T arg1, U arg2, V arg3, X arg4)
        {
            Delegate d;
            if (_eventTable.TryGetValue(eventType, out d))
            {
                Callback<T, U, V, X> callback = d as Callback<T, U, V, X>;

                if (callback != null)
                {
                    callback(arg1, arg2, arg3, arg4);
                }
                else
                {
                    throw CreateBroadcastSignatureException(eventType);
                }
            }
        }
    }
}