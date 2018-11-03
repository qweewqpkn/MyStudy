using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AvatarMain : MonoBehaviour {

    public Button mButtonHair;
    public Button mButtonFace;
    public Button mButtonBody;

    // Use this for initialization
    void Start () {
        GameObject obj = AvatarManager.Instance.CreateAvatar("AvatarWrap");
        AvatarManager.Instance.ChangePart(obj, Avatar.PartType.eSkeleton, "Female/female_skeleton");
        AvatarManager.Instance.ChangePart(obj, Avatar.PartType.eHair, "Female/female_hair-1_red");
        AvatarManager.Instance.ChangePart(obj, Avatar.PartType.eFace, "Female/female_face-1");
        AvatarManager.Instance.ChangePart(obj, Avatar.PartType.eBody, "Female/female_top-1_blue");

        List<string> hairList = new List<string>() { "Female/female_hair-1_brown", "Female/female_hair-1_red", "Female/female_hair-1_yellow", "Female/female_hair-2_cyan", "Female/female_hair-2_dark", "Female/female_hair-2_pink" };
        List<string> faceList = new List<string>() { "Female/female_face-1", "Female/female_face-2" };
        List<string> bodyList = new List<string>() { "Female/female_top-1_blue", "Female/female_top-1_green", "Female/female_top-1_pink", "Female/female_top-2_green", "Female/female_top-2_orange", "Female/female_top-2_purple" };

        int hairIndex = 0;
        int faceIndex = 0;
        int bodyIndex = 0;

        mButtonHair.onClick.AddListener(() =>
        {
            if(hairIndex < hairList.Count)
            {
                AvatarManager.Instance.ChangePart(obj, Avatar.PartType.eHair, hairList[hairIndex]);
                hairIndex = (hairIndex + 1) % hairList.Count;
            }
        });

        mButtonFace.onClick.AddListener(() =>
        {
            if (faceIndex < faceList.Count)
            {
                AvatarManager.Instance.ChangePart(obj, Avatar.PartType.eFace, faceList[faceIndex]);
                faceIndex = (faceIndex + 1) % faceList.Count;
            }
        });

        mButtonBody.onClick.AddListener(() =>
        {
            if (bodyIndex < bodyList.Count)
            {
                AvatarManager.Instance.ChangePart(obj, Avatar.PartType.eBody, bodyList[bodyIndex]);
                bodyIndex = (bodyIndex + 1) % bodyList.Count;
            }
        });
    }
}
