using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabyphoneIoT.BabyphoneDataAccess
{
    /// <summary>
    /// Allows access to the Domoticz floorplan infrastructure.
    /// </summary>
    public class DomoticzHospital : DomoticzDataAccess, IHospital
    {
        #region Constructors
        /// <summary>
        /// Initiate an object for accessing the Domoticz floorplan infrastructure.
        /// </summary>
        public DomoticzHospital()
            : base()
        {

        }
        #endregion

        #region Functions
        #region IHospital Interface
        /// <summary>
        /// Gets the rooms in a hospital which have babies in them.
        /// </summary>
        /// <returns>Returns a list of room names in the hospital.</returns>
        public Dictionary<string, string> GetRooms()
        {
            string apiPath = "type=plans&" +
                             "order=name&" +
                             "used=true";

            dynamic results = CallApi(apiPath);

            Dictionary<string, string> rooms = new Dictionary<string, string>();

            foreach (dynamic room in results)
            {
                rooms.Add(Convert.ToString(room.idx), Convert.ToString(room.Name));
            }

            return rooms;
        }

        /// <summary>
        /// Gets the babies in a specified room.
        /// </summary>
        /// <param name="roomId">The room id specifying the room.</param>
        /// <returns>Returns a list of baby monitoring devices in the room.</returns>
        public Dictionary<string, string> GetBabies(string roomId)
        {
            string apiPath = "type=command&" +
                             "param=getplandevices&" +
                             $"idx={roomId}";

            dynamic results = CallApi(apiPath);

            Dictionary<string, string> devices = new Dictionary<string, string>();

            foreach (dynamic device in results)
            {
                devices.Add(Convert.ToString(device.idx), Convert.ToString(device.Name));
            }

            return devices;
        }
        
        /// <summary>
        /// Gets the baby monitoring data interface for a baby.
        /// </summary>
        /// <param name="babyId">The baby montoring device identity.</param>
        /// <returns>The baby monitoring data access.</returns>
        public IBabyphone GetBabyphone(string babyId)
        {
            return new DomoticBabyphone(babyId);
        }
        
        #endregion
        #endregion
    }
}
