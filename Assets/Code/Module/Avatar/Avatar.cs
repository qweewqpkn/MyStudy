using AssetLoad;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Avatar : MonoBehaviour
{
    public enum PartType
    {
        eSkeleton = 0,
        eHair,
        eFace,
        eGlasses,
        eWing,
        eBody,
        eCard,
        eMax
    }

    public class PartData
    {
        public GameObject mObj;
        public PartType mType;
        public string mName;
        public Dictionary<string, List<string>> mBonesDict = new Dictionary<string, List<string>>(); //缓存骨骼对象名字，注意: 一定要美术哥保证，一个部件下的多个SkinMeshRenderer的名字要不同，不然会取出的骨骼列表将不匹配！！！
    }

    private Dictionary<PartType, PartData> mPartDic = new Dictionary<PartType, PartData>();
    private bool mIsSuit; //是否是套装

    private UIComponentBind mComponentBind;
    public UIComponentBind ComponentBind
    {
        get
        {
            if(mComponentBind == null)
            {
                if(mIsSuit)
                {
                    mComponentBind = GetComponent<UIComponentBind>();
                }
                else
                {
                    if (mPartDic.ContainsKey(PartType.eSkeleton))
                    {
                        PartData partData = mPartDic[PartType.eSkeleton];
                        if(partData != null && partData.mObj != null)
                        {
                            mComponentBind = partData.mObj.GetComponent<UIComponentBind>();
                        }
                    }
                }
            }

            return mComponentBind;
        }
    }

    private Animator mSkeletonAnimator;
    public Animator SkeletonAnimator
    {
        get
        {
            if(mSkeletonAnimator == null)
            {
                if(mIsSuit)
                {
                    mSkeletonAnimator = gameObject.GetComponentInChildren<Animator>();
                }
                else
                {
                    PartData partData;
                    if (mPartDic.TryGetValue(PartType.eSkeleton, out partData))
                    {
                        mSkeletonAnimator = partData.mObj.GetComponentInChildren<Animator>();
                    }
                    else
                    {
                        Debuger.LogError("other", "骨骼对象不存在，请确认!");
                    }
                }
            }

            return mSkeletonAnimator;
        }
    }

    private Animator mFaceAnimator;
    public Animator FaceAnimator
    {
        get
        {
            //去掉了缓存mFaceAnimator，每次都要从新取，原因是：因为我们会换脸，换脸后，mFaceAnimator保存的是之前脸的动画，所以会导致脸和身体不匹配
            PartData partData;
            if (mPartDic.TryGetValue(PartType.eFace, out partData))
            {
                mFaceAnimator = partData.mObj.GetComponentInChildren<Animator>();
            }
            else
            {
                Debuger.LogError("other", "骨骼对象不存在，请确认!");
            }

            return mFaceAnimator;
        }
    }

    public static void LoadSuit(string suitName, Action<Avatar> callback)
    {
        ResourceManager.Instance.LoadPrefabAsync(suitName, suitName, (obj, args) =>
        {
            if(callback != null)
            {
                Avatar avatar = obj.AddComponent<Avatar>();
                avatar.mIsSuit = true;
                callback(avatar);
            }
        });
    }

    public static void LoadAvatar(string wrapName, string skletonName, string hairName, string faceName, string bodyName, string cardName, string glassesName, string wingName, Action<Avatar> callback)
    {
        ResourceManager.Instance.LoadPrefabAsync(wrapName, wrapName, (obj, args) =>
        {
            Avatar avatar = obj.GetComponent<Avatar>();
            avatar.mIsSuit = false;
            if (avatar == null)
            {
                Debuger.LogError("other", "Avatar script is missing, please Check!");
                if(callback != null)
                {
                    callback(null);
                }
            }
            else
            {
                int num = 0;
                Action CheckPackageOver = () =>
                {
                    num++;
                    if (num == (int)PartType.eMax)
                    {
                        if(callback != null)
                        {
                            callback(avatar);
                        }
                    }
                };

                //加载骨骼
                avatar.ChangePart(Avatar.PartType.eSkeleton, skletonName, () =>
                {
                    CheckPackageOver();

                    //加载头发
                    avatar.ChangePart(Avatar.PartType.eHair, hairName, () =>
                    {
                        CheckPackageOver();
                    });

                    //加载脸部
                    avatar.ChangePart(Avatar.PartType.eFace, faceName, () =>
                    {
                        CheckPackageOver();
                    });

                    //加载身体
                    avatar.ChangePart(Avatar.PartType.eBody, bodyName, () =>
                    {
                        CheckPackageOver();
                    });

                    //加载牌
                    avatar.ChangePart(Avatar.PartType.eCard, cardName, () =>
                    {
                        CheckPackageOver();
                    });

                    //加载眼镜
                    avatar.ChangePart(Avatar.PartType.eGlasses, glassesName, () =>
                    {
                        CheckPackageOver();
                    });

                    //加载翅膀
                    avatar.ChangePart(Avatar.PartType.eWing, wingName, () =>
                    {
                        CheckPackageOver();
                    });
                });
            }
        });
    }

    public void ChangePart(PartType partType, string partID, Action callback)
    {
        if(string.IsNullOrEmpty(partID))
        {
            if(callback != null)
            {
                callback();
            }
            else
            {
                Debuger.LogError("avatar", "换装的回调为空");
            }
            return;
        }

        if(mIsSuit)
        {
            Debuger.LogError("avatar", "套装不能替换部件!!");
            return;
        }

        DestroyPart(partType);
        switch (partType)
        {
            case PartType.eSkeleton:
            case PartType.eGlasses:
            case PartType.eWing:
                {
                    ChangePartReplace(partType, partID, callback);
                }
                break;
            default:
                {
                    ChangePartShareSkeleton(partType, partID, callback);
                }
                break;
        }
    }

    private void ChangePartReplaceReal(PartData partData, Action callback)
    {
        if (this == null)
        {
            //当加载完成的时候,avatar自身已经被销毁了,所以这里要做判断
            if (callback != null)
            {
                callback();
            }

            if(partData != null && partData.mObj != null)
            {
                AvatarManager.Instance.CachePart(partData);
            }
            return;
        }

        if (partData == null || partData.mObj == null)
        {
            if (callback != null)
            {
                callback();
            }
            return;
        }

        Transform parentTrans = GetPartParent(partData.mType);
        if(parentTrans != null)
        {
            partData.mObj.SetActive(true);
            mPartDic[partData.mType] = partData;
            partData.mObj.transform.SetParent(parentTrans, false);
            ResetTransform(partData.mObj);

            switch (partData.mType)
            {
                case PartType.eSkeleton:
                    {
                        Animator animator = partData.mObj.GetComponent<Animator>();
                        if (animator != null)
                        {
                            animator.enabled = true;
                        }
                    }
                    break;
            }
        }  

        if (callback != null)
        {
            callback();
        }
    }

    private void ChangePartReplace(PartType partType, string partID, Action callback)
    {
        PartData cachePartData = AvatarManager.Instance.GetPart(partID);
        if(cachePartData != null)
        {
            ChangePartReplaceReal(cachePartData, callback);
        }
        else
        {
            ResourceManager.Instance.LoadPrefabAsync(partID, partID, (res, args) =>
            {
                if (res != null)
                {
                    PartData partData = new PartData();
                    partData.mObj = res;
                    partData.mType = partType;
                    partData.mName = partID;
                    ChangePartReplaceReal(partData, callback);
                }
            });
        }
    }

    private void ChangePartShareSkeletonReal(PartData partData, Action callback)
    {
        if (this == null)
        {
            //当加载完成的时候,avatar自身已经被销毁了,所以这里要做判断
            if (callback != null)
            {
                callback();
            }

            if (partData != null && partData.mObj != null)
            {
                AvatarManager.Instance.CachePart(partData);
            }

            Debuger.Log("other", "ChangePartShareSkeletonReal this is null");
            return;
        }

        if (partData == null || partData.mObj == null)
        {
            if (callback != null)
            {
                callback();
            }

            Debuger.Log("other", "ChangePartShareSkeletonReal partData is null");
            return;
        }

        Transform parentTrans = GetPartParent(partData.mType);
        if(parentTrans != null)
        {
            partData.mObj.SetActive(true);
            mPartDic[partData.mType] = partData;
            partData.mObj.transform.SetParent(parentTrans, false);
            ResetTransform(partData.mObj);
            SkinnedMeshRenderer[] allmr = partData.mObj.GetComponentsInChildren<SkinnedMeshRenderer>();
            if (allmr != null)
            {
                for (int m = 0; m < allmr.Length; m++)
                {
                    if(allmr[m] != null && mPartDic[PartType.eSkeleton].mObj != null)
                    {
                        SkinnedMeshRenderer mr = allmr[m];
                        //首先缓存骨骼名字
                        List<string> bonesNameList;
                        if(!partData.mBonesDict.TryGetValue(mr.gameObject.name, out bonesNameList))
                        {
                            bonesNameList = new List<string>();
                            partData.mBonesDict[mr.gameObject.name] = bonesNameList;
                            for(int i = 0; i < mr.bones.Length; i++)
                            {
                                if(mr.bones[i] == null)
                                {
                                    Debuger.LogError("other", "ChangePartShareSkeletonReal mr.bones[i] is null please check!!!!");
                                    break;
                                }
                                else
                                {
                                    bonesNameList.Add(mr.bones[i].gameObject.name);
                                }
                            }
                        }
                        
                        //将骨骼部件 替换到 指定的部件
                        Transform[] skeletonTrans = mPartDic[PartType.eSkeleton].mObj.GetComponentsInChildren<Transform>();
                        if (skeletonTrans != null && bonesNameList != null)
                        {
                            List<Transform> newBoneList = new List<Transform>();
                            for (int i = 0; i < bonesNameList.Count; i++)
                            {
                                for (int j = 0; j < skeletonTrans.Length; j++)
                                {
                                    if(skeletonTrans[j] != null)
                                    {
                                        if (bonesNameList[i] == skeletonTrans[j].name)
                                        {
                                            newBoneList.Add(skeletonTrans[j]);
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        Debuger.LogError("other", "ChangePartShareSkeletonReal mr.bones[i] skeletonTrans[j] is null");
                                    }
                                }
                            }

                            mr.bones = newBoneList.ToArray();
                            mr.updateWhenOffscreen = true;
                        }
                        else
                        {
                            Debuger.LogError("other", "ChangePartShareSkeletonReal skeletonTrans is null");
                        }
                    }
                    else
                    {
                        Debuger.LogError("other", "ChangePartShareSkeletonReal allmr is null");
                    }
                }
            }

            switch (partData.mType)
            {
                case PartType.eFace:
                    {
                        if (partData.mObj.GetComponent<Animator>() == null)
                        {
                            Animator animator = partData.mObj.AddComponent<Animator>();
                            animator.runtimeAnimatorController = SkeletonAnimator.runtimeAnimatorController;
                            animator.avatar = SkeletonAnimator.avatar;
                            animator.enabled = true;
                        }
                        else
                        {
                            Animator animator = partData.mObj.GetComponent<Animator>();
                            animator.enabled = true;
                        }
                    }
                    break;
            }
        }

        if (callback != null)
        {
            callback();
        }

    }

    private void ChangePartShareSkeleton(PartType partType, string partID, Action callback)
    {
        PartData cachePartData = AvatarManager.Instance.GetPart(partID);
        if (cachePartData != null)
        {
            ChangePartShareSkeletonReal(cachePartData, callback);
        }
        else
        {
            ResourceManager.Instance.LoadPrefabAsync(partID, partID, (res, args) =>
            {
                if (res != null)
                {
                    PartData partData = new PartData();
                    partData.mObj = res;
                    partData.mType = partType;
                    partData.mName = partID;
                    ChangePartShareSkeletonReal(partData, callback);
                }
            });
        }
    }

    //获取部件的绑定点
    public Transform GetPartBindPos(string bindName)
    {
        if(ComponentBind != null)
        {
            return ComponentBind.GetComponentData<Transform>(bindName);
        }

        return null;
    }

    //获取具体部件
    public Transform GetPart(PartType type)
    {
        PartData partData;
        if(mPartDic.TryGetValue(type, out partData))
        {
            return partData.mObj.transform;
        }

        return null;
    }

    //获取部件的父对象
    public Transform GetPartParent(PartType type)
    {
        switch(type)
        {
            case PartType.eHair:
            case PartType.eFace:
            case PartType.eGlasses:
            case PartType.eCard:
            case PartType.eBody:
            case PartType.eSkeleton:
                {
                    return transform;
                }
            case PartType.eWing:
                {
                    return GetPartBindPos("b_point_wing");
                }
        }

        return null;
    }

    private void ResetTransform(GameObject obj)
    {
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localEulerAngles = Vector3.zero;
        obj.transform.localScale = Vector3.one;
    }

	public void SetTrigger(string actionName)
	{
        //Debug.LogError ("设置actionName------------actionName=" + actionName);
        SkeletonAnimator.SetTrigger (actionName);
        FaceAnimator.SetTrigger(actionName);
    }

	public void SetDelayTrigger(float time, string actionName)
	{
		StartCoroutine (handleDelayTrigger (time, actionName));
	}
	private IEnumerator handleDelayTrigger(float time, string actionName)

	{
		yield return new WaitForSeconds (time);
		if (null != SkeletonAnimator) {
            SkeletonAnimator.SetTrigger (actionName);
            FaceAnimator.SetTrigger(actionName);
        }
		yield return null;
	}

    //销毁部件
    public void DestroyPart(PartType partType)
    {
        if (mPartDic.ContainsKey(partType))
        {
            AvatarManager.Instance.CachePart(mPartDic[partType]);
            mPartDic.Remove(partType);
        }
    }

    public void DestroyAll()
    {
        GameObject.Destroy(gameObject);
    }

    //avatar对象被销毁的时候，可能还有部件在身上，我们要进行缓存！！
    private void OnDestroy()
    {
        DestroyPart(Avatar.PartType.eSkeleton);
        DestroyPart(Avatar.PartType.eHair);
        DestroyPart(Avatar.PartType.eFace);
        DestroyPart(Avatar.PartType.eGlasses);
        DestroyPart(Avatar.PartType.eWing);
        DestroyPart(Avatar.PartType.eBody);
        DestroyPart(Avatar.PartType.eCard);
    }
}
