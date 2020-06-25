using System.Collections.Generic;

namespace BabyphoneIoT.BabyphoneDataAccess
{
    /// <summary>
    /// An interface for going through the hospital's floorplan and selecting a baby monitor.
    /// </summary>
    public interface IHospital
    {
        /// <summary>
        /// Gets the rooms in a hospital which have babies in them.
        /// </summary>
        /// <returns>Returns a list of room names in the hospital.</returns>
        Dictionary<string, string> GetRooms();
        /// <summary>
        /// Gets the babies in a specified room.
        /// </summary>
        /// <param name="roomId">The room id specifying the room.</param>
        /// <returns>Returns a list of baby monitoring devices in the room.</returns>
        Dictionary<string, string> GetBabies(string roomId);
        /// <summary>
        /// Gets the baby monitoring data interface for a baby.
        /// </summary>
        /// <param name="babyId">The baby montoring device identity.</param>
        /// <returns>The baby monitoring data access.</returns>
        IBabyphone GetBabyphone(string babyId);
    }
}
