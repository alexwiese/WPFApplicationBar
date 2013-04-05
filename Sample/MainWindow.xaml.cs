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
using WpfApplicationBar;

namespace WpfApplicationBar.Sample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ApplicationBarWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        
        private void ChangeEdgeButton_Click(object sender, RoutedEventArgs e)
        {
            this.CurrentEdge = this.CurrentEdge == DisplayEdge.Top ? this.CurrentEdge = DisplayEdge.Bottom : this.CurrentEdge = DisplayEdge.Top;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
