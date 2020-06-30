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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BabyphoneIoT.Interface.Nursing
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class NurseConfigurationWindow : Window
    {
        #region Fields
        private Nurse _nurse;
        private List<Room> _rooms;
        private List<Baby> _babies;

        private CancellationTokenSource _ctsCaretakerListUpdate;
        private CancellationTokenSource _ctsListenForHelp;
        #endregion

        #region Constructors
        public NurseConfigurationWindow(string username)
        {
            InitializeComponent();

            // Initialise nurse
            _nurse = new Nurse(username);
            _rooms = _nurse.GetRooms();

            // Get rooms in hospital and add to combobox
            cbRooms.Items.Clear();
            _rooms.ForEach(room =>
            {
                cbRooms.Items.Add(new ComboboxItem<Room>(room, item => {
                    return item.RoomName;
                }));
            });
            cbRooms.SelectedIndex = 0;

            // Passively listen for available caretakers and update the list
            _ = Task.Run(() => UpdateCaretakerList());
        }
        #endregion

        #region Event Handlers
        #region Combobox Room
        private void RoomSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Get selected room and get new baby list based on the room
            Room room = ((sender as ComboBox).SelectedItem as ComboboxItem<Room>).Value;
            _babies = room.GetBabies();

            // Update baby list
            cbBabies.Items.Clear();
            _babies.ForEach(baby =>
            {
                cbBabies.Items.Add(new ComboboxItem<Baby>(baby, item => {
                    return item.MonitorName.Split(new string[] { "-babyphone" }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
                }));
            });
            cbBabies.SelectedIndex = 0;
        }
        #endregion

        #region Buttons
        private void ButtonAttachCaretakerClicked(object sender, RoutedEventArgs e)
        {
            // Check if values exist for the baby and caretaker
            if (cbBabies.SelectedIndex == -1 || cbCaretakers.SelectedIndex == -1)
            {
                TriggerAttachmentError();
                return;
            }

            // Get selected baby
            Baby baby = ((cbBabies.SelectedItem as ComboboxItem<Baby>)?.Value);

            // Get selected caretaker
            Caretaker caretaker = ((cbCaretakers.SelectedItem as ComboboxItem<Caretaker>)?.Value);

            // Trigger error if an unknown value was found
            if (baby == null || caretaker == null)
            {
                TriggerAttachmentError();
                return;
            }

            // Attach caretaker to baby monitor
            baby.AssignNurse(_nurse);
            baby.AttachCaretaker(new AddressData(caretaker.Name, caretaker.Address));

            // Start listening for unsubscribe and help requests
            _ = Task.Run(() => ListenForHelpRequest(baby));
            baby.CaretakerUnsubscribed += ct => ListenForUnsubscribe(ct);
        }
        private void Detach_parent_button_clicked(object sender, RoutedEventArgs e)
        {
            //string selected = babyCombobox.Text;
            //attachmentUpdate.Text = "Detached " + selected;

        }
        #endregion
        #endregion

        #region Methods
        #region Caretaker Combobox
        private void UpdateCaretakerList()
        {
            _ctsCaretakerListUpdate = new CancellationTokenSource();

            while (!_ctsCaretakerListUpdate.IsCancellationRequested)
            {
                // Get selected value and get new baby list
                List<Caretaker> caretakers = _nurse.GetCaretakers();
                Caretaker selectedCaretaker = null;

                Dispatcher.Invoke(() =>
                {
                    selectedCaretaker = (cbCaretakers.SelectedItem as ComboboxItem<Caretaker>)?.Value;
                });

                // Update baby list
                Dispatcher.Invoke(() =>
                {
                    cbCaretakers.Items.Clear();
                    caretakers.ForEach(caretaker =>
                    {
                        cbCaretakers.Items.Add(new ComboboxItem<Caretaker>(caretaker, item => {
                            return item.Name;
                        }));
                    });
                });

                // Set selected index to previously selected item or to the first item
                if (cbCaretakers.Items.Count != 0)
                {
                    if (selectedCaretaker == null)
                    {
                        Dispatcher.Invoke(() => { cbBabies.SelectedIndex = 0; });
                        return;
                    }

                    Dispatcher.Invoke(() =>
                    {
                        ComboboxItem<Caretaker> ctItem = cbCaretakers.Items.OfType<ComboboxItem<Caretaker>>()
                                                                     .Where(ct => ct.Value.Name == selectedCaretaker.Name)
                                                                     .FirstOrDefault();
                        int index = cbCaretakers.Items.IndexOf(ctItem);
                        index = index != -1 ? index : 0;
                        cbBabies.SelectedIndex = index;
                    });
                }

                // Wait for 1 second before retrying
                Thread.Sleep(1000);
            }
        }
        #endregion

        #region Input Checks
        private void TriggerAttachmentError()
        {
            MessageBox.Show("Er is geen ouder of baby opgegeven!",
                            "Nurse Monitoring - Koppeling Fout",
                            MessageBoxButton.OK,
                            MessageBoxImage.Exclamation);
        }
        #endregion

        #region Listening Handlers
        private void ListenForHelpRequest(Baby baby)
        {
            _ctsListenForHelp = new CancellationTokenSource();

            while (!_ctsListenForHelp.IsCancellationRequested)
            {
                string babyId = _nurse.AwaitNotification();

                if (baby.BabyId == babyId)
                    Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show($"Hulp gevraagd voor baby ‘{baby.MonitorName.Split(new string[] { "-babyphone" }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault()}’",
                                        "Nurse Monitoring - Hulpvraag",
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Information);
                    });
            }
        }

        private void ListenForUnsubscribe(string caretaker)
        {
            Dispatcher.Invoke(() =>
            {
                MessageBox.Show($"{caretaker} heeft zich uitgeschreven.",
                                "Nurse Monitoring - Gebruiker Ontkoppeld",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
            });
        }
        #endregion
        #endregion

        #region Helper Classes
        /// <summary>
        /// Helper class to display to add custom objects to a combobox.
        /// </summary>
        /// <typeparam name="T">The type of the combobox item value.</typeparam>
        internal class ComboboxItem<T>
        {
            public T Value { get; private set; }
            private Func<T, string> _toStringFunc;

            public ComboboxItem(T item, Func<T, string> toStringFunction)
            {
                this.Value = item;
                this._toStringFunc = toStringFunction;
            }

            public override string ToString()
            {
                return _toStringFunc(Value);
            }
        }
        #endregion
    }
}
