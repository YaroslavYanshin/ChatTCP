using System;
using System.Text;
using System.Net.Sockets;

namespace ChatServer
{
    class ClientObject
    {
        protected internal string Id { get; private set; }
        protected internal NetworkStream Stream { get; private set; }
        string _userName;
        TcpClient _client;
        ServerObject _server;

        public ClientObject(TcpClient tcpClient, ServerObject serverObject)
        {
            Id = Guid.NewGuid().ToString();
            _client = tcpClient;
            _server = serverObject;
            serverObject.AddConnection(this);
        }

        public void Process()
        {
            try
            {
                Stream = _client.GetStream();
                string message = GetMessage();
                _userName = message;

                message = _userName + "Entered chat";
                _server.BroadcastMessage(message, this.Id);
                Console.WriteLine(message);

                while (true)
                {
                    try
                    {
                        message = GetMessage();
                        message = String.Format("{0}: {1}", _userName, message);
                        Console.WriteLine(message);
                        _server.BroadcastMessage(message, this.Id);
                    }
                    catch
                    {
                        message = String.Format("{0}: left the chat", _userName);
                        Console.WriteLine(message);
                        _server.BroadcastMessage(message, this.Id);
                        break;
                    }

                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                // в случае выхода из цикла закрываем ресурсы
                _server.RemoveConnection(this.Id);
                Close();
            }
            

        }    
        
        private string GetMessage()
        {
            byte[] data = new byte[64];
            StringBuilder builder = new StringBuilder();
            int _bytes = 0;
            do
            {
                _bytes = Stream.Read(data, 0, data.Length);
                builder.Append(Encoding.Unicode.GetString(data, 0, _bytes));
            }
            while (Stream.DataAvailable);

            return builder.ToString();
        }

        protected internal void Close()
        {
            if (Stream != null)
                Stream.Close();
            if (_client != null)
                _client.Close();
        }
    }
}
        
    

