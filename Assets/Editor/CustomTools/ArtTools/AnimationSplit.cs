using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections;
using System.Xml.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;

public class AnimationSplit
{
    [MenuItem("ArtTools/动画分割")]
    public static void AnimationsClip()
    {
        if (Selection.gameObjects == null || Selection.gameObjects.Length == 0) return;
        if(Selection.gameObjects.Length >1)
        {
            Debug.Log("请一次只选择一个对象");
            return;
        }

        string path = AssetDatabase.GetAssetPath(Selection.gameObjects[0]);

        ModelImporter importer = AssetImporter.GetAtPath(path) as ModelImporter;
        if(importer == null)
        {
            Debug.Log("选择的对象不是模型！");
            return;
        }

        string aniTxt = File.ReadAllText(EditorUtility.OpenFilePanel("打开动画描述文件", "", "txt"));
        string[] lines = aniTxt.Replace("\r\n", "\n").Split('\n');

        var anisList = new System.Collections.Generic.List<AnimationInfo>();

        foreach(var line in lines)
        {
            var matchs =  Regex.Matches(line, "[^\\s]+");
            if(matchs.Count < 3)
            {
                continue;
            }
            var name = matchs[0].Value;
            var start = int.Parse(matchs[1].Value);
            var end = int.Parse(matchs[2].Value);

            anisList.Add(new AnimationInfo()
            {
                Name = name,
                StartFrame = start,
                EndFrame = end
            });
        }

        importer.importAnimation = true;
        importer.SaveAndReimport();
        importer.animationCompression = ModelImporterAnimationCompression.KeyframeReduction;
        var clips = new System.Collections.Generic.List<ModelImporterClipAnimation>();
        foreach(var info in anisList)
        {
            clips.Add(new ModelImporterClipAnimation()
                {
                    name = info.Name,
                    firstFrame = info.StartFrame,
                    lastFrame = info.EndFrame,
                    loop  = info.Loop,
                    loopTime = info.Loop,
                    wrapMode = WrapMode.Loop,
                    loopPose = info.Loop
                });
        }
        importer.clipAnimations = clips.ToArray();
        //for(var i=0;i<anisList.Count;i++)
        //{
        //    var info = anisList[i];
        //    importer.clipAnimations[i] = new ModelImporterClipAnimation();
        //    importer.clipAnimations[i].name = info.Name;
        //    importer.clipAnimations[i].firstFrame = info.StartFrame;
        //    importer.clipAnimations[i].lastFrame = info.EndFrame;
        //    importer.clipAnimations[i].loop = info.Loop;
        //    importer.clipAnimations[i].loopPose = info.Loop;
        //}
        importer.SaveAndReimport();
    }

    class AnimationInfo
    {
        private string _Name;
        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
                if (_Name.StartsWith("Idle"))
                {
                    Loop = true;
                    return;
                }
                if (_Name.StartsWith("Run"))
                {
                    Loop = true;
                    return;
                }
                if (_Name.StartsWith("Stun"))
                {
                    Loop = true;
                    return;
                }
            }
        }

        public int StartFrame;

        public int EndFrame;

        public bool Loop;
    }
}
