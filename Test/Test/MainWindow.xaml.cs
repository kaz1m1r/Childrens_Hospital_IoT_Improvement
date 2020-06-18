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
using System.Windows.Navigation;
using System.Windows.Shapes;
using TestLibrary.Logic;

namespace TestLibrary
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            btnBullshit.Content = "Second";
        }

        private void BullshitClicked(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Beschrijving",
                "Titel",
                MessageBoxButton.YesNo,
                MessageBoxImage.Error);

            Class1 thing = new Class1();
            thing.PrintBullshit();
        }
    }
}
