using System.Collections.Generic;

//思路就是：将世界中的所有物体，根据其坐标位置，分别将其划分到不同的网格之中。
//好处：
//1.比如我们要探测附近敌人，我们不用遍历整个世界中的物体，然后判断是否是敌人。
//而只用，探测我们所处网格和该网格附近网格总的物体，然后判断是否是敌人。减少遍历。
//2.对于slg的游戏，我们的世界地图很大，但是我们摄像机只能看到某一块或这几块地形，所以我们就不用请求所有的地块，只有请求当前摄像机所在附近的地块数据，进行显示和更新就可以了。
//3.对于碰撞检测，我们也可以只检测玩家所在网格的物体，其他距离很远根本不可能发生碰撞。

//不足：
//当世界中的物体都聚集在一个网格时，这时候网格划分就没有任何意义了，因为还是要遍历世界中的所有物体。
public class GridManager
{
    public class Unit
    {
        public float mX;
        public float mY;
    }

    public class Grid
    {
        public List<Unit> mUnits = new List<Unit>();
    }


    public static GridManager Instance = new GridManager();
    private static int mGridHeight = 100;
    private static int mGridWidth = 100;
    private Grid[,] mGrids;

    public GridManager()
    {

    }

    public void Init(int width, int height)
    {
        int widthNum = width / mGridWidth + 1;
        int heightNum = height / mGridHeight + 1;
        mGrids = new Grid[widthNum, heightNum];
    }

    public void AddUnit(Unit unit)
    {
        int indexX, indexY;
        GetGridIndex(unit, out indexX, out indexY);
        mGrids[indexX, indexY].mUnits.Add(unit);
    }

    public void RemoveUnit(Unit unit)
    {
        int indexX, indexY;
        GetGridIndex(unit, out indexX, out indexY);
        mGrids[indexX, indexY].mUnits.Remove(unit);
    }

    private Grid GetUnitGrid(Unit unit)
    {
        int indexX, indexY;
        GetGridIndex(unit, out indexX, out indexY);
        return mGrids[indexX, indexY];
    }

    public void MoveUnit(Unit unit, float x, float y)
    {
        int oldX = 0, oldY = 0;
        GetGridIndex(unit, out oldX, out oldY);

        int newX = 0, newY = 0;
        GetGridIndex(x, y, out newX, out newY);
        
        if(oldX == newX && oldY == newY)
        {
            return;
        }

        RemoveUnit(unit);
        AddUnit(unit);
    }

    private void GetGridIndex(Unit unit, out int indexX, out int indexY)
    {
        GetGridIndex(unit.mX, unit.mY, out indexX, out indexY);
    }

    private void GetGridIndex(float x, float y, out int indexX, out int indexY)
    {
        indexX = (int)x / mGridWidth;
        indexY = (int)y / mGridHeight;
    }
}
