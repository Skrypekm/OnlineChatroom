using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ObjectOrientedHW3
{
    class Chatroom
    {
        public static List<Socket> CurrentUsers = new List<Socket>();   //holds all currently connected users
        private static int _connectionCount = 0;

        //function to create a new thread whenever a connection is made from the server
        public void OnConnectionMade(object source, ConnectionEventArgs e)
        {
            Thread newThread = new Thread(HandleConnection);
            newThread.Start(e.socket); //the client is passed to a new thread
        }

        private void HandleConnection(object newSocketConnection)
        {
            Socket connectedUser = (Socket)newSocketConnection; //defined the passed object as a socket
            string name = "";
            ASCIIEncoding asen = new ASCIIEncoding();
            _connectionCount++;

            //try catch block to keep server up if someone closes the connection on their end.
            try
            {
                Console.WriteLine("Connection accepted from " + connectedUser.RemoteEndPoint); //writes user to the console
                CurrentUsers.Add(connectedUser);
                Console.WriteLine("Connected user: ");
                foreach (Socket x in CurrentUsers)
                {
                    Console.WriteLine(x.RemoteEndPoint);
                }

                Console.WriteLine();

                //get the users name put into a new funciton
                connectedUser.Send(asen.GetBytes("Welcome to the server."));
                connectedUser.Send(asen.GetBytes("\r\nPlease enter your name: "));

                byte[] nameBuffer = new byte[255];
                int k = connectedUser.Receive(nameBuffer);

                name = "[";
                for (int i = 0; i < k; i++)
                {
                    name += Convert.ToChar(nameBuffer[i]);
                }

                if (name == "[\r\n")
                {
                    name = Convert.ToString("[User #" + _connectionCount);
                }

                name += "]";
                Console.WriteLine(name + " is now messaging...");
                connectedUser.Send(asen.GetBytes("-----------------------------------\r\n"));

                //let the user enter a message could turn to function
                while (true)
                {
                    string message = "";
                    byte[] messageBuffer = new byte[255];
                    int m = connectedUser.Receive(messageBuffer);

                    for (int i = 0; i < m; i++)
                    {
                        message += Convert.ToChar(messageBuffer[i]);
                    }

                    if (message == "\r\n")
                        continue;

                    if (message == "exit")
                        break;

                    foreach (Socket x in CurrentUsers)
                    {
                        x.Send(asen.GetBytes(name + ": " + message + "\r\n"));
                    }
                }

                //function to remove the user from the chat
                //code to remove the current user from the list
                int count = 0;
                foreach (var x in CurrentUsers)
                {
                    if (connectedUser == CurrentUsers[count])
                    {
                        CurrentUsers.RemoveAt(count);
                        break;
                    }

                    count++;
                }

                Console.WriteLine(name + " Has disconnected from the server...");
                foreach (Socket x in CurrentUsers)
                {
                    x.Send(asen.GetBytes(name + " has disconnected from the server...\r\n"));
                }

                Console.WriteLine(name + " has left the server.");

                connectedUser.Close();
            }
            catch (Exception e)
            {
                //if something happens to the client remove them from the list so server continues to function
                int count = 0;
                foreach (var x in CurrentUsers)
                {
                    if (connectedUser == CurrentUsers[count])
                    {
                        CurrentUsers.RemoveAt(count);
                        break;
                    }

                    count++;
                }

                foreach (Socket x in CurrentUsers)
                {
                    x.Send(asen.GetBytes(name + " has left the server...\r\n"));
                }

                Console.WriteLine(name + " has disconnected from the server.");
            }
        }
    }
}