using UnityEngine;

public class CGridPosition
{
    #region 成员变量
    int m_iX;
    int m_iY;
    #endregion

    #region 属性访问
    public int x
    {
        get { return m_iX; }
        set { m_iX = value; }
    }
    public int y
    {
        get { return m_iY; }
        set { m_iY = value; }
    }
    public int width
    {
        get { return m_iX; }
        set { m_iX = value; }
    }
    public int length
    {
        get { return m_iY; }
        set { m_iY = value; }
    }
    public int row
    {
        get { return m_iY; }
        set { m_iY = value; }
    }
    public int col
    {
        get { return m_iX; }
        set { m_iX = value; }
    }
    public float magnitude
    {
        get { return Mathf.Sqrt(m_iX * m_iX + m_iY * m_iY); }
    }
    #endregion

    #region 构造函数
    public CGridPosition()
    {
        m_iX = 0;
        m_iY = 0;
    }
    public CGridPosition(CGridPosition gp)
    {
        m_iX = gp.m_iX;
        m_iY = gp.m_iY;
    }
    public CGridPosition(int iX, int iY)
    {
        m_iX = iX;
        m_iY = iY;
    }
    #endregion

    #region 外部访问
    public void Set(int x, int y)
    {
        m_iX = x;
        m_iY = y;
    }
    public override string ToString()
    {
        return "(" + m_iX.ToString() + ", " + m_iY.ToString() + ")";
    }
    #endregion

    #region 操作符重载
    public static CGridPosition operator +(CGridPosition l, CGridPosition r)
    {
        return new CGridPosition(l.x + r.x, l.y + r.y);
    }
    public static CGridPosition operator -(CGridPosition l, CGridPosition r)
    {
        return new CGridPosition(l.x - r.x, l.y - r.y);
    }
    public static bool operator ==(CGridPosition l, CGridPosition r)
    {
        return l.x == r.x && l.y == r.y;
    }
    public static bool operator !=(CGridPosition l, CGridPosition r)
    {
        return l.x != r.x || l.y != r.y;
    }
    #endregion
}
