using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class OptmizeAnimationClip : MonoBehaviour {
    [MenuItem("ArtTools/优化动画", false, 10)]
    public static void optmizeAnimation()
    {
        UnityEngine.Object[] SelectionAsset = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Unfiltered);
        for (int i = 0; i < SelectionAsset.Length; i++)
        {
            AnimationClip[] clips = CopyAnimationFromFBX(SelectionAsset[i]);
            for(int j = 0; j < clips.Length; j++)
            {
                EditorUtility.DisplayProgressBar("优化动画", string.Format("{0}/{1}", j + 1, clips.Length), (j + 1) * 1.0f / clips.Length);
                OptmizeAnimationScaleCurve(clips[j]);
                OptmizeAnimationFloat(clips[j]);
            }
        }


        EditorUtility.DisplayProgressBar("刷新本地资源", "", 0.0f);
        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();
        EditorUtility.DisplayProgressBar("刷新本地资源", "", 1.0f);
        EditorUtility.ClearProgressBar();
    }

    static AnimationClip[] CopyAnimationFromFBX(Object FBX)
    {
        if(FBX == null)
        {
            return null;
        }

        string path = AssetDatabase.GetAssetPath(FBX);
        string animationPath = Path.GetDirectoryName(path) + "/Animation/";
        if (Directory.Exists(animationPath))
        {
            Directory.Delete(animationPath, true);
        }
        Directory.CreateDirectory(animationPath);

        //拿到FBX所有AnimatoinClip对象
        Object[] FBXObjects = AssetDatabase.LoadAllAssetsAtPath(path);
        List<AnimationClip> FBXClipList = new List<AnimationClip>();
        for (int i = 0; i < FBXObjects.Length; i++)
        {
            AnimationClip srcClip = FBXObjects[i] as AnimationClip;
            if (srcClip != null)
            {
                FBXClipList.Add(srcClip);
            }
        }

        List<AnimationClip> clipList = new List<AnimationClip>();
        for (int i = 0; i < FBXClipList.Count; i++)
        {
            EditorUtility.DisplayProgressBar("拷贝FBX动画", string.Format("{0}/{1}", i + 1, FBXClipList.Count), (i + 1) * 1.0f / FBXClipList.Count);
            AnimationClip newClip = new AnimationClip();
            newClip.name = FBXClipList[i].name;
            EditorUtility.CopySerialized(FBXClipList[i], newClip);
            AssetDatabase.CreateAsset(newClip, animationPath + newClip.name + ".anim");
            clipList.Add(newClip);

            if (clipList.Count > 13)
            {
                break;
            }
            //不使用遍历拷贝值的方法,太慢了
            //AnimationClipSettings srcSetting = AnimationUtility.GetAnimationClipSettings(srcClip);
            //AnimationUtility.SetAnimationClipSettings(newClip, srcSetting);
            //
            //EditorCurveBinding[] binds = AnimationUtility.GetCurveBindings(srcClip);
            //for (int j = 0; j < binds.Length; j++)
            //{
            //    AnimationCurve curve = AnimationUtility.GetEditorCurve(srcClip, binds[j]);
            //    AnimationUtility.SetEditorCurve(newClip, binds[j], curve);
            //}
        }

        return clipList.ToArray();
    }

    /// <summary>
    /// 优化浮点数精度
    /// </summary>
    static void OptmizeAnimationFloat(AnimationClip clip)
    {
        EditorCurveBinding[] binds =  AnimationUtility.GetCurveBindings(clip);
        Keyframe[] keyFrames;
        for (int i = 0; i < binds.Length; i++)
        {
            AnimationCurve curve = AnimationUtility.GetEditorCurve(clip, binds[i]);
            if(curve.keys != null)
            {
                keyFrames = curve.keys;
                for (int j = 0; j < keyFrames.Length; j++)
                {
                    Keyframe key = keyFrames[j];
                    key.value = float.Parse(key.value.ToString("f3"));
                    key.inTangent = float.Parse(key.inTangent.ToString("f3"));
                    key.outTangent = float.Parse(key.outTangent.ToString("f3"));
                    keyFrames[j] = key;
                }
                curve.keys = keyFrames;
            }

            clip.SetCurve(binds[i].path, binds[i].type, binds[i].propertyName, curve);
        }

        //浮点数精度压缩到f3
        //AnimationClipCurveData[] curves = null;
        //curves = AnimationUtility.GetAllCurves(clip);
        //Keyframe key;
        //Keyframe[] keyFrames;
        //for (int ii = 0; ii < curves.Length; ++ii)
        //{
        //    AnimationClipCurveData curveDate = curves[ii];
        //    if (curveDate.curve == null || curveDate.curve.keys == null)
        //    {
        //        //Debug.LogWarning(string.Format("AnimationClipCurveData {0} don't have curve; Animation name {1} ", curveDate, animationPath));
        //        continue;
        //    }
        //    keyFrames = curveDate.curve.keys;
        //    for (int i = 0; i < keyFrames.Length; i++)
        //    {
        //        key = keyFrames[i];
        //        key.value = float.Parse(key.value.ToString("f3"));
        //        key.inTangent = float.Parse(key.inTangent.ToString("f3"));
        //        key.outTangent = float.Parse(key.outTangent.ToString("f3"));
        //        keyFrames[i] = key;
        //    }
        //    curveDate.curve.keys = keyFrames;
        //    clip.SetCurve(curveDate.path, curveDate.type, curveDate.propertyName, curveDate.curve);
        //}
    }

    /// <summary>
    /// 优化scale曲线
    /// </summary>
    static AnimationClip OptmizeAnimationScaleCurve(AnimationClip clip)
    {
        //去除scale曲线
        foreach (EditorCurveBinding bind in AnimationUtility.GetCurveBindings(clip))
        {
            string name = bind.propertyName.ToLower();
            if (name.Contains("scale"))
            {
                AnimationUtility.SetEditorCurve(clip, bind, null);
            }
        }

        return clip;
    }

}


