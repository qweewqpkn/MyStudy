using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SceneObjClick : EventTrigger {

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);

        Debug.Log("Scene Obj Click");
    }

    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);

        Debug.Log("Scene Obj Drag");
    }
}
