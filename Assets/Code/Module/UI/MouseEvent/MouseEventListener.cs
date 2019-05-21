using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using XLua;

public class MouseEventListener : MonoBehaviour, IPointerClickHandler, IPointerUpHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, IDragHandler
{
    [CSharpCallLua]
    public delegate void MouseEvent(GameObject obj);

    public MouseEvent mOnMouseClick;
    public MouseEvent mOnMouseUp;
    public MouseEvent mOnMouseDown;
    public MouseEvent mOnMouseExit;
    public MouseEvent mOnMouseEnter;
    public MouseEvent mOnMouseDrag;

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("OnPointerClick");
        if(mOnMouseClick != null)
        {
            mOnMouseClick(gameObject);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("OnPointerUp");
        if (mOnMouseUp != null)
        {
            mOnMouseUp(gameObject);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("OnPointerDown");
        if (mOnMouseDown != null)
        {
            mOnMouseDown(gameObject);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("OnPointerExit");
        if (mOnMouseExit != null)
        {
            mOnMouseExit(gameObject);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("OnPointerEnter");
        if (mOnMouseEnter != null)
        {
            mOnMouseEnter(gameObject);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("OnDrag");
        if (mOnMouseDrag != null)
        {
            mOnMouseDrag(gameObject);
        }
    }
}
