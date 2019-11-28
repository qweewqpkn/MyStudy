using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAutoRenderOrder : MonoBehaviour {

    public int mOffsetSortingOrder;
    public int mOffsetRenderQueue;
    public string mSortingLayerName;
    public int mSpriteMaskFrontOffsetOrder;
    public int mSpriteMaskBackOffsetOrder;
    private Canvas mCanvas;
    private int mCanvasOrder;
    private List<Renderer> mChildRendererList = new List<Renderer>();
    private List<SpriteMask> mSpriteMaskList = new List<SpriteMask>();
    private Mask mMask;
    private RectMask2D mMask2D;

	// Use this for initialization
	void Start () {
        mCanvas = GetComponentInParent<Canvas>();
        mCanvasOrder = mCanvas.sortingOrder;
        Renderer[] rendererList = GetComponentsInChildren<Renderer>();
        if(rendererList.Length > 0)
        {
            mChildRendererList.AddRange(rendererList);
            SetChildSortingOrder();
        }
        SpriteMask[] spriteMaskList = GetComponentsInChildren<SpriteMask>();
        if (spriteMaskList.Length > 0)
        {
            mSpriteMaskList.AddRange(spriteMaskList);
            SetSpriteMaskSortingOrder();
        }
        if (mCanvas == null)
        {
            Debug.LogError("UIAutoRenderOrder canvas is null");
        }
	}

    private void SetClipRect()
    {
        if (mMask == null)
        {
            mMask = GetComponentInParent<Mask>();
        }
        if (mMask != null)
        {
            Vector3[] corner = new Vector3[4];
            mMask.rectTransform.GetWorldCorners(corner);
            for (int i = 0; i < mChildRendererList.Count; i++)
            {
                mChildRendererList[i].sharedMaterial.SetVector("_ClipArea", new Vector4(corner[0].x, corner[2].x, corner[0].y, corner[2].y));
            }
        }

        if (mMask2D == null)
        {
            mMask2D = GetComponentInParent<RectMask2D>();
        }
        if (mMask2D != null)
        {
            Vector3[] corner = new Vector3[4];
            mMask2D.rectTransform.GetWorldCorners(corner);
            for (int i = 0; i < mChildRendererList.Count; i++)
            {
                mChildRendererList[i].sharedMaterial.SetVector("_ClipArea", new Vector4(corner[0].x, corner[2].x, corner[0].y, corner[2].y));
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
		if(mCanvas != null)
        {
            if(mCanvas.sortingOrder != mCanvasOrder)
            {
                mCanvasOrder = mCanvas.sortingOrder;
                SetChildSortingOrder();
                SetSpriteMaskSortingOrder();
            }

            SetClipRect();
        }
	}

    private void SetSpriteMaskSortingOrder()
    {
        for (int i = 0; i < mSpriteMaskList.Count; i++)
        {
            if (mSpriteMaskList[i] != null)
            {
                if (string.IsNullOrEmpty(mSortingLayerName))
                {
                    mSpriteMaskList[i].frontSortingLayerID = 0;
                    mSpriteMaskList[i].backSortingLayerID = 0;
                }
                else
                {
                    mSpriteMaskList[i].frontSortingLayerID = SortingLayer.NameToID(mSortingLayerName);
                    mSpriteMaskList[i].backSortingLayerID = SortingLayer.NameToID(mSortingLayerName);
                }

                mSpriteMaskList[i].frontSortingOrder = mCanvasOrder + mSpriteMaskFrontOffsetOrder;
                mSpriteMaskList[i].backSortingOrder = mCanvasOrder + mSpriteMaskBackOffsetOrder;
            }
        }
    }

    private void SetChildSortingOrder()
    {
        for (int i = 0; i < mChildRendererList.Count; i++)
        {
            if(mChildRendererList[i] != null)
            {
                if(string.IsNullOrEmpty(mSortingLayerName))
                {
                    mChildRendererList[i].sortingLayerID = 0;
                }
                else
                {
                    mChildRendererList[i].sortingLayerID = SortingLayer.NameToID(mSortingLayerName);
                }
                mChildRendererList[i].sortingOrder = mCanvasOrder + mOffsetSortingOrder;
                if(mChildRendererList[i].sharedMaterial != null)
                {
                    mChildRendererList[i].sharedMaterial.renderQueue = mChildRendererList[i].sharedMaterial.renderQueue + mOffsetRenderQueue;
                }
            }
        }
    }
}
