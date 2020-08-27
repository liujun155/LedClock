using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Windows.Threading;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Threading;
//using System.Threading;

namespace LedClock
{
    /// <summary>
    /// Interaction logic for DigitalClock.xaml
    /// </summary>
    public partial class LedClockC : UserControl
    {
        public static readonly DependencyProperty HideSecondProperty =
            DependencyProperty.Register("HideSecond", typeof(bool), typeof(LedClockC),
            new PropertyMetadata(false, new PropertyChangedCallback(OnHideSecondInvalidated)));
        //依赖属性值改变后触发的事件
        public static readonly RoutedEvent HideSecondChangedEvent = EventManager.RegisterRoutedEvent("HideSecondChanged",
            RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<bool>),
            typeof(LedClockC));
        public bool HideSecond
        {
            get { return (bool)GetValue(HideSecondProperty); }
            set { SetValue(HideSecondProperty, value); }
        }
        //static LedClockC()
        //{
        //    DefaultStyleKeyProperty.OverrideMetadata(typeof(LedClockC), new FrameworkPropertyMetadata(typeof(LedClockC)));
        //}
        private static void OnHideSecondInvalidated(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LedClockC clock = (LedClockC)d;
            bool old = (bool)e.OldValue;
            bool newV = (bool)e.NewValue;
            clock.OnHideSecondChanged(old, newV);
        }

        protected virtual void OnHideSecondChanged(bool oldValue, bool newValue)
        {
            if (newValue)
            {
                //g1.Visibility = Visibility.Hidden;
                ptMin.Visibility = Visibility.Hidden;
                v1.Visibility = Visibility.Hidden;
                v2.Visibility = Visibility.Hidden;
                g.ColumnDefinitions[5].Width = new GridLength(0);
                g.ColumnDefinitions[6].Width = new GridLength(0);
                g.ColumnDefinitions[7].Width = new GridLength(0);
            }
            else
            {
                //g1.Visibility = Visibility.Visible;
                ptMin.Visibility = Visibility.Visible;
                v1.Visibility = Visibility.Visible;
                v2.Visibility = Visibility.Visible;
                g.ColumnDefinitions[5].Width = new GridLength(0.5, GridUnitType.Star);
                g.ColumnDefinitions[6].Width = new GridLength(2);
                g.ColumnDefinitions[7].Width = new GridLength(0.5, GridUnitType.Star);
            }
            RoutedPropertyChangedEventArgs<bool> args = new RoutedPropertyChangedEventArgs<bool>(oldValue, newValue);
            args.RoutedEvent = LedClockC.HideSecondChangedEvent;
            RaiseEvent(args);
        }

        private DateTime currentTime = DateTime.Now;
        private DispatcherTimer timer;
        public LedClockC()
        {
            InitializeComponent();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1000);
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }
        public Brush DigitBrush
        {
            get { return p0.Fill; }
            set
            {
                p0.Fill = value;
                p1.Fill = value;
                p3.Fill = value;
                p4.Fill = value;
                p6.Fill = value;
                p7.Fill = value;
                p00.Fill = value;
                p11.Fill = value;
                p22.Fill = value;
                p33.Fill = value;
                p55.Fill = value;
                p66.Fill = value;
                p88.Fill = value;
                p99.Fill = value;
            }
        }
        public Brush DotBrush
        {
            get { return d0.Fill; }
            set
            {
                d0.Fill = value;
                d1.Fill = value;
                d2.Fill = value;
                d3.Fill = value;
                d4.Fill = value;
                d5.Fill = value;
            }
        }

        public DateTime CurrentTime
        {
            get
            {
                return currentTime;
            }
            set
            {
                currentTime = value;
                #region year
                string year = value.Year.ToString();
                p00.Data = (GeometryGroup)g.Resources["p0" + year[0]];
                p11.Data = (GeometryGroup)g.Resources["p0" + year[1]];
                p22.Data = (GeometryGroup)g.Resources["p0" + year[2]];
                p33.Data = (GeometryGroup)g.Resources["p0" + year[3]];
                #endregion
                #region month
                if(value.Month > 9)
                {
                    p55.Data = (GeometryGroup)g.Resources["p0" + (value.Month / 10)];
                    p66.Data = (GeometryGroup)g.Resources["p0" + (value.Month % 10)];
                }
                else
                {
                    p55.Data = (GeometryGroup)g.Resources["p00"];
                    p66.Data = (GeometryGroup)g.Resources["p0" + value.Month];
                }
                #endregion
                #region day
                if(value.Day > 9)
                {
                    p88.Data = (GeometryGroup)g.Resources["p0" + (value.Day / 10)];
                    p99.Data = (GeometryGroup)g.Resources["p0" + (value.Day % 10)];
                }
                else
                {
                    p88.Data = (GeometryGroup)g.Resources["p00"];
                    p99.Data = (GeometryGroup)g.Resources["p0" + value.Day];
                }
                #endregion
                #region hours
                //string strv;
                if (value.Hour > 9)
                {
                    //p0.Value = value.Hour / 10;//int.Parse(value.Hour.ToString()[0].ToString());
                    //p1.Value = value.Hour % 10;//int.Parse(value.Hour.ToString()[1].ToString());
                    //strv = "p0" + (value.Hour / 10);
                    p0.Data = (GeometryGroup)g.Resources["p0" + (value.Hour / 10)];
                    p1.Data = (GeometryGroup)g.Resources["p0" + (value.Hour % 10)];
                }
                else
                {
                    //p0.Value = 0;
                    //p1.Value = value.Hour;//int.Parse(value.Hour.ToString()[0].ToString());
                    p0.Data = (GeometryGroup)g.Resources["p00"];
                    p1.Data = (GeometryGroup)g.Resources["p0" + value.Hour];
                }
                #endregion
                #region minutes
                if (value.Minute > 9)
                {
                    //p3.Value = value.Minute / 10;//int.Parse(value.Minute.ToString()[0].ToString());
                    //p4.Value = value.Minute % 10;//int.Parse(value.Minute.ToString()[1].ToString());
                    p3.Data = (GeometryGroup)g.Resources["p0" + (value.Minute / 10)];
                    p4.Data = (GeometryGroup)g.Resources["p0" + (value.Minute % 10)];
                }
                else
                {
                    //p3.Value = 0;
                    //p4.Value = value.Minute;//int.Parse(value.Minute.ToString()[0].ToString());
                    p3.Data = (GeometryGroup)g.Resources["p00"];
                    p4.Data = (GeometryGroup)g.Resources["p0" + value.Minute];
                }
                #endregion
                #region seconds
                if (value.Second > 9)
                {
                    //p6.Value = value.Second / 10;//int.Parse(value.Second.ToString()[0].ToString());
                    //p7.Value = value.Second % 10;//int.Parse(value.Second.ToString()[1].ToString());
                    p6.Data = (GeometryGroup)g.Resources["p0" + (value.Second / 10)];
                    p7.Data = (GeometryGroup)g.Resources["p0" + (value.Second % 10)];
                }
                else
                {
                    //p6.Value = 0;
                    //p7.Value = value.Second;//int.Parse(value.Second.ToString()[0].ToString());
                    p6.Data = (GeometryGroup)g.Resources["p00"];
                    p7.Data = (GeometryGroup)g.Resources["p0" + value.Second];
                }
                #endregion
            }
        }
        private void timer_Tick(Object sender, EventArgs e)
        {
            CurrentTime = DateTime.Now;
        }
    }
}
