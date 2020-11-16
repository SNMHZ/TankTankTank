using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tank_Practice
{
    public class Bullet
    {
        public PointF init_Pos;
        public PointF current_Pos;
        public int v;
        public double deg;
        public double g;
        public bool hit;
        public Rectangle map_Rect;
        public Thread bullet_Thread;
        public Bullet(PointF init_Pos, int v, double deg, Rectangle map_Rect)
        {
            this.init_Pos = init_Pos;
            this.v = v;
            this.deg = deg - 90;
            this.g = 9.8;
            this.hit = false;
            this.map_Rect = map_Rect;
            this.bullet_Thread = new Thread(moveBullet);
            this.bullet_Thread.IsBackground = true;
            this.bullet_Thread.Start();
        }

        private void moveBullet()
        {
            double t = 0;
            double rad = this.deg * Math.PI / 180.0;
            double vx, vy;
            while (true)
            {
                vx = this.v * Math.Cos(rad);
                vy = this.v * Math.Sin(rad) + this.g * t;
                this.current_Pos.X = init_Pos.X + (float)(vx * t);
                this.current_Pos.Y = init_Pos.Y + (float)(vy * t - 0.5 * this.g * t * t);
                t += 0.1;
                if (this.map_Rect.Bottom < this.current_Pos.Y)
                {
                    break;
                }
                Thread.Sleep(10);
            }
            this.hit = true;
        }
    }
    public class Tank
    {
        public Point center;
        public Point gun_Axis;
        public Rectangle body_Rect;
        public Rectangle gun_Rect;
        public Rectangle map_Rect;
        public int cannon_Len;
        public double deg;
        public int power;
        public bool L, R, U, D;
        public bool charge_Cannon;
        public List<Bullet> bullets;

        public Tank(Point center, Point gun_Axis, Rectangle body_Rect, Rectangle gun_Rect, Rectangle map_Rect)
        {
            this.center = center;
            this.gun_Axis = gun_Axis;
            this.body_Rect = body_Rect;
            this.gun_Rect = gun_Rect;
            this.map_Rect = map_Rect;
            this.cannon_Len = (int)this.gun_Rect.Width;
            this.deg = 45;
            this.power = 0;
            this.L = false;
            this.R = false;
            this.U = false;
            this.D = false;
            this.charge_Cannon = false;
            bullets = new List<Bullet>();
        }

        public PointF getRotatedPos(double deg, int radius, Point gun_Axis)
        {
            double rad = deg * Math.PI / 180.0;
            double dx = Math.Sin(rad) * radius;
            double dy = Math.Cos(rad) * radius;
            return new PointF((float)(gun_Axis.X + dx), (float)(gun_Axis.Y - dy));
        }

        public void shoot(int power)
        {
            this.power = power;
            PointF pos = getRotatedPos(this.deg, this.cannon_Len, this.gun_Axis);
            this.bullets.Add(new Bullet(pos, this.power, this.deg, this.map_Rect));
        }
    }
}
