using UnityEngine;
using System.Collections;

namespace Framework
{
	public delegate void ScheduleHandler();

	public class SchedulerEntry : IPoolableObject
	{
		public enum EntryStatus
		{
			Idle,
			Active,
			ToDelete,
			ToInsert
		};

		protected float _interval = 0;
        protected EntryStatus _status = EntryStatus.Idle;
		protected float _elapsed = 0;
		protected ScheduleHandler _handler;

		public SchedulerEntry()
		{

		}

		public void Reset()
		{
			_interval = 0;
			_elapsed = 0;
			_handler = null;
            _status = EntryStatus.Idle;
		}

		public void Update (float dt) 
		{
			_elapsed += dt;
			if (_elapsed > _interval) 
			{
				_elapsed = 0;
				_handler();
			}
		}

		public EntryStatus Status
		{
			get { return _status;}
			set { _status = value;}
		}

		public float Interval
		{
			get { return _interval;}
			set { _interval = value;}
		}

		public float Elapsed
		{
			get { return _elapsed;}
			set { _elapsed = value;}
		}

		public ScheduleHandler Handler
		{
			get { return _handler;}
			set { _handler = value;}
		}

	}
}