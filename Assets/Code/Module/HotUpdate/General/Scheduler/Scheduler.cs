using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Framework
{
	public class Scheduler : SingletonSpawningMonoBehaviour<Scheduler> 
	{
		protected List<SchedulerEntry> _listeners = new List<SchedulerEntry> ();
		protected bool _isDispatching = false;

		//timer handles that to insert, or to delete 
		protected List<SchedulerEntry> _pendings = new List<SchedulerEntry> ();

		protected static ObjectPool<SchedulerEntry> _pool;
		public static int MAX_POOL_ENTRY = 10;


		protected override void Awake()
		{
			base.Awake ();
			_pool = ObjectPool<SchedulerEntry>.Instance;
			_pool.SetCapacity(MAX_POOL_ENTRY);
		}

		void Update()
		{
			Tick (Time.deltaTime);
		}

		public void AddListener(ScheduleHandler handler,float interval = 0f)
		{
			if (HasListener (handler)) 
			{
				return;		
			}
			SchedulerEntry entry = _pool.GetObject();
			entry.Elapsed = 0;
			entry.Interval = interval;
			entry.Handler = handler;
			if (!_isDispatching) 
			{
				entry.Status = SchedulerEntry.EntryStatus.Active;
				_listeners.Add(entry);
			} 
			else 
			{
				entry.Status = SchedulerEntry.EntryStatus.ToDelete;
				_pendings.Add(entry);
			}
		}

		public bool HasListener(ScheduleHandler handler)
		{
			bool bRet = false;
			for (int i = 0; i < _listeners.Count; i++)
			{
				SchedulerEntry entry = _listeners[i];
				if (entry.Handler == handler)
				{
					bRet = true;
					break;
				}
			}

			// when dispatching,check the toInsert,toDelete
			if (_isDispatching) 
			{
				int count = _pendings.Count;
				int insertTimes = 0;
				for(int i = 0; i < count; ++i)
				{
					if(_pendings[i].Handler == handler)
					{
						if(_pendings[i].Status == SchedulerEntry.EntryStatus.ToInsert)
						{
							insertTimes ++;
							if(insertTimes> 1)
							{
								insertTimes = 1;
							}
						}
						else if(_pendings[i].Status == SchedulerEntry.EntryStatus.ToDelete)
						{
							insertTimes --;
							if(insertTimes < -1)
							{
								insertTimes = -1;
							}
						}
					}
				}
				if(bRet && (insertTimes < 0)) 
				{
					bRet = false;
				}
				if(!bRet && (insertTimes > 0))
				{
					bRet = true;
				}
			}
			return bRet;
		}

		private SchedulerEntry.EntryStatus GetStatusInPending(ScheduleHandler handler)
		{
			SchedulerEntry.EntryStatus status = SchedulerEntry.EntryStatus.Idle;
			int count = _pendings.Count;
			SchedulerEntry entry = null;
			for (int i = count - 1; i > -1; --i) {
				entry = _pendings [i];
				if (entry.Handler == handler) {
					if (entry.Status == SchedulerEntry.EntryStatus.ToInsert) {
						status = SchedulerEntry.EntryStatus.ToInsert;
						break;
					} else if (entry.Status == SchedulerEntry.EntryStatus.ToDelete) {
						status = SchedulerEntry.EntryStatus.ToDelete;
						break;
					}
				}
			}
			return status;
		}

		public void RemoveListener(ScheduleHandler handler)
		{
			if (_isDispatching) 
			{
				SchedulerEntry entry = _pool.GetObject();
				entry.Status = SchedulerEntry.EntryStatus.ToDelete;
				entry.Handler = handler;
				_pendings.Add(entry);
			} 
			else 
			{
				SafeRemoveListener(handler);
			}
		}

		private void SafeRemoveListener(ScheduleHandler handler)
		{
			int count = _listeners.Count;
			for(int i = count -1; i> -1; --i)
			{
				if(_listeners[i].Handler == handler)
				{
					_pool.DisposeObject(_listeners[i]);
					_listeners.RemoveAt(i);
					break;
				}
			}
		}

		private void Tick(float dt)
		{
			_isDispatching = true;
			for (int index = 0; index < _listeners.Count; index++)
			{
				SchedulerEntry entry = _listeners[index];
				var status = GetStatusInPending (entry.Handler);
				if (status != SchedulerEntry.EntryStatus.ToDelete) { //不是待移除状态才处理
					entry.Update(dt);
				}
			}
			_isDispatching = false;

			//deal with pendings
			int count = _pendings.Count;
			for (int i = 0; i < count; ++i) 
			{
				if(_pendings[i].Status == SchedulerEntry.EntryStatus.ToInsert)
				{
					_pendings[i].Status = SchedulerEntry.EntryStatus.Active;
					_listeners.Add(_pendings[i]);
				}
				else if(_pendings[i].Status == SchedulerEntry.EntryStatus.ToDelete)
				{
					SafeRemoveListener(_pendings[i].Handler);
					_pool.DisposeObject(_pendings[i]);
				}
			}
			_pendings.Clear();
		}
	}

}
