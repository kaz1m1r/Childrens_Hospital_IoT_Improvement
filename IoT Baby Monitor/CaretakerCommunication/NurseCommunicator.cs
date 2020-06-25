using BabyphoneIoT.DataEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabyphoneIoT.CaretakerCommunication
{
    public class NurseCommunicator : INurseCommunicator
    {
        #region Constructors

        #endregion

        #region Functions
        #region INurseCommunicator Interface
        /// <summary>
        /// Gets all currently broadcasting caretakers.
        /// </summary>
        /// <returns>Returns address data from any broadcasting caretaker communicators.</returns>
        public List<AddressData> GetCaretakers()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Connect to a specific caretaker.
        /// </summary>
        /// <param name="connectionData">The connection to a caretaker.</param>
        public void ConnectToCaretaker(AddressData connectionData)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Get the currently connected caretaker connection data.
        /// </summary>
        /// <returns>Returns the connection data.</returns>
        public AddressData GetConnectedCaretaker()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Attach a baby monitor id to the currently connected caretaker.
        /// </summary>
        /// <param name="babyId">The baby monitor id.</param>
        public void AttachBabyMonitorToCaretaker(string babyId)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Detach the currently connected caretaker from the baby monitors they're currently assigned to.
        /// </summary>
        public void DetachCaretakerFromBabyMonitor()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Listen for any requests from the connected caretaker to call upon the nurse.
        /// </summary>
        /// <returns>Returns the baby monitor id.</returns>
        public string ListenForRequests()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Listen for any request to unsubscribe from the connected caretaker.
        /// </summary>
        public void ListenForUnsubscribe()
        {
            throw new NotImplementedException();
        }
        #endregion
        #endregion
    }
}
