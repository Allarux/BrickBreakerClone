﻿using System;
using System.Collections.Generic;
using System.Linq;
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
        private Paddle paddle;
        private long score;
        private long highScore;

        public MainWindow()
        {
            InitializeComponent();

            // ball init
            Random rnd = new Random();
            balls.Add(new Ball(40, 81, (int)wpfCanvas.Height - 100, 0, 3));
            foreach (Ball curBall in balls) {
                // attach balls to canvas
                wpfCanvas.Children.Add(curBall.GetEllipse());
                Canvas.SetTop(curBall.GetEllipse(), curBall.Y);
                Canvas.SetLeft(curBall.GetEllipse(), curBall.X);
            }

            // paddle init
            paddle = new Paddle(0, (int) wpfCanvas.Height - 50, 100, 30);
            wpfCanvas.Children.Add(paddle.GetRectangle());
            Canvas.SetTop(paddle.GetRectangle(), paddle.Y);
            Canvas.SetLeft(paddle.GetRectangle(), paddle.X);

            // brick init
            bricks.Add(new Brick(75, 50, 80, 30));
            foreach (Brick curBrick in bricks)
            {
                wpfCanvas.Children.Add(curBrick.GetRectangle());
                Canvas.SetTop(curBrick.GetRectangle(), curBrick.Y);
                Canvas.SetLeft(curBrick.GetRectangle(), curBrick.X);
            }

            // animation timer
            gameTimer.Interval = new TimeSpan(16000);
            gameTimer.Tick += gameTick;
            gameTimer.IsEnabled = true;

            // score board
            score = 0;
            highScore = 0;
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
                ball.UpdatePosition();

                // update ball canvas position
                Canvas.SetTop(ball.GetEllipse(), ball.Y);
                Canvas.SetLeft(ball.GetEllipse(), ball.X);

                // edge canvas ball bounce
                if (ball.Y <= 0) ball.Dy *= -1; // top
                if (ball.Y + ball.Diameter >= wpfCanvas.Height) ball.Dy *= -1; // bottom
                if (ball.X + ball.Diameter >= wpfCanvas.Width) ball.Dx *= -1; // right
                if (ball.X <= 0) ball.Dx *= -1; // right

                // paddle collision
                if (paddle.X <= ball.X + ball.GetRadius() && ball.X + ball.GetRadius() <= paddle.X + paddle.Width) // ball in paddle X range?
                {
                    if (paddle.Y <= ball.Y + ball.Diameter) // ball at paddle Y level?
                    {
                        // hit paddle
                        ball.Dy *= -1;
                        IncreaseScore();
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
                        IncreaseScore();
                    }
                    if (topLeftHit)
                    {
                        CornerBounce(ball, paddle.X, paddle.Y);
                        IncreaseScore();
                    }
                }

                // brick collision
                foreach (Brick brick in bricks)
                {
                    Point ballCenter = ball.getCenter();

                    Console.WriteLine(ball.GetRadius());
                    Console.WriteLine(brick.Width);
                    Console.WriteLine(brick.X - ball.GetRadius() + " <= " + ballCenter.X + " <= " + (brick.X + brick.Width + ball.GetRadius()));
                    if (brick.X - ball.GetRadius() <= ballCenter.X && ballCenter.X <= brick.X + brick.Width + ball.GetRadius())
                    {
                        if (brick.Y - ball.GetRadius() <= ballCenter.Y && ballCenter.Y <= brick.Y + brick.Height + ball.GetRadius())
                        {
                            // ball is within range to bounce on top, bottom, right, left or maybe the corners

                            // top and bottom
                            if (brick.X <= ballCenter.X && ballCenter.X <= brick.X + brick.Width)
                            {
                                ball.Y *= -1;
                            }
                        }
                    }

                    if (brick.X <= ballCenter.X && ballCenter.X <= brick.X + brick.Width) // ball in brick X range?
                    {
                        if (brick.Y <= ballCenter.Y && ballCenter.Y <= brick.Y + brick.Height) // ball in brick Y range?
                        {

                        }
                    } else
                    {
                        // maybe 
                    }
                }

                // Game Over: ball too far back to hit end game
                if (ball.Y + ball.GetRadius() >= paddle.Y)
                {
                    //    gameTimer.IsEnabled = false;
                    //    labelGameOver.Visibility = Visibility.Visible;
                    PauseEvent(sender, null);
                }
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

        private void StartEvent(object sender, RoutedEventArgs e)
        {
            gameTimer.IsEnabled = true;
        }

        private void PauseEvent(object sender, RoutedEventArgs e)
        {
            gameTimer.IsEnabled = false;

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
        }

        private void IncreaseScore()
        {

        }
    }
}
