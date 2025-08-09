using System.Collections.Generic;
using System.Drawing;
using 迷宫游戏2_逃离先辈;

public class PathFinder
{
    bool[,] rows, colunms;
    enum fx
    {
        up,
        down,
        left,
        right,
    }
    struct BlockFx
    {
        public bool up;
        public bool down;
        public bool left;
        public bool right;
    }

    struct dgr
    {
        public bool status;
        public List<Point> curPath;
    }
    List<Point> rus;
    public Point dest;
    public PathFinder(bool[,] row,bool[,] colunm)
    {
        this.rows = row;
        this.colunms = colunm;
    }
    List<Point> findedPath = new List<Point>();
    public List<Point> GetPath(Point start)
    {
        findedPath.Clear();
        var blockOpened = GetMoveableNoFx(start);
        List<Point> ret = null;
        if (blockOpened.up)
        {
            var k = dg(new Point(start.X, start.Y - 1), fx.down); //前方向必须取反
            if (k.status)
            {
                //return k.curPath;

                ret = k.curPath;
            }
        }
        findedPath.Clear();
        if (blockOpened.down)
        {
            var k = dg(new Point(start.X, start.Y + 1), fx.up);
            if (k.status)
            {
                //return k.curPath;

                if (ret is null)
                {
                    ret = k.curPath;
                }
                else
                {
                    if (k.curPath.Count < ret.Count)
                    {
                        ret = k.curPath;
                    }
                }
            }
        }
        findedPath.Clear();
        if (blockOpened.left)
        {
            var k = dg(new Point(start.X - 1, start.Y), fx.right);
            if (k.status)
            {
                //return k.curPath;

                if (ret is null)
                {
                    ret = k.curPath;
                }
                else
                {
                    if (k.curPath.Count < ret.Count)
                    {
                        ret = k.curPath;
                    }
                }
            }
        }
        findedPath.Clear();
        if (blockOpened.right)
        {
            var k = dg(new Point(start.X + 1, start.Y), fx.left);
            if (k.status)
            {
                //return k.curPath;

                if (ret is null)
                {
                    ret = k.curPath;
                }
                else
                {
                    if (k.curPath.Count < ret.Count)
                    {
                        ret = k.curPath;
                    }
                }
            }
        }
        return ret;
    }
    BlockFx GetMoveable(Point p,fx f)
    {
        BlockFx ret = new BlockFx();
        try
        {
            if (!rows[p.X, p.Y] && f != fx.up)
                ret.up = true;
            if (!rows[p.X, p.Y + 1] && f != fx.down)
                ret.down = true;
            if (!colunms[p.X, p.Y] && f != fx.left)
                ret.left = true;
            if (!colunms[p.X + 1, p.Y] && f != fx.right)
                ret.right = true;
        }
        catch { }
        return ret;
    }  //返回的结构，true表示可以走的方向不包含f参数的方向
    BlockFx GetMoveableNoFx(Point p)
    {
        BlockFx ret = new BlockFx();
        if (!rows[p.X, p.Y])
            ret.up = true;
        if (!rows[p.X, p.Y + 1])
            ret.down = true;
        if (!colunms[p.X, p.Y])
            ret.left = true;
        if (!colunms[p.X + 1, p.Y])
            ret.right = true;

        return ret;
    }  //返回的结构，true表示可以走的方向
    int GetBlockOpenCount(Point p,fx f)
    {
        int ret = 0;
        try
        {
            
            if (!rows[p.X, p.Y] && f != fx.up)
                ret += 1;
            if (!rows[p.X, p.Y + 1] && f != fx.down)
                ret += 1;
            if (!colunms[p.X, p.Y] && f != fx.left)
                ret += 1;
            if (!colunms[p.X + 1, p.Y] && f != fx.right)
                ret += 1;
        }
        catch
        {
            return 0;
        }
        return ret;
    }  //返回的可用方向不包括背后（之前的路径）
    fx GetBack(fx f)
    {
        if (f == fx.up)
            return fx.down;
        else if (f == fx.down)
            return fx.up;
        else if (f == fx.left)
            return fx.right;
        else if(f == fx.right)         
            return fx.left;

        return fx.down;
    }
    dgr dg(Point p,fx lfx)
    {
        fx sfx = lfx;
        List<Point> curlst = new List<Point>();
        Point curP = new Point(p.X, p.Y);
        while (true)
        {
            if(findedPath.Contains(new Point(curP.X,curP.Y)))
            {
                return new dgr { status = false };
            }
            else
            {
                findedPath.Add(new Point(curP.X, curP.Y));
            }
            BlockFx blockOpened = GetMoveable(curP,sfx);
            int moveCount = GetBlockOpenCount(curP, sfx);
            curlst.Add(new Point(curP.X,curP.Y));
            //pushc += 1;
            //dd.Text = pushc.ToString() + "  " + curP.ToString() + "  " + $"{blockOpened.up}:{blockOpened.down}:{blockOpened.left}:{blockOpened.right}";
            //gg.DrawEllipse(new Pen(new SolidBrush(Color.Red), 5), new Rectangle(curP.X * 20, curP.Y * 20, 20, 20));
            if (curP == dest)
            {
                return new dgr() { status = true, curPath = curlst };
            }
            if (moveCount == 0) //没路了，直接返回死路
            {
                return new dgr() { status = false };
            }
            else if (moveCount == 1) //还可以继续往前走
            {
                if(blockOpened.up)
                {
                    curP.Y -= 1;
                    sfx = fx.down;
                }else if(blockOpened.down)
                {
                    curP.Y += 1;
                    sfx = fx.up;
                }else if(blockOpened.left)
                {
                    curP.X -= 1;
                    sfx = fx.right;
                }else if(blockOpened.right)
                {
                    curP.X += 1;
                    sfx = fx.left;
                }
                continue;
            }
            else //有分岔路，开递归分头行动
            {
                               
                if (blockOpened.left)
                {
                    var k = dg(new Point(curP.X - 1, curP.Y), fx.right);
                    if (k.status)
                    {
                        curlst.AddRange((IEnumerable<Point>)k.curPath);
                        return new dgr { status = true, curPath = curlst };

                    }
                }
                if (blockOpened.right)
                {
                    var k = dg(new Point(curP.X + 1, curP.Y), fx.left);
                    if (k.status)
                    {
                        curlst.AddRange((IEnumerable<Point>)k.curPath);
                        return new dgr { status = true, curPath = curlst };
                    }
                }
                if (blockOpened.down)
                {
                    var k = dg(new Point(curP.X, curP.Y + 1), fx.up);
                    if (k.status)
                    {
                        curlst.AddRange((IEnumerable<Point>)k.curPath);
                        return new dgr { status = true, curPath = curlst };
                    }
                }
                if (blockOpened.up)
                {
                    var k = dg(new Point(curP.X, curP.Y - 1), fx.down); //前方向必须取反
                    if (k.status)
                    {
                        curlst.AddRange((IEnumerable<Point>)k.curPath);
                        return new dgr { status = true, curPath = curlst };                       
                    }
                }
                
                
                return new dgr { status = false };
            }
        }
    }
}