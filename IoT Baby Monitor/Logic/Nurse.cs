using BabyphoneIoT.BabyphoneDataAccess;
using BabyphoneIoT.CaretakerCommunication;
using BabyphoneIoT.DataEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabyphoneIoT.Logic
{
    public class Nurse : User
    {
        #region Fields
        private IHospital _iotDal;
        private INurseCommunicator _communicator;
        #endregion

        #region Properties

        #endregion

        #region Constructors
        public Nurse(string name, IHospital hospitalIoTDAL = null, INurseCommunicator communicator = null)
            : base(name)
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

        public Baby AwaitNotification()
        {
            string babyId = _communicator.ListenForRequests();
            // TODO: Return baby request.

            throw new NotImplementedException();
        }
        #endregion
    }
}
