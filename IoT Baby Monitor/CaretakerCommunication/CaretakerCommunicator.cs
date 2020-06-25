using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabyphoneIoT.CaretakerCommunication
{
    public class CaretakerCommunicator : ICaretakerCommunicator
    {
        #region Constructors

        #endregion

        #region Functions
        #region ICaretakerCommunicator
        /// <summary>
        /// Broadcasts itself for any nurse communicator to identify them.
        /// </summary>
        public void BroadcastSelf()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Listen for a nurse communicator to connect them to a baby monitor.
        /// </summary>
        /// <returns>Returns the baby monitor id.</returns>
        public string GetBabyMonitor()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Listen for a nurse communicator to actively disconnect them from a baby monitor.
        /// </summary>
        /// <returns>Returns true if the caretaker is forced to disconnect.</returns>
        public bool ListenForDetachmentRequest()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Send a request to call upon the assigned nurse.
        /// </summary>
        public void SendNurseRequest()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Send a request to unsubscribe from the baby monitor.
        /// </summary>
        public void SendUnsubscribe()
        {
            throw new NotImplementedException();
        }
        #endregion
        #endregion
    }
}
