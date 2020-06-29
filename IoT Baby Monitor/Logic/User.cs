using BabyphoneIoT.DataEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabyphoneIoT.Logic
{
    /// <summary>
    /// Defines an abstract user from which basic communication to baby monitors is possible.
    /// </summary>
    public abstract class User
    {
        #region Properties
        /// <summary>
        /// The username.
        /// </summary>
        public string Name { get; protected set; }
        /// <summary>
        /// Any attached baby monitors.
        /// </summary>
        public List<IBabyMonitor> BabyMonitors { get; protected set; }
        /// <summary>
        /// Contains the last status call to the baby monitor.
        /// </summary>
        public BabyStatus LastBabyStatus { get; protected set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Initiate a user with a user identity.
        /// </summary>
        /// <param name="name">The user's identity.</param>
        public User(string name)
        {
            this.Name = name;
        }
        #endregion

        #region Functions
        /// <summary>
        /// Gets the baby status of the monitor of the baby on the given index.
        /// </summary>
        /// <param name="index">The index in the baby monitoring list.</param>
        /// <returns>Returns the status of the baby. Returns <see cref="BabyStatus.None"/> if not found.</returns>
        public BabyStatus MonitorBaby(int index)
        {
            // Check if baby monitor is in range, return BabyStatus.None if not.
            if (index >= BabyMonitors.Count || index < 0)
                return BabyStatus.None;

            // Get and return the baby's status
            LastBabyStatus = BabyMonitors[index].MonitorBaby();
            return LastBabyStatus;
        }
        /// <summary>
        /// Adds a baby monitor to the user.
        /// </summary>
        /// <param name="babyMonitor">The baby monitoring interface.</param>
        public void AddBabyMonitor(IBabyMonitor babyMonitor)
        {
            this.BabyMonitors.Add(babyMonitor);
        }
        /// <summary>
        /// Removes a baby monitor from the user.
        /// </summary>
        /// <param name="babyMonitor">The baby monitor to remove from the user.</param>
        public void RemoveBabyMonitor(IBabyMonitor babyMonitor)
        {
            this.BabyMonitors.Remove(babyMonitor);
        }
        #endregion
    }
}
