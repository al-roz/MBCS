using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

public enum ResultExecutingCommand
{
    CompletedSuccessfully = 0,
    UserInList = 1,
    UserNotInList = 2,
    UserLogged = 3
}

static public class JsonManager
{
    static private MenuItem Users;
    
    private const string FILE_PATH = @"F:\VS\MBCS\Server\Server\user.json";

    static public List<string> loggedUsers = new List<string>();
    static private MenuItem LoadUsers()
    {
        return JsonConvert.DeserializeObject<MenuItem>(File.ReadAllText(FILE_PATH));
    }

    static private User FindUserInListOnLogin(string userLogin)
    {
        return Users.users.FirstOrDefault(i => i.login == userLogin);
    }
    
    static private User FindUserInListOnLoginAndPassword(string userLogin, string userPassword)
    {
        return Users.users.FirstOrDefault(i => i.login == userLogin && i.passwordHash == userPassword);
    }

    static private void UploadUsers()
    {
        File.WriteAllText(FILE_PATH, JsonConvert.SerializeObject(Users));
    }

    static public User GetClient(string userLogin)
    {
        return FindUserInListOnLogin(userLogin);
    }

    static public ResultExecutingCommand CheckUserOnLogin(string userLogin)
    {
        return loggedUsers.Any(t => t == userLogin) ? ResultExecutingCommand.UserLogged : ResultExecutingCommand.CompletedSuccessfully;
    }
    
    static public ResultExecutingCommand AddUser(User newUser)
    {
        if (File.Exists(FILE_PATH))
        {
            Users = LoadUsers();
            
            if (Users == null)
            {
                Users = new MenuItem();
                Users.users = new List<User>();
            }
            
            if (FindUserInListOnLogin(newUser.login) != null)
            {
                return ResultExecutingCommand.UserInList;
            }

            Users.users.Add(newUser);
            UploadUsers();
        }
        return ResultExecutingCommand.CompletedSuccessfully;
    }

    static public ResultExecutingCommand RemoveUser(string userLogin)
    {
        if (File.Exists(FILE_PATH))
        {
            Users = LoadUsers();
            User delUser = FindUserInListOnLogin(userLogin);
            if ( delUser != null)
            {
                Users.users.Remove(delUser);
                UploadUsers();
            }
            else
            {
                return ResultExecutingCommand.UserNotInList;
            }
        }
        return ResultExecutingCommand.CompletedSuccessfully;
    }

    static public ResultExecutingCommand Login(User incomingUser)
    {
        if (File.Exists(FILE_PATH))
        {
            Users = LoadUsers();
            if (FindUserInListOnLoginAndPassword(incomingUser.login,incomingUser.passwordHash) == null)
            {
                return ResultExecutingCommand.UserNotInList;
            }
        }
        loggedUsers.Add(incomingUser.login);
        return ResultExecutingCommand.CompletedSuccessfully;
    }

    static public ResultExecutingCommand ChangePassword(string userLogin, string oldPassword, string newPassword)
    {
        if (File.Exists(FILE_PATH))
        {
            Users = LoadUsers();
            User user = FindUserInListOnLoginAndPassword(userLogin,oldPassword);
            if (user == null)
            {
                return ResultExecutingCommand.UserNotInList;
            }

            Users.users.Remove(user);
            user.passwordHash = newPassword;
            Users.users.Add(user);
            
            UploadUsers();

        }
        
        return ResultExecutingCommand.CompletedSuccessfully;
    }

    public static ResultExecutingCommand AddGroupToUser(string userLogin, string groupName)
    {
        if (File.Exists(FILE_PATH))
        {
            Users = LoadUsers();
            User user = FindUserInListOnLogin(userLogin);
            if (userLogin == null)
            {
                return ResultExecutingCommand.UserNotInList;
            }

            Users.users.Remove(user);
            user.groups ??= new List<string>();
            user.groups.Add(groupName);
            Users.users.Add(user);
            
            UploadUsers();
        }
        return ResultExecutingCommand.CompletedSuccessfully;
    }

    public static ResultExecutingCommand RemoveGroupToUser(string userLogin, string groupName)
    {
        if (File.Exists(FILE_PATH))
        {
            Users = LoadUsers();
            User user = FindUserInListOnLogin(userLogin);
            if (user == null)
            {
                return ResultExecutingCommand.UserNotInList;
            }
            Users.users.Remove(user);
            user.groups ??= new List<string>();
            user.groups.Remove(groupName);
            Users.users.Add(user);
            
            UploadUsers();
        }
        return ResultExecutingCommand.CompletedSuccessfully;
    }
}
