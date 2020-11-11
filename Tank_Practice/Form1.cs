using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tank_Practice
{
    public partial class Form1 : Form
    {
        ProgressBar gauge_p1;
        bool bRunning;
        Tank tank_p1;
        Thread tank_Thread_p1;
        Thread charge_Gauge_Thread_p1;
        RectangleF map_Rect;
        Point gauge_p1_Pos;
        int power;
        public Form1()
        {
            InitializeComponent();
            map_Rect = this.ClientRectangle;
            resetGame();
            bRunning = true;
            tank_Thread_p1 = new Thread(() => operateTank(tank_p1, gauge_p1, gauge_p1_Pos));
            // tank_Thread_p1.IsBackground = true;
            tank_Thread_p1.Start();
            draw();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            bRunning = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            /*
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.UpdateStyles();
            */
        }

        private void resetGame()
        {
            Size body = new Size(100, 20);
            Size turret = new Size(40, 20);
            int x = body.Width / 2;
            int y = (int)map_Rect.Height - (body.Height + turret.Height / 2 + 1);
            tank_p1 = new Tank(new Point(x, y), new Rectangle(new Point(x - body.Width / 2, y + turret.Height / 2), body),
                new Rectangle(new Point(x - turret.Width / 2, y - turret.Height / 2), turret), Rectangle.Round(map_Rect));
            gauge_p1 = new ProgressBar();
            gauge_p1.Step = 1;
            gauge_p1.Size = body;
            gauge_p1.Style = ProgressBarStyle.Continuous;
            gauge_p1_Pos = new Point(x - body.Width / 2, y - (turret.Height + tank_p1.cannon_Len));
            gauge_p1.Location = gauge_p1_Pos;
            gauge_p1.Visible = true;
            this.Controls.Add(gauge_p1);
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            using (g)
            {
                // draw(g);
            }
            g.Dispose();
        }

        private void draw()
        {
            System.Drawing.Graphics g = this.CreateGraphics();
            using (BufferedGraphics bg = BufferedGraphicsManager.Current.Allocate(g, this.ClientRectangle))
            {
                bg.Graphics.Clear(BackColor);
                bg.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                bg.Graphics.DrawRectangle(Pens.Black, tank_p1.body_Rect);
                bg.Graphics.DrawRectangle(Pens.Black, tank_p1.turret_Rect);
                Point cannon_Start = tank_p1.center;
                PointF cannon_End = tank_p1.getRotatedPos(tank_p1.deg, tank_p1.cannon_Len, cannon_Start);
                Pen pen = new Pen(Color.Black, 5);
                bg.Graphics.DrawLine(pen, cannon_Start, cannon_End);
                int bullet_Size = 5;
                double x, y;
                for (int i = tank_p1.bullets.Count - 1; i >= 0; --i)
                {
                    if (tank_p1.bullets[i].hit)
                    {
                        tank_p1.bullets.RemoveAt(i);
                    }
                    else
                    {
                        x = tank_p1.bullets[i].current_Pos.X - bullet_Size;
                        y = tank_p1.bullets[i].current_Pos.Y - bullet_Size;
                        bg.Graphics.FillEllipse(Brushes.Black, (float)x, (float)y, bullet_Size * 2, bullet_Size * 2);
                    }
                }
                Font font = new Font("Ariel", 16);
                bg.Graphics.DrawString(power.ToString(), font, Brushes.Black, new Point(0, 0));
                bg.Render(g);
                bg.Dispose();
            }
            g.Dispose();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    tank_p1.U = true;
                    break;
                case Keys.Down:
                    tank_p1.D = true;
                    break;
                case Keys.Left:
                    tank_p1.L = true;
                    break;
                case Keys.Right:
                    tank_p1.R = true;
                    break;
                case Keys.Space:
                    tank_p1.charge_Cannon = true;
                    charge_Gauge_Thread_p1 = new Thread(() => chargeGauge(tank_p1, gauge_p1));
                    charge_Gauge_Thread_p1.Start();
                    break;
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    tank_p1.U = false;
                    break;
                case Keys.Down:
                    tank_p1.D = false;
                    break;
                case Keys.Left:
                    tank_p1.L = false;
                    break;
                case Keys.Right:
                    tank_p1.R = false;
                    break;
                case Keys.Space:
                    power = gauge_p1.Value;
                    tank_p1.shoot(power);
                    tank_p1.charge_Cannon = false;
                    break;
            }
        }

        private void operateTank(Tank tank_Obj, ProgressBar gauge, Point gauge_Pos)
        {
            int speed;
            double deg;
            bool bDraw;
            while (bRunning)
            {
                speed = 1;
                Point tank_Body_Pos = tank_Obj.body_Rect.Location;
                Point tank_Turret_Pos = tank_Obj.turret_Rect.Location;
                if (tank_Obj.L)
                {
                    tank_Obj.center.X -= speed;
                    tank_Body_Pos.X -= speed;
                    tank_Turret_Pos.X -= speed;
                    if (tank_Obj.center.X < tank_Obj.body_Rect.Width / 2)
                    {
                        tank_Obj.center.X = tank_Obj.body_Rect.Width / 2;
                        tank_Body_Pos.X = 0;
                        tank_Turret_Pos.X = tank_Obj.body_Rect.Width / 2 - tank_Obj.turret_Rect.Width / 2;
                    }
                }
                else if (tank_Obj.R)
                {
                    tank_Obj.center.X += speed;
                    tank_Body_Pos.X += speed;
                    tank_Turret_Pos.X += speed;
                    if (tank_Obj.center.X > tank_Obj.map_Rect.Width - tank_Obj.body_Rect.Width / 2)
                    {
                        tank_Obj.center.X = tank_Obj.map_Rect.Width - tank_Obj.body_Rect.Width / 2;
                        tank_Body_Pos.X = tank_Obj.map_Rect.Width - tank_Obj.body_Rect.Width;
                        tank_Turret_Pos.X = tank_Obj.map_Rect.Width - (tank_Obj.body_Rect.Width / 2 + tank_Obj.turret_Rect.Width / 2);
                    }
                }
                tank_Obj.body_Rect.Location = tank_Body_Pos;
                tank_Obj.turret_Rect.Location = tank_Turret_Pos;
                gauge_Pos.X = tank_Obj.center.X - tank_Obj.body_Rect.Width / 2;
                gauge.Location = gauge_Pos;
                deg = 1;
                if (tank_Obj.U)
                {
                    tank_Obj.deg -= deg;
                    if (tank_Obj.deg < 0)
                    {
                        tank_Obj.deg = 0;
                    }
                }
                else if (tank_Obj.D)
                {
                    tank_Obj.deg += deg;
                    if (tank_Obj.deg > 90)
                    {
                        tank_Obj.deg = 90;
                    }
                }
                bDraw = tank_Obj.U || tank_Obj.D || tank_Obj.L || tank_Obj.R || bRunning;
                for (int i = 0; i < tank_Obj.bullets.Count; ++i)
                {
                    bDraw |= true;
                    /*
                    if (!tank_Obj.bullets[i].hit)
                    {
                        bDraw |= true;
                    }
                    */
                }
                if (bDraw)
                {
                    draw();
                }
                Thread.Sleep(1);
            }
        }

        private void chargeGauge(Tank tank_Obj, ProgressBar gauge)
        {
            while (tank_Obj.charge_Cannon && bRunning)
            {
                gauge.PerformStep();
                gauge.Refresh();
                Thread.Sleep(5);
            }
            gauge_p1.Value = 0;
            gauge_p1.Update();
            gauge_p1.Refresh();
        }
    }
}