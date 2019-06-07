using System;
using System.Net;
using System.Net.Sockets;

namespace ObjectOrientedHW3
{
    class Server
    {
        public delegate void ConnectionMadeEventHandler(object source, ConnectionEventArgs args);

        public event ConnectionMadeEventHandler ConnectionMade;

        private Socket _connectedUser;

        public void StartServer()
        {
            
            //start a tcp listener looking for local machine connections
            TcpListener listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 245);
            listener.Start();

            Console.WriteLine("Server has started. Listening on: " + listener.LocalEndpoint);

            //continually listen for tcp connections and create a new thread with the connection
            while (true)
            {
                _connectedUser = listener.AcceptSocket(); //locks thread until a connection is made
                OnConnectionMade();
            }
        }

        protected virtual void OnConnectionMade()
        {
            ConnectionMade?.Invoke(this, new ConnectionEventArgs() { socket = _connectedUser });
        }
    }
}