using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using XLua;

public class MouseEventPointer : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [CSharpCallLua]
    public delegate void MouseEvent(GameObject obj, PointerEventData data);

    public MouseEvent mClickAction;
    public MouseEvent mDownAction;
    public MouseEvent mUpAction;
    public MouseEvent mExitAction;
    public MouseEvent mEnterAction;

    public void OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData)
    {
        if(mDownAction != null)
        {
            mDownAction(gameObject, eventData);
        }
    }

    public void OnPointerUp(UnityEngine.EventSystems.PointerEventData eventData)
    {
        if (mUpAction != null)
        {
            mUpAction(gameObject, eventData);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (mEnterAction != null)
        {
            mEnterAction(gameObject, eventData);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (mExitAction != null)
        {
            mExitAction(gameObject, eventData);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (mClickAction != null)
        {
            mClickAction(gameObject, eventData);
        }
    }
}
