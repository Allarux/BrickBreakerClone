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
    class PowerUp
    {
        private Rectangle rectangle;
        private double x, y, dy;
        private string type;

        public PowerUp(double x, double y, double dy, string type)
        {
            this.x = x;
            this.y = y;
            this.dy = dy;
            this.type = type;

            rectangle = new Rectangle();
            rectangle.Width = 20;
            rectangle.Height = 20;
            SetPowerUpTexture(type);
        }

        public Rectangle GetRectangle()
        {
            return rectangle;
        }

        public void UpdatePosition()
        {
            y += dy;
        }

        public double X
        {
            get { return x; }
        }

        public double Y
        {
            get { return y; }
        }

        public double Width
        {
            get { return rectangle.Width; }
        }

        public double Height
        {
            get { return rectangle.Height; }
        }

        public string Type
        {
            get { return type; }
        }

        private void SetPowerUpTexture(string type)
        {
            ImageBrush brush = new ImageBrush();
            if (type == "bullets")
            {
                brush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Textures/tileRed.png"));
            }
            else if (type == "wide_paddle")
            {
                brush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Textures/tileBlue.png"));
            }
            else if (type == "extra_ball")
            {
                brush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Textures/tileGreen.png"));
            }

            rectangle.Fill = brush;
        }
    }
}
