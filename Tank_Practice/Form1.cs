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
        //탱크 관리용 변수
        ProgressBar gauge_p1;
        ProgressBar hp_p1;
        bool bRunning;
        Tank tank_p1;
        Thread tank_Thread_p1;
        Thread charge_Gauge_Thread_p1;
        Rectangle map_Rect;
        Rectangle ground_Rect;
        Point gauge_p1_Pos;
        Point hp_p1_Pos;
        int power;
        Image BG, exPlo;

        //네트워크 커넥터 폼용 변수
        ConnectorForm ConnFrm;
        public bool server_connected, client_connected;

        public Form1()
        {
            InitializeComponent();
            map_Rect = this.ClientRectangle;
            BG = Properties.Resources.map5;
            exPlo = Properties.Resources.explosion;
            resetGame();
            bRunning = true;
            tank_Thread_p1 = new Thread(() => operateTank(tank_p1, hp_p1, hp_p1_Pos));
            // tank_Thread_p1.IsBackground = true;
            tank_Thread_p1.Start();
            draw();
            server_connected = client_connected = false;
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
            this.KeyPreview = true;
        }

        private void resetGame()
        {
            Size body = new Size(100, 40);
            Size gun = new Size(50, 9);
            int ground_Height = 100;
            ground_Rect = new Rectangle(new Point(0, map_Rect.Height - ground_Height),
                new Size(map_Rect.Width, ground_Height));
            int x = body.Width / 2;
            int y = map_Rect.Height - (ground_Height + body.Height * 3 / 4);
            tank_p1 = new Tank(new Point(x, y), new Point(x + body.Width / 8, y), new Rectangle(new Point(x - body.Width / 2, y - body.Height / 4), body),
                new Rectangle(new Point(x, y - gun.Height / 2), gun),
                new Rectangle(new Point(0, 0), new Size(map_Rect.Width, map_Rect.Height - ground_Height)));
            /*
            Size body = new Size(100, 20);
            Size turret = new Size(40, 20);
            int ground_Height = 100;
            ground_Rect = new RectangleF(new Point(0, (int)map_Rect.Height - ground_Height),
                new Size((int)map_Rect.Width, ground_Height));
            int x = body.Width / 2;
            int y = (int)map_Rect.Height - (ground_Height + body.Height + turret.Height / 2 + 1);
            tank_p1 = new Tank(new Point(x, y), new Rectangle(new Point(x - body.Width / 2, y + turret.Height / 2), body),
                new Rectangle(new Point(x - turret.Width / 2, y - turret.Height / 2), turret),
                new Rectangle(new Point(0, 0), new Size((int)map_Rect.Width, (int)map_Rect.Height - ground_Height)));
            */
            gauge_p1 = new ProgressBar();
            gauge_p1.Step = 1;
            gauge_p1.Size = new Size(300, 20);
            gauge_p1.Style = ProgressBarStyle.Continuous;
            gauge_p1_Pos = new Point(50, this.ClientRectangle.Height - 80);
            gauge_p1.Location = gauge_p1_Pos;
            gauge_p1.Visible = true;
            this.Controls.Add(gauge_p1);
            hp_p1 = new ProgressBar();
            hp_p1.Size = new Size(100, 20);
            hp_p1.Style = ProgressBarStyle.Continuous;
            hp_p1_Pos = new Point(x - body.Width / 2, y - (tank_p1.cannon_Len + hp_p1.Height));
            hp_p1.Location = hp_p1_Pos;
            hp_p1.ForeColor = Color.Blue;
            hp_p1.Maximum = 2000;
            hp_p1.Value = 2000;
            hp_p1.Visible = true;
            this.Controls.Add(hp_p1);
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
                bg.Graphics.DrawImage(BG, 0, 0);
                bg.Graphics.FillRectangle(Brushes.Black, ground_Rect);
                Size resize = tank_p1.gun_Rect.Size;
                Bitmap gun_Resized = new Bitmap(Properties.Resources.tank_gun_blue, resize);
                Bitmap gun_Rotated = rotateImage(gun_Resized, (float)(tank_p1.deg - 90));
                bg.Graphics.DrawImage(gun_Rotated, tank_p1.gun_Axis.X, tank_p1.gun_Axis.Y - (gun_Rotated.Height - tank_p1.gun_Rect.Height / 2));
                bg.Graphics.DrawImage(Properties.Resources.tank_body_blue, tank_p1.body_Rect.Left,
                    tank_p1.body_Rect.Top, tank_p1.body_Rect.Width, tank_p1.body_Rect.Height);
                Point cannon_Start = tank_p1.gun_Axis;
                PointF cannon_End = tank_p1.getRotatedPos(tank_p1.deg, tank_p1.cannon_Len, cannon_Start);
                /*
                PointF gun_Center = new PointF(tank_p1.gun_Rect.Width / 2, tank_p1.gun_Rect.Height / 2);
                Bitmap gun = rotateImage(Properties.Resources.tank_gun_blue, gun_Center, (float)(tank_p1.deg - 90));
                bg.Graphics.DrawImage(gun, tank_p1.gun_Rect.Left, tank_p1.gun_Rect.Top,
                    tank_p1.gun_Rect.Width, tank_p1.gun_Rect.Height);
                */
                /*
                bg.Graphics.DrawRectangle(Pens.Black, tank_p1.body_Rect);
                bg.Graphics.DrawRectangle(Pens.Black, tank_p1.turret_Rect);
                Point cannon_Start = tank_p1.center;
                PointF cannon_End = tank_p1.getRotatedPos(tank_p1.deg, tank_p1.cannon_Len, cannon_Start);
                Pen pen = new Pen(Color.Black, 5);
                bg.Graphics.DrawLine(pen, cannon_Start, cannon_End);
                */
                int bullet_Size = 5;
                int hit_Area_Size = 10;
                double x, y;
                for (int i = tank_p1.bullets.Count - 1; i >= 0; --i)
                {
                    if (tank_p1.bullets[i].hit)
                    {
                        x = tank_p1.bullets[i].current_Pos.X - hit_Area_Size;
                        y = tank_p1.bullets[i].current_Pos.Y - hit_Area_Size;
                        bg.Graphics.DrawImage(exPlo, (float)x, (float)y, hit_Area_Size * 2, hit_Area_Size * 2);
                        //bg.Graphics.FillRectangle(Brushes.Red, (float)x, (float)y, hit_Area_Size * 2, hit_Area_Size * 2);
                        tank_p1.bullets.RemoveAt(i);
                    }
                    else
                    {
                        x = tank_p1.bullets[i].current_Pos.X - bullet_Size;
                        y = tank_p1.bullets[i].current_Pos.Y - bullet_Size;
                        bg.Graphics.FillEllipse(Brushes.Black, (float)x, (float)y, bullet_Size * 3, bullet_Size * 3);
                    }
                }
                Font font = new Font("Ariel", 16);
                bg.Graphics.DrawString(power.ToString(), font, Brushes.Black, new Point(0, 0));
                bg.Render(g);
                bg.Dispose();
            }
            g.Dispose();
        }

        private Bitmap rotateImage(Image img, float angle)
        {
            int newWidth = 0;
            int newHeight = 0;
            Bitmap originalImg = new Bitmap(img.Width, img.Height);
            if (angle < 0)
            {
                angle += 360;
            }
            if (angle <= 90)
            {
                newWidth = (int)(originalImg.Width * Math.Cos(2 * Math.PI * angle / 360) + originalImg.Height * Math.Sin(2 * Math.PI * angle / 360));
                newHeight = (int)(originalImg.Height * Math.Cos(2 * Math.PI * angle / 360) + originalImg.Width * Math.Sin(2 * Math.PI * angle / 360));
            }
            else if (angle > 90 && angle <= 180)
            {
                newWidth = (int)(originalImg.Width * -Math.Cos(2 * Math.PI * angle / 360) + originalImg.Height * Math.Sin(2 * Math.PI * angle / 360));
                newHeight = (int)(originalImg.Height * -Math.Cos(2 * Math.PI * angle / 360) + originalImg.Width * Math.Sin(2 * Math.PI * angle / 360));
            }
            else if (angle > 180 && angle <= 270)
            {
                newWidth = (int)(originalImg.Width * -Math.Cos(2 * Math.PI * angle / 360) + originalImg.Height * -Math.Sin(2 * Math.PI * angle / 360));
                newHeight = (int)(originalImg.Height * -Math.Cos(2 * Math.PI * angle / 360) + originalImg.Width * -Math.Sin(2 * Math.PI * angle / 360));
            }
            else if (angle > 270 && angle <= 360)
            {
                newWidth = (int)(originalImg.Width * Math.Cos(2 * Math.PI * angle / 360) + originalImg.Height * -Math.Sin(2 * Math.PI * angle / 360));
                newHeight = (int)(originalImg.Height * Math.Cos(2 * Math.PI * angle / 360) + originalImg.Width * -Math.Sin(2 * Math.PI * angle / 360));
            }
            Bitmap rotatedImg = new Bitmap(newWidth, newHeight);
            rotatedImg.SetResolution(img.HorizontalResolution, img.VerticalResolution);
            using (Graphics g = Graphics.FromImage(rotatedImg))
            {
                g.TranslateTransform(newWidth / 2, newHeight / 2);
                g.RotateTransform(angle);
                g.TranslateTransform(-originalImg.Width / 2, -originalImg.Height / 2);
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(img, new PointF(0, 0));
            }
            return rotatedImg;
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
                speed = 5;
                Point tank_Body_Pos = tank_Obj.body_Rect.Location;
                Point tank_Gun_Pos = tank_Obj.gun_Rect.Location;
                if (tank_Obj.L)
                {
                    tank_Obj.center.X -= speed;
                    tank_Obj.gun_Axis.X = tank_Obj.center.X + tank_Obj.body_Rect.Width / 8;
                    tank_Body_Pos.X -= speed;
                    tank_Gun_Pos.X -= speed;
                    if (tank_Obj.center.X < tank_Obj.body_Rect.Width / 2)
                    {
                        tank_Obj.center.X = tank_Obj.body_Rect.Width / 2;
                        tank_Obj.gun_Axis.X = tank_Obj.center.X + tank_Obj.body_Rect.Width / 8;
                        tank_Body_Pos.X = 0;
                        tank_Gun_Pos.X = tank_Obj.body_Rect.Width / 2;
                    }
                }
                else if (tank_Obj.R)
                {
                    tank_Obj.center.X += speed;
                    tank_Obj.gun_Axis.X = tank_Obj.center.X + tank_Obj.body_Rect.Width / 8;
                    tank_Body_Pos.X += speed;
                    tank_Gun_Pos.X += speed;
                    if (tank_Obj.center.X > tank_Obj.map_Rect.Width - tank_Obj.body_Rect.Width / 2)
                    {
                        tank_Obj.center.X = tank_Obj.map_Rect.Width - tank_Obj.body_Rect.Width / 2;
                        tank_Obj.gun_Axis.X = tank_Obj.center.X + tank_Obj.body_Rect.Width / 8;
                        tank_Body_Pos.X = tank_Obj.map_Rect.Width - tank_Obj.body_Rect.Width;
                        tank_Gun_Pos.X = tank_Obj.map_Rect.Width - tank_Obj.body_Rect.Width / 2;
                    }
                }
                tank_Obj.body_Rect.Location = tank_Body_Pos;
                tank_Obj.gun_Rect.Location = tank_Gun_Pos;
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
                Thread.Sleep(20);
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

        private void ConnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //항복버튼 활성화
            //ResignToolStripMenuItem.Enabled = true;

            foreach (Form frm in Application.OpenForms)
            {
                if (frm.Name == "ConnectorForm")
                {
                    frm.Activate();
                    return;
                }
            }

            ConnFrm = new ConnectorForm();
            ConnFrm.Owner = this;
            ConnFrm.parent = this;

            ConnFrm.Show();
        }

        public m_Server server;
        public void openServer()
        {
            server = new m_Server(ConnFrm);
            server.StartServer();
        }

        public m_Client client;
        public void connectServer()
        {
            client = new m_Client(ConnFrm);
            client.ConnectToServer();
        }
    }
}