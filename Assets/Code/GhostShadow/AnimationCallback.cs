using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimationCallback : MonoBehaviour {

    private static Vector4 ParaseVector4(string s)
    {
        //s格式:x,x,x,x
        float x = 0;
        float y = 0;
        float z = 0;
        float w = 0;

        string[] valueList = s.Split(',');
        if(valueList.Length == 4)
        {
            x = float.Parse(valueList[0]);
            y = float.Parse(valueList[1]);
            z = float.Parse(valueList[2]);
            w = float.Parse(valueList[3]);
        }

        return new Vector4(x, y, z, w);
    }

    public void CreateGhostShadow(string s)
    {
        //解析数据
        float holdTime = 0;
        float freneselIntensity = 0;
        Vector4 beginColor = Vector4.zero;
        Vector4 endColor = Vector4.zero;

        string[] paramsList = s.Split(';');
        for(int i = 0; i < paramsList.Length; i++)
        {
            string[] nameValueList = paramsList[i].Split(':');
            switch(nameValueList[0])
            {
                case "chixushijian(s)":
                    {
                        holdTime = float.Parse(nameValueList[1]);
                    }
                    break;
                case "bianyuankuandu":
                    {
                        freneselIntensity = float.Parse(nameValueList[1]);
                    }
                    break;
                case "kaishiyanse(RGBA)":
                    {
                        beginColor = ParaseVector4(nameValueList[1]);
                    }
                    break;
                case "jieshuyanse(RGBA)":
                    {
                        endColor = ParaseVector4(nameValueList[1]);
                    }
                    break;
            }
        }

        GhostShadow.CreateGhostShadow(gameObject, holdTime, freneselIntensity, beginColor, endColor);
    }
}
