using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarManager {

    public static AvatarManager Instance = new AvatarManager();
    private AvatarManager() { }

    //加载包装avatar prefab
    public Avatar CreateAvatar(string avatarWrap)
    {
        GameObject wrapObj = Resources.Load<GameObject>(avatarWrap);
        wrapObj = GameObject.Instantiate(wrapObj);
        Avatar avatar = wrapObj.GetComponent<Avatar>();
        if(avatar == null)
        {
            Debug.LogError("Avatar script is missing, please Check!");
        }

        return avatar;
    }
}
