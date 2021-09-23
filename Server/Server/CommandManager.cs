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
    private const int CHANGE_PASSWORD_LEN = 0;
    private const int GET_USERS_INFO_LEN = 0;

    private const string UNK_COMMAND = "Unknown command";

    private const string FILE_PATH = @"F:\VS\MBCS\Server\Server\user.json";

    static private MenuItem Users;
    

    static private string AddUser(string login, string passwordHash, string privileges)
    {
        if (File.Exists(FILE_PATH))
        {
            Users = JsonConvert.DeserializeObject<MenuItem>(File.ReadAllText(FILE_PATH));
            if (Users == null)
            {
                Users = new MenuItem();
                Users.users = new List<User>();
            }
        }
        
        if (Users != null && Users.users.Any(i => login == i.login))
        {
            return $"{login} already exist";
        }
        
        Users?.users.Add(new User(login,passwordHash,privileges == "admin"));
        
        File.WriteAllText(FILE_PATH, JsonConvert.SerializeObject(Users));
        
        // ResultExecutingCommand result = JsonManager.AddUser(new User(login,passwordHash,privileges == "admin"));
        //
        // if (result == ResultExecutingCommand.CompletedSuccessfully)
        // {
        //     return "complited";
        // }
        //
        // return "wrong";
        
        return "";
    }

    static private string Login(string login, string passwordHash)
    {
        if (File.Exists(FILE_PATH))
        {
            Users = JsonConvert.DeserializeObject<MenuItem>(File.ReadAllText(FILE_PATH));
        }

        if (Users != null && Users.users.Any(i => (login == i.login) && (passwordHash == i.passwordHash)))
        {
            return "accept";
        }
        return "invalid log or password";
    }

    static private string Remove_user(string userLogin)
    {
        if (File.Exists(FILE_PATH))
        {
            Users = JsonConvert.DeserializeObject<MenuItem>(File.ReadAllText(FILE_PATH));
        }

        foreach (var i in Users.users.Where(i => i.login == userLogin))
        {
            Users.users.Remove(i);
            break;
        }
        
        File.WriteAllText(FILE_PATH, JsonConvert.SerializeObject(Users));
        
        return "accept";
    }

    static public User GetUser(string userLogin)
    {
        foreach (var i in Users.users.Where(i => i.login == userLogin))
        {
            return i;
        }

        return null;
    }

    static public string HandleCommand(string command)
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
                }
                break;
            }
            case REMOVE_USER:
            {
                if (args.Length >= REMOVE_USER_LEN)
                {
                    result = Remove_user(args[1]);
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

