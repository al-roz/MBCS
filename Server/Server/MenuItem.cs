using System.Collections.Generic;
using Newtonsoft.Json;


public class User
{
    [JsonProperty("login")] public string login;
    [JsonProperty("passwordHash")] public string passwordHash;
    [JsonProperty("privileges")] public bool isAdmin;

    [JsonIgnore] public DirectoryManager userDirectory;

    public User(string newLogin, string newPasswordHash, bool newIsAdmin)
    {
        login = newLogin;
        passwordHash = newPasswordHash;
        isAdmin = newIsAdmin;
    }
}

public class MenuItem
{
    public List<User> users;
}
