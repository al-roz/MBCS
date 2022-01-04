using System;
using System.Collections.Generic;
using System.Text;


static class CommandManager
{
    private const string result_complited = "complited";
    private const string result_wrong = "wrong";


    private const string CD = "cd";
    private const string HELP = "help";
    private const string WRITE = "write";
    private const string READ = "read";
    private const string LS = "ls";
    private const string LOGOUT = "logout";
    private const string LOGIN = "login";
    private const string PATH = "path";
    private const string HOME = "home";
    private const string CREATE_DIR = "create_dir";
    private const string CMOD = "cmod";
    private const string RR = "rr";
    

    private const string CMD_HELP = CD + "\n" + CREATE_DIR + "\n" + HELP + "\n" + WRITE + "\n" + READ + "\n" + LS + "\n" + LOGOUT + "\n"
                                    + LOGIN + "\n" + PATH + "\n" + HOME + "\n" + ADD_USER + "\n" + REMOVE_USER + "\n"
                                    + CHANGE_PASSWORD + "\n" + GET_USERS_INFO + "\n" + ADD_USER_GROUP + "\n" +
                                    REMOVE_USER_GROUP + "\n" + REMOVE_GROUP + "\n" + ADD_GROUP + "\n" + CMOD + "\n" + RR;

    private const int CD_LEN = 2;
    private const int HELP_LEN = 0;
    private const int WRITE_LEN = 3;
    private const int READ_LEN = 2;
    private const int LS_LEN = 0;
    private const int LOGUT_LEN = 0;
    private const int LOGIN_LEN = 3;
    private const int CREATE_DIR_LEN = 2;
    private const int CMOD_LEN = 4;
    private const int RR_LEN = 3;

    private const string ADD_USER = "adduser";
    private const string REMOVE_USER = "remove_user";
    private const string CHANGE_PASSWORD = "change_pswd";
    private const string GET_USERS_INFO = "get_users_info";
    private const string ADD_USER_GROUP = "add_user_to_group";
    private const string REMOVE_USER_GROUP = "remove_user_to_group";
    private const string REMOVE_GROUP = "remove_group";
    private const string ADD_GROUP = "add_group";

    private const int ADD_USER_LEN = 4;
    private const int REMOVE_USER_LEN = 2;
    private const int CHANGE_PASSWORD_LEN = 4;
    private const int GET_USERS_INFO_LEN = 0;
    private const int ADD_USER_GROUP_LEN = 3;
    private const int REMOVE_USER_GROUP_LEN = 3;
    private const int REMOVE_GROUP_LEN = 2;
    private const int ADD_GROUP_LEN = 2;


    private const string UNK_COMMAND = "Unknown command";


    static private string AddUser(string login, string passwordHash, string privileges)
    {
        var newClient = new User(login, passwordHash, privileges == "admin");
        ResultExecutingCommand result = JsonManager.AddUser(newClient);

        if (result == ResultExecutingCommand.CompletedSuccessfully)
        {
            newClient.userDirectory = new DirectoryManager(newClient.login);
            DeskretModel.InitNewObj(newClient.userDirectory.path.ToString());
            DeskretModel.SetOnUserRulsOnObj(newClient.userDirectory.path.ToString(),newClient.login, Ruls.canOpen);
            return result_complited;
        }

        return "wrong";
    }

    static private string Login(string login, string passwordHash)
    {
        ResultExecutingCommand result = JsonManager.Login(new User(login, passwordHash, false));
        if (result == ResultExecutingCommand.CompletedSuccessfully)
        {
            return result_complited;
        }

        return "invalid log or password";
    }

    static private string RemoveUser(string userLogin)
    {
        ResultExecutingCommand result = JsonManager.RemoveUser(userLogin);
        if (result == ResultExecutingCommand.CompletedSuccessfully)
        {
            return result_complited;
        }

        return "wrong";
    }

    static private string ChangePassword(string userLogin, string oldPassword, string newPassword)
    {
        ResultExecutingCommand result = JsonManager.ChangePassword(userLogin, oldPassword, newPassword);
        if (result == ResultExecutingCommand.CompletedSuccessfully)
        {
            return result_complited;
        }

        return "wrong";
    }

    static private string GetUserInfo(User clinet)
    {
        return clinet == null ? "Client null" : clinet.login;
    }

    static private string AddUserToGroup(string userLogin, string groupName)
    {
        ResultExecutingCommand result = JsonManager.AddGroupToUser(userLogin, groupName);
        return result == ResultExecutingCommand.CompletedSuccessfully ? result_complited : result_wrong;
    }

    static private string RemoveUserFromGroup(string userLogin, string groupName)
    {
        ResultExecutingCommand result = JsonManager.RemoveGroupToUser(userLogin, groupName);
        return result == ResultExecutingCommand.CompletedSuccessfully ? result_complited : result_wrong;
    }

