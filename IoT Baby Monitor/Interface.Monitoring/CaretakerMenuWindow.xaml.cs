using BabyphoneIoT.DataEntities;
using BabyphoneIoT.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BabyphoneIoT.Interface.Monitoring
{
    /// <summary>
    /// Interaction logic for WaitForAttachment.xaml
    /// </summary>
    public partial class CaretakerMenuWindow : Window
    {
        #region Fields
        private Caretaker _caretaker;

        private bool _newCry = false;

        private CancellationTokenSource _ctsWaitingAnimation;
        private CancellationTokenSource _ctsBabyStatus;
        #endregion

        #region Constructors
        public CaretakerMenuWindow(string username)
        {
            InitializeComponent();

            // Initialise caretaker
            _caretaker = new Caretaker(username);

            // Broadcast identity to nurses and wait for attachment
            _ = Task.Run(() => WaitForAttachment());
        }
        #endregion

        #region Event Handlers
        private void BtnRequestBabyHelp(object sender, RoutedEventArgs e)
        {
            _caretaker.RequestNurse();

            MessageBox.Show("De hulpvraag voor de verpleger is gemeld.",
                            "Caretaker Monitoring - Verpleger gewaarschuwd!",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
        }

        private void BtnStopMonitoring(object sender, RoutedEventArgs e)
        {
            _ctsBabyStatus.Cancel();
            _ctsWaitingAnimation.Cancel();
            _caretaker.Unsubscribe();

            MessageBox.Show("Monitoren gestopt.",
                            "Caretaker Monitoring - Tot Ziens!",
                            MessageBoxButton.OK,
                            MessageBoxImage.None);

            this.Close();
        }
        #endregion

        #region Methods
        #region Listening
        /// <summary>
        /// Broadcast self and wait for attachment.
        /// </summary>
        private async void WaitForAttachment()
        {
            // Wait for connection
            Task<string> getBaby = Task.Run(() => _caretaker.Broadcast());
            _ = Task.Run(() => AnimateWaitingInterface());

            // Get the connected baby and stop the animation
            string baby = await getBaby;
            _ctsWaitingAnimation.Cancel();

            // Notify caretaker of connection and update user interface
            SetUserInterfaceConnected();
        }
        #endregion

        #region User Interface
        private void AnimateWaitingInterface()
        {
            // Set cancelation token
            _ctsWaitingAnimation = new CancellationTokenSource();

            int dotCount = 1;

            // Animate waiting label
            while(!_ctsWaitingAnimation.IsCancellationRequested)
            {
                Dispatcher.Invoke(() =>
                {
                    lblWaiting.Content = "Wachten op koppeling" + new string('.', dotCount);
                });

                dotCount = dotCount != 3 ? dotCount+1 : 1;

                Thread.Sleep(250);
            }
        }

        private void SetUserInterfaceConnected()
        {
            // Get baby name
            string baby = _caretaker.GetBabyName(0);

            // Notify caretaker
            MessageBox.Show($"Gekoppeld aan baby ‘{baby}’.",
                            "Caretaker Monitoring - Baby Gekoppeld",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);

            // Update user interface
            _ = Task.Run(() => SetBabyStatus());
            Dispatcher.Invoke(() => { panelConnectedButtons.Visibility = Visibility.Visible; });
        }

        private void SetBabyStatus()
        {
            // Set cancelation token
            _ctsBabyStatus = new CancellationTokenSource();

            // Update the baby's status
            while (!_ctsBabyStatus.IsCancellationRequested)
            {
                // Get status
                BabyStatus status = _caretaker.MonitorBaby(0);
                string message;

                switch (status)
                {
                    case BabyStatus.Crying:
                        message = "De baby huilt.";
                        if (_newCry)
                            NotifyNewCry();
                        _newCry = false;
                        break;
                    case BabyStatus.Quiet:
                        message = "De baby is stil.";
                        _newCry = true;
                        break;
                    default:
                        message = "De status is onbekend.";
                        break;
                }

                // Display status
                Dispatcher.Invoke(() =>
                {
                    lblWaiting.Content = $"Baby Status van '{_caretaker.GetBabyName(0).Split(new string[] { "-babyphone" }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault()}': {message}";
                });

                Thread.Sleep(250);
            }
        }

        private void NotifyNewCry()
        {
            string babyName = _caretaker.GetBabyName(0).Split(new string[] { "-babyphone" }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();

            MessageBox.Show($"De baby '{babyName}' is aan het huilen!",
                            "Caretaker Monitoring - Baby Huilt!",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
        }
        #endregion
        #endregion
    }
}
