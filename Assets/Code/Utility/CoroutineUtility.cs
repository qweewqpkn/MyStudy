using System.Collections;
using UnityEngine;

public class CoroutineUtility : Singleton<CoroutineUtility> {

    private static DumnMono mDumnMono;
    private static GameObject mDumnObj;

    public class DumnMono : MonoBehaviour{}

    public override void Init()
    {
        mDumnObj = new GameObject();
        GameObject.DontDestroyOnLoad(mDumnObj);
        mDumnMono = mDumnObj.AddComponent<DumnMono>();
    }

    public void StartCoroutine(IEnumerator coroutine)
    {
        mDumnMono.StartCoroutine(coroutine);
    }
}
