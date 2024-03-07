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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PR3WPF
{
    public partial class HistoryWindow : Window
    {
        public int i { get; set; }

        public List<string> music { get; set; }

        public HistoryWindow(List<string> playlist)
        {
            InitializeComponent();

            music = playlist;
            HistoryList.ItemsSource = music;
        }

        private void PlayHistory_Click(object sender, RoutedEventArgs e)
        {
            if (HistoryList.SelectedIndex >= 0)
            {
                i = HistoryList.SelectedIndex;
                DialogResult = true;
                Close();
            }
        }
    }
}
