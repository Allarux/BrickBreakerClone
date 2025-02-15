﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Brick_Breaker {
    class Bullet
    {
        private Ellipse ellipse;
        private double x, y, dx, dy;
        private double diameter;

        public Bullet(double diameter, double startX, double startY, double startDX, double startDY)
        {
            // shape
            ellipse = new Ellipse();
            ImageBrush brush = new ImageBrush();
            brush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Textures/bullet.png"));
            ellipse.Fill = brush;
            ellipse.Width = diameter;
            ellipse.Height = diameter;

            // ball data
            this.diameter = diameter;
            x = startX;
            y = startY;
            dx = startDX;
            dy = startDY;
        }

        public Ellipse GetEllipse()
        {
            return ellipse;
        }

        public double Diameter {
            get { return diameter; }
            set { diameter = value; }
        }

        public double GetRadius()
        {
            return diameter / 2;
        }

        public Point getCenter()
        {
            return new Point(x + GetRadius(), y + GetRadius());
        }

        public void UpdatePosition()
        {
            //x += dx;
            y += dy;
        }

        public double X 
        {
            get { return x; }
            set { x = value; }
        }

        public double Y 
        {
            get { return y; }
            set { y = value; }
        }

        public double Dx 
        {
            get { return dx; }
            set { dx = value; }
        }

        public double Dy 
        {
            get { return dy; }
            set { dy = value; }
        }

    }
}
