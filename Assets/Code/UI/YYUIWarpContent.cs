using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

[DisallowMultipleComponent]
public class YYUIWarpContent : MonoBehaviour {

	public delegate void OnInitializeItem(GameObject go,int dataIndex);

	public OnInitializeItem onInitializeItem;
    //滑动到底部
    public Action<int> mOnReachBottom;

    public enum Arrangement
	{
		Horizontal,
		Vertical,
	}
    
	/// <summary>
	/// Type of arrangement -- vertical or horizontal.
	/// </summary>

	public Arrangement arrangement = Arrangement.Horizontal;

	/// <summary>
	/// Maximum children per line.
	/// If the arrangement is horizontal, this denotes the number of columns.
	/// If the arrangement is vertical, this stands for the number of rows.
	/// </summary>
	[Range(1,50)]
	public int maxPerLine = 1;
    public float paddingTop = 0;
	/// <summary>
	/// The width of each of the cells.
	/// </summary>

	public float cellWidth = 200f;

	/// <summary>
	/// The height of each of the cells.
	/// </summary>

	public float cellHeight = 200f;

	/// <summary>
	/// The Width Space of each of the cells.
	/// </summary>
	[Range(0, 60)]
	public float cellWidthSpace = 0f;

	/// <summary>
	/// The Height Space of each of the cells.
	/// </summary>
	[Range(0, 50)]
	public float cellHeightSpace = 0f;


	[Range(0,30)]
	public int viewCount = 5;

	public ScrollRect scrollRect;

	public RectTransform content;

	public GameObject goItemPrefab;

	private int _dataCount;

	private int curScrollPerLineIndex = -1;

	private List<YYUIWarpContentItem> listItem;

	private Queue<YYUIWarpContentItem> unUseItem;

    public int dataCount
    {
        get
        {
            return _dataCount;
        }
    }

    public bool is_init = false;

	void Awake(){
        if (listItem == null)
		    listItem = new List<YYUIWarpContentItem> ();

        if (unUseItem == null)
            unUseItem = new Queue<YYUIWarpContentItem> ();
	}

    public bool isNeedInit(int p_now_count)
    {
        return !is_init || p_now_count != _dataCount;
    }

    public void Init(int dataCount, OnInitializeItem initfun, Action<int> OnReachBottom = null, bool initContentPos = true)
	{
        is_init = true;

        onInitializeItem = initfun;
        mOnReachBottom = OnReachBottom;

        if (scrollRect == null || content == null || goItemPrefab == null) {
			Debug.LogError ("异常:请检测<"+gameObject.name+">对象上UIWarpContent对应ScrollRect、Content、GoItemPrefab 是否存在值...."+scrollRect+" _"+content+"_"+goItemPrefab);
			return;
		}

  //      if (dataCount <= 0)
		//{
		//	return;
		//}
        //是否重置content的位置
        if(initContentPos)
        {
            InitContent();
        }
		setDataCount (dataCount);
       
		scrollRect.onValueChanged.RemoveAllListeners();
		scrollRect.onValueChanged.AddListener (onValueChanged);

        if (listItem == null)
            listItem = new List<YYUIWarpContentItem>();

        if (unUseItem == null)
            unUseItem = new Queue<YYUIWarpContentItem>();

        unUseItem.Clear();
        if (content.childCount > 0)
        {
            for(int i = 0; i < content.childCount; i++)
            {
                YYUIWarpContentItem item = content.GetChild(i).GetComponent<YYUIWarpContentItem>();
                unUseItem.Enqueue(item);
                item.Index = -10;
            }
        }
		listItem.Clear ();
        // 
		setUpdateRectItem (getCurScrollPerLineIndex());
	}

	private void setDataCount(int count)
	{
		if (_dataCount == count) 
		{
			return;
		}
		_dataCount = count;
		setUpdateContentSize ();
	}


    public void ForceReflush()
    {
        int _curScrollPerLineIndex = getCurScrollPerLineIndex();

        if (_curScrollPerLineIndex == curScrollPerLineIndex)
        {
            return;
        }

        setUpdateRectItem(_curScrollPerLineIndex);

    }

	public void onValueChanged(Vector2 vt2){
            switch (arrangement)
            {
                case Arrangement.Vertical:
                if (Mathf.Abs(content.anchoredPosition.y) < 1)
                    break;
                float y = vt2.y;
                    if (y >= 1.0f || y < -0.01f)
                    {
                        return;
                    }
                    break;
                case Arrangement.Horizontal:
                if (Mathf.Abs(content.anchoredPosition.x) >1)
                    break;
                float x = vt2.x;
                    if (x < -0.01f || x >= 1.0f)
                    {
                        return;
                    }

                    break;
            }
        



        int _curScrollPerLineIndex = getCurScrollPerLineIndex ();

        if (_curScrollPerLineIndex == curScrollPerLineIndex){
			return;
		}

        int viewMaxIndex = _curScrollPerLineIndex + viewCount;
        if (mOnReachBottom != null && viewMaxIndex >= dataCount)
        {
            mOnReachBottom(viewMaxIndex - 1);
        }
        
		setUpdateRectItem (_curScrollPerLineIndex);
	}

