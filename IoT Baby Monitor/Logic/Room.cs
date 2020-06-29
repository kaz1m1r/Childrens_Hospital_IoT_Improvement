using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BabyphoneIoT.BabyphoneDataAccess;

namespace BabyphoneIoT.Logic
{
    /// <summary>
    /// Defines a room from which to list babies to monitor.
    /// </summary>
    public class Room
    {
        #region Fields
        private IHospital _iotDal;
        #endregion

        #region Properties
        /// <summary>
        /// The room's id.
        /// </summary>
        public string RoomId { get; private set; }
        /// <summary>
        /// The room's name.
        /// </summary>
        public string RoomName { get; private set; }
        /// <summary>
        /// Gets a list of babies based on the last call to the IoT environment.
        /// </summary>
        public List<Baby> Babies { get; private set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Initiate a room from which to get the present baby monitors from.
        /// </summary>
        /// <param name="roomId">The rooms identifier.</param>
        /// <param name="roomName">The rooms display name.</param>
        /// <param name="hospitalIoTDAL">The data access object to the IoT environment floorplanning.</param>
        public Room(string roomId, string roomName, IHospital hospitalIoTDAL = null)
        {
            this.RoomId = roomId;
            this.RoomName = roomName;
            this._iotDal = hospitalIoTDAL ?? new DomoticzHospital();
        }
        #endregion

        #region Functions
        /// <summary>
        /// Get an updated list of babies via a call to the IoT environment.
        /// </summary>
        /// <returns></returns>
        public List<Baby> GetBabies()
        {
            // Get babies in the room
            Dictionary<string, string> babies = _iotDal.GetBabies(RoomId);

            // TODO: Translate data into baby objects
            throw new NotImplementedException();
        }
        #endregion
    }
}
