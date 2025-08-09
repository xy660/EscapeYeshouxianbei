
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

public class GameEngine
{
    Form window;
    public bool EnableSyncFrame = false;
    public GameEngine(Form f, int width, int height)
    {
        window = f;
        WindowWidth = width;
        WindowHeight = height;
    }
    public int graFrame = 0;
    Graphics WindowGraphic;
    public int WindowHeight;
    public int WindowWidth;
    Bitmap gameScr = new Bitmap(800, 600);
    SolidBrush bru = new SolidBrush(Color.Red);
    public List<Element> drawList = new List<Element>();
    void GameLoop()
    {
        var sg = Graphics.FromImage(gameScr);
        while (true)
        {
            
            if (syncSleep > 0 && EnableSyncFrame)
            {
                Thread.Sleep(syncSleep);
            }
            try
            {
                sg.Clear(Color.Black);
                foreach (var cc in drawList)
                {
                    cc.graph(sg);
                }
                sg.DrawString(fpsstr, new Font("微软雅黑", 9.0F), bru, 0, 0);
            }
            catch
            {

            }
            DrawToScreen();
            graFrame += 1;
        }
    }
    void DrawToScreen()
    {
        WindowGraphic.DrawImage(gameScr, 0, 0);
    }

    public void GameInit()
    {
        window.ClientSize = (Size)new Point(800, 600);
        WindowHeight = 600;
        WindowWidth = 800;
        WindowGraphic = window.CreateGraphics();
        window.KeyDown += OnKeyDown;
        window.KeyUp += OnKeyUp;
        window.MouseDown += OnMouseDown;
        window.MouseUp += OnMouseUp;
        window.MouseMove += OnMouseMove;
        CreateThread(GameLoop);


        CreateThread(FpsShow);
    }
    void OnKeyDown(object s,KeyEventArgs e)
    {
        try
        {
            foreach (var cc in drawList)
            {
                cc.OnKeyDown(s, e);
            }
        }
        catch
        {

        }
    }
    void OnKeyUp(object s, KeyEventArgs e)
    {
        try
        {
            foreach (var cc in drawList)
            {
                cc.OnKeyUp(s, e);
            }
        }
        catch
        {

        }
    }
    void OnMouseDown(object s,MouseEventArgs e)
    {
        try
        {
            foreach (var cc in drawList)
            {
                cc.OnMouseDown(s, e);
            }
        }
        catch
        {

        }
    }
    void OnMouseUp(object s, MouseEventArgs e)
    {
        try
        {
            foreach (var cc in drawList)
            {
                cc.OnMouseUp(s, e);
            }
        }
        catch
        {

        }
    }
    void OnMouseMove(object s, MouseEventArgs e)
    {
        try
        {
            foreach (var cc in drawList)
            {
                cc.OnMouseMove(s, e);
            }
        }
        catch
        {

        }
    }
    public static Thread CreateThread(ThreadStart act)
    {
        var xc = new Thread(act);
        xc.IsBackground = true;
        xc.Start();
        return xc;
    }
    string fpsstr;
    int syncSleep = 0;
    void FpsShow()
    {
        while (true)
        {
            try
            {
                fpsstr = $"Fps={graFrame}";
                syncSleep = (int)((1 - (60 / (double)graFrame)) * 1000 / 60);
                
                //Debug.Print(k.ToString());
                graFrame = 0;                
            }
            catch
            {
            }
            Thread.Sleep(1000);
        }
    }

}


public class Element
{
    public int x;
    public int y;
    public int width;
    public int height;
    public bool isShow;
    public virtual void graph(Graphics g)
    {

    }
    public virtual void OnKeyDown(object sender,KeyEventArgs e)
    {

    }
    public virtual void OnKeyUp(object sender,KeyEventArgs e)
    {

    }
    public virtual void OnMouseDown(object sender,MouseEventArgs e)
    {

    }
    public virtual void OnMouseUp(object sender, MouseEventArgs e)
    {

    }
    public virtual void OnMouseMove(object sender, MouseEventArgs e)
    {

    }
    public Element()
    {

    }
    public Element(int x, int y, bool Show)
    {
        this.x = x;
        this.y = y;
        this.isShow = Show;
    }
}



