using System.Collections.Generic;
using Newtonsoft.Json;


public class Group
{
    [JsonProperty("name")] public string groupName;
    [JsonProperty("users")] public List<string> usersName;

    

    public Group(string newGroup)
    {
        groupName = newGroup;
        usersName ??= new List<string>();
    }
}
public class GroupItem
{
    public List<Group> groups;
}