	/**
	 * @des:设置更新区域内item
	 * 功能:
	 * 1.隐藏区域之外对象
	 * 2.更新区域内数据
	 */
	public void setUpdateRectItem(int scrollPerLineIndex)
	{
		if (scrollPerLineIndex < 0) 
		{
			return;
		}
		curScrollPerLineIndex = scrollPerLineIndex;
		int startDataIndex = curScrollPerLineIndex * maxPerLine;
		int endDataIndex = (curScrollPerLineIndex + viewCount) * maxPerLine;
        //移除
        for (int i = listItem.Count - 1; i >= 0; i--)
        {
            YYUIWarpContentItem item = listItem[i];
            int index = item.Index;
            if (index < startDataIndex || index >= endDataIndex)
            {
                item.Index = -10; //-1 改到 -10 避免下拉的时候被看到
                listItem.Remove(item);
                unUseItem.Enqueue(item);
            }
        }
        //显示
        for (int dataIndex = startDataIndex;dataIndex<endDataIndex;dataIndex++)
		{
			if (dataIndex >= _dataCount) 
			{
				continue;
			}
			if (isExistDataByDataIndex (dataIndex)) 
			{
				continue;
			}
			createItem (dataIndex);
		}
	}



    /**
	 * @des:添加当前数据索引数据
	 */
    public void AddItem(int dataIndex)
    {
        if (dataIndex < 0 || dataIndex > _dataCount)
        {
            return;
        }

        if (listItem.Count == 0)
        {
            createItem(0);
            setDataCount(_dataCount + 1);
            setUpdateRectItem(getCurScrollPerLineIndex());
            return;
        }

		//检测是否需添加gameObject
		bool isNeedAdd = false;
		for (int i = listItem.Count-1; i>=0 ; i--) {
			YYUIWarpContentItem item = listItem [i];
			if (item.Index >= (_dataCount - 1)) {
				isNeedAdd = true;
				break;
			}
		}
		setDataCount (_dataCount+1);

		if (isNeedAdd) {
			for (int i = 0; i < listItem.Count; i++) {
				YYUIWarpContentItem item = listItem [i];
				int oldIndex = item.Index;
				if (oldIndex>=dataIndex) {
					item.Index = oldIndex+1;
				}
				item = null;
			}
			setUpdateRectItem (getCurScrollPerLineIndex());
		} else {
			//重新刷新数据
			for (int i = 0; i < listItem.Count; i++) {
				YYUIWarpContentItem item = listItem [i];
				int oldIndex = item.Index;
				if (oldIndex>=dataIndex) {
					item.Index = oldIndex;
				}
				item = null;
			}
		}

	}

	/**
	 * @des:删除当前数据索引下数据
	 */
	public void DelItem(int dataIndex){
		if (dataIndex < 0 || dataIndex >= _dataCount) {
			return;
		}
		//删除item逻辑三种情况
		//1.只更新数据，不销毁gameObject,也不移除gameobject
		//2.更新数据，且移除gameObject,不销毁gameObject
		//3.更新数据，销毁gameObject

		bool isNeedDestroyGameObject = (listItem.Count >= _dataCount);
		setDataCount (_dataCount-1);

		for (int i = listItem.Count-1; i>=0 ; i--) {
			YYUIWarpContentItem item = listItem [i];
			int oldIndex = item.Index;
			if (oldIndex == dataIndex) {
				listItem.Remove (item);
				if (isNeedDestroyGameObject) {
					GameObject.Destroy (item.gameObject);
				} else {
					item.Index = -10; //-1 改到 -10 避免下拉的时候被看到
                    unUseItem.Enqueue (item);			
				}
			}
			if (oldIndex > dataIndex) {
				item.Index = oldIndex - 1;
			}
		}
		setUpdateRectItem(getCurScrollPerLineIndex());
	}
    /**
	 * @des:刷新指定索引的显示物体
	 */
    public void RefreshItem(int dataIndex)
    {
        if (onInitializeItem == null || listItem == null || listItem.Count <= 0) return;
        if (dataIndex < 0 || dataIndex >= _dataCount) return;
        YYUIWarpContentItem item;
        for (int i = listItem.Count - 1; i >= 0; i--)
        {
            item = listItem[i];
            if (item.Index == dataIndex)
            {
                onInitializeItem(item.gameObject,item.Index);
                return;
            }
        }
    }
    /**
	 * @des:刷新显示中的物体
	 */
    public void RefreshShowItems()
    {
        if (onInitializeItem == null || listItem == null || listItem.Count <= 0) return;
        YYUIWarpContentItem item;
        for (int i = listItem.Count - 1; i >= 0; i--)
        {
            item = listItem[i];
            onInitializeItem(item.gameObject, item.Index);  
        }
    }

