using BabyphoneIoT.CaretakerCommunication;
using System.Threading.Tasks;

namespace BabyphoneIoT.Logic
{
    /// <summary>
    /// Defines a user of type caretaker who can monitor a baby only when assigned to one.
    /// </summary>
    public class Caretaker : User
    {
        #region Fields
        private ICaretakerCommunicator _communicator;

        private bool _isBroadcasting;
        private bool _firstBroadcast;
        #endregion

        #region Properties
        /// <summary>
        /// The communicator for the caretaker to communicate with a nurse.
        /// </summary>
        internal ICaretakerCommunicator Communicator
        {
            get { return _communicator; }
            private set { _communicator = value; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initiates a caretaker with a given identity.
        /// </summary>
        /// <param name="name">The user's identity.</param>
        public Caretaker(string name)
            : base(name)
        {
            
        }
        /// <summary>
        /// Initiates a caretaker with a given identity, matched with an address.
        /// </summary>
        /// <param name="name">The user's identity.</param>
        /// <param name="address">The user's IP address.</param>
        public Caretaker(string name, string address)
            : base(name, address)
        {
            _communicator = new CaretakerCommunicator(name);
            _firstBroadcast = true;
        }
        #endregion

        #region Functions
        /// <summary>
        /// Broadcast the caretaker for all nurse's to see and listen for a baby monitor to attach.
        /// </summary>
        public void Broadcast()
        {
            if (!HasBabyMonitor())
            {
                _ = Task.Run(() => _communicator.BroadcastSelf());
                _ = Task.Run(() => GetBabyMonitor());
                _isBroadcasting = true;
            }
        }
        /// <summary>
        /// Send a help request to the assigned nurse of the caretaker's baby.
        /// </summary>
        public void RequestNurse()
        {
            if (HasBabyMonitor())
                BabyMonitors[0].NotifyNurse();
        }
        public void Unsubscribe()
        {
            if (HasBabyMonitor())
                BabyMonitors[0].Unsubscribe();
        }
        #endregion

        #region Methods
        #region Explicit Methods
        private bool HasBabyMonitor()
        {
            return !_isBroadcasting || _firstBroadcast;
        }
        #endregion

        #region Listeners
        /// <summary>
        /// Listen for a baby monitor from a nurse to be assigned to them.
        /// </summary>
        private void GetBabyMonitor()
        {
            // Get baby monitor
            string babyId = _communicator.GetBabyMonitor();
            BabyMonitors.Add(new Baby(babyId, _communicator));
            _isBroadcasting = false;

            // Listen for proper disconnection
            while (!ListenForDetachmentRequest())
            {

            }
        }
        /// <summary>
        /// Listen for the nurse to detach them from the baby.
        /// </summary>
        /// <returns>Returns true if the request was made.</returns>
        private bool ListenForDetachmentRequest()
        {
            return _communicator.ListenForDetachmentRequest();
        }
        #endregion
        #endregion
    }
}
