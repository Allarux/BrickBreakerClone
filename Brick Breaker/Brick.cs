using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Brick_Breaker
{
    class Brick
    {
        private Rectangle rectangle;
        private int x, y, width, height;
        private bool remove;

        public Brick(int x, int y, int width, int height)
        {
            // brick data
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.remove = false;

            // brick shape
            CreateRectangle();
        }

        private void CreateRectangle()
        {
            rectangle = new Rectangle();
            rectangle.Width = width;
            rectangle.Height = height;
            rectangle.Fill = Brushes.Thistle;
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
        }

        public bool Remove
        {
            get { return remove; }
            set { remove = value; }
        }
    }
}
