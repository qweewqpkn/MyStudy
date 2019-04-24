//using LuaInterface;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XLua;

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

    public T GetComponentData<T>(string name) where T : UnityEngine.Object
    {
        T ret = null;
        for (int i = 0; i < mComponentDataList.Count; ++i)
        {
            if (mComponentDataList[i].name == name)
            {
                ret = mComponentDataList[i].component as T;
                break;
            }
        }
        return ret;
    }


    //运行时将控件赋值给lua变量
    public static void BindToLua(GameObject obj, LuaTable table)
    {
        if(obj == null || table == null)
        {
            return;
        }
   
        UIComponentBind bind = obj.GetComponent<UIComponentBind>();
        if(bind == null)
        {
            return;
        }

        table.Set("transform", obj.transform);
        table.Set("gameObject", obj);

        for (int i = 0; i < bind.mComponentDataList.Count; i++)
        {
            ComponentData data = bind.mComponentDataList[i];
            switch(data.type)
            {
                case "Button":
                    {
                        table.Set(data.name, data.component as Button);
                        //table[data.name] = data.component as Button;
                    }
                    break;
                case "Dropdown":
                    {
                        table.Set(data.name, data.component as Dropdown);
                        //table[data.name] = data.component as Dropdown;
                    }
                    break;
                case "InputField":
                    {
                        table.Set(data.name, data.component as InputField);
                        //table[data.name] = data.component as InputField;
                    }
                    break;
                case "Slider":
                    {
                        table.Set(data.name, data.component as Slider);
                        //table[data.name] = data.component as Slider;
                    }
                    break;
                case "Toggle":
                    {
                        table.Set(data.name, data.component as Toggle);
                        //table[data.name] = data.component as Toggle;
                    }
                    break;
                case "GridLayoutGroup":
                    {
                        table.Set(data.name, data.component as GridLayoutGroup);
                        //table[data.name] = data.component as GridLayoutGroup;
                    }
                    break;
                case "VerticalLayoutGroup":
                    {
                        table.Set(data.name, data.component as VerticalLayoutGroup);
                        //table[data.name] = data.component as VerticalLayoutGroup;
                    }
                    break;
                case "ScrollRect":
                    {
                        table.Set(data.name, data.component as ScrollRect);
                        //table[data.name] = data.component as ScrollRect;
                    }
                    break;
                case "Image":
                    {
                        table.Set(data.name, data.component as Image);
                        //table[data.name] = data.component as Image;
                    }
                    break;
                case "ImageExt":
                    {
                        //table.Set(data.name, data.component as ImageExt);
                        //table[data.name] = data.component as ImageExt;
                    }
                    break;
                case "RawImage":
                    {
                        table.Set(data.name, data.component as RawImage);
                        //table[data.name] = data.component as RawImage;
                    }
                    break;
                case "RawImageExt":
                    {
                        //table.Set(data.name, data.component as RawImageExt);
                        //table[data.name] = data.component as RawImageExt;
                    }
                    break;
                case "Text":
                    {
                        table.Set(data.name, data.component as Text);
                        //table[data.name] = data.component as Text;
                    }
                    break;
                case "RectTransform":
                    {
                        table.Set(data.name, data.component as RectTransform);
                        //table[data.name] = data.component as RectTransform;
                    }
                    break;
                case "Transform":
                    {
                        table.Set(data.name, data.component as Transform);
                        //table[data.name] = data.component as Transform;
                    }
                    break;
            }
        }
    }
}
