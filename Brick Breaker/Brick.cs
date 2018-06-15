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
    class Brick
    {
        private Rectangle rectangle;
        private int x, y, width, height;
        private bool remove;
        private string powerUp;

        public Brick(int x, int y, int width, int height, string color, string powerUp)
        {
            // brick data
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.remove = false;
            this.powerUp = powerUp;
            // brick shape
            CreateRectangle(color);
        }

        private void CreateRectangle(string color)
        {
            rectangle = new Rectangle();
            rectangle.Width = width;
            rectangle.Height = height;
            SetBrickTexture(color);
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

        public string PowerUp
        {
               get { return powerUp; }
        }

        public void SetBrickTexture(string brickColor) 
        {
            ImageBrush brush = new ImageBrush();

            switch(brickColor)
            {
                case "red":
                    brush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Textures/redBrick.png"));
                    break;

                case "blue":
                    brush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Textures/blueBrick.png"));
                    break;

                case "purple":
                    brush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Textures/purpleBrick.png"));
                    break;

                case "gold":
                    brush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Textures/goldBrick.png"));
                    break;
                case "green":
                    brush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Textures/greenBrick.png"));
                    break;
            }

            
            this.rectangle.Fill = brush;
        }
    }
}
