using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace ChatServer
{
    class ServerObject
    {
        static TcpListener _tcpListener;
        List<ClientObject> clients = new List<ClientObject>();

        protected internal void AddConnection (ClientObject clientObject)
        {
            clients.Add(clientObject);
        }
        protected internal void RemoveConnection (string id)
        {
            ClientObject client = clients.FirstOrDefault(c => c.Id == id);
            if (client != null)
                clients.Remove(client);
        }

        protected internal void Listen()
        {
            try
            {
                _tcpListener = new TcpListener(IPAddress.Any, 8888);
                _tcpListener.Start();
                Console.WriteLine("Сервер запущен.Ожидание подключений...");

                while (true)
                {
                    TcpClient _tcpClient = _tcpListener.AcceptTcpClient();

                    ClientObject _clientObject = new ClientObject(_tcpClient, this);
                    Thread _clientThread = new Thread(new ThreadStart(_clientObject.Process));
                    _clientThread.Start();

                }
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Disconnect();
            }
        }

        protected internal void BroadcastMessage(string message, string id)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            for (int i = 0; i < clients.Count; i++)
            {
                if(clients[i].Id!=id)
                {
                    clients[i].Stream.Write(data, 0, data.Length);
                }
            }
        }

        protected internal void Disconnect()
        {
            _tcpListener.Stop();
            for (int i = 0; i < clients.Count; i++)
            {
                clients[i].Close();
            }
            Environment.Exit(0);
        }
    }
}
