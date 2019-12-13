/*-------------------------------------------------------------------
// Copyright (C)
//
// Module: MainThreadRunner
// Author:
// Date:
// Description: 主线程安全函数执行管理器.
// Revision history:
//     2017.09.07   huangxin    修改单例实现模式为继承SingletonSpawningMonoBehaviour
//-----------------------------------------------------------------*/

using UnityEngine;
using System.Collections.Generic;
using System.Threading;
using System;
using System.Collections;


namespace Framework
{
    public class MainThreadRunner : SingletonSpawningMonoBehaviour<MainThreadRunner> 
    {
        private Queue<Action> _methods = new Queue<Action>();
        private Queue<Action<object>> _methodsWithPara= new Queue<Action<object>>();
        private Queue<object> _paras = new Queue<object>(); 
        public void RunOnMainThread(Action method)
        {
            lock (_methods)
            {
                _methods.Enqueue(method);
            }
        }

        public void RunOnMainThread(Action<object> action,object para)
        {
            lock (_methodsWithPara)
            {
                _methodsWithPara.Enqueue(action);
                _paras.Enqueue(para);
            }
        }

        void Update()
        {
            while (_methods.Count > 0)
            {
                Action method = null;
                lock (_methods)
                {
                    method = _methods.Dequeue();
                }
                if (method != null)
                {
                    try
                    {
                        method.Invoke();
                    }
                    catch (Exception e)
                    {
                        CLogger.LogError(e.Message+'\n'+method.Method.Name+"\n"+e.StackTrace);
                    }
                }
            }
            while (_methodsWithPara.Count > 0)
            {
                Action<object> method = null;
                object p = null;
                lock (_methodsWithPara)
                {
                    method = _methodsWithPara.Dequeue();
                    p = _paras.Dequeue();
                }
                if (method != null)
                {
                    try
                    {
                        method.Invoke(p);
                    }
                    catch (Exception e)
                    {
                        CLogger.LogError(e.Message + '\n' + method.Method.Name);
                    }
                }
            }
        }


        public Coroutine WaitAndExecute(float delay, Action action)
        {
            return base.StartCoroutine(this.iWaitAndExecute(delay, action));
        }

        public Coroutine WaitOneFrame(Action action)
        {
            return base.StartCoroutine(this.iWaitOneFrame(action));
        }

        public Coroutine WaitFramesAndExecute(int count, Action action)
        {
            return base.StartCoroutine(this.iWaitFramesAndExecute(count, action));
        }

        private IEnumerator iWaitAndExecute(float delay, Action action)
        {
            yield return new WaitForSeconds(delay);
            action();
        }

        private IEnumerator iWaitOneFrame(Action action)
        {
            yield return new WaitForEndOfFrame();
            action();
        }

        private IEnumerator iWaitFramesAndExecute(int count, Action action)
        {
            for(int i = 0; i < count; ++i)
            {
                yield return new WaitForEndOfFrame();
            }
            action();
        }
	
    }
}

