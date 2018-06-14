﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Brick_Breaker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer gameTimer = new DispatcherTimer();
        private List<Ball> balls = new List<Ball>();
        private List<Brick> bricks = new List<Brick>();
        private List<PowerUp> powerUps = new List<PowerUp>();
        private List<Bullet> bullets = new List<Bullet>();
        private Paddle paddle;
        private long score;
        private int level, maxLevels;
        private double defaultSpeed, incrementSpeed;
        private long highScore;
        private readonly string currentDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
        private int ammo = 5;

        public MainWindow()
        {
            InitializeComponent();

            // speed settings
            defaultSpeed = 2.7;
            incrementSpeed = 1.0;

            //// ball init
            //Random rnd = new Random();
            //balls.Add(new Ball(40, 81, (int)wpfCanvas.Height - 100, 0, 3));
            //foreach (Ball curBall in balls) {
            //    // attach balls to canvas
            //    wpfCanvas.Children.Add(curBall.GetEllipse());
            //    Canvas.SetTop(curBall.GetEllipse(), curBall.Y);
            //    Canvas.SetLeft(curBall.GetEllipse(), curBall.X);
            //}

            ////paddle init
            //paddle = new Paddle(0, (int)wpfCanvas.Height - 50, 100, 30);
            //wpfCanvas.Children.Add(paddle.GetRectangle());
            //Canvas.SetTop(paddle.GetRectangle(), paddle.Y);
            //Canvas.SetLeft(paddle.GetRectangle(), paddle.X);

            //// brick init
            //bricks.Add(new Brick(75, 50, 80, 30));
            //bricks.Add(new Brick(150, 350, 80, 30));
            //bricks.Add(new Brick(550, 50, 80, 30));
            //bricks.Add(new Brick(350, 90, 80, 30));
            //bricks.Add(new Brick(650, 150, 80, 30));
            //bricks.Add(new Brick(500, 170, 10, 10));
            //bricks.Add(new Brick(470, 450, 80, 30));
            //foreach (Brick curBrick in bricks)
            //{
            //    wpfCanvas.Children.Add(curBrick.GetRectangle());
            //    Canvas.SetTop(curBrick.GetRectangle(), curBrick.Y);
            //    Canvas.SetLeft(curBrick.GetRectangle(), curBrick.X);
            //}

            // load level
            level = 1;
            maxLevels = 3;
            LoadLevel(level);

            // animation timer
            gameTimer.Interval = new TimeSpan(16000);
            gameTimer.Tick += gameTick;
            //gameTimer.IsEnabled = true;

            // score board
            score = 0;
            highScore = 0;
        }

        private void LoadLevel(int level)
        {
            // remove existing bricks if any
            List<Brick> removeBricks = new List<Brick>();
            removeBricks.AddRange(bricks);
            foreach (Brick brick in removeBricks)
            {
                wpfCanvas.Children.Remove(brick.GetRectangle());
            }
            removeBricks.Clear();
            bricks.Clear();

            // remove existing paddle if any
            if (paddle != null) wpfCanvas.Children.Remove(paddle.GetRectangle());
            paddle = null;

            // remove existing balls if any
            List<Ball> removeBalls = new List<Ball>();
            removeBalls.AddRange(balls);
            foreach (Ball ball in removeBalls)
            {
                wpfCanvas.Children.Remove(ball.GetEllipse());
            }
            removeBalls.Clear();
            balls.Clear();


            // open file and get default width and height
            string[] lines = File.ReadAllLines("../../level" + level + ".csv");
            int width = Int32.Parse(lines[0].Split(',')[0]);
            int height = Int32.Parse(lines[0].Split(',')[1]);

            // create bricks
            foreach (string line in lines.Skip(1))
            {
                string[] values = line.Split(',');
                int x =  Int32.Parse(values[0]);
                int y = Int32.Parse(values[1]);
                string color = values[2];
                string powerUp = values[4];

                bricks.Add(new Brick(x, y, width, height, color, powerUp));
            }

            // add bricks to canvas
            foreach (Brick curBrick in bricks)
            {
                wpfCanvas.Children.Add(curBrick.GetRectangle());
                Canvas.SetTop(curBrick.GetRectangle(), curBrick.Y);
                Canvas.SetLeft(curBrick.GetRectangle(), curBrick.X);

               
            }

            // recreate and reset paddle
            paddle = new Paddle(0, (int)wpfCanvas.Height - 50, 100, 30);
            paddle.X = (int) (wpfCanvas.Width / 2) - (paddle.Width / 2);
            wpfCanvas.Children.Add(paddle.GetRectangle());
            Canvas.SetTop(paddle.GetRectangle(), paddle.Y);
            Canvas.SetLeft(paddle.GetRectangle(), paddle.X);

            // recreate and resest ball
            balls.Add(new Ball(40, wpfCanvas.Width / 2, 3 * (wpfCanvas.Height / 4), 0.70710678, -0.70710678));
            foreach (Ball ball in balls)
            {
                wpfCanvas.Children.Add(ball.GetEllipse());
                Canvas.SetTop(ball.GetEllipse(), ball.Y);
                Canvas.SetLeft(ball.GetEllipse(), ball.X);
            }

            //update ammo counter
            this.labelAmmoCounter.Content = ammo.ToString();

        }

        private void gameTick(object sender, EventArgs e)
        {
            //////////////////
            // paddle logic //
            //////////////////

            // move paddle
            if (paddle.GoRight)
            {
                paddle.GoRight = false;
                paddle.X += 10;
            }
            if (paddle.GoLeft)
            {
                paddle.GoLeft = false;
                paddle.X -= 10;
            }

            // paddle collision (if out of bounds set it back in bounds)
            if (paddle.X <= 0) paddle.X = 0;
            if (paddle.X + paddle.Width >= wpfCanvas.Width) paddle.X = (int) wpfCanvas.Width - paddle.Width;

            // update paddle canvas position
            Canvas.SetTop(paddle.GetRectangle(), paddle.Y);
            Canvas.SetLeft(paddle.GetRectangle(), paddle.X);

            ////////////////
            // ball logic //
            ////////////////
            foreach (Ball ball in balls)
            {
                ball.UpdatePosition(defaultSpeed, incrementSpeed, level);

                // update ball canvas position
                Canvas.SetTop(ball.GetEllipse(), ball.Y);
                Canvas.SetLeft(ball.GetEllipse(), ball.X);

                // edge canvas ball bounce
                if (ball.Y <= 0) { ball.Dy *= -1; WallHit(); } // top
                if (ball.Y + ball.Diameter >= wpfCanvas.Height) { ball.Dy *= -1; WallHit(); } // bottom
                if (ball.X + ball.Diameter >= wpfCanvas.Width) { ball.Dx *= -1; WallHit(); } // right
                if (ball.X <= 0) { ball.Dx *= -1; WallHit(); } // right

                // paddle collision
                if (paddle.X <= ball.X + ball.GetRadius() && ball.X + ball.GetRadius() <= paddle.X + paddle.Width) // ball in paddle X range?
                {
                    if (paddle.Y <= ball.Y + ball.Diameter) // ball at paddle Y level?
                    {
                        // hit paddle
                        ball.Dy *= -1;
                        PaddleHit(paddle);//added this - mg
                    }
                }
                else // not on top of paddle, maybe it is on the corners
                {
                    // ball corner collision with paddle
                    Point ballCenter = ball.getCenter();
                    bool topRightHit = CalcDistance(ballCenter.X, ballCenter.Y, paddle.X + paddle.Width, paddle.Y) <= ball.GetRadius();
                    bool topLeftHit = CalcDistance(ballCenter.X, ballCenter.Y, paddle.X, paddle.Y) <= ball.GetRadius();

                    if (topRightHit)
                    {
                        CornerBounce(ball, paddle.X + paddle.Width, paddle.Y);
                        PaddleHit(paddle);
                    }
                    if (topLeftHit)
                    {
                        CornerBounce(ball, paddle.X, paddle.Y);
                        PaddleHit(paddle);
                    }
                }

                // brick collision
                List<Brick> removeBricks = new List<Brick>();
                foreach (Brick brick in bricks)
                {
                    Point ballCenter = ball.getCenter();

                    if (brick.X - ball.GetRadius() <= ballCenter.X && ballCenter.X <= brick.X + brick.Width + ball.GetRadius())
                    {
                        if (brick.Y - ball.GetRadius() <= ballCenter.Y && ballCenter.Y <= brick.Y + brick.Height + ball.GetRadius())
                        {
                            // ball is within range to bounce on top, bottom, right, left or maybe the corners

                            // top and bottom
                            if (brick.X <= ballCenter.X && ballCenter.X <= brick.X + brick.Width)
                            {
                                ball.Dy *= -1;
                                BrickHit(brick);
                            }
                            else if (brick.Y <= ballCenter.Y && ballCenter.Y <= brick.Y + brick.Height) // left and right
                            {
                                ball.Dx *= -1;
                                BrickHit(brick);
                            }
                            else
                            {
                                // corners
                                if (CalcDistance(ballCenter.X, ballCenter.Y, brick.X, brick.Y) <= ball.GetRadius()) // top left
                                {
                                    CornerBounce(ball, brick.X, brick.Y);
                                    BrickHit(brick);
                                }
                                if (CalcDistance(ballCenter.X, ballCenter.Y, brick.X + brick.Width, brick.Y) <= ball.GetRadius()) // top right
                                {
                                    CornerBounce(ball, brick.X + brick.Width, brick.Y);
                                    BrickHit(brick);
                                }
                                if (CalcDistance(ballCenter.X, ballCenter.Y, brick.X, brick.Y + brick.Height) <= ball.GetRadius()) // bottom left
                                {
                                    CornerBounce(ball, brick.X, brick.Y + brick.Height);
                                    BrickHit(brick);
                                }
                                if (CalcDistance(ballCenter.X, ballCenter.Y, brick.X + brick.Width, brick.Y + brick.Height) <= ball.GetRadius()) // bottom right
                                {
                                    CornerBounce(ball, brick.X + brick.Width, brick.Y + brick.Height);
                                    BrickHit(brick);
                                }
                            }
                        }
                    }

                    // add to remove list to be removed outside of enumeration
                    if (brick.Remove) removeBricks.Add(brick);
                }

                // remove bricks from canvas and list
                foreach (Brick brick in removeBricks)
                {
                    wpfCanvas.Children.Remove(brick.GetRectangle());
                    bricks.Remove(brick);
                }
                removeBricks.Clear();
            }//end of ball forloop

            //////////////////
            // bullet logic //
            //////////////////

            List<Bullet> removeBullets = new List<Bullet>();
            foreach (Bullet bullet in bullets) 
            {
                bullet.UpdatePosition();
                // bullet deletion
                if (bullet.Y <= 0) { wpfCanvas.Children.Remove(bullet.GetEllipse()); removeBullets.Add(bullet); } // top
                if (bullet.Y + bullet.Diameter >= wpfCanvas.Height) { wpfCanvas.Children.Remove(bullet.GetEllipse()); removeBullets.Add(bullet); } // bottom
                if (bullet.X + bullet.Diameter >= wpfCanvas.Width) { wpfCanvas.Children.Remove(bullet.GetEllipse()); removeBullets.Add(bullet); } // right
                if (bullet.X <= 0) { wpfCanvas.Children.Remove(bullet.GetEllipse()); removeBullets.Add(bullet); } // right
                // update ball canvas position
                Canvas.SetTop(bullet.GetEllipse(), bullet.Y);
                Canvas.SetLeft(bullet.GetEllipse(), bullet.X);
            }

            //bullet brick collision
            List<Brick> shotBricks = new List<Brick>();
            foreach (Bullet bullet in bullets) {
                foreach (Brick brick in bricks) {
                    Point bulletCenter = bullet.getCenter();
                    // top and bottom
                    //TODO: fix the collision
                    if (brick.X <= bulletCenter.X + bullet.GetRadius() && bulletCenter.X - bullet.GetRadius() <= brick.X + brick.Width) {
                        if (brick.Y <= bulletCenter.Y + bullet.GetRadius() && bulletCenter.Y - bullet.GetRadius() <= brick.Y + brick.Height) // left and right
                        {
                            //manually remove the bullet from canvas
                            //wpfCanvas.Children.Remove(bullet.GetEllipse());
                            
                            removeBullets.Add(bullet);
                            BrickHit(brick);
                        }
                        //BrickHit(brick);
                    }
                    //else if (brick.Y <= bulletCenter.Y && bulletCenter.Y <= brick.Y + brick.Height) // left and right
                    //{
                        
                     //   BrickHit(brick);
                    //}
                    if (brick.Remove) shotBricks.Add(brick);
                }
                // remove bricks from canvas and list
                foreach (Brick brick in shotBricks) {
                    wpfCanvas.Children.Remove(brick.GetRectangle());
                    bricks.Remove(brick);
                }

                
            }//end of bullet forloop

            //remove bullets
            foreach (Bullet bulletToRemove in removeBullets) {
                wpfCanvas.Children.Remove(bulletToRemove.GetEllipse());
                bullets.Remove(bulletToRemove);
            }
            removeBullets.Clear();


            ////////////////////
            // Power up logic //
            ////////////////////
            List<PowerUp> removePowerUps = new List<PowerUp>();
            foreach (PowerUp powerUp in powerUps)
            {
                // update position
                powerUp.UpdatePosition();
                Canvas.SetTop(powerUp.GetRectangle(), powerUp.Y);
                Canvas.SetLeft(powerUp.GetRectangle(), powerUp.X);

                // paddle collision
                if (powerUp.Y + powerUp.Height + 1 >= paddle.Y)
                {
                    // power up on paddle
                    if (paddle.X <= powerUp.X + powerUp.Width && powerUp.X - powerUp.Width <= paddle.X + paddle.Width)
                    {
                        // power up hit paddle

                        // TODO: paddle hit power up so add power up to game
                    }

                    // remove power up
                    removePowerUps.Add(powerUp);
                }
            }

            foreach (PowerUp powerUp in removePowerUps)
            {
                wpfCanvas.Children.Remove(powerUp.GetRectangle());
                powerUps.Remove(powerUp);
            }
            removePowerUps.Clear();

            // Game Over: balls too far back to hit end game
            if (EndGame() || EndLevel())
            {
                //    labelGameOver.Visibility = Visibility.Visible;
                PauseEvent(sender, null);
            }
        }

        private void CornerBounce(Ball gb, double cornerX, double cornerY)
        {
            // https://gamedev.stackexchange.com/questions/10911/a-ball-hits-the-corner-where-will-it-deflect
            double gbCenterX = gb.X + gb.GetRadius();
            double gbCenterY = gb.Y + gb.GetRadius();

            double x = gbCenterX - cornerX;
            double y = gbCenterY - cornerY;

            double c = -2 * (gb.Dx * x + gb.Dy * y) / (x * x + y * y);

            gb.Dx = gb.Dx + c * x;
            gb.Dy = gb.Dy + c * y;
        }

        private double CalcDistance(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt(Math.Pow((x1 - x2), 2) + Math.Pow((y1 - y2), 2));
        }

        private bool EndGame()
        {
            // check if a ball is above paddle
            foreach (Ball ball in balls)
            {
                if (ball.Y + ball.GetRadius() <= paddle.Y) return false;
            }

            // no balls above paddle
            return true;
        }

        private bool EndLevel()
        {
            return bricks.Count == 0;
        }

        private bool CompleteGame()
        {
            return level == maxLevels && bricks.Count == 0;
        }

        private void StartEvent(object sender, RoutedEventArgs e)
        {
            // player loses or wins game
            if (EndGame() || CompleteGame())
            {
                // restart game
                level = 1;
                LoadLevel(level);
                score = 0;
                UpdateScore();

                // remove lose or win message
                labelWinner.Visibility = Visibility.Hidden;
                labelGameOver.Visibility = Visibility.Hidden;
            }

            labelLevel.Visibility = Visibility.Hidden;

            //else if (EndLevel()) // advance to next leve
            //{
            //    LoadLevel(++level);
            //}

            gameTimer.IsEnabled = true;
        }

        private void PauseEvent(object sender, RoutedEventArgs e)
        {
            gameTimer.IsEnabled = false;

            // player wins game
            if (CompleteGame())
            {
                this.labelAmmoCounter.Content = "0";
                labelWinner.Visibility = Visibility.Visible;
                GameWinSound();
            }
            else if (EndGame()) // player loses game
            {
                this.labelAmmoCounter.Content = "0";
                labelGameOver.Visibility = Visibility.Visible;
                GameOverSound();
            }
            else if (EndLevel()) // advance to next level
            {
                LoadLevel(++level);
                labelLevel.Content = "Level: " + level;
                labelLevel.Visibility = Visibility.Visible;
                NextLevelSound();

            }
        }

        private void SettingsEvent(object sender, RoutedEventArgs e)
        {
            PauseEvent(sender, e);

            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.textBoxDefaultSpeed.Text = defaultSpeed.ToString();
            settingsWindow.textBoxSpeedIncrement.Text = incrementSpeed.ToString();

            settingsWindow.ShowDialog();

            try
            {
                double tempDefaultSpeed = Double.Parse(settingsWindow.textBoxDefaultSpeed.Text);
                double tempIncrementSpeed = Double.Parse(settingsWindow.textBoxSpeedIncrement.Text);

                defaultSpeed = tempDefaultSpeed;
                incrementSpeed = tempIncrementSpeed;
            }
            catch (Exception)
            {
                MessageBoxResult result = MessageBox.Show("New setting contained invalid data. New settings are not applied.", "Bad settings", MessageBoxButton.OK);
            }
            

            // reset game
            level = 1;
            LoadLevel(level);
        }

        private void HelpEvent(object sender, RoutedEventArgs e)
        {

        }

        private void ExitEvent(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void StartPauseToggleEvent(object sender, RoutedEventArgs e)
        {
            if (gameTimer.IsEnabled)
            {
                PauseEvent(sender, e);
            }
            else
            {
                StartEvent(sender, e);
            }
        }

        private void wpfKeyDown(object sender, KeyEventArgs e)
        {
            // move paddle by key press
            if (e.Key == Key.Left)
            {
                paddle.GoLeft = true;
            }
            if (e.Key == Key.Right)
            {
                paddle.GoRight = true;
            }

            // shortcuts

            // toggle start/pause shortcut 
            if (e.Key == Key.S)
            {
                StartPauseToggleEvent(sender, e);
            }

            // quit shortcut
            if (e.Key == Key.Q)
            {
                this.Close();
            }

            //shoot bullets
            if(e.Key == Key.Space) 
            {
                //create bullets
                if(ammo > 0) {
                    Point center = paddle.GetPaddleCenter;
                    Bullet bullet = new Bullet(15, center.X, center.Y, 0.70710678, -2);
                    bullets.Add(bullet);
                    wpfCanvas.Children.Add(bullet.GetEllipse());
                    ammo--;
                    this.labelAmmoCounter.Content = ammo.ToString();
                }
            }
        }

      

        private void BrickHit(Brick brick)
        {
            // score and remove
            UpdateScore(1);
            brick.Remove = true;

            // power up drop
            if (brick.PowerUp != "none")
            {
                PowerUp newPowerUp = new PowerUp(brick.X + (brick.Width / 2) - 7.5, brick.Y, 3, brick.PowerUp);
                powerUps.Add(newPowerUp);
                wpfCanvas.Children.Add(newPowerUp.GetRectangle());
                Canvas.SetTop(newPowerUp.GetRectangle(), newPowerUp.Y);
                Canvas.SetLeft(newPowerUp.GetRectangle(), newPowerUp.X);
            }

            // sound
            string fileName = "brickHit.wav";
            string path = System.IO.Path.Combine("../../Sounds/", fileName);
            (new SoundPlayer(path)).Play();
        }

        private void PaddleHit(Paddle paddle)
        {
            // play sound
            string fileName = "ping_pong_8bit_beeep.wav";
            string path = System.IO.Path.Combine("../../Sounds/", fileName);
            (new SoundPlayer(path)).Play();

        }

        private void WallHit()
        {
            string fileName = "betterWallHit.wav";
            string path = System.IO.Path.Combine("../../Sounds/", fileName);
            (new SoundPlayer(path)).Play();
        }

        private void NextLevelSound()
        {
            string fileName = "nextLevel.wav";
            string path = System.IO.Path.Combine("../../Sounds/", fileName);
            (new SoundPlayer(path)).Play();
        }

        private void GameOverSound()
        {
            string fileName = "youLose.wav";
            string path = System.IO.Path.Combine("../../Sounds/", fileName);
            (new SoundPlayer(path)).Play();
        }

        private void GameWinSound()
        {
            string fileName = "win.wav";
            string path = System.IO.Path.Combine("../../Sounds/", fileName);
            (new SoundPlayer(path)).Play();
        }

        private void UpdateScore(int increment = 0)
        {
            score += increment;
            labelScoreNum.Content = score;
        }

        private void AddBrickTexture()
        {
            ImageBrush brush = new ImageBrush();
            brush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Textures/brickTexture.jpg"));
            
        }
    }
}
