﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UIComponentBindEditor {
    [MenuItem("Tools/自动绑定UI组件")]
    public static void AutoBindUIComponent()
    {
        GameObject obj = Selection.activeGameObject;
        BindComponent(obj, null);
        GenerateLuaCode(obj);
    }

    private static void BindComponent(GameObject obj, UIComponentBind bind)
    {
        if(bind == null)
        {
            bind = AddComponent<UIComponentBind>(obj);
            bind.mObjName = obj.name;
            bind.mComponentDataList.Clear();
        }
    
        for (int i = 0; i < obj.transform.childCount; i++)
        {
            Transform trans = obj.transform.GetChild(i);
            if (trans.name.StartsWith("b_"))
            {
                UIComponentBind.ComponentData data = new UIComponentBind.ComponentData();
                InitComponentData(trans, data);
                bind.mComponentDataList.Add(data);
            }

            if (trans.name.StartsWith("b_t_"))
            {
                //模版对象
                BindComponent(trans.gameObject, null);
            }
            else
            {
                BindComponent(trans.gameObject, bind);
            }
        }
    }

    private static T AddComponent<T>(GameObject obj) where T : Component
    {
        T component = obj.GetComponent<T>();
        if(component == null)
        {
            component = obj.AddComponent<T>();
        }

        return component;
    }

    private static void InitComponentData(Transform obj, UIComponentBind.ComponentData data)
    {
        data.name = obj.name;
        if (obj.GetComponent<Button>() != null)
        {
            data.type = "Button";
            data.component = obj.GetComponent<Button>();
        }
        else if (obj.GetComponent<Dropdown>() != null)
        {
            data.type = "Dropdown";
            data.component = obj.GetComponent<Dropdown>();
        }
        else if (obj.GetComponent<InputField>() != null)
        {
            data.type = "InputField";
            data.component = obj.GetComponent<InputField>();
        }
        else if (obj.GetComponent<Slider>() != null)
        {
            data.type = "Slider";
            data.component = obj.GetComponent<Slider>();
        }
        else if (obj.GetComponent<Toggle>() != null)
        {
            data.type = "Toggle";
            data.component = obj.GetComponent<Toggle>();
        }
        else if (obj.GetComponent<GridLayoutGroup>() != null)
        {
            data.type = "GridLayoutGroup";
            data.component = obj.GetComponent<GridLayoutGroup>();
        }
        else if (obj.GetComponent<VerticalLayoutGroup>() != null)
        {
            data.type = "VerticalLayoutGroup";
            data.component = obj.GetComponent<VerticalLayoutGroup>();
        }
        else if (obj.GetComponent<ScrollRect>() != null)
        {
            data.type = "ScrollRect";
            data.component = obj.GetComponent<ScrollRect>();
        }
        else if (obj.GetComponent<Image>() != null)
        {
            data.type = "Image";
            data.component = obj.GetComponent<Image>();
        }
        //else if (obj.GetComponent<ImageExt>() != null)
        //{
        //    data.type = "ImageExt";
        //    data.component = obj.GetComponent<ImageExt>();
        //}
        else if (obj.GetComponent<RawImage>() != null)
        {
            data.type = "RawImage";
            data.component = obj.GetComponent<RawImage>();
        }
        //else if (obj.GetComponent<RawImageExt>() != null)
        //{
        //    data.type = "RawImageExt";
        //    data.component = obj.GetComponent<RawImageExt>();
        //}
        else if (obj.GetComponent<Text>() != null)
        {
            data.type = "Text";
            data.component = obj.GetComponent<Text>();
        }
        else if (obj.GetComponent<RectTransform>() != null)
        {
            data.type = "RectTransform";
            data.component = obj.GetComponent<RectTransform>();
        }
        else if (obj.GetComponent<Transform>() != null)
        {
            data.type = "Transform";
            data.component = obj.GetComponent<Transform>();
        }
    }

    private static void GenerateLuaCode(GameObject obj)
    {
        string name = obj.name;
        string luaPath = Application.dataPath + "/../../ClientRes/Lua/";
        FileInfo fileInfo = GetTargetFile(luaPath, name);
        FileStream fs;
        if (fileInfo == null)
        {
            fs = File.Open(luaPath + name + ".lua", FileMode.CreateNew, FileAccess.ReadWrite, FileShare.ReadWrite);
        }
        else
        {
            fs = fileInfo.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
        }

        StreamReader sr = new StreamReader(fs);
        StreamWriter sw = new StreamWriter(fs);
        string content = sr.ReadToEnd();
        if (string.IsNullOrEmpty(content))
        {
            content = string.Format("local {0} = class(\"{1}\",UIBase)\r\n", name, name);
        }

        //首先寻找是否已经自动写入过
        int startFlagIndex = content.IndexOf("--@start");
        int endFlagIndex = content.IndexOf("--@end");
        if (startFlagIndex != -1 && endFlagIndex != -1)
        {
            content = content.Remove(startFlagIndex, endFlagIndex - startFlagIndex + 6);
        }

        //在指定位置开始写入
        string startContent;
        string endContent;
        int index = content.IndexOf("class(\"" + name);
        if (index != -1)
        {
            index = content.IndexOf("\r\n", index);
            startContent = content.Substring(0, index + 2);
            endContent = content.Substring(index + 2);
            endContent = endContent.TrimStart('\r', '\n');

            fs.Seek(0, SeekOrigin.Begin);
            fs.SetLength(0);

            //写入开始内容
            sw.Write(startContent);
            sw.WriteLine("");
            //写入插入的
            sw.WriteLine("--@start 自动绑定ui组件,请勿手动修改");
            UIComponentBind[] bindDatas = Selection.activeGameObject.GetComponentsInChildren<UIComponentBind>(true);

           for (int i = 0; i < bindDatas[0].mComponentDataList.Count; i++)
           {
               UIComponentBind.ComponentData data = bindDatas[0].mComponentDataList[i];
               sw.WriteLine(string.Format("local {0} --{1}", data.name, data.type));
           }

            for(int i = 1; i < bindDatas.Length; i++)
            {
                sw.WriteLine("");
                sw.WriteLine(string.Format("--{0}的导出的子元素", bindDatas[i].mObjName));
                for(int j = 0; j < bindDatas[i].mComponentDataList.Count; j++)
                {
                    UIComponentBind.ComponentData data = bindDatas[i].mComponentDataList[j];
                    sw.WriteLine(string.Format("--{0} {1}", data.name, data.type));
                }
            }

            sw.WriteLine("");
            sw.WriteLine(string.Format("function {0}:BindUI()", name));
            for (int i = 0; i < bindDatas[0].mComponentDataList.Count; i++)
            {
                UIComponentBind.ComponentData data = bindDatas[0].mComponentDataList[i];
                sw.WriteLine(string.Format("{0} = self.{1}", data.name, data.name));
            }

            sw.WriteLine("end");
            sw.WriteLine("--@end");
            sw.WriteLine("");

            //写入后面内容
            sw.Write(endContent);
        }
        sw.Dispose();
        sw.Close();
        fs.Close();
    }

    private static void GetAllFile(string path, List<FileInfo> fileList)
    {
        DirectoryInfo directory = new DirectoryInfo(path);
        fileList.AddRange(directory.GetFiles());

        DirectoryInfo[] directoryInfos = directory.GetDirectories();
        for(int i = 0; i < directoryInfos.Length; i++)
        {
            GetAllFile(directoryInfos[i].FullName, fileList);
        }
    }

    public static FileInfo GetTargetFile(string path, string name)
    {
        List<FileInfo> fileList = new List<FileInfo>();
        GetAllFile(path, fileList);

        string realName = name + ".lua";
        for(int i = 0; i < fileList.Count; i++)
        {
            if(fileList[i].Name == realName)
            {
                return fileList[i];
            }
        }

        return null;
    }
}
