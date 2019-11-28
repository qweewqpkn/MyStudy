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
    public MouseEvent mLongPressAction;
    public float mLongPressDurationTime = 1;
    private float mLongPressStartTime = 0;
    private bool mLongPressIsTrigger = false;
    private PointerEventData mLongPressEventData = null;
    private bool mIsPointerDown = false;

    public void Update()
    {
        if (mLongPressAction != null && mIsPointerDown && !mLongPressIsTrigger)
        {
            if (Time.time - mLongPressStartTime > mLongPressDurationTime)
            {
                mLongPressIsTrigger = true;
                mLongPressAction(gameObject, mLongPressEventData);
            }
        }
    }

    public void OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData)
    {
        if(mDownAction != null)
        {
            mDownAction(gameObject, eventData);
        }

        mLongPressStartTime = Time.time;
        mIsPointerDown = true;
        mLongPressIsTrigger = false;
        mLongPressEventData = eventData;
    }

    public void OnPointerUp(UnityEngine.EventSystems.PointerEventData eventData)
    {
        if (mUpAction != null)
        {
            mUpAction(gameObject, eventData);
        }

        mIsPointerDown = false;
        mLongPressIsTrigger = false;
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

        mIsPointerDown = false;
        mLongPressIsTrigger = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (mClickAction != null && !mLongPressIsTrigger)
        {
            mClickAction(gameObject, eventData);
        }
    }
}
