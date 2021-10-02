using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;


static class CommandManager
{
    
    private const string CD = "cd";
    private const string HELP = "help";
    private const string WRITE = "write";
    private const string READ = "read";
    private const string LS = "ls";
    private const string LOGOUT = "logout";
    private const string LOGIN = "login";
    
    private const int CD_LEN = 0;
    private const int HELP_LEN = 0;
    private const int WRITE_LEN = 0;
    private const int READ_LEN = 0;
    private const int LS_LEN = 0;
    private const int LOGUT_LEN = 0;
    private const int LOGIN_LEN = 3;

    private const string ADD_USER = "adduser";
    private const string REMOVE_USER = "remove_user";
    private const string CHANGE_PASSWORD = "change_pswd";
    private const string GET_USERS_INFO = "get_users_info";

    private const int ADD_USER_LEN = 4;
    private const int REMOVE_USER_LEN = 2;
    private const int CHANGE_PASSWORD_LEN = 4;
    private const int GET_USERS_INFO_LEN = 0;

    private const string UNK_COMMAND = "Unknown command";


    static private string AddUser(string login, string passwordHash, string privileges)
    {
        ResultExecutingCommand result = JsonManager.AddUser(new User(login,passwordHash,privileges == "admin"));

        if (result == ResultExecutingCommand.CompletedSuccessfully)
        {
            return "complited";
        }
        return "wrong";
    }

    static private string Login(string login, string passwordHash)
    {
        ResultExecutingCommand result = JsonManager.Login(new User(login, passwordHash,false));
        if (result == ResultExecutingCommand.CompletedSuccessfully)
        {
            return "complited";
        }
        return "invalid log or password";
    }

    static private string RemoveUser(string userLogin)
    {

        ResultExecutingCommand result = JsonManager.RemoveUser(userLogin);
        if (result == ResultExecutingCommand.CompletedSuccessfully)
        {
            return "complited";
        }
        return "wrong";
    }

    static private string ChangePassword(string userLogin, string oldPassword, string newPassword)
    {
        ResultExecutingCommand result = JsonManager.ChangePassword(userLogin, oldPassword, newPassword);
        if (result == ResultExecutingCommand.CompletedSuccessfully)
        {
            return "complited";
        }
        return "wrong";
    }

    static private string GetUserInfo(User clinet)
    {
        return clinet == null ? "Client null" : clinet.login;
    }

    static public string HandleCommand(string command, ref User client)
    {
        string[] args = command.Split(' ');
        string key = args[0].ToLower();
        string result = "";
        switch (key)
        {
            case ADD_USER:
            {
                if (args.Length >= ADD_USER_LEN)
                {
                    result = AddUser(args[1], args[2], args[3]);
                }
                break;
            }
            case LOGIN:
            {
                if (args.Length >= LOGIN_LEN)
                {
                    result = Login(args[1], args[2]);
                    if (result == "complited")
                    {
                        client = JsonManager.GetClient(args[1]);
                    }
                }
                break;
            }
            case REMOVE_USER:
            {
                if (args.Length >= REMOVE_USER_LEN)
                {
                    result = RemoveUser(args[1]);
                }
                break;
            }
            case CHANGE_PASSWORD:
            {
                if (args.Length >= CHANGE_PASSWORD_LEN)
                {
                    result = ChangePassword(args[1], args[2], args[3]);
                }
                break;
            }
            case GET_USERS_INFO:
            {
                result = GetUserInfo(client);
                break;
            }
            case HELP:
            {
                break;
            }
            default:
            {
                result = UNK_COMMAND;
                break;
            }
        }

        return result;

    }
}

