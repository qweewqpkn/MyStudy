using System.Collections.Generic;
using UnityEngine;
using System;


namespace Framework
{	
	public class ObjectPool<T>:Singleton<ObjectPool<T>>
	{
		private static Queue<T> _objects = new Queue<T>();
		private static int _capactiy = 30;
	
		public int GetObjectCount()
		{ 
			return _objects.Count;
		}
		
		public void SetCapacity(int capacity)
		{
			_capactiy = capacity;
		}
		
		public T GetObject(params object[] list)
		{
			if (_objects.Count > 0)
			{
				return (T)_objects.Dequeue();
			}
			else 
			{
				return (T)System.Activator.CreateInstance(typeof(T), list);
			}
		}
		
		public void DisposeObject(T obj)
		{
			if (obj != null)
			{	
				IPoolableObject poolableObj = obj as IPoolableObject;
				if(poolableObj != null)
				{
					poolableObj.Reset();
				}
				if(_objects.Count < _capactiy)
				{
					_objects.Enqueue(obj);
				}
			}
		}

		public void ClearObjects()
		{
			_objects.Clear();
		}
		
	}
}
