using System;
using System.Collections.Generic;
using UnityEngine;

public class TimerManager : MonoBehaviour {

    public class TimerData
    {
        public int mID;
        public float mCurTime;
        public float mDeltaTime;
        public int mTimes;
        public Action mAction;
    }

    private int mID = 0;
    private Dictionary<int, TimerData> mTimerDic = new Dictionary<int, TimerData>();
    private List<TimerData> mRemoveTimerList = new List<TimerData>();

    private static TimerManager mInstance;
	public static TimerManager Instance
    {
        get
        {
            if(mInstance == null)
            {
                GameObject obj = new GameObject();
                mInstance = obj.AddComponent<TimerManager>();
            }

            return mInstance;
        }
    }
	
    public int AddTimer(float deltaTime, int times, Action action)
    {
        mID = mID + 1;
        TimerData data = new TimerData();
        data.mDeltaTime = deltaTime;
        data.mTimes = times;
        data.mAction = action;
        data.mID = mID;
        mTimerDic.Add(mID, data);
        return mID;
    }

    public void StopTimer(int id)
    {
        if(mTimerDic.ContainsKey(id))
        {
            mTimerDic.Remove(id);
        }
    }


	// Update is called once per frame
	void Update () {
		foreach(var item in mTimerDic)
        {
            TimerData data = item.Value;
            data.mCurTime += Time.deltaTime;
            if(data.mCurTime >= data.mDeltaTime)
            {
                data.mCurTime = data.mDeltaTime - data.mCurTime;
                if(data.mAction != null)
                {
                    data.mAction();
                    if(data.mTimes != -1)
                    {
                        data.mTimes--;
                        if (data.mTimes == 0)
                        {
                            mRemoveTimerList.Add(data);
                        }
                    }
                }
            }
        }

        for(int i = 0; i < mRemoveTimerList.Count; i++)
        {
            mTimerDic.Remove(mRemoveTimerList[i].mID);
        }
        mRemoveTimerList.Clear();
	}
}
