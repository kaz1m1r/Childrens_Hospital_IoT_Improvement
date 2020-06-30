using BabyphoneIoT.DataEntities;

namespace BabyphoneIoT.BabyphoneDataAccess
{
    /// <summary>
    /// An interface for monitoring the a baby.
    /// </summary>
    public interface IBabyphone
    {
        /// <summary>
        /// Get the status of a baby.
        /// </summary>
        /// <returns>Returns the baby's status.</returns>
        BabyStatus GetBabyState();
        /// <summary>
        /// Gets the baby monitor name.
        /// </summary>
        /// <returns>Returns the baby monitor name.</returns>
        string GetBabyName();
    }
}
