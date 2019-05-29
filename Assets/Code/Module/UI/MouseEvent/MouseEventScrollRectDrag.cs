using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XLua;

public class MouseEventScrollRectDrag : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    [CSharpCallLua]
    public delegate void MouseEvent(GameObject obj, PointerEventData data);
    public MouseEvent mMouseDrag;
    public MouseEvent mMouseBeginDrag;
    public MouseEvent mMouseEngDrag;
    private LoopScrollRect mScrollRect;

    public void Awake()
    {
        mScrollRect = GetComponentInParent<LoopScrollRect>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log("OnDrag");
        if (mScrollRect != null)
        {
            mScrollRect.OnDrag(eventData);
        }

        if (mMouseDrag != null)
        {
            mMouseDrag(gameObject, eventData);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //Debug.Log("OnBeginDrag");
        if (mScrollRect != null)
        {
            mScrollRect.OnBeginDrag(eventData);
        }

        if (mMouseBeginDrag != null)
        {
            mMouseBeginDrag(gameObject, eventData);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //Debug.Log("OnEndDrag");
        if (mScrollRect != null)
        {
            mScrollRect.OnEndDrag(eventData);
        }

        if (mMouseEngDrag != null)
        {
            mMouseEngDrag(gameObject, eventData);
        }
    }
}
