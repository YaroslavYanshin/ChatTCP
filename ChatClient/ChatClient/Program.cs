using System;
using System.Text;
using System.Threading;
using System.Net.Sockets;

namespace ChatClient
{
    class Program
    {
        static string _userName;
        private const string _host = "127.0.0.1";
        private const int _port = 8888;
        static TcpClient _client;
        static NetworkStream _stream;
        static void Main(string[] args)
        {
            Console.WriteLine("Enter your name: ");
            _userName = Console.ReadLine();
            _client = new TcpClient();

            try
            {
                _client.Connect(_host, _port);
                _stream = _client.GetStream();

                string _message = _userName;
                byte[] data = Encoding.Unicode.GetBytes(_message);
                _stream.Write(data, 0, data.Length);

                Thread _receiveThread = new Thread(new ThreadStart(ReceiveMessage));
                _receiveThread.Start();
                Console.WriteLine("Welcom, {0}", _userName);
                SendMessage();

            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Disconnect();
            }
        }
        static void SendMessage()
        {
            Console.WriteLine("Enter your message: ");
            while (true)
            {
                string _message = Console.ReadLine();
                byte[] data = Encoding.Unicode.GetBytes(_message);
                _stream.Write(data, 0, data.Length);
            }
        }
        static void ReceiveMessage()
        {
            while (true)
            {
                try
                {
                    byte[] data = new byte[64];
                    StringBuilder _builder = new StringBuilder();
                    int _bytes = 0;
                    do
                    {
                        _bytes = _stream.Read(data, 0, data.Length);
                        _builder.Append(Encoding.Unicode.GetString(data, 0, _bytes));
                    }
                    while (_stream.DataAvailable);

                    string _message = _builder.ToString();
                    Console.WriteLine(_message);
                }
                catch
                {
                    Console.WriteLine("Connection aborted!");
                    Console.ReadLine();
                    Disconnect();
                }
            }
        }
        static void Disconnect()
        {
            if (_stream != null)
                _stream.Close();
            if (_client != null)
                _client.Close();
            Environment.Exit(0);
        }

    }
}
