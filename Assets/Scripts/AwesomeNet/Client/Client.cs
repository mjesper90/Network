using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using NetworkLib.Common;
using NetworkLib.Common.DTOs;
using NetworkLib.Common.Interfaces;
using NetworkLib.Common.Logger;

namespace NetworkLib.GameClient
{
    public class Client
    {
        // Logger
        public static ILogNetwork Log;

        // Message Factory
        public readonly MessageFactory MsgFactory;
        public Guid Id;
        public Network NetworkHandler;
        public TcpClient Tcp;
        public Guid _clientId = Guid.NewGuid();

        private IMatch _match;
        private SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        private NetworkStream _tcpStream;

        public Client(ILogNetwork log, TcpClient socket, MessageFactory mf)
        {
            Id = Guid.NewGuid();
            NetworkHandler = new Network();
            Tcp = socket;
            Log = log;
            Tcp.ReceiveBufferSize = CONSTANTS.BufferSize;
            Tcp.SendBufferSize = CONSTANTS.BufferSize;
            _tcpStream = Tcp.GetStream();
            MsgFactory = mf;
        }

        public void Disconnect()
        {
            Log.Log("Client disconnected");
            Tcp?.Close();
            Tcp = null;
            _tcpStream = null;
        }

        public bool IsConnected()
        {
            return Tcp?.Connected == true;
        }

        public void Send(Message msg)
        {
            if (Tcp != null && _tcpStream?.CanWrite == true)
            {
                byte[] bytes = MsgFactory.Serialize(msg);
                Send(bytes);
            }
            else
            {
                // Handle the case when the object is disposed
                Log.LogWarning("Cannot send message. The TcpClient or NetworkStream is disposed.");
            }
        }

        public async Task SendAsync(Message msg, CancellationToken cancellationToken = default)
        {
            if (_tcpStream.CanWrite)
            {
                byte[] bytes = MsgFactory.Serialize(msg);
                await SendAsync(bytes, cancellationToken);
            }
        }

        public async Task SendAsync(Message[] msg, CancellationToken cancellationToken = default)
        {
            if (_tcpStream.CanWrite)
            {
                byte[] bytes = MsgFactory.Serialize(msg);
                await SendAsync(bytes, cancellationToken);
            }
        }

        public async Task StartReceiving()
        {
            try
            {
                while (IsConnected())
                {
                    byte[] bytes = new byte[CONSTANTS.BufferSize];
                    int receivedLength = await _tcpStream.ReadAsync(bytes, 0, CONSTANTS.BufferSize);
                    if (receivedLength > 0)
                    {
                        byte[] data = new byte[receivedLength];
                        Array.Copy(bytes, data, receivedLength);

                        if (data.Length > 0)
                        {
                            try
                            {
                                object msg = MsgFactory.Deserialize<object>(data);
                                ProcessMessage(msg);
                            }
                            catch (Exception e)
                            {
                                Log.LogWarning($"Error processing received data: {e}");
                                //Log.LogWarning($"Corrupted data: {BitConverter.ToString(data)}");
                            }
                        }
                        else
                        {
                            Log.LogWarning("Received empty data - Disconnecting");
                            Disconnect();
                            break;
                        }
                    }
                    else
                    {
                        Log.Log("Received 0 bytes - Disconnecting");
                        Disconnect();
                        break;
                    }
                }
            }
            catch (ObjectDisposedException)
            {
                Log.Log("The TcpClient or NetworkStream is disposed while receiving data.");
                Disconnect();
            }
            catch (IOException e)
            {
                Log.LogWarning($"IOException while receiving TCP data: {e}");
                Disconnect();
                throw;
            }
            catch (Exception e)
            {
                Log.LogWarning($"Error receiving TCP data: {e}");
                Disconnect();
            }
        }

        protected void ProcessMessage(object obj)
        {
            if (obj is Message message)
            {
                NetworkHandler.Enqueue(message);
            }
            else if (obj is Message[] messages)
            {
                Task.Run(() =>
                {
                    foreach (Message msg in messages)
                    {
                        NetworkHandler.Enqueue(msg);
                    }
                });
            }
        }

        protected void Send(byte[] bytes)
        {
            try
            {
                _tcpStream.Write(bytes, 0, bytes.Length);
                _tcpStream.Flush();
            }
            catch (Exception ex)
            {
                // Handle the exception appropriately, e.g., log the error, retry, or notify the caller
                Log.LogWarning($"Error sending data: {ex.Message}");
            }
        }

        protected async Task SendAsync(byte[] bytes, CancellationToken cancellationToken = default)
        {
            await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                await _tcpStream.WriteAsync(bytes, 0, bytes.Length, cancellationToken).ConfigureAwait(false);
                await _tcpStream.FlushAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                // Handle cancellation if necessary
                Log.LogWarning("Sending operation was cancelled.");
            }
            catch (Exception e)
            {
                // Handle the exception appropriately, e.g., log the error, retry, or notify the caller
                Log.LogError($"Error sending data: {e}");
                Disconnect();
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
