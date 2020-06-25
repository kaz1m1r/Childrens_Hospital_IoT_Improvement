using BabyphoneIoT.DataEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabyphoneIoT.CaretakerCommunication
{
    public interface INurseCommunicator
    {
        /// <summary>
        /// Gets all currently broadcasting caretakers.
        /// </summary>
        /// <returns>Returns address data from any broadcasting caretaker communicators.</returns>
        List<AddressData> GetCaretakers();
        /// <summary>
        /// Connect to a specific caretaker.
        /// </summary>
        /// <param name="connectionData">The connection to a caretaker.</param>
        void ConnectToCaretaker(AddressData connectionData);
        /// <summary>
        /// Get the currently connected caretaker connection data.
        /// </summary>
        /// <returns>Returns the connection data.</returns>
        AddressData GetConnectedCaretaker();
        /// <summary>
        /// Attach a baby monitor id to the currently connected caretaker.
        /// </summary>
        /// <param name="babyId">The baby monitor id.</param>
        void AttachBabyMonitorToCaretaker(string babyId);
        /// <summary>
        /// Detach the currently connected caretaker from the baby monitors they're currently assigned to.
        /// </summary>
        void DetachCaretakerFromBabyMonitor();
        /// <summary>
        /// Listen for any requests from the connected caretaker to call upon the nurse.
        /// </summary>
        /// <returns>Returns the baby monitor id.</returns>
        string ListenForRequests();
        /// <summary>
        /// Listen for any request to unsubscribe from the connected caretaker.
        /// </summary>
        void ListenForUnsubscribe();
    }
}
