using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class TestHierarchy {
    
    static TestHierarchy()
    {
        EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyWindow;
    }

    public static void OnHierarchyWindow(int instanceID, Rect selectionRect)
    {
        //GameObject obj = (GameObject)EditorUtility.InstanceIDToObject(instanceID);
        //if (obj == null) return;
        //Debug.Log(obj.name);
    }
}
