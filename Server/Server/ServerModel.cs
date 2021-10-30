using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;



class ServerModel
{
    private const int port = 7777;

    private const string address = "127.0.0.1";

    private TcpListener listener;

    public void StartServer()
    {
        try
        {
            listener = new TcpListener(IPAddress.Parse(address), port);
            listener.Start();
            Console.WriteLine("Server started");

            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();

                ClientThread clientThread = new ClientThread(client);

                Thread x = new Thread(clientThread.Process);
                x.Start();
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            listener?.Stop();
        }
    }
}

