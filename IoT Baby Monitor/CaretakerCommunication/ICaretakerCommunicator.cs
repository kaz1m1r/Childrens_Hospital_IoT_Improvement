using BabyphoneIoT.DataEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabyphoneIoT.CaretakerCommunication
{
    public interface ICaretakerCommunicator
    {
        /// <summary>
        /// Broadcasts itself for any nurse communicator to identify them.
        /// </summary>
        void BroadcastSelf();
        /// <summary>
        /// Listen for a nurse communicator to connect them to a baby monitor.
        /// </summary>
        /// <returns>Returns the baby monitor id.</returns>
        string GetBabyMonitor();
        /// <summary>
        /// Listen for a nurse communicator to actively disconnect them from a baby monitor.
        /// </summary>
        /// <returns>Returns true if the caretaker is forced to disconnect.</returns>
        bool ListenForDetachmentRequest();
        /// <summary>
        /// Send a request to call upon the assigned nurse.
        /// </summary>
        void SendNurseRequest();
        /// <summary>
        /// Send a request to unsubscribe from the baby monitor.
        /// </summary>
        void SendUnsubscribe();
    }
}
