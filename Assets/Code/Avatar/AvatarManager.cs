using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarManager {

    public static AvatarManager Instance = new AvatarManager();
    private AvatarManager() { }

    //加载包装avatar prefab
    public GameObject CreateAvatar(string avatarWrap)
    {
        GameObject wrapObj = Resources.Load<GameObject>(avatarWrap);
        wrapObj = GameObject.Instantiate(wrapObj);
        Avatar avatar = wrapObj.GetComponent<Avatar>();
        if(avatar == null)
        {
            Debug.LogError("Avatar script is missing, please Check!");
        }

        return wrapObj;
    }

    //改变avatar的子部件
    public void ChangePart(GameObject target, Avatar.PartType partType, string partID)
    {
        Avatar avatar = target.GetComponent<Avatar>();
        if(avatar == null)
        {
            Debug.LogError("Avatar script is missing, please Check!");
        }
        else
        {
            avatar.ChangePart(partType, partID);
        }
    }

    //改变子部件的材质
    public void ChangeMaterial(GameObject target, Avatar.PartType partType, string partID)
    {
        Avatar avatar = target.GetComponent<Avatar>();
        if (avatar == null)
        {
            Debug.LogError("Avatar script is missing, please Check!");
        }
        else
        {
            avatar.ChangeMaterial(partType, partID);
        }
    }
}
