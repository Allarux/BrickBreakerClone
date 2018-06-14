using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Brick_Breaker
{
    class Paddle
    {
        private Rectangle rectangle;
        private int x, y, width, height;
        private bool goLeft, goRight;
        public Paddle(int x, int y, int width, int height)
        {
            // paddle data
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            goLeft = false;
            goRight = false;

            // paddle shape
            CreateRectangle();
        }

        /// <summary>
        /// Creates a new rectangle shape representation of the paddle. Calling this allows the paddle to update its shape size.
        /// </summary>
        private void CreateRectangle()
        {
            rectangle = new Rectangle();
            rectangle.Width = width;
            rectangle.Height = height;
            ImageBrush brush = new ImageBrush();
            brush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Textures/paddle-1.png"));
            rectangle.Fill = brush;
        }

        public Rectangle GetRectangle()
        {
            return rectangle;
        }

        public int X
        {
            get { return x; }
            set { x = value; }
        }

        public int Y
        {
            get { return y; }
            set { y = value; }
        }

        public int Width
        {
            get { return width; }
        }

        public int Height
        {
            get { return height; }
            set
            {
                height = value;
                CreateRectangle();
            }
        }

        public bool GoLeft
        {
            get { return goLeft; }
            set { goLeft = value; }
        }

        public bool GoRight
        {
            get { return goRight; }
            set { goRight = value; }
        }
    }
}
