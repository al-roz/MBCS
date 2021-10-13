using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Security.Cryptography;

class ClientModel
{
    private int Port { get; set; }
    private string UserName { get; set; }
    private string Address { get; set; }
    private string UserPassword { get; set; }

    private int PasswordHash { get; set; }
    private NetworkStream Stream { get; set; }
    private TcpClient Client { get; set; }

    

    public ClientModel()
    {
        Port = 0;
        Address = "";
        Client = null;
        Stream = null;
    }

    private void ReadPortAndAddress()
    {
        Console.Write("Port: ");
        Port = Convert.ToInt32(Console.ReadLine());
        Console.Write("Address: ");
        Address = Console.ReadLine();
    }

    private string GetHash(string password)
    {
        SHA256 sha256 = SHA256.Create();
        byte[] sourceByte = Encoding.UTF8.GetBytes(password);
        byte[] hash = sha256.ComputeHash(sourceByte);
        return BitConverter.ToString(hash).Replace("-",String.Empty);
    }

    private void Login()
    {
        while (true)
        {
            Console.Write("User Name: ");
            UserName = Console.ReadLine();
            Console.Write("User Password: ");
            UserPassword = Console.ReadLine();
        
            UserPassword = GetHash(UserPassword);
        
            SendMsg("Login " + UserName + " " + UserPassword);
            string receiveMsg = ReceiveMsg().ToString();
        
             if (receiveMsg == "complited")
             {
                 break;
             }
        }
    }

    private void Connect()
    {
        try
        {
            Client = new TcpClient(Address, Port);
            Stream = Client.GetStream();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        if (Stream != null)
        {
            Console.WriteLine("complited");
        }
    }

    private void SendMsg(string msg)
    {
        byte[] data = Encoding.Unicode.GetBytes(msg);
        Stream.Write(data, 0, data.Length);
    }

    private StringBuilder ReceiveMsg()
    {
        StringBuilder builderRet = new StringBuilder();
        int bytes = 0;
        byte[] data = new byte[4096];
        do
        {
            bytes = Stream.Read(data, 0, data.Length);
            builderRet.Append(Encoding.Unicode.GetString(data, 0, bytes));
        } while (Stream.DataAvailable);
        return builderRet;
    }

    private const string ADD_USER = "adduser";
    private const string LOGOUT = "logout";
    private const string CHANGE_PASSWORD = "change_pswd";
    private string ReadRequest()
    {
        Console.Write(UserName + ": ");
        string message = Console.ReadLine();

        string[] args = message?.Split(' ');

        switch (args?[0])
        {
            case ADD_USER:
            {
                if (args.Length >= 4)
                {
                    args[2] = GetHash(args[2]);
                    StringBuilder newMsg = new StringBuilder();
                    message = newMsg.Append(args[0] + " " + args[1] + " " + args[2] + " " + args[3]).ToString();
                    
                }
                break;
            }
            case CHANGE_PASSWORD:
            {
                if (args.Length >= 4)
                {
                    args[2] = GetHash(args[2]);
                    args[3] = GetHash(args[3]);
                    StringBuilder newMsg = new StringBuilder();
                    message = newMsg.Append(args[0] + " " + args[1] + " " + args[2] + " " + args[3]).ToString();
                }
                break;
            }
                
        }
        
        return message;
    }

    public void StartClient()
    {
        ReadPortAndAddress();
        Connect();
        Login();
        while (true)
        {
            string msg = ReadRequest();
            SendMsg(msg);
            StringBuilder serverMsg = ReceiveMsg();
            if (serverMsg.ToString() != "complited")
            {
                Console.WriteLine(serverMsg.ToString());
            }

            if (serverMsg.ToString() == "relog")
            {
                Login();
            }
        }
    }
}

