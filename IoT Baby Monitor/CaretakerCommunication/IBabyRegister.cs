using BabyphoneIoT.DataEntities;
using System.Collections.Generic;

namespace BabyphoneIoT.CaretakerCommunication
{
    /// <summary>
    /// Allows addressing registration for babies' caretakers and assigned nurse.
    /// </summary>
    public interface IBabyRegister
    {
        /// <summary>
        /// Register a nurse to a baby.
        /// </summary>
        /// <param name="babyId">The baby monitor identifier.</param>
        /// <param name="addressData">The nurse' address info.</param>
        void RegisterNurseToBaby(string babyId, AddressData addressData);
        /// <summary>
        /// Register a caretaker to a baby.
        /// </summary>
        /// <param name="babyId">The baby monitor identifier.</param>
        /// <param name="addressData">The caretaker's address info.</param>
        void RegisterCaretakerToBaby(string babyId, AddressData addressData);
        /// <summary>
        /// Deregister a caretaker.
        /// </summary>
        /// <param name="addressData">The address info.</param>
        void DeregisterCaretaker(AddressData addressData);
        /// <summary>
        /// Register a nurse to a baby.
        /// </summary>
        /// <param name="babyId">The baby monitor identifier.</param>
        /// <param name="addressData">The nurse's address info.</param>
        void DeregisterNurseFromBaby(string babyId, AddressData addressData);
        /// <summary>
        /// Get a nurse's addressing data for a baby.
        /// </summary>
        /// <param name="babyId">The baby monitor identifier.</param>
        /// <returns>Returns the nurse's address.</returns>
        AddressData GetNurseFromBaby(string babyId);
        /// <summary>
        /// Get a nurse's addressing data for a baby.
        /// </summary>
        /// <param name="babyId">The baby monitor identifier.</param>
        /// <returns>Returns the addresses of listening caretakers.</returns>
        List<AddressData> GetCaretakersFromBaby(string babyId);
    }
}
