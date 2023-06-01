using System;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;
using NetworkLib.Common.DTOs;
using NetworkLib.Common.Interfaces;
using NetworkLib.Common.Logger;

namespace NetworkLib.GameClient
{
    public class Client
    {
        public Guid Id;
        public TcpClient Tcp;
        public Network NetworkHandler;

        private NetworkStream _tcpStream;
        private SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        private BinaryFormatter _binaryFormatter;
        private IMatch _match;

        // Logger
        public static ILogNetwork Log;

        public Client(ILogNetwork log, TcpClient socket)
        {
            Id = Guid.NewGuid();
            _binaryFormatter = new BinaryFormatter();
            NetworkHandler = new Network(this);
            Tcp = socket;
            Log = log;
            Tcp.ReceiveBufferSize = CONSTANTS.BufferSize;
            Tcp.SendBufferSize = CONSTANTS.BufferSize;
            _tcpStream = Tcp.GetStream();
        }

        public void SetMatch(IMatch match)
        {
            _match = match;
            Log.Log($"Client {Id} joined a match");
        }

        public bool IsConnected()
        {
            return Tcp?.Connected == true;
        }

        public void Disconnect()
        {
            Log.LogWarning("Client disconnected");
            Tcp?.Close();
            Tcp = null;
            _tcpStream = null;
        }

        public async Task SendAsync(byte[] bytes, CancellationToken cancellationToken = default)
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

        public async Task SendAsync(object obj, CancellationToken cancellationToken = default)
        {
            if (_tcpStream.CanWrite)
            {
                byte[] bytes = Serialize(obj);
                await SendAsync(bytes, cancellationToken);
            }
        }

        public async Task SendAsync(Message msg, CancellationToken cancellationToken = default)
        {
            if (_tcpStream.CanWrite)
            {
                byte[] bytes = Serialize(msg);
                await SendAsync(bytes, cancellationToken);
            }
        }

        public void Send(object obj)
        {
            byte[] bytes = Serialize(obj);
            Send(bytes);
        }

        public void Send(byte[] bytes)
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

        public byte[] Serialize(object obj)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    _binaryFormatter.Serialize(ms, obj);
                    return ms.ToArray();
                }
            }
            catch (Exception ex)
            {
                // Handle the exception appropriately, e.g., log the error or throw a custom exception
                Log.LogWarning($"Error serializing object: {ex.Message}");
                throw;
            }
        }

        public T Deserialize<T>(byte[] data)
        {
            try
            {
                // Create a new byte array to hold a copy of the data
                byte[] dataCopy = new byte[data.Length];
                Array.Copy(data, dataCopy, data.Length);

                // Deserialize the copied data
                using (MemoryStream memoryStream = new MemoryStream(dataCopy))
                {
                    return (T)_binaryFormatter.Deserialize(memoryStream);
                }
            }
            catch (Exception e)
            {
                // Log the error and the corrupted data
                Log.LogWarning($"Error deserializing data: {e}");
                Log.LogWarning($"Corrupted data: {BitConverter.ToString(data)}");
                throw;
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
                                object msg = Deserialize<object>(data);
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
                        Log.LogWarning("Received 0 bytes - Disconnecting");
                        Disconnect();
                        break;
                    }
                }
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

        private void ProcessMessage(object obj)
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
    }
}
