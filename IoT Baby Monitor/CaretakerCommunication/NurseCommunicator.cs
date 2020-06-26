using BabyphoneIoT.DataEntities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BabyphoneIoT.CaretakerCommunication
{
    /// <summary>
    /// Implementation of the communication protocol for the <see cref="INurseCommunicator"/> interface.
    /// </summary>
    public class NurseCommunicator : INurseCommunicator
    {
        #region Constants
        const string REGISTERFILE = "dbRegister.json";
        #endregion

        #region Fields
        private string _identity;
        private int _listeningDiscoveryPort;
        private int _listeningConnectedPort;
        private int _targetPort;
        private string _localIP;
        private string _targetIP;

        private AddressData _caretakerConnection;

        private CancellationTokenSource _ctSourceGetRequests;
        private CancellationTokenSource _ctSourceListenUnsubscribe;
        #endregion

        #region Constructors
        public NurseCommunicator(string identity, string localIP = "127.0.0.1", int ncDiscoveryPort = 25555, int ccPort=25556, int ncConnectedPort = 25557)
        {
            if (!IPAddress.TryParse(localIP, out IPAddress ip))
                throw new ArgumentException($"Local ip address '{localIP}' is not a valid ip address.");

            this._identity = identity;
            this._listeningDiscoveryPort = ncDiscoveryPort;
            this._listeningConnectedPort = ncConnectedPort;
            this._targetPort = ccPort;
            this._localIP = localIP;
            this._targetIP = string.Empty;
        }
        #endregion

        #region Functions
        #region INurseCommunicator Interface
        /// <summary>
        /// Gets all currently broadcasting caretakers.
        /// </summary>
        /// <returns>Returns address data from any broadcasting caretaker communicators.</returns>
        public List<AddressData> GetCaretakers()
        {
            TcpListener listener = null;
            List<AddressData> caretakers = new List<AddressData>();

            try
            {
                // Initiate listening
                IPAddress locAdress = IPAddress.Parse(_localIP);
                listener = new TcpListener(locAdress, _listeningDiscoveryPort);
                listener.Start();

                // Accept a new client so long the timeout (2 seconds) hasn't passed.
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                while (stopwatch.Elapsed.Seconds < 2)
                {
                    if (listener.Pending())
                    {
                        // Reset the timeout timer and accept client
                        TcpClient client = listener.AcceptTcpClient();

                        // Read the request
                        using (NetworkStream ns = client.GetStream())
                        {
                            // Get the command of the request and execute on broadcast
                            StreamReader sr = new StreamReader(ns);

                            if (Enum.TryParse(sr.ReadLine(), out NurseCommand command) && command == NurseCommand.Broadcast)
                            {
                                // Check if identity already registered
                                string identity = sr.ReadLine();

                                if (caretakers.Count(caretaker => caretaker.identity == identity) == 0)
                                {
                                    // Reset stopwatch and add new identity
                                    stopwatch.Reset();
                                    stopwatch.Start();

                                    IPEndPoint ipEndpoint = client.Client.RemoteEndPoint as IPEndPoint;
                                    string ipaddress = ipEndpoint.Address.ToString();
                                    caretakers.Add(new AddressData(identity, ipaddress));
                                }
                            }
                        }
                    }

                    Thread.Sleep(50);
                }
            }
            catch (SocketException)
            {

            }
            catch (ObjectDisposedException)
            {

            }
            finally
            {
                listener.Stop();
            }

            return caretakers;
        }
        /// <summary>
        /// Connect to a specific caretaker.
        /// </summary>
        /// <param name="connectionData">The connection to a caretaker.</param>
        public void ConnectToCaretaker(AddressData connectionData)
        {
            _caretakerConnection = connectionData;
        }
        /// <summary>
        /// Get the currently connected caretaker connection data.
        /// </summary>
        /// <returns>Returns the connection data.</returns>
        public AddressData GetConnectedCaretaker()
        {
            return _caretakerConnection;
        }
        /// <summary>
        /// Attach a baby monitor id to the currently connected caretaker.
        /// </summary>
        /// <param name="babyId">The baby monitor id.</param>
        public void AttachBabyMonitorToCaretaker(string babyId)
        {
            if (_caretakerConnection.identity != string.Empty &&
                _caretakerConnection.address != string.Empty &&
                IPAddress.TryParse(_caretakerConnection.address, out IPAddress address))
            {
                try
                {
                    TcpClient client = new TcpClient(_caretakerConnection.address, _targetPort);

                    using (NetworkStream ns = client.GetStream())
                    {
                        StreamWriter sw = new StreamWriter(ns);
                        sw.WriteLine(CaretakerCommand.Connect.ToString());
                        sw.WriteLine(_identity);
                        sw.WriteLine(babyId);
                        sw.Flush();
                    }
                }
                catch (SocketException e)
                {
                    throw new Exception($"Could not find address '{_caretakerConnection.address}'", e);
                }
                catch
                {

                }
            }
            else
            {
                throw new NullReferenceException("Cannot operate without a destination address.");
            }
        }
        /// <summary>
        /// Detach the currently connected caretaker from the baby monitors they're currently assigned to.
        /// </summary>
        public void DetachCaretakerFromBabyMonitor()
        {
            if (_caretakerConnection.identity != string.Empty &&
                _caretakerConnection.address != string.Empty &&
                IPAddress.TryParse(_caretakerConnection.address, out IPAddress address))
            {
                try
                {
                    TcpClient client = new TcpClient(_caretakerConnection.address, _targetPort);

                    using (NetworkStream ns = client.GetStream())
                    {
                        StreamWriter sw = new StreamWriter(ns);
                        sw.WriteLine(CaretakerCommand.Disconnect.ToString());
                        sw.WriteLine(true.ToString());
                        sw.Flush();
                    }
                }
                catch (SocketException e)
                {
                    throw new Exception($"Could not find address '{_caretakerConnection.address}'", e);
                }
                catch
                {

                }
            }
            else
            {
                throw new NullReferenceException("Cannot operate without a destination address.");
            }
        }
        /// <summary>
        /// Listen for any requests from the connected caretaker to call upon the nurse.
        /// </summary>
        /// <returns>Returns the baby monitor id.</returns>
        public string ListenForRequests()
        {
            TcpListener listener = null;

            try
            {
                // Initiate listening
                IPAddress locAdress = IPAddress.Parse(_localIP);
                listener = new TcpListener(locAdress, _listeningConnectedPort);
                listener.Start();

                _ctSourceGetRequests = new CancellationTokenSource();

                // Accept a new client so long the request is not cancelled
                while (!_ctSourceGetRequests.IsCancellationRequested)
                {
                    if (listener.Pending())
                    {
                        // Reset the timeout timer and accept client
                        TcpClient client = listener.AcceptTcpClient();

                        // Read the request
                        using (NetworkStream ns = client.GetStream())
                        {
                            // Get the command of the request and execute on broadcast
                            StreamReader sr = new StreamReader(ns);

                            if (Enum.TryParse(sr.ReadLine(), out NurseCommand command) && command == NurseCommand.Request)
                            {
                                // Cancel any broadcasting and quit listening for a new caretaker command or connection
                                _ctSourceGetRequests.Cancel();
                                listener.Stop();

                                // Return the response, which is true
                                return sr.ReadLine();
                            }
                        }
                    }

                    Thread.Sleep(500);
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (ObjectDisposedException)
            {

            }
            finally
            {
                listener.Stop();
            }

            return string.Empty;
        }
        /// <summary>
        /// Listen for any request to unsubscribe from the connected caretaker.
        /// </summary>
        public void ListenForUnsubscribe()
        {
            TcpListener listener = null;

            try
            {
                // Initiate listening
                IPAddress locAdress = IPAddress.Parse(_localIP);
                listener = new TcpListener(locAdress, _listeningDiscoveryPort);
                listener.Start();

                _ctSourceListenUnsubscribe = new CancellationTokenSource();

                // Accept a new client so long the request is not cancelled
                while (!_ctSourceListenUnsubscribe.IsCancellationRequested)
                {
                    if (listener.Pending())
                    {
                        // Reset the timeout timer and accept client
                        TcpClient client = listener.AcceptTcpClient();

                        // Read the request
                        using (NetworkStream ns = client.GetStream())
                        {
                            // Get the command of the request and execute on broadcast
                            StreamReader sr = new StreamReader(ns);

                            if (Enum.TryParse(sr.ReadLine(), out NurseCommand command) && command == NurseCommand.Unsubscribe)
                            {
                                // Cancel any broadcasting and quit listening for a new caretaker command or connection
                                _ctSourceListenUnsubscribe.Cancel();
                                listener.Stop();

                                // Return the response, which is true
                                string identity = sr.ReadLine();

                                if (identity == _caretakerConnection.identity)
                                {
                                    _caretakerConnection = new AddressData();
                                    _ctSourceGetRequests.Cancel();
                                    _ctSourceListenUnsubscribe.Cancel();
                                }
                                    
                            }
                        }
                    }

                    Thread.Sleep(500);
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (ObjectDisposedException)
            {

            }
            finally
            {
                listener.Stop();
            }
        }
        #endregion
        #endregion
    }
}
