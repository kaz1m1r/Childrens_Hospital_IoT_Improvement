using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabyphoneIoT.Logic
{
    /// <summary>
    /// Defines a user of type caretaker who can monitor a baby only when assigned to one.
    /// </summary>
    public class Caretaker : User
    {
        #region Properties
        /// <summary>
        /// The caretaker's IP address.
        /// </summary>
        public string Address { get; private set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Initiates a caretaker with a given identity.
        /// </summary>
        /// <param name="name">The user's identity.</param>
        public Caretaker(string name)
            : base(name)
        {

        }
        /// <summary>
        /// Initiates a caretaker with a given identity, matched with an address.
        /// </summary>
        /// <param name="name">The user's identity.</param>
        /// <param name="address">The user's IP address.</param>
        public Caretaker(string name, string address)
            : this(name)
        {
            this.Address = address;
        }
        #endregion

        #region Functions
        /// <summary>
        /// Send a help request to the assigned nurse of the caretaker's baby.
        /// </summary>
        public void RequestNurse()
        {
            if (BabyMonitors.Count > 0)
                BabyMonitors[0].NotifyNurse();
        }
        #endregion
    }
}
