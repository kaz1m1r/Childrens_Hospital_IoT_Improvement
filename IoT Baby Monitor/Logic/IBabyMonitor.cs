using BabyphoneIoT.DataEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabyphoneIoT.Logic
{
    /// <summary>
    /// An interface to monitor a baby.
    /// </summary>
    public interface IBabyMonitor
    {
        /// <summary>
        /// Get the baby's status.
        /// </summary>
        /// <returns></returns>
        BabyStatus MonitorBaby();
        /// <summary>
        /// Notify the nurse of the baby that they are requested.
        /// </summary>
        void NotifyNurse();
    }
}
