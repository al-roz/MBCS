using System.Collections.Generic;
using System.Text;



static class CommandManager
{
    
    private const string CD = "cd";
    private const string HELP = "help";
    private const string WRITE = "write";
    private const string READ = "read";
    private const string LS = "ls";
    private const string LOGOUT = "logout";
    private const string LOGIN = "login";
    private const string PATH = "path";
    private const string HOME = "home";

    private const string CMD_HELP = CD + "\n" + HELP + "\n" + WRITE + "\n" + READ + "\n" + LS + "\n" + LOGOUT + "\n" + LOGOUT + "\n"
                                    + LOGIN + "\n" + PATH + "\n" + HOME + "\n" + ADD_USER + "\n" + REMOVE_USER + "\n" + CHANGE_PASSWORD + "\n" + GET_USERS_INFO; 
    
    private const int CD_LEN = 2;
    private const int HELP_LEN = 0;
    private const int WRITE_LEN = 3;
    private const int READ_LEN = 2;
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
                    result = client.isAdmin ? AddUser(args[1], args[2], args[3]) : "you don't have the right";
                }
                break;
            }
            case LOGIN:
            {
                if (args.Length >= LOGIN_LEN)
                {
                    if (JsonManager.CheckUserOnLogin(args[1]) == ResultExecutingCommand.UserLogged)
                    {
                        result = "UserLogged";
                        break;
                    }
                    result = Login(args[1], args[2]);
                    if (result == "complited")
                    {
                        client = JsonManager.GetClient(args[1]);
                        client.userDirectory = new DirectoryManager(client.login);
                    }
                }
                break;
            }
            case REMOVE_USER:
            {
                if (args.Length >= REMOVE_USER_LEN)
                {
                    result = client.isAdmin ? RemoveUser(args[1]) : "you don't have the right";
                }
                break;
            }
            case CHANGE_PASSWORD:
            {
                if (args.Length >= CHANGE_PASSWORD_LEN)
                {
                    result = client.isAdmin ? ChangePassword(args[1], args[2], args[3]) : "you don't have the right";
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
                result = CMD_HELP;
                break;
            }
            case CD:
            {
                if (args.Length >= CD_LEN)
                {
                    client.userDirectory.DownToTheDirectory(args[1]);    
                }
                else
                {
                    client.userDirectory.UpToTheParentDirectory();
                }

                result = "complited";
                break;
            }
            case LS:
            {
                List<string> res = client.userDirectory.GetFileAndDir();
                StringBuilder tmpMsg = new StringBuilder();
                foreach (var i in res)
                {
                    tmpMsg.Append(i + " ");
                }

                result = tmpMsg.ToString();
                break;
            }
            case READ:
            {
                if (args.Length >= READ_LEN)
                   result = client.userDirectory.ReadFile(args[1]);
                break;
            }
            case WRITE:
            {
                if (args.Length >= WRITE_LEN)
                {
                    client.userDirectory.WriteFile(args[1],args[2]);
                }
                result = "complited";
                break;
            }
            case LOGOUT:
            {
                result = "";
                break;
            }
            case PATH:
            {
                result = client.userDirectory.path.ToString();
                break;
            }
            case HOME:
            {
                client.userDirectory.BackToStartDirectory();
                result = "complited";
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

