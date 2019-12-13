using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Framework
{
    public class Event
    {
        private int _eventId;
        private Dictionary<string, object> _paramList;

        public Event()
        {
            _paramList = new Dictionary<string, object>();
        }

        public Event(int id)
        {
            _eventId = id;
            _paramList = new Dictionary<string, object>();
        }

        public int GetEventId()
        {
            return _eventId;
        }

        public void AddParam(string name, object value)
        {
            _paramList[name] = value;
        }

        public object GetParam(string name)
        {
            if (_paramList.ContainsKey(name))
            {
                return _paramList[name];
            }
            return null;
        }

        public bool HasParam(string name)
        {
            if (_paramList.ContainsKey(name))
            {
                return true;
            }
            return false;
        }

        public int GetParamCount()
        {
            return _paramList.Count;
        }

        public Dictionary<string, object> GetParamList()
        {
            return _paramList;
        }
    }
}