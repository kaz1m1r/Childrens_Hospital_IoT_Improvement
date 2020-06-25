using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace BabyphoneIoT.BabyphoneDataAccess
{
    /// <summary>
    /// Allows for communication to the domoticz web interface.
    /// </summary>
    public abstract class DomoticzDataAccess
    {
        #region Fields
        /// <summary>
        /// The http client responsible for web requests.
        /// </summary>
        private HttpClient _webClient;
        #endregion

        #region Properties
        /// <summary>
        /// The base address for the Domoticz Data Access.
        /// </summary>
        public string ApiAddressBase { get; private set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Initiate an object for accessing the Domoticz babyphone system.
        /// </summary>
        /// <param name="ipAddress">The address of the Domoticz server.</param>
        /// <param name="port">The port of the Domoticz server.</param>
        public DomoticzDataAccess(string ipAddress="127.0.0.1", string port="8080")
        {
            this._webClient = new HttpClient();
            this.ApiAddressBase = $"http://{ipAddress}:{port}";

            this._webClient.BaseAddress = new Uri(this.ApiAddressBase);
            this._webClient.DefaultRequestHeaders.Accept.Clear();
            this._webClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }
        #endregion

        #region Functions
        /// <summary>
        /// Call the Domoticz web api.
        /// </summary>
        /// <param name="apiPath">The api path.</param>
        /// <returns>The contents of the api response in dynamic json format.</returns>
        protected dynamic CallApi(string apiPath)
        {
            string apiCall = $"/json.htm?{apiPath}";

            HttpResponseMessage response = _webClient.GetAsync(apiCall).Result;
            response.EnsureSuccessStatusCode();

            string jsonObj = response.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<dynamic>(jsonObj).result;
        }
        #endregion
    }
}
