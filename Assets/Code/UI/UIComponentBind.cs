//using LuaInterface;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIComponentBind : MonoBehaviour {

    [Serializable]
    public class ComponentData
    {
        public string name;
        public string type;
        public string path;
        public UnityEngine.Object component;
    }

    public string mObjName;
    public List<ComponentData> mComponentDataList = new List<ComponentData>();

    //运行时将控件赋值给lua变量
    //public static void BindToLua(GameObject obj, LuaTable table)
    //{
    //    if(obj == null || table == null)
    //    {
    //        return;
    //    }
   
    //    UIComponentBind bind = obj.GetComponent<UIComponentBind>();
    //    if(bind == null)
    //    {
    //        return;
    //    }

    //    for(int i = 0; i < bind.mComponentDataList.Count; i++)
    //    {
    //        ComponentData data = bind.mComponentDataList[i];
    //        switch(data.type)
    //        {
    //            case "Button":
    //                {
    //                    table[data.name] = data.component as Button;
    //                }
    //                break;
    //            case "Dropdown":
    //                {
    //                    table[data.name] = data.component as Dropdown;
    //                }
    //                break;
    //            case "InputField":
    //                {
    //                    table[data.name] = data.component as InputField;
    //                }
    //                break;
    //            case "Slider":
    //                {
    //                    table[data.name] = data.component as Slider;
    //                }
    //                break;
    //            case "Toggle":
    //                {
    //                    table[data.name] = data.component as Toggle;
    //                }
    //                break;
    //            case "GridLayoutGroup":
    //                {
    //                    table[data.name] = data.component as GridLayoutGroup;
    //                }
    //                break;
    //            case "VerticalLayoutGroup":
    //                {
    //                    table[data.name] = data.component as VerticalLayoutGroup;
    //                }
    //                break;
    //            case "ScrollRect":
    //                {
    //                    table[data.name] = data.component as ScrollRect;
    //                }
    //                break;
    //            case "Image":
    //                {
    //                    table[data.name] = data.component as Image;
    //                }
    //                break;
    //            case "ImageExt":
    //                {
    //                    table[data.name] = data.component as ImageExt;
    //                }
    //                break;
    //            case "RawImage":
    //                {
    //                    table[data.name] = data.component as RawImage;
    //                }
    //                break;
    //            case "RawImageExt":
    //                {
    //                    table[data.name] = data.component as RawImageExt;
    //                }
    //                break;
    //            case "Text":
    //                {
    //                    table[data.name] = data.component as Text;
    //                }
    //                break;
    //            case "RectTransform":
    //                {
    //                    table[data.name] = data.component as RectTransform;
    //                }
    //                break;
    //            case "Transform":
    //                {
    //                    table[data.name] = data.component as Transform;
    //                }
    //                break;
    //        }
    //    }
    //}
}
