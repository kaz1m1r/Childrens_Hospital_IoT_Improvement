using BabyphoneIoT.DataEntities;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace BabyphoneIoT.CaretakerCommunication
{
    /// <summary>
    /// Implementation of the communication protocol for the <see cref="ICaretakerCommunicator"/> interface.
    /// </summary>
    public class CaretakerCommunicator : ICaretakerCommunicator
    {
        #region Constants
        const string REGISTERFILE = "dbRegister.json";
        #endregion

        #region Fields
        private string _identity;
        private int _listeningPort;
        private int _targetDiscoveryPort;
        private int _targetConnectedPort;
        private string _localIP;
        private string _targetIP;

        private AddressData _nurse;
        private string _babyId;

        private CancellationTokenSource _ctSourceBroadcast;
        private CancellationTokenSource _ctSourceGetMonitor;
        private CancellationTokenSource _ctSourceDetachMonitor;
        #endregion

        #region Constructors
        /// <summary>
        /// Initiate a caretaker communicator object to communicate with a nurse communicator.
        /// </summary>
        /// <param name="identity">The identity of the caretaker.</param>
        /// <param name="localIP">The local address to listen in on.</param>
        /// <param name="port">The port over which to communicate.</param>
        public CaretakerCommunicator(string identity, string targetIP = "127.0.0.1", int ccPort = 25556, int ncDiscoveryPort = 25555, int ncConnectedPort = 25557)
        {
            if (!IPAddress.TryParse(targetIP, out IPAddress ip))
                throw new ArgumentException($"Local ip address '{targetIP}' is not a valid ip address.");

            this._identity = identity;
            this._listeningPort = ccPort;
            this._targetDiscoveryPort = ncDiscoveryPort;
            this._targetConnectedPort = ncConnectedPort;
            this._localIP = "127.0.0.1";
            this._targetIP = targetIP;
        }
        #endregion

        #region Functions
        #region ICaretakerCommunicator
        /// <summary>
        /// Broadcasts itself for any nurse communicator to identify them.
        /// </summary>
        public void BroadcastSelf()
        {
            _ctSourceBroadcast = new CancellationTokenSource();

            while (!_ctSourceBroadcast.IsCancellationRequested)
            {
                try
                {
                    TcpClient client = new TcpClient(_targetIP, _targetDiscoveryPort);

                    using (NetworkStream ns = client.GetStream())
                    {
                        StreamWriter sw = new StreamWriter(ns);
                        sw.WriteLine(NurseCommand.Broadcast.ToString());
                        sw.WriteLine(_identity);
                        sw.Flush();
                    }
                }
                catch (SocketException)
                {

                }
                catch (ObjectDisposedException)
                {

                }
                Thread.Sleep(250);
            }
        }
        /// <summary>
        /// Listen for a nurse communicator to connect them to a baby monitor.
        /// </summary>
        /// <returns>Returns the baby monitor id.</returns>
        public string GetBabyMonitor()
        {
            TcpListener listener = null;

            try
            {
                // Initiate listening
                IPAddress locAdress = IPAddress.Parse(_localIP);
                listener = new TcpListener(locAdress, _listeningPort);
                listener.Start();

                _ctSourceGetMonitor = new CancellationTokenSource();

                // Accept a new client so long the request is not cancelled
                while (!_ctSourceGetMonitor.IsCancellationRequested)
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

                            if (Enum.TryParse(sr.ReadLine(), out CaretakerCommand command) && command == CaretakerCommand.Connect)
                            {
                                // Cancel any broadcasting and quit listening for a new caretaker command or connection
                                _ctSourceBroadcast.Cancel();
                                _ctSourceGetMonitor.Cancel();
                                listener.Stop();

                                string identity = sr.ReadLine();
                                IPEndPoint ipEndpoint = client.Client.RemoteEndPoint as IPEndPoint;
                                string ipaddress = ipEndpoint.Address.ToString();
                                _nurse = new AddressData(identity, ipaddress);

                                _babyId = sr.ReadLine();
                                return _babyId;
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

            // Return an empty string if no response found
            return string.Empty;
        }
        /// <summary>
        /// Listen for a nurse communicator to actively disconnect them from a baby monitor.
        /// </summary>
        /// <returns>Returns true if the caretaker is forced to disconnect.</returns>
        public bool ListenForDetachmentRequest()
        {
            TcpListener listener = null;

            try
            {
                // Initiate listening
                IPAddress locAdress = IPAddress.Parse(_localIP);
                listener = new TcpListener(locAdress, _listeningPort);
                listener.Start();

                _ctSourceDetachMonitor = new CancellationTokenSource();

                // Accept a new client so long the request is not cancelled
                while (!_ctSourceDetachMonitor.IsCancellationRequested)
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

                            if (Enum.TryParse(sr.ReadLine(), out CaretakerCommand command) && command == CaretakerCommand.Disconnect)
                            {
                                // Cancel any broadcasting and quit listening for a new caretaker command or connection
                                _ctSourceDetachMonitor.Cancel();
                                _ctSourceBroadcast.Cancel();
                                _ctSourceGetMonitor.Cancel();
                                listener.Stop();

                                _nurse = new AddressData();
                                _babyId = string.Empty;

                                // Return the response, which is true
                                return Convert.ToBoolean(sr.ReadLine());
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

            // Return false if an error occured
            return false;
        }
        /// <summary>
        /// Send a request to call upon the assigned nurse.
        /// </summary>
        public void SendNurseRequest()
        {
            if (_nurse.identity != string.Empty &&
                _nurse.address != string.Empty &&
                IPAddress.TryParse(_nurse.address, out IPAddress address))
            {
                try
                {
                    TcpClient client = new TcpClient(_nurse.address, _targetConnectedPort);

                    using (NetworkStream ns = client.GetStream())
                    {
                        StreamWriter sw = new StreamWriter(ns);
                        sw.WriteLine(NurseCommand.Request.ToString());
                        sw.WriteLine(_babyId);
                        sw.Flush();
                    }
                }
                catch (SocketException e)
                {
                    throw new Exception($"Could not find address '{_nurse.address}'", e);
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
        /// Send a request to unsubscribe from the baby monitor.
        /// </summary>
        public void SendUnsubscribe()
        {
            if (_nurse.identity != string.Empty &&
                _nurse.address != string.Empty &&
                IPAddress.TryParse(_nurse.address, out IPAddress address))
            {
                try
                {
                    TcpClient client = new TcpClient(_nurse.address, _targetDiscoveryPort);

                    using (NetworkStream ns = client.GetStream())
                    {
                        StreamWriter sw = new StreamWriter(ns);
                        sw.WriteLine(NurseCommand.Unsubscribe.ToString());
                        sw.WriteLine(_identity);
                        sw.Flush();

                        _babyId = string.Empty;
                        _nurse = new AddressData();

                        _ctSourceBroadcast.Cancel();
                        _ctSourceDetachMonitor.Cancel();
                        _ctSourceGetMonitor.Cancel();
                    }
                }
                catch (SocketException e)
                {
                    throw new Exception($"Could not find address '{_nurse.address}'", e);
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
        #endregion
        #endregion
    }
}
