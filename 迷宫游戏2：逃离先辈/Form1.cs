using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace 迷宫游戏2_逃离先辈
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        GameEngine engine;
        string how_to_play_tips = "玩法：\r\n" +
            "在随机生成的迷宫中，你在游戏开始有30秒准备时间查看钥匙位置和迷宫结构\r\n" +
            "游戏目标：不被野兽先辈抓住的情况下搜集5个钥匙并从出口（右下角）逃生\r\n" +
            "注意：出口在收集完5个钥匙之后才会打开，每搜集一个钥匙野兽先辈移动将加快";



        void StartGame()
        {
            engine.drawList.Clear();
            var ldui = new LoadingPage(800, 600);
            engine.drawList.Add(ldui);
            Thread.Sleep(2000);
            GamingCtl.curPlayer = new player();
            GamingCtl.curHunter = new hunter();
            GamingCtl.curCamera = new GameCanera(GamingCtl.curPlayer, GamingCtl.curHunter);
            GamingCtl.curCamera.Init();
            ldui.PlayAllow = false;
            engine.drawList.Clear();
            engine.drawList.Add(GamingCtl.curCamera);
        }
        void ShowHowToPlay()
        {
            engine.drawList.Remove(stb);
            engine.drawList.Remove(htp);
            var title = new TextShow("这游戏怎么玩？", Color.Gold, new Rectangle(0, 100, 800, 80), new Font("微软雅黑", 25));
            var content = new TextShow(how_to_play_tips, Color.White, new Rectangle(0, 150, 800, 200), new Font("微软雅黑", 15));
            var msk = new Mask(Color.FromArgb(178, 0, 0, 0));
            EngineButton backbtn;
            backbtn = new EngineButton(null, new Point(450, 450), 150, 60, Color.FromArgb(150, 255, 255, 255), Color.Black);
            backbtn.cb = onBackClick;
            void onBackClick()
            {
                engine.drawList.Remove(title);
                engine.drawList.Remove(content);
                engine.drawList.Remove(msk);
                engine.drawList.Remove(backbtn);
                engine.drawList.Add(stb);
                engine.drawList.Add(htp);
            }
            backbtn.Text = "确定";
            engine.drawList.Add(msk);
            engine.drawList.Add(title);
            engine.drawList.Add(content);
            engine.drawList.Add(backbtn);
        }
        EngineButton stb, htp;

        public void DisplayMainUI()
        {
            var mpg = new WelcomePage();
            var stbtn = new EngineButton(() =>
            {
                GameEngine.CreateThread(StartGame);
            }, new Point(300, 430), 200, 60, Color.FromArgb(255, 240, 240, 240), Color.Black);
            stbtn.Text = "开始游戏";
            var htpbtn = new EngineButton(() =>
            {
                ShowHowToPlay();
            }, new Point(300, 500), 200, 60, Color.FromArgb(255, 240, 240, 240), Color.Black);
            htpbtn.Text = "查看玩法";
            engine.drawList.Add(mpg);
            engine.drawList.Add(stbtn);
            engine.drawList.Add(htpbtn);
            this.stb = stbtn;
            this.htp = htpbtn;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            engine = new GameEngine(this, 800, 600);
            engine.GameInit();
            GamingCtl.curEngine = engine;
            if (System.IO.File.Exists(Application.StartupPath + "\\FpsLimit.sign"))
            {
                engine.EnableSyncFrame = true;
            }
            if (System.IO.File.Exists(Application.StartupPath + "\\debugSize.txt"))
            {
                GamingCtl.mgWidth = int.Parse(System.IO.File.ReadAllText(Application.StartupPath + "\\debugSize.txt"));
            }
            else
            {
                GamingCtl.mgWidth = 10;
            }
            GamingCtl.GodMode = System.IO.File.Exists(Application.StartupPath + "\\GodMode.sign");
            GamingCtl.InitImageResource();
            GamingCtl.instanceForm = this;
            Form.CheckForIllegalCrossThreadCalls = false;
            GameEngine.CreateThread(() =>
            {
                DisplayMainUI();
            });
        }
    }
    public static class GamingCtl
    {
        public static Form1 instanceForm;
        public static Image playerImg;
        public static Image hunterImg;
        public static Image welcomeImg;
        public static GameEngine curEngine;
        public static Font gameFont;
        public static player curPlayer;
        public static hunter curHunter;
        public static Image keyimg;
        public static bool GodMode;
        public static GameCanera curCamera;
        public static int mgWidth; //长宽

        public static Image GetImageFromSize(Image img, int wid, int hei)
        {
            var ret = new Bitmap(wid, hei);
            using (var g = Graphics.FromImage(ret))
            {
                g.DrawImage(img, new Rectangle(0, 0, wid, hei));
            }
            return ret;
        }

        public static void InitImageResource()
        {
            gameFont = new Font("微软雅黑", 15);
            playerImg = GetImageFromSize(Image.FromFile(Application.StartupPath + "\\res\\player.png"), 50, 50);
            hunterImg = GetImageFromSize(Image.FromFile(Application.StartupPath + "\\res\\hunter.png"), 50, 50);
            welcomeImg = GetImageFromSize(Image.FromFile(Application.StartupPath + "\\res\\mainpg.png"), 800, 600);
            keyimg = GetImageFromSize(Image.FromFile(Application.StartupPath + "\\res\\key.png"), 50, 50);
        }
    }
    public class EngineButton : Element
    {
        public Action cb;
        Color bg, borderg, fg;
        Brush bgb, fgb;
        Pen borderPen;
        public static EngineButton Init(Action callback)
        {
            return new EngineButton(callback, new Point(0, 0), 100, 100, Color.FromArgb(180, 255, 255, 255), Color.DeepSkyBlue);
        }
        public EngineButton(Action click_callback, Point pos, int wid, int hei, Color backg, Color forg)
        {
            cb = click_callback;
            this.x = pos.X; this.y = pos.Y;
            this.width = wid;
            this.height = hei;
            bg = backg;
            fg = forg;
            borderg = Color.FromArgb(221, 186, 26);
            bgb = new SolidBrush(bg);
            fgb = new SolidBrush(fg);
            borderPen = new Pen(borderg, 3);
        }
        public string Text;
        public bool drawborder = false;
        public override void graph(Graphics g)
        {
            Rectangle btnrect = new Rectangle(this.x, this.y, this.width, this.height);
            g.FillRectangle(bgb, btnrect); //画背景

            var format = new StringFormat();
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;
            g.DrawString(Text, GamingCtl.gameFont, fgb, btnrect, format); //画文字

            if (drawborder)
            {
                g.DrawRectangle(borderPen, btnrect); //画外框
            }
            base.graph(g);
        }
        bool inRect = false;
        public override void OnMouseMove(object sender, MouseEventArgs e)
        {
            Rectangle btnrect = new Rectangle(this.x, this.y, this.width, this.height);
            if (btnrect.Contains(new Point(e.X, e.Y)))
            {
                inRect = true;
                drawborder = true;
            }
            else
            {
                inRect = false;
                drawborder = false;
            }
            base.OnMouseMove(sender, e);
        }
        public override void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (!inRect)
                return;
            drawborder = false;
            base.OnMouseDown(sender, e);
        }
        public override void OnMouseUp(object sender, MouseEventArgs e)
        {
            if (!inRect)
                return;
            drawborder = true;
            cb.Invoke();
            base.OnMouseUp(sender, e);
        }
    }
    public class WelcomePage : Element
    {
        public WelcomePage()
        {

        }
        public override void graph(Graphics g)
        {
            g.DrawImage(GamingCtl.welcomeImg, 0, 0);
            base.graph(g);
        }
    }
    public class Mask : Element
    {
        Brush bgc;
        public Mask(Color bg)
        {
            bgc = new SolidBrush(bg);
        }
        public override void graph(Graphics g)
        {
            g.FillRectangle(bgc, new Rectangle(0, 0, 800, 600));
            base.graph(g);
        }
    }
    public class TextShow : Element
    {
        Brush clr;
        public string Text;
        Font cf;
        public TextShow(string text, Color textcolor, Rectangle rect, Font fs)
        {
            this.Text = text;
            clr = new SolidBrush(textcolor);
            this.x = rect.X;
            this.y = rect.Y;
            this.width = rect.Width;
            this.height = rect.Height;
            cf = fs;
        }
        public override void graph(Graphics g)
        {
            var format = new StringFormat();
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;
            g.DrawString(this.Text, cf, clr, new Rectangle(this.x, this.y, this.width, this.height), format); //画文字
            base.graph(g);
        }
    }
    public class EngineImage :Element
    {
        public Image image;
        public Point pos;
        public EngineImage(Image img,Rectangle rect)
        {
            image = GamingCtl.GetImageFromSize(img, rect.Width, rect.Height);
            pos = new Point(rect.X, rect.Y);
        }
        public override void graph(Graphics g)
        {
            g.DrawImage(image, pos);
            base.graph(g);
        }
    }
    public class LoadingPage : Element
    {
        List<Image> loadingImgs;
        Image curPlay;
        public LoadingPage(int w, int h)
        {
            loadingImgs = new List<Image>();
            string ldrp = Application.StartupPath + "\\res\\loading\\";
            for (int i = 0; i <= 15; i++)
            {
                loadingImgs.Add(Bitmap.FromFile($"{ldrp}loading{i:00}.png"));
            }
            drPoint = new Point((w / 2 - 60), h / 2 - 75);
            this.width = w; this.height = h;
            GameEngine.CreateThread(Play);
        }
        public bool PlayAllow = true;
        void Play()
        {
            while (PlayAllow)
            {
                foreach (var cc in loadingImgs)
                {
                    curPlay = cc;
                    Thread.Sleep(99);
                }
            }
        }
        Font ft = new Font("微软雅黑", 20.0F);
        public string ShowText = "加载中………";
        Point drPoint;
        public override void graph(Graphics g)
        {

            using (var b = new SolidBrush(Color.White))
            {
                var format = new StringFormat();
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Far;
                g.DrawImage(curPlay, drPoint);
                g.DrawString(ShowText, ft, b, new Rectangle(0, 0, width, height), format);
            }
            base.graph(g);
        }

        public class GameCamera : Element
        {
            player pl;
            hunter hut;
            public GameCamera(player player, hunter hunter)
            {
                this.pl = player;
                this.hut = hunter;
            }
        }
        public class player
        {

        }
        public class hunter
        {

        }
    }
    public class GameCanera : Element
    {
        player player;
        hunter hunter;
        Bitmap mazeBmp;
        Bitmap globalmap;
        bool[,] r, c;
        public GameCanera(player p,hunter h)
        {
            player = p;
            hunter = h;
        }
        MazeHelper maze;
        public void Init()
        {
            player.x = 0;
            player.y = 0;
            maze = new MazeHelper(GamingCtl.mgWidth, GamingCtl.mgWidth, null);
            maze.Summon();
            r = maze.rows;
            c = maze.colunms;      
            var rnd = new Random();
            for(int i = 0;i < 5;i++)
            {
                try
                {
                    int tr = rnd.Next(1, GamingCtl.mgWidth);
                    int tc = rnd.Next(1, GamingCtl.mgWidth);
                    List<int> tmpr = new List<int>();
                    List<int> tmpc = new List<int>();
                    for (int j = 1; j < GamingCtl.mgWidth; j++)
                    {
                        if (r[j, tr])
                        {
                            tmpr.Add(j);
                        }
                        if (c[j, tc])
                        {
                            tmpc.Add(j);
                        }
                    }
                    r[tmpr[rnd.Next(0, tmpr.Count - 1)], tr] = false;
                    c[tmpc[rnd.Next(0, tmpc.Count - 1)], tc] = false;
                }
                catch
                {

                }
            }
            mazeBmp = maze.GetBmp(50);
            hunter.x = GamingCtl.mgWidth - 1;
            hunter.y = GamingCtl.mgWidth - 1;
            hunter.px = hunter.x * 50;
            hunter.py = hunter.y * 50;
            FindKeys = new Dictionary<Point, openkey>();
            for(int i = 0;i < 5;i++)
            {
                readd:
                Point keyp = new Point(rnd.Next(1, GamingCtl.mgWidth - 1), rnd.Next(1, GamingCtl.mgWidth - 1));
                var keyObj = new openkey() { pos = keyp };
                if(FindKeys.ContainsKey(keyp))
                {
                    goto readd;
                }
                FindKeys.Add(keyp,keyObj);
            }
            pathfinder = new PathFinder(r, c);
            hunterMoveSleep = 20;
            GameEngine.CreateThread(ShowMap);           
        }
        bool isShowMap;
        int hunterMoveSleep;
        bool SkipShowMap;
        void ShowMap()
        {        
            isShowMap = true;
            Image cloneMaze = (Image)mazeBmp.Clone();
            using (var g = Graphics.FromImage(cloneMaze))
            {
                foreach (var cc in FindKeys)
                {
                    g.DrawImage(GamingCtl.keyimg, new Point(cc.Key.X * 50, cc.Key.Y * 50));
                }
                g.DrawImage(GamingCtl.playerImg, new Point(player.px, player.py));
                g.DrawImage(GamingCtl.hunterImg, new Point(hunter.px, hunter.py));
            }
            
            globalmap = (Bitmap)GamingCtl.GetImageFromSize(cloneMaze, 500, 500);
            var st = new TextShow("", Color.Gold, new Rectangle(0, 10, 800, 40), new Font("微软雅黑", 25));
            var Skipbtn = new EngineButton(()=> {
                SkipShowMap = true;
            }, new Point(710, 530), 60, 50, Color.Transparent, Color.White);
            Skipbtn.Text = "跳过";
            GamingCtl.curEngine.drawList.Add(st);
            GamingCtl.curEngine.drawList.Add(Skipbtn);
            for(int i = 30;i > 0;i--)
            {
                if (SkipShowMap)
                    break;

                st.Text = $"查看全图；剩余秒数：{i}";
                Thread.Sleep(1000);
            }
            GamingCtl.curEngine.drawList.Remove(st);
            GamingCtl.curEngine.drawList.Remove(Skipbtn);
            GameEngine.CreateThread(Move);
            GameEngine.CreateThread(hunterMove);
            isShowMap = false;
        }
        BlockFx GetMoveableNoFx(Point p)
        {
            BlockFx ret = new BlockFx();
            try
            {
                if (!r[p.X, p.Y])
                    ret.up = true;
            }
            catch { }
            try
            {
                if (!r[p.X, p.Y + 1])
                    ret.down = true;
            }
            catch { }
            try
            {
                if (!c[p.X, p.Y])
                    ret.left = true;
            }
            catch { }
            try
            {
                if (!c[p.X + 1, p.Y])
                    ret.right = true;
            }
            catch { }

            return ret;
        }
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
        public override void graph(Graphics g)
        {
            if (isShowMap)
            {
                g.DrawImage(globalmap, new Point(150, 50));
            }
            else
            {
                g.DrawImage(mazeBmp, new Point(375 - player.px, 275 - player.py));
                g.DrawImage(GamingCtl.playerImg, new Point(375, 275));
                g.DrawImage(GamingCtl.hunterImg, new Point(hunter.px - player.px + 375, hunter.py - player.py + 275));
                foreach (var cc in FindKeys)
                {
                    g.DrawImage(GamingCtl.keyimg, new Point(cc.Value.pos.X * 50 - player.px + 375, cc.Value.pos.Y * 50 - player.py + 275));
                }
            }
            base.graph(g);
        }
        public override void OnKeyDown(object sender, KeyEventArgs e)
        {

            //isMoving = true;
            if (e.KeyCode == Keys.W || e.KeyCode == Keys.Up)
            {
                curFx = fx.up;
            }else if(e.KeyCode == Keys.S || e.KeyCode == Keys.Down)
            {
                curFx = fx.down;
            }else if(e.KeyCode == Keys.A || e.KeyCode == Keys.Left)
            {
                curFx = fx.left;
            }else if(e.KeyCode == Keys.D || e.KeyCode == Keys.Right)
            {
                curFx = fx.right;
            }
            mre.Set();
            base.OnKeyDown(sender, e);
        }
        //bool isMoving = false;
        bool StopSignal = true;
        ManualResetEvent mre = new ManualResetEvent(false);
        fx curFx;
        void Move()
        {            
            while (StopSignal)
            {
                mre.WaitOne();
                var able = GetMoveableNoFx(new Point(player.x,player.y));
                if (able.left && curFx == fx.left)
                {
                    for (int i = 0; i < 50; i++)
                    {
                        player.px -= 1;
                        Thread.Sleep(10);
                    }
                    player.x -= 1;
                }
                else if(able.right && curFx == fx.right)
                {
                    for (int i = 0; i < 50; i++)
                    {
                        player.px += 1;
                        Thread.Sleep(10);
                    }
                    player.x += 1;
                }else if(able.up && curFx == fx.up)
                {
                    for (int i = 0; i < 50; i++)
                    {
                        player.py -= 1;
                        Thread.Sleep(10);
                    }
                    player.y -= 1;
                }else if(able.down && curFx == fx.down)
                {
                    for (int i = 0; i < 50; i++)
                    {
                        player.py += 1;
                        Thread.Sleep(10);
                    }
                    player.y += 1;
                }
                rightPath = false;
                var pt = new Point(player.x, player.y);
                if(FindKeys.ContainsKey(pt))
                {
                    FindKeys.Remove(pt);
                    hunterMoveSleep -= 2;
                    if (FindKeys.Count == 0)
                    {
                        GameEngine.CreateThread(OpenExit);
                    }
                }
                if(player.x == GamingCtl.mgWidth - 1 && player.y == GamingCtl.mgWidth)
                {
                    GameVictory();
                    return;
                }
                //GamingCtl.instanceForm.Text = $"x={player.x}  y={player.y}";
            }
        }
        Dictionary<Point,openkey> FindKeys;
        PathFinder pathfinder;
        bool rightPath = false;
        void OpenExit()
        {
            r[GamingCtl.mgWidth - 1, GamingCtl.mgWidth] = false;
            mazeBmp = maze.GetBmp(50);
            var t = new TextShow("你已经收集5个钥匙，现在出口已打开", Color.Red, new Rectangle(0, 0, 800, 600), new Font("微软雅黑", 25));
            GamingCtl.curEngine.drawList.Add(t);
            Thread.Sleep(5000);
            GamingCtl.curEngine.drawList.Remove(t);
        }
        void GameFail()
        {
            if (GamingCtl.GodMode)
                return;

            if (victory)
                return;

            StopSignal = false;
            victory = true;
            GamingCtl.curEngine.drawList.Clear();
            var tt = new TextShow("你死了", Color.Red, new Rectangle(0, 150, 800, 100), new Font("微软雅黑", 30));
            var tc = new TextShow("cxk被野兽先辈撅力（悲）\r\n\r\n你干嘛，哎呦", Color.White, new Rectangle(0, 270, 800, 100), new Font("微软雅黑", 20));
            var mask = new Mask(Color.FromArgb(115, 87, 13));
            var btn = new EngineButton(ExitGame, new Point(250, 450), 300, 50, Color.FromArgb(180, 255, 255, 255), Color.Black);
            btn.Text = "返回主界面";
            GamingCtl.curEngine.drawList.Add(mask);
            GamingCtl.curEngine.drawList.Add(tt);
            GamingCtl.curEngine.drawList.Add(tc);
            GamingCtl.curEngine.drawList.Add(btn);
        }
        bool victory;
        void GameVictory()
        {
            victory = true;
            StopSignal = false;        
            //var ip = new EngineImage(GamingCtl.welcomeImg, new Rectangle(0, 0, 800, 600));
            var tt = new TextShow("你赢了", Color.Gold, new Rectangle(0, 150, 800, 100), new Font("微软雅黑", 30));
            var tc = new TextShow("cxk成功逃离野兽先辈\r\n\r\n哼哼哼，啊啊啊啊啊啊啊啊啊啊", Color.White, new Rectangle(0, 270, 800, 100), new Font("微软雅黑", 20));
            var mask = new Mask(Color.FromArgb(150, 0, 0, 0));
            var btn = new EngineButton(ExitGame, new Point(250, 450), 300, 50, Color.FromArgb(180, 255, 255, 255), Color.Black);
            btn.Text = "返回主界面";
            GamingCtl.curEngine.drawList.Add(mask);
            GamingCtl.curEngine.drawList.Add(tt);
            GamingCtl.curEngine.drawList.Add(tc);
            GamingCtl.curEngine.drawList.Add(btn);
        }
        void StopGame()
        {
            StopSignal = true;            
        }
        void ExitGame()
        {
            StopGame();
            GamingCtl.curEngine.drawList.Clear();
            GamingCtl.instanceForm.DisplayMainUI();
        }
        void hunterMove()
        {
            List<Point> path;
            while (StopSignal)
            {
                pathfinder.dest = new Point(player.x, player.y);
                path = pathfinder.GetPath(new Point(hunter.x, hunter.y));
                rightPath = true;
                
                if (path is null)
                    continue;

                foreach (var p in path)
                {
                    if (!rightPath)
                        break;

                    int mx, my;
                    mx = p.X - hunter.x;
                    my = p.Y - hunter.y;
                    hunter.x += mx;
                    hunter.y += my;
                    if (mx != 0)
                    {
                        for (int i = 0; i < 50; i++)
                        {
                            if (victory)
                                return;
                            hunter.px += mx;
                            if (new Rectangle(player.px + 5, player.py + 5, 40, 40).IntersectsWith(new Rectangle(hunter.px + 5, hunter.py + 5, 40,40)))
                            {
                                GameFail();
                                return;
                            }
                            Thread.Sleep(hunterMoveSleep);
                        }
                    }
                    if (my != 0)
                    {
                        for (int i = 0; i < 50; i++)
                        {
                            hunter.py += my;
                            if (victory)
                            {
                                return;
                            }
                            if (new Rectangle(player.px + 5, player.py + 5, 40, 40).IntersectsWith(new Rectangle(hunter.px + 5, hunter.py + 5, 40, 40)))
                            {
                                GameFail();
                                return;
                            }                           
                            Thread.Sleep(hunterMoveSleep);
                        }
                    }
                }
            }
        }
        public override void OnKeyUp(object sender, KeyEventArgs e)
        {
            //isMoving = false;
            mre.Reset();
            if(e.KeyCode == Keys.Escape)
            {
                StopSignal = true;
                victory = true;
                ExitGame();
            }
            base.OnKeyUp(sender, e);
        }
    }
    public class openkey
    {
        public Point pos;
    }
    public class player
    {
        public int x;
        public int y;
        public int px;
        public int py;
    }
    public class hunter
    {
        public int x;
        public int y;
        public int px;
        public int py;
    }
}