    static private string RemoveGroup(string groupName)
    {
        ResultExecutingCommand result = JsonManager.RemoveGroup(groupName);
        return result == ResultExecutingCommand.CompletedSuccessfully ? result_complited : result_wrong;
    }

    static private string AddGroup(string groupName)
    {
        ResultExecutingCommand result = JsonManager.AddGroup("GROUP_" + groupName);
        return result == ResultExecutingCommand.CompletedSuccessfully ? result_complited : result_wrong;
    }

    static private string CreateDir(User client, string newDir)
    {
        client.userDirectory.CreateDir(newDir);
        StringBuilder tmpPath = new StringBuilder(client.userDirectory.path.ToString());
        tmpPath.Append('\\' + newDir);
        DeskretModel.InitNewObj(tmpPath.ToString());
        DeskretModel.SetOnUserRulsOnObj(tmpPath.ToString(),client.login,Ruls.all);
        return result_complited;
    }

    static private string ReadFile(User client, string fileName)
    {
        var result = result_complited;
        StringBuilder tmpPath = new StringBuilder(client.userDirectory.path.ToString());
        tmpPath.Append('\\' + fileName);
        if (DeskretModel.HaveReadRights(tmpPath.ToString(), client))
        {
            result = client.userDirectory.ReadFile(fileName);
        }
        else
        {
            result = "deskret model false";
        }

        return result;
    }

    static private string WriteFile(User client, string fileName,string text)
    {
        var result = result_complited;
        StringBuilder tmpPath = new StringBuilder(client.userDirectory.path.ToString());
        tmpPath.Append('\\' + fileName);
        if (client.userDirectory.CheckFilesInFolder(fileName))
        {
            if (DeskretModel.HaveWriteRights(tmpPath.ToString(), client))
            {
                client.userDirectory.WriteFile(fileName, text);
                result = result_complited;
            }
            else
            {
                result = "deskret model false";
            }
        }
        else
        {
            DeskretModel.InitNewObj(tmpPath.ToString());
            DeskretModel.SetOnUserRulsOnObj(tmpPath.ToString(),client.login,Ruls.all);
            client.userDirectory.WriteFile(fileName, text);
            result = result_complited;
        }
        
        

        return result;
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
                    DeskretModel.RemoveUserOrGroupOnAllObj(args[1]);
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
                    if (DeskretModel.HaveCanOpenRights(client.userDirectory.path.ToString(), client) == false)
                    {
                        client.userDirectory.UpToTheParentDirectory();
                        result = "deskret model false";
                    }
                    else
                    {
                        result = result_complited;
                    }
                }
                else
                {
                    client.userDirectory.UpToTheParentDirectory();
                    result = result_complited;
                }
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
                    result = ReadFile(client, args[1]);
                break;
            }
            case WRITE:
            {
                if (args.Length >= WRITE_LEN)
                {
                    result = WriteFile(client, args[1], args[2]);
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
            case ADD_USER_GROUP:
            {
                if (args.Length >= ADD_USER_GROUP_LEN && client.isAdmin)
                {
                    result = AddUserToGroup(args[1], args[2]);
                }
                else
                {
                    result = result_wrong;
                }

                break;
            }
            case REMOVE_USER_GROUP:
            {
                if (args.Length >= REMOVE_USER_GROUP_LEN && client.isAdmin)
                {
                    result = RemoveUserFromGroup(args[1], args[2]);
                }
                else
                {
                    result = result_wrong;
                }

                break;
            }
            case REMOVE_GROUP:
            {
                if (args.Length >= REMOVE_GROUP_LEN && client.isAdmin)
                {
                    DeskretModel.RemoveUserOrGroupOnAllObj("GROUP_" + args[1]);
                    result = RemoveGroup("GROUP_" + args[1]);
                    
                }
                else
                {
                    result = result_wrong;
                }

                break;
            }
            case ADD_GROUP:
            {
                if (args.Length >= ADD_GROUP_LEN && client.isAdmin)
                {
                    result = AddGroup(args[1]);
                }
                else
                {
                    result = result_wrong;
                }

                break;
            }
            case CREATE_DIR:
            {
                if (args.Length >= CREATE_DIR_LEN)
                {
                    result = CreateDir(client, args[1]);
                }
                else
                {
                    result = result_wrong;
                }
                break;
            }
            case CMOD:
            {
                if (args.Length >= CMOD_LEN)
                {
                    var r = Ruls.nothing + Convert.ToInt32(args[3]);
                    DeskretModel.SetOnUserRulsOnObj(args[1],args[2], r);
                    result = result_complited;
                }
                else
                {
                    result = result_wrong;
                }
                
                break;
            }
            case RR:
            {
                if (args.Length >= RR_LEN)
                {
                    result = DeskretModel.GetObjRusl(args[1], args[2]);
                }
                else
                {
                    result = result_wrong;
                }
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