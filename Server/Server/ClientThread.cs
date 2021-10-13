using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Threading;
using System.Net.Sockets;


class ClientThread
{
    private TcpClient Client { get; set; }

    private NetworkStream stream;

    private User UserInfo;
    
    public ClientThread (TcpClient newTcpClient)
    {
        Client = newTcpClient;
    }

    private StringBuilder ReadData()
    {
        StringBuilder builderRet = new StringBuilder();
        byte[] data = new byte[4096];
        do
        {
            int bytes = stream.Read(data, 0, data.Length);
            builderRet.Append(Encoding.Unicode.GetString(data, 0, bytes));
        } while (stream.DataAvailable);
        
        return builderRet;
    }

    private void SendMsg(string msg)
    {
        byte[] data = Encoding.Unicode.GetBytes(msg);
        stream.Write(data, 0, data.Length);
    }

    public void Process()
    {
        stream = null;
        try
        {
            stream = Client.GetStream();
            while (true)
            {
                string msg = ReadData().ToString();
                
                string resultMsg = CommandManager.HandleCommand(msg, ref UserInfo);

                if (resultMsg != "")
                {
                    msg = resultMsg;
                    SendMsg(msg);
                }
                else
                {
                    JsonManager.loggedUsers.Remove(UserInfo.login);
                    msg = "relog";
                    SendMsg(msg);
                }

            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            JsonManager.loggedUsers.Remove(UserInfo.login);
            stream?.Close();
            Client?.Close();
        }
    }
}

