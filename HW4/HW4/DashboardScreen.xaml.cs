using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Data.SqlClient;

namespace HW4
{
    /// <summary>
    /// Interaction logic for DashboardScreen.xaml
    /// </summary>
    public partial class DashboardScreen : UserControl
    {
        public DashboardScreen()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //
        }

        private void Dashboard_Loaded(object sender, RoutedEventArgs e)
        {
            //
        }
        private void Dashboard_Unloaded(object sender, RoutedEventArgs e)
        {
            //
        }
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                //this.DragMove();
            }
        }

        bool IsMaximized = false;

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (IsMaximized)
                {
                    //this.WindowState = WindowState.Normal;
                    //this.Width = 1280;
                    //this.Height = 780;

                    bool IsMaximized = false;
                }
                else
                {
                    //this.WindowState = WindowState.Maximized;
                    bool IsMaximized = true;
                }
            }
        }

    }
}
