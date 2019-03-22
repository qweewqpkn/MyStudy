﻿using AssetLoad;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabAutoDestory : MonoBehaviour {

    public HRes mRes;
    void OnDestroy()
    {
        if(mRes != null)
        {
            mRes.Release();
        }
    }
}
