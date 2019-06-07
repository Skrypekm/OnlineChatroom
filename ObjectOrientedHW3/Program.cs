using System;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace ObjectOrientedHW3
{
    class Program
    {
        static void Main(string[] args)
        {
            Server svr = new Server();
            Chatroom chtrm = new Chatroom();

            svr.ConnectionMade += chtrm.OnConnectionMade;
            svr.StartServer();

            while (true)
            {
                Thread.Sleep(100);
            }
        }
    }

    //custom argument class to send the socket data to the chatroom
    public class ConnectionEventArgs : EventArgs
    {
        public Socket socket { get; set; }
    }
}