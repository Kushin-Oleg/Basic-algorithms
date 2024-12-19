using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;



namespace _3;

public partial class MainWindow : Window
{
    private const int Ball_Count = 6;
    
    private Ellipse[] balls;
    
    private bool is_stop = false;
    
    private bool movingRight1 = true;
    private bool movingRight2 = true;
    private bool movingRight3 = true;
    private bool movingRight4 = true;
    private bool movingRight5 = true;
    private bool movingRight6 = true;
    
    
    private object locker_1 = new object();
    private object locker_2 = new object(); 
    
    private int speed = 1;
    
    private Mutex mutex;
    private Semaphore semaphore;
    
    public MainWindow()
    {
        InitializeComponent();
    }

    private void Initializing_Balls()
    {
        balls = new Ellipse[Ball_Count];
        for (int i = 0; i < Ball_Count; i++)
        {
            balls[i] = new Ellipse
            {
                Width = 30,
                Height = 30,
                Fill = Brushes.Green,
                Stroke = Brushes.Black
            };
            Canvas.SetLeft(balls[i], 0);
            Canvas.SetTop(balls[i], i * 40);
            canvas.Children.Add(balls[i]);
        }
    }

    async private void Start_Move()
    {
        try
        {
            this.is_stop = false;
            mutex = new Mutex();
            semaphore = new Semaphore(1, 1);
            
            Thread thread_ball_1 = new Thread(() => Move_Ball(0))
            {
                Priority = ThreadPriority.Highest
            };
            thread_ball_1.Start();
            
            Thread thread_ball_2 = new Thread(() => Move_Ball_Monitor(1))
            {
                Priority = ThreadPriority.Normal
            };
            thread_ball_2.Start();
            
            Task.Run(() => Move_Ball_Mutex(2));
            Task.Run(() => Move_Ball_Semaphone(3));
            Task.Run(() => Move_Ball_Parralel_For(4));
            await Task.Run(() => Move_Ball_Async(5));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private void Move_Ball(int index)
    {
        lock (locker_1)
        {
            while (!this.is_stop)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    double left = Canvas.GetLeft(balls[index]);

                    if (left <= 0)
                    {
                        movingRight1 = true;
                    }
                    else if (left >= 700)
                    {
                        movingRight1 = false;
                    }

                    Canvas.SetLeft(balls[index], left + (movingRight1 ? speed : -speed));
                });

                if (is_stop == true)
                {
                    break;
                }

                Thread.Sleep(10);
            }
        }
    }
    
    private void Move_Ball_Monitor(int index)
    {
        bool acquiredLock = false;
        try
        {
            Monitor.Enter(locker_2, ref acquiredLock);
            while (!this.is_stop)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    double left = Canvas.GetLeft(balls[index]);

                    if (left <= 0)
                    {
                        movingRight2 = true;
                    }
                    else if (left >= 700)
                    {
                        movingRight2 = false;
                    }

                    Canvas.SetLeft(balls[index], left + (movingRight2 ? speed : -speed));
                });

                if (is_stop == true)
                {
                    break;
                }

                Thread.Sleep(10);
            }
        }
        finally
        {
            if (acquiredLock) Monitor.Exit(locker_2);
        }
    }

    private void Move_Ball_Mutex(int index)
    {
        {
            mutex.WaitOne();
            while (!this.is_stop)
            {
                
                Application.Current.Dispatcher.Invoke(() =>
                {
                    double left = Canvas.GetLeft(balls[index]);

                    if (left <= 0)
                    {
                        movingRight3 = true;
                    }
                    else if (left >= 700)
                    {
                        movingRight3 = false;
                    }

                    Canvas.SetLeft(balls[index], left + (movingRight3 ? speed : -speed));
                });
                
                
                if (is_stop == true)
                {
                    break;
                }

                Thread.Sleep(10);
            }
            mutex.ReleaseMutex();
        }
    }
    
    private void Move_Ball_Semaphone(int index)
    {
        {
            
            while (!this.is_stop)
            {
                semaphore.WaitOne();
                Application.Current.Dispatcher.Invoke(() =>
                {
                    double left = Canvas.GetLeft(balls[index]);

                    if (left <= 0)
                    {
                        movingRight4 = true;
                    }
                    else if (left >= 700)
                    {
                        movingRight4 = false;
                    }

                    Canvas.SetLeft(balls[index], left + (movingRight4 ? speed : -speed));
                });
                semaphore.Release();
                
                if (is_stop == true)
                {
                    break;
                }

                Thread.Sleep(10);
            }
            
        }
    }
    
    private void Move_Ball_Parralel_For(int index)
    {
        {
            Parallel.For(0, 700, _ =>
            {
                while (!this.is_stop)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        double left = Canvas.GetLeft(balls[index]);

                        if (left <= 0)
                        {
                            movingRight5 = true;
                        }
                        else if (left >= 700)
                        {
                            movingRight5 = false;
                        }

                        Canvas.SetLeft(balls[index], left + (movingRight5 ? speed : -speed));
                    });

                    if (is_stop == true)
                    {
                        break;
                    }

                    Thread.Sleep(10);
                }
            });
        }
    }
    
    private async Task Move_Ball_Async(int index)
    {
        while (!this.is_stop)
        {
            await Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    double left = Canvas.GetLeft(balls[index]);

                    if (left <= 0)
                    {
                        movingRight6 = true;
                    }
                    else if (left >= 700)
                    {
                        movingRight6 = false;
                    }

                    Canvas.SetLeft(balls[index], left + (movingRight6 ? speed : -speed));
                });
                
                Thread.Sleep(10);
            });
            if (is_stop == true)
            {
                break;
            }
        }
    }
    
    private void Button_Create_OnClick(object sender, RoutedEventArgs e)
    {
        Initializing_Balls();
    }

    private void Button_Start_OnClick(object sender, RoutedEventArgs e)
    {
        Start_Move();
    }

    private void Button_Stop_OnClick(object sender, RoutedEventArgs e)
    {
        is_stop = true;
    }
}
