using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BabyphoneIoT.DataEntities;

namespace BabyphoneIoT.Logic
{

    public class Baby : IBabyMonitor
    {
        #region Constructors

        #endregion

        #region Functions
        #region IBabyMonitor Interface
        public BabyStatus MonitorBaby()
        {
            throw new NotImplementedException();
        }

        public void NotifyNurse()
        {
            throw new NotImplementedException();
        }
        #endregion
        #endregion
    }
}
