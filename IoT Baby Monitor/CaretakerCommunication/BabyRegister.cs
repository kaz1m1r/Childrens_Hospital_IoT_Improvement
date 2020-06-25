using BabyphoneIoT.DataEntities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BabyphoneIoT.CaretakerCommunication
{
    public class BabyRegister : IBabyRegister
    {
        #region Properties
        /// <summary>
        /// The file location of the register.
        /// </summary>
        public string Address { get; private set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Initiate an object that manages a babies' caretakers and assigned nurse.
        /// </summary>
        /// <param name="address"></param>
        public BabyRegister(string address)
        {
            this.Address = address;

            InstantiateFile();
        }
        #endregion

        #region Functions
        #region IBabyRegister Interface
        /// <summary>
        /// Register a nurse to a baby.
        /// </summary>
        /// <param name="babyId">The baby monitor identifier.</param>
        /// <param name="addressData">The nurse' address info.</param>
        public void RegisterNurseToBaby(string babyId, AddressData addressData)
        {
            ModifyData((data) =>
            {
                List<BabyData> existingEntries = data.Where(baby => baby.babyId == babyId).ToList();

                if (existingEntries.Count >= 1)
                    data[data.FindIndex(baby => baby.babyId == babyId)].assignedNurse = addressData;
                else
                    data.Add(new BabyData(babyId) { assignedNurse = addressData });

                return data;
            });
        }
        /// <summary>
        /// Register a caretaker to a baby.
        /// </summary>
        /// <param name="babyId">The baby monitor identifier.</param>
        /// <param name="addressData">The caretaker's address info.</param>
        public void RegisterCaretakerToBaby(string babyId, AddressData addressData)
        {
            ModifyData((data) =>
            {
                List<BabyData> existingEntries = data.Where(baby => baby.babyId == babyId).ToList();

                if (existingEntries.Count >= 1)
                {
                    int entryIndex = data.FindIndex(baby => baby.babyId == babyId);
                    if (data[entryIndex].caretakers.Count(address => address.identity == addressData.identity) < 1)
                        data[entryIndex].caretakers.Add(addressData);
                } 
                else
                {
                    BabyData baby = new BabyData(babyId);
                    baby.caretakers.Add(addressData);
                    data.Add(baby);
                }

                return data;
            });
        }
        /// <summary>
        /// Deregister a caretaker.
        /// </summary>
        /// <param name="addressData">The address info.</param>
        public void DeregisterCaretaker(AddressData addressData)
        {
            ModifyData((data) =>
            {
                foreach (BabyData baby in data)
                {
                    baby.caretakers.RemoveAll(address => address.identity == addressData.identity);
                }
                return data;
            });
        }
        /// <summary>
        /// Register a nurse to a baby.
        /// </summary>
        /// <param name="babyId">The baby monitor identifier.</param>
        /// <param name="addressData">The nurse's address info.</param>
        public void DeregisterNurseFromBaby(string babyId, AddressData addressData)
        {
            ModifyData((data) =>
            {
                List<BabyData> existingEntries = data.Where(baby => baby.babyId == babyId).ToList();

                if (existingEntries.Count >= 1)
                {
                    int entryIndex = data.FindIndex(baby => baby.babyId == babyId);

                    if (data[entryIndex].assignedNurse.identity == addressData.identity)
                        data[entryIndex].assignedNurse = new AddressData();
                }

                return data;
            });
        }
        /// <summary>
        /// Get a nurse's addressing data for a baby.
        /// </summary>
        /// <param name="babyId">The baby monitor identifier.</param>
        /// <returns>Returns the nurse's address.</returns>
        public AddressData GetNurseFromBaby(string babyId)
        {
            List<BabyData> data = ReadData();
            return data.Where(baby => baby.babyId == babyId).FirstOrDefault().assignedNurse;
        }
        /// <summary>
        /// Get a nurse's addressing data for a baby.
        /// </summary>
        /// <param name="babyId">The baby monitor identifier.</param>
        /// <returns>Returns the addresses of listening caretakers.</returns>
        public List<AddressData> GetCaretakersFromBaby(string babyId)
        {
            List<BabyData> data = ReadData();
            return data.Where(baby => baby.babyId == babyId).FirstOrDefault().caretakers;
        }
        #endregion
        #endregion

        #region Methods
        /// <summary>
        /// Makes sure there is a file on instantiation of a class.
        /// </summary>
        private void InstantiateFile()
        {
            if(!File.Exists(Address))
            {
                string path = Address.Substring(0, Address.LastIndexOf('\\') + 1);
                Directory.CreateDirectory(path);

                List<BabyData> data = new List<BabyData>();
                WriteData(data);
            }
        }
        /// <summary>
        /// Reads the data from the file.
        /// </summary>
        /// <returns>Returns the data.</returns>
        private List<BabyData> ReadData()
        {
            using (StreamReader sr = new StreamReader(Address))
            {
                return JsonConvert.DeserializeObject<List<BabyData>>(sr.ReadToEnd());
            }
        }
        /// <summary>
        /// Writes the data to the file.
        /// </summary>
        /// <param name="data">The data to be written over the file.</param>
        private void WriteData(List<BabyData> data)
        {
            using (StreamWriter sw = new StreamWriter(Address))
            {
                sw.Write(JsonConvert.SerializeObject(data));
            }
        }
        /// <summary>
        /// Modifies the data in a file.
        /// </summary>
        /// <param name="modificationAction"></param>
        private void ModifyData(Func<List<BabyData>, List<BabyData>> modificationAction)
        {
            List<BabyData> data = ReadData();
            WriteData(modificationAction(data));
        }
        #endregion

        #region Data Structures
        internal class BabyData
        {
            public string babyId;
            public List<AddressData> caretakers;
            public AddressData assignedNurse;

            public BabyData(string babyId)
            {
                this.babyId = babyId;
                this.caretakers = new List<AddressData>();
                this.assignedNurse = new AddressData(string.Empty, string.Empty);
            }
        }
        #endregion
    }
}
