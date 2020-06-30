using BabyphoneIoT.DataEntities;

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
        /// <summary>
        /// Notifies the nurse that the caretaker unsubscribed.
        /// </summary>
        void Unsubscribe();
        /// <summary>
        /// Gets the baby monitor name.
        /// </summary>
        /// <returns>Returns the name of the baby monitor.</returns>
        string GetBabyName();
        /// <summary>
        /// Gets the monitor id.
        /// </summary>
        /// <returns>Returns the baby monitor id.</returns>
        string GetMonitorId();
    }
}
