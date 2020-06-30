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

/// <summary>
/// Parental login
/// </summary>
namespace BabyphoneIoT.Interface.Monitoring
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        #region Constructors
        public LoginWindow()
        {
            InitializeComponent();
        }
        #endregion

        #region Event Handlers
        private void Login_Button_Click(object sender, RoutedEventArgs e)
        {
            Login();
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                Login();
            }
        }
        #endregion

        #region Methods
        private void Login()
        {
            if (string.IsNullOrWhiteSpace(tbUsername.Text))
            {
                MessageBox.Show("Je hebt nog geen gebruikersnaam ingegeven!",
                                "Caretaker Monitoring - Login",
                                MessageBoxButton.OK,
                                MessageBoxImage.Exclamation);

                return;
            }

            Window window = new CaretakerMenuWindow(tbUsername.Text);
            window.Show();
            this.Close();
        }
        #endregion
    }
}
