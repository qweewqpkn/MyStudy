using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AvatarTest : MonoBehaviour {

    public Button mHairBtn;
    public Button mFaceBtn;
    public Button mBodyBtn;
    public Button mWingBtn;
    private Avatar mAvatar;
    public class ResData
    {
        public int index = 1;
        public List<string> resList;
    }
    private Dictionary<Avatar.PartType, ResData> resDict = new Dictionary<Avatar.PartType, ResData>();

    // Use this for initialization
    void Start () {
        AvatarManager.Instance.CreateAvatar("male_skeleton_01", "male_hair_01", "male_head_01", "male_body_01", "", "", "chibang_emo", (avatar) =>
        {
            mAvatar = avatar;
            mAvatar.transform.Rotate(new Vector3(0.0f, 180.0f, 0.0f));
        });

        resDict[Avatar.PartType.eHair] = new ResData();
        resDict[Avatar.PartType.eHair].resList = new List<string>() { "male_hair_01", "male_hair_02", "male_hair_03"};
        resDict[Avatar.PartType.eFace] = new ResData();
        resDict[Avatar.PartType.eFace].resList = new List<string>() { "male_head_01", "male_head_02"};
        resDict[Avatar.PartType.eBody] = new ResData();
        resDict[Avatar.PartType.eBody].resList = new List<string>() { "male_body_01", "male_body_02"};
        resDict[Avatar.PartType.eWing] = new ResData();
        resDict[Avatar.PartType.eWing].resList = new List<string>() { "chibang_emo", "chibang_jichebao", "chibang_jingling", "chibang_ribendao", "chibang_tianshi", "chibang_zhisan"};

        mHairBtn.onClick.AddListener(() =>
        {
            Change(Avatar.PartType.eHair);
        });

        mFaceBtn.onClick.AddListener(() =>
        {
            Change(Avatar.PartType.eFace);
        });

        mBodyBtn.onClick.AddListener(() =>
        {
            Change(Avatar.PartType.eBody);
        });

        mWingBtn.onClick.AddListener(() =>
        {
            Change(Avatar.PartType.eWing);
        });
    }

    public void Change(Avatar.PartType type)
    {
        int index = resDict[type].index++ % resDict[type].resList.Count;
        List<string> reslist = resDict[type].resList;
        mAvatar.ChangePart(type, reslist[index], null);
    }

	// Update is called once per frame
	void Update () {
		
	}
}
