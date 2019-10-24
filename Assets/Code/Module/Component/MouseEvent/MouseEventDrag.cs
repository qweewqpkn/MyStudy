using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using XLua;

public class MouseEventDrag : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    [CSharpCallLua]
    public delegate void MouseEvent(GameObject obj, PointerEventData eventData);
    public MouseEvent mMouseDrag;
    public MouseEvent mMouseBeginDrag;
    public MouseEvent mMouseEngDrag;

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log("OnDrag");
        if (mMouseDrag != null)
        {
            mMouseDrag(gameObject, eventData);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //Debug.Log("OnBeginDrag");
        if (mMouseBeginDrag != null)
        {
            mMouseBeginDrag(gameObject, eventData);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //Debug.Log("OnEndDrag");
        if (mMouseEngDrag != null)
        {
            mMouseEngDrag(gameObject, eventData);
        }
    }
}
