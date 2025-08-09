using System;
using System.Drawing;

public class MazeHelper
{
    int height;
    int width;
    Graphics graph;
    public bool[,] rows;
    public bool[,] colunms;
    Random rnds = new Random(Guid.NewGuid().GetHashCode());

    public MazeHelper(int h, int w, Graphics g)
    {
        height = h;
        width = w;
        graph = g;
        rows = new bool[w, h + 1];
        colunms = new bool[w + 1, h];
    }

    void setBorder()
    {
        for (int i = 0; i < width; i++)
        {
            rows[i, 0] = true;
            rows[i, height] = true;
        }
        for (int i = 0; i < height; i++)
        {
            colunms[0, i] = true;
            colunms[width, i] = true;
        }
        //rows[0, 0] = false;
        //rows[width - 1, height] = false;
    }
    private int rnd(int start, int end)
    {
        return rnds.Next(start, end + 1);
    }  //生成区间在[start,end]的随机数

    void dg(Rectangle rect)  //递归分割算法生成迷宫
    {
        if (rect.Width <= 1 || rect.Height <= 1)
        {
            return;  //如果区域边小于等于1就退出递归
        }

        int col = rnd(1, rect.Width - 1);
        int row = rnd(1, rect.Height - 1);  //划分出随机十字

        void SetRow(int x, int y, bool value)
        {
            rows[x + rect.X, y + rect.Y] = value;
        }
        void SetColnum(int x, int y, bool value)
        {
            colunms[x + rect.X, y + rect.Y] = value;
        } //定义两个匿名方法来根据相对坐标设置值

        for (int i = 0; i < rect.Width; i++)  //画出横线
        {
            SetRow(i, row, true);
        }
        for (int i = 0; i < rect.Height; i++) //画出竖线
        {
            SetColnum(col, i, true);
        }

        int kg = rnd(0, 3); //用随机数决定闭口区域
        if (kg != 0) //left
        {
            SetRow(rnd(0, col - 1), row, false);
        }
        if (kg != 1) //top
        {
            SetColnum(col, rnd(0, row - 1), false);
        }
        if (kg != 2) //down
        {
            SetColnum(col, rnd(row, rect.Height - 1), false);
        }
        if (kg != 3) //right
        {
            SetRow(rnd(col, rect.Width - 1), row, false);
        }

        //递归下一个处理程序
        dg(new Rectangle() { X = rect.X, Y = rect.Y, Width = col, Height = row }); //A1
        dg(new Rectangle() { X = rect.X + col, Y = rect.Y, Width = rect.Width - col, Height = row }); //A2
        dg(new Rectangle() { X = rect.X, Y = rect.Y + row, Width = col, Height = rect.Height - row }); //B1
        dg(new Rectangle() { X = rect.X + col, Y = rect.Y + row, Width = rect.Width - col, Height = rect.Height - row }); //B2
    }

    public void Summon()
    {
        setBorder();
        dg(new Rectangle() { X = 0, Y = 0, Width = width, Height = height });
    }

    public void GraphToScreen()
    {
        Graph();
    }

    Pen gp = new Pen(Color.Green,5);

    void Graph()
    {
        graph.Clear(Color.Black);
        for (int y = 0; y <= height; y++)
        {
            for (int x = 0; x < width; x++)  //画出所有横线
            {
                if (rows[x, y])
                    graph.DrawLine(gp, x * 10, y * 10, (x + 1) * 10, y * 10);
            }
        }

        for (int y = 0; y < height; y++)  //画出所有竖线
        {
            for (int x = 0; x <= width; x++)
            {
                if (colunms[x, y])
                    graph.DrawLine(gp, x * 10, y * 10, x * 10, (y + 1) * 10);
            }
        }
    }
    public Bitmap GetBmp(int jgWidth)
    {
        var ret = new Bitmap(width * jgWidth + 2, height * jgWidth + 2);
        var rg = Graphics.FromImage(ret);
        //rg.Clear(Color.Black);
        for (int y = 0; y <= height; y++)
        {
            for (int x = 0; x < width; x++)  //画出所有横线
            {
                if (rows[x, y])
                    rg.DrawLine(gp, x * jgWidth, y * jgWidth, (x + 1) * jgWidth, y * jgWidth);
            }
        }

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x <= width; x++)  //画出所有竖线
            {
                if (colunms[x, y])
                    rg.DrawLine(gp, x * jgWidth, y * jgWidth, x * jgWidth, (y + 1) * jgWidth);
            }
        }
        rg.Dispose();
        return ret;
    }
}