
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using Newtonsoft.Json;

public enum ResultExecutingCommand
{
    CompletedSuccessfully = 0,
    UserInList = 1,
    UserNotInList = 2,
        
}

static public class JsonManager
{
    static private MenuItem Users;
    
    private const string FILE_PATH = @"F:\VS\MBCS\Server\Server\user.json";
    
    

    static private MenuItem LoadUsers()
    {
        return JsonConvert.DeserializeObject<MenuItem>(File.ReadAllText(FILE_PATH));
    }

    static private bool CheckUserInList(User testedUser)
    {
        return Users.users.Any(i => i.login == testedUser.login && i.passwordHash == testedUser.passwordHash);
    }

    static private void UploadUsers()
    {
        File.WriteAllText(FILE_PATH, JsonConvert.SerializeObject(Users));
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
            
            if (CheckUserInList(newUser))
            {
                return ResultExecutingCommand.UserInList;
            }

            Users.users.Append(newUser);
            UploadUsers();
        }
        return ResultExecutingCommand.CompletedSuccessfully;
    }

    static public ResultExecutingCommand RemoveUser(User delUser)
    {
        if (File.Exists(FILE_PATH))
        {
            Users = LoadUsers();
            if (CheckUserInList(delUser))
            {
                Users.users.Remove(delUser);
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
            if (!CheckUserInList(incomingUser))
            {
                return ResultExecutingCommand.UserNotInList;
            }
        }
        return ResultExecutingCommand.CompletedSuccessfully;
    }
}