	/**
	 * @des:获取当前index下对应Content下的本地坐标
	 * @param:index
	 * @内部使用
	*/
	public Vector3 getLocalPositionByIndex(int index){
		float x = 0f;
		float y = 0f;
		float z = 0f;
		switch (arrangement) {
		case Arrangement.Horizontal: //水平方向
			x = (index / maxPerLine) * (cellWidth + cellWidthSpace);
            y = -(index % maxPerLine) * (cellHeight + cellHeightSpace) + paddingTop;
			break;
		case  Arrangement.Vertical://垂着方向
			x =  (index % maxPerLine) * (cellWidth + cellWidthSpace);
            y = -(index / maxPerLine) * (cellHeight + cellHeightSpace) + paddingTop;
			break;
		}
		return new Vector3(x,y,z);
	}

	/**
	 * @des:创建元素
	 * @param:dataIndex
	 */
	private void createItem(int dataIndex){
		YYUIWarpContentItem item;
		if (unUseItem.Count > 0) {
			item = unUseItem.Dequeue();
		} else {
			item = addChild (goItemPrefab, content).AddComponent<YYUIWarpContentItem>();
           // item.name = string.Format("YYUIWarpContentItem{0}", dataIndex);
        }
        listItem.Add(item);
        item.WarpContent = this;
		item.Index = dataIndex;
	}

	/**
	 * @des:当前数据是否存在List中
	 */
	private bool isExistDataByDataIndex(int dataIndex){
		if (listItem == null || listItem.Count <= 0) {
			return false;
		}
		for (int i = 0; i < listItem.Count; i++) {
			if (listItem [i].Index == dataIndex) {
				return true;
			}
		}
		return false;
	}


	/**
	 * @des:根据Content偏移,计算当前开始显示所在数据列表中的行或列
	 */
	private int getCurScrollPerLineIndex()
	{
        switch (arrangement)
        {
            case Arrangement.Horizontal: //水平方向
                if (content.anchoredPosition.x > 0) return 0;
                return Mathf.FloorToInt(-content.anchoredPosition.x / (cellWidth + cellWidthSpace));
            case Arrangement.Vertical://垂着方向
                if (content.anchoredPosition.y < 0) return 0;
                return Mathf.FloorToInt(content.anchoredPosition.y / (cellHeight + cellHeightSpace));
        }
        return 0;

    }

	/**
	 * @des:更新Content SizeDelta
	 */
	private void setUpdateContentSize()
	{
		int lineCount = Mathf.CeilToInt((float)_dataCount/maxPerLine);
		switch (arrangement)
		{
		 case Arrangement.Horizontal:
			content.sizeDelta = new Vector2(cellWidth * lineCount + cellWidthSpace * (lineCount - 1), content.sizeDelta.y);
			break;
		 case Arrangement.Vertical:
			content.sizeDelta = new Vector2(content.sizeDelta.x, cellHeight * lineCount + cellHeightSpace * (lineCount - 1));
			break;
		}


	}

    private void InitContent()
    {
        switch (arrangement)
        {
            case Arrangement.Horizontal:
                content.localPosition = new Vector3(0, content.localPosition.y, content.localPosition.z);
                break;
            case Arrangement.Vertical:
                content.localPosition = new Vector3(content.localPosition.x, 0, content.localPosition.z);
               
                break;
        }
    }


	/**
	 * @des:实例化预设对象 、添加实例化对象到指定的子对象下
	 */
	private GameObject addChild(GameObject goPrefab,Transform parent)
	{
		if (goPrefab == null || parent == null) {
			Debuger.LogError("other","异常。UIWarpContent.cs addChild(goPrefab = null  || parent = null)");
			return null;
		}
		GameObject goChild = GameObject.Instantiate (goPrefab) as GameObject;
        goChild.SetActive(true);
		goChild.layer = parent.gameObject.layer;
		goChild.transform.SetParent (parent,false);

		return goChild;
	}

	void OnDestroy(){
		
		scrollRect = null;
		content = null;
		goItemPrefab = null;
		onInitializeItem = null;

		listItem.Clear ();
		unUseItem.Clear ();

		listItem = null;
		unUseItem = null;

	}
}
