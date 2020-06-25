﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BabyphoneIoT.DataEntities;
using Microsoft.CSharp.RuntimeBinder;

namespace BabyphoneIoT.BabyphoneDataAccess
{
    /// <summary>
    /// Allows monitoring of a baby via the Domoticz infrastructure.
    /// </summary>
    public class DomoticBabyphone : DomoticzDataAccess, IBabyphone
    {
        #region Fields
        /// <summary>
        /// The monitoring device id.
        /// </summary>
        public string babyId;
        #endregion

        #region Constructors
        /// <summary>
        /// Initiate an object for monitoring a baby via the Domoticz infrastructure.
        /// </summary>
        /// <param name="babyId">The identity of the baby monitor.</param>
        public DomoticBabyphone(string babyId)
        {
            this.babyId = babyId;

            string apiPath = "type=devices&" +
                             $"rid={babyId}";

            try
            {
                CallApi(apiPath);
            }
            catch (RuntimeBinderException e)
            {
                throw new ArgumentException("The baby monitor id was not found in the Domoticz system.", e);
            }
        }
        #endregion

        #region Functions
        #region IBabyphone Interface
        /// <summary>
        /// Get the status of a baby.
        /// </summary>
        /// <returns>Returns the baby's status.</returns>
        public BabyStatus GetBabyState()
        {
            string apiPath = "type=devices&" +
                             $"rid={babyId}";

            dynamic results = CallApi(apiPath);

            switch(Convert.ToString(results[0].Status))
            {
                case "On":
                    return BabyStatus.Crying;
                case "Off":
                    return BabyStatus.Quiet;
                default:
                    return BabyStatus.Quiet;
            }
        }
        #endregion
        #endregion
    }
}
