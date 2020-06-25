using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabyphoneIoT.DataEntities
{
    public struct AddressData
    {
        #region Fields
        public string identity;
        public string address;
        #endregion

        #region Constructor
        public AddressData(string identity, string address)
        {
            this.identity = identity;
            this.address = address;
        }
        #endregion
    }
}
