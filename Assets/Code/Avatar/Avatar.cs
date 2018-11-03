using System;
using System.Collections.Generic;
using UnityEngine;

public class Avatar : MonoBehaviour
{
    public enum PartType
    {
        eNone,
        eSkeleton,
        eHair,
        eFace,
        eBody,
        eWeapon,
        eMax
    }

    [Serializable]
    public class PartData
    {
        public PartType mPartType;
        public GameObject mObj;
    }

    public List<PartData> mPartParentList= new List<PartData>();
    private Dictionary<PartType, GameObject> mPartDic = new Dictionary<PartType, GameObject>();

    public void ChangeMaterial(PartType partType, string partID)
    {
        GameObject obj;
        if(mPartDic.TryGetValue(partType, out obj))
        {
            Material material = Resources.Load<Material>(partID);
            SkinnedMeshRenderer smr = obj.GetComponentInChildren<SkinnedMeshRenderer>();
            if (smr != null)
            {
                smr.material = material;
            }
        }
    }

    public void ChangePart(PartType partType, string partID)
    {
        switch(partType)
        {
            case PartType.eSkeleton:
            case PartType.eWeapon:
                {
                    ChangePartReplace(partType, partID);
                }
                break;
            default:
                {
                    ChangePartShareSkeleton(partType, partID);
                }
                break;
        }
    }

    private void ChangePartReplace(PartType partType, string partID)
    {
        GameObject obj = null;
        if (mPartDic.TryGetValue(partType, out obj))
        {
            Destroy(obj);
        }

        GameObject parentObj = null;
        PartData partData = mPartParentList.Find((item)=> { return item.mPartType == partType; });
        if(partData != null)
        {
            parentObj = partData.mObj;
        }


        obj = Resources.Load<GameObject>(partID);
        if (obj != null && parentObj != null)
        {
            mPartDic[partType] = Instantiate(obj);
            mPartDic[partType].transform.SetParent(parentObj.transform, false);
        }
    }

    private void ChangePartShareSkeleton(PartType partType, string partID)
    {
        GameObject obj = null;
        if (mPartDic.TryGetValue(partType, out obj))
        {
            Destroy(obj);
        }

        GameObject parentObj = null;
        PartData partData = mPartParentList.Find((item) => { return item.mPartType == partType; });
        if (partData != null)
        {
            parentObj = partData.mObj;
        }

        obj = Resources.Load<GameObject>(partID);
        if(obj != null && parentObj != null)
        {
            mPartDic[partType] = Instantiate(obj);
            mPartDic[partType].transform.SetParent(parentObj.transform, false);
            SkinnedMeshRenderer mr = mPartDic[partType].GetComponentInChildren<SkinnedMeshRenderer>();
            if (mr != null)
            {
                List<Transform> newBoneList = new List<Transform>();
                Transform[] skeletonTrans = mPartDic[PartType.eSkeleton].GetComponentsInChildren<Transform>();
                for (int i = 0; i < mr.bones.Length; i++)
                {
                    for (int j = 0; j < skeletonTrans.Length; j++)
                    {
                        if (mr.bones[i].gameObject.name == skeletonTrans[j].name)
                        {
                            newBoneList.Add(skeletonTrans[j]);
                            break;
                        }
                    }
                }

                mr.bones = newBoneList.ToArray();
            }
        }
    }
}
