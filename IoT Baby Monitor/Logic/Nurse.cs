using BabyphoneIoT.BabyphoneDataAccess;
using BabyphoneIoT.CaretakerCommunication;
using System.Collections.Generic;

namespace BabyphoneIoT.Logic
{
    /// <summary>
    /// Defines a user of type nurse who can monitor multiple babies or assist in assigning a caretaker to a baby monitor.
    /// </summary>
    public class Nurse : User
    {
        #region Fields
        private IHospital _iotDal;
        private INurseCommunicator _communicator;
        #endregion

        #region Properties
        /// <summary>
        /// The communicator for the nurse to communicate with caretakers.
        /// </summary>
        internal INurseCommunicator Communicator {
            get { return _communicator; }
            private set { _communicator = value; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initiates a nurse with a given identity.
        /// </summary>
        /// <param name="name">The user's identity.</param>
        /// <param name="hospitalIoTDAL">The data access to the IoT environment floorplanning.</param>
        /// <param name="communicator">The communicator to caretakers.</param>
        public Nurse(string name, IHospital hospitalIoTDAL = null, INurseCommunicator communicator = null)
            : base(name)
        {
            this._iotDal = hospitalIoTDAL ?? new DomoticzHospital();
            this._communicator = communicator ?? new NurseCommunicator(name);
        }
        /// <summary>
        /// Initiates a nurse with a given identity, matched with an address.
        /// </summary>
        /// <param name="name">The user's identity.</param>
        /// <param name="address">The user's IP address.</param>
        /// <param name="hospitalIoTDAL">The data access to the IoT environment floorplanning.</param>
        /// <param name="communicator">The communicator to caretakers.</param>
        public Nurse(string name, string address, IHospital hospitalIoTDAL = null, INurseCommunicator communicator = null)
            : base(name, address)
        {
            this._iotDal = hospitalIoTDAL ?? new DomoticzHospital();
            this._communicator = communicator ?? new NurseCommunicator(name);
        }
        #endregion

        #region Functions
        /// <summary>
        /// Get all existing rooms in the IoT environment.
        /// </summary>
        /// <returns>Returns a list of rooms.</returns>
        public List<Room> GetRooms()
        {
            Dictionary<string, string> roomData = _iotDal.GetRooms();

            List<Room> rooms = new List<Room>();

            foreach (KeyValuePair<string, string> room in roomData)
            {
                rooms.Add(new Room(room.Key, room.Value, this._iotDal));
            }

            return rooms;
        }
        /// <summary>
        /// Gets the currently active caretakers.
        /// </summary>
        /// <returns>Returns the found caretakers.</returns>
        public List<Caretaker> GetCaretakers()
        {
            List<Caretaker> caretakers = new List<Caretaker>();

            _communicator.GetCaretakers().ForEach(caretaker =>
            {
                caretakers.Add(new Caretaker(caretaker.identity, caretaker.address));
            });

            return caretakers;
        }
        /// <summary>
        /// Awaits any requests from caretakers.
        /// </summary>
        /// <returns>Returns the name of the baby who needs to be check out.</returns>
        public string AwaitNotification()
        {
            return _communicator.ListenForRequests();
        }
        #endregion
    }
}
