using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BabyphoneIoT.BabyphoneDataAccess;
using BabyphoneIoT.CaretakerCommunication;
using BabyphoneIoT.DataEntities;

namespace BabyphoneIoT.Logic
{
    /// <summary>
    /// Allows the monitoring of a baby and handles the communication between a nurse and a caretaker.
    /// </summary>
    public class Baby : IBabyMonitor
    {
        #region Fields
        private IBabyphone _iotDal;
        private INurseCommunicator _nurseCommunicator;
        private ICaretakerCommunicator _caretakerCommunicator;
        #endregion

        #region Properties
        /// <summary>
        /// The baby monitor identity.
        /// </summary>
        public string BabyId { get; private set; }
        /// <summary>
        /// The monitor name.
        /// </summary>
        public string MonitorName { get; private set; }
        /// <summary>
        /// The room id where the monitor resides.
        /// </summary>
        public string RoomId { get; private set; }
        /// <summary>
        /// The assigned nurse.
        /// </summary>
        public Nurse Nurse { get; private set; }
        /// <summary>
        /// The asigned caretakers.
        /// </summary>
        public List<Caretaker> Caretakers { get; private set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Instantiate a baby monitor of a baby as a nurse.
        /// </summary>
        /// <param name="babyId">The baby monitor id.</param>
        /// <param name="monitorName">The monitor name.</param>
        /// <param name="roomId">The room id.</param>
        /// <param name="hospitalIoTDAL">The hospital floorplanning IoT data access.</param>
        public Baby(string babyId, string monitorName, string roomId, IHospital hospitalIoTDAL = null)
        {
            this.BabyId = babyId;
            this.MonitorName = monitorName;
            this.RoomId = roomId;

            this.Caretakers = new List<Caretaker>();
            this._iotDal = hospitalIoTDAL != null ? hospitalIoTDAL.GetBabyphone(babyId) : new DomoticBabyphone(babyId);
        }
        /// <summary>
        /// Instantiate a baby monitor of a baby as a caretaker.
        /// </summary>
        /// <param name="babyId">The baby monitor id.</param>
        /// <param name="caretakerCom">The caretaker communicator.</param>
        /// <param name="hospitalIoTDAL">The hospital floorplanning IoT data access.</param>
        public Baby(string babyId, ICaretakerCommunicator caretakerCom, IHospital hospitalIoTDAL = null)
        {
            this.BabyId = babyId;

            this.Caretakers = new List<Caretaker>();
            this._iotDal = hospitalIoTDAL != null ? hospitalIoTDAL.GetBabyphone(babyId) : new DomoticBabyphone(babyId);

            this._caretakerCommunicator = caretakerCom;
        }
        #endregion

        #region Functions
        #region IBabyMonitor Interface
        /// <summary>
        /// Gets the status of the baby.
        /// </summary>
        /// <returns>Returns baby monitor status.</returns>
        public BabyStatus MonitorBaby()
        {
            return _iotDal.GetBabyState();
        }
        /// <summary>
        /// Notify the nurse of the baby that they are requested.
        /// </summary>
        public void NotifyNurse()
        {
            if (!IsClientCaretaker())
                throw new NullReferenceException("Cannot execute function if the client is not a caretaker.");

            // Handle communication
            _caretakerCommunicator.SendNurseRequest();
        }
        /// <summary>
        /// Notifies the nurse that the caretaker unsubscribed.
        /// </summary>
        public void Unsubscribe()
        {
            if (!IsClientCaretaker())
                throw new NullReferenceException("Cannot execute function if the client is not a caretaker.");

            // Handle communication
            _caretakerCommunicator.SendUnsubscribe();
        }
        #endregion

        #region Explicit Functions
        /// <summary>
        /// Attach a caretaker to the baby monitor.
        /// </summary>
        /// <param name="caretaker">The caretaker identity and their address.</param>
        public void AttachCaretaker(AddressData caretaker)
        {
            if (!HasNurseAssigned())
                throw new NullReferenceException("Cannot execute function without a nurse assigned.");

            // Handle communication
            _nurseCommunicator.ConnectToCaretaker(caretaker);
            _nurseCommunicator.AttachBabyMonitorToCaretaker(BabyId);

            // Start listening on communication line
            if (Caretakers.Count == 0)
            {
                _ = Task.Run(() => ListenUnsubscribe());
            }

            // Add caretaker to monitor list
            Caretakers.Add(new Caretaker(caretaker.identity, caretaker.address));
        }
        /// <summary>
        /// Detach a caretaker from the baby monitor.
        /// </summary>
        /// <param name="caretaker">The caretaker identity and their address.</param>
        public void DetachCaretaker(AddressData caretaker)
        {
            if (!HasNurseAssigned())
                throw new NullReferenceException("Cannot execute function without a nurse assigned.");

            if (Caretakers.Count(ct => ct.Name == caretaker.identity && ct.Address == caretaker.address) > 0)
            {
                // Handle communication
                _nurseCommunicator.ConnectToCaretaker(caretaker);
                _nurseCommunicator.DetachCaretakerFromBabyMonitor();

                // Remove caretaker from monitor list
                Caretakers.RemoveAll(ct => ct.Name == caretaker.identity && ct.Address == caretaker.address);
            }
        }
        /// <summary>
        /// Assign a nurse to the baby monitor
        /// </summary>
        /// <param name="nurse">The nurse object to assign to the monitor.</param>
        public void AssignNurse(Nurse nurse)
        {
            this.Nurse = nurse;
            this._nurseCommunicator = nurse.Communicator;
        }
        #endregion
        #endregion

        #region Methods
        /// <summary>
        /// Checks if there is a nurse assigned to the baby monitor.
        /// </summary>
        /// <returns>Returns true if nurse is properly set.</returns>
        private bool HasNurseAssigned()
        {
            return Nurse != null && _nurseCommunicator != null;
        }
        /// <summary>
        /// Checks if the client is a caretaker or a nurse.
        /// </summary>
        /// <returns>Returns true if baby monitor is initiated as a caretaker.</returns>
        private bool IsClientCaretaker()
        {
            return _caretakerCommunicator != null;
        }

        #region Nurse Listening
        /// <summary>
        /// Listen for an unsubscription.
        /// </summary>
        private void ListenUnsubscribe()
        {
            while (Caretakers.Count > 0)
            {
                string caretaker = _nurseCommunicator.ListenForUnsubscribe();
                CaretakerUnsubscribed(caretaker);
                Caretakers.RemoveAll(ct => ct.Name == caretaker);
            }
        }
        #endregion
        #endregion

        #region Events
        public delegate void CaretakerUnsubscribedHandler(string caretaker);
        public event CaretakerUnsubscribedHandler CaretakerUnsubscribed;
        #endregion
    }
}
