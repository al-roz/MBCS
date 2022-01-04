
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
public enum Ruls
{
    nothing = 0,
    all = 1,
    readOnly = 2,
    writeOnly = 3,
    canOpen = 4
}

static public class DeskretModel
{
    static private Dictionary<string, Dictionary<string,Ruls>> Model;
    
    private const string DESKRET_MODEL_FILE_PATH = @"F:\VS\MBCS\Server\Server\DModel.json";

    static private void loadModel()
    {
        Model = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string,Ruls>>>(File.ReadAllText(DESKRET_MODEL_FILE_PATH));
        Model ??= new Dictionary<string, Dictionary<string, Ruls>>();
    }

    static private void UploadModel()
    {
        File.WriteAllText(DESKRET_MODEL_FILE_PATH, JsonConvert.SerializeObject(Model));
    }

    static public void SetOnUserRulsOnObj(string objName, string userLogin, Ruls setedRuls)
    {
        loadModel();
        try
        {
            Model[objName][userLogin] = setedRuls;
        }
        catch (Exception e)
        {
            InitNewObj(objName);
            Model[objName][userLogin] = setedRuls;
        }
        
        UploadModel();
    }

    static public string GetObjRusl(string objName, string userLogin)
    {
        loadModel();
        try
        {
            var res = Model[objName][userLogin];
            return res.ToString();
        }
        catch (Exception e)
        {
            return "0";
        }
    }

    static public void InitNewObj(string obj)
    {
        loadModel();
        Model.Add(obj,new Dictionary<string, Ruls>());
        UploadModel();
    }

    static public bool HaveReadRights(string obj, User user)
    {
        loadModel();
        try
        {
            var ruls = Model[obj][user.login];
            bool result = (ruls == Ruls.all || ruls == Ruls.readOnly);
            if (user.groups != null && user.groups.Count > 0)
            {
                foreach (var group in user.groups)
                {
                    ruls = Model[obj][group];
                    result = result && (ruls == Ruls.all || ruls == Ruls.readOnly);
                    if (result == false)
                        return false;
                }
            }
            
            return result;
        }
        catch (Exception e)
        {
            return false;
        }
        
    }
    
    static public bool HaveWriteRights(string obj, User user)
    {
        loadModel();
        try
        {
            var ruls = Model[obj][user.login];
            bool result =  (ruls == Ruls.all || ruls == Ruls.writeOnly);
            if (user.groups != null && user.groups.Count > 0)
            {
                foreach (var group in user.groups)
                {
                    ruls = Model[obj][group];
                    result = result && (ruls == Ruls.all || ruls == Ruls.writeOnly);
                    if (result == false)
                        return false;
                }
            }
            
            return result;
            
        }
        catch (Exception e)
        {
            return false;
        }
        
    }

    static public bool HaveCanOpenRights(string obj, User user)
    {
        loadModel();
        try
        {
            var ruls = Model[obj][user.login];
            bool result =  ruls == Ruls.all || ruls == Ruls.canOpen;
            if (user.groups != null && user.groups.Count > 0)
            {
                foreach (var group in user.groups)
                {
                    ruls = Model[obj][group];
                    result = result && (ruls == Ruls.all || ruls == Ruls.canOpen);
                    if (result == false)
                        return false;
                }
            }
            
            return result;
        }
        catch (Exception e)
        {
            return false;
        }
    }

    static public void RemoveUserOrGroupOnAllObj(string userLogin)
    {
        loadModel();
        if (userLogin.Contains("GROUP_"))
        {
            foreach (var i in Model)
            {
                Model.Remove(userLogin);
            }
        }
        else
        {
            var tmp = Model.ToList();
            foreach (var i in tmp)
            {
                if (i.Key.Contains('\\' + userLogin))
                {
                    Model.Remove(i.Key);
                }
            }
        }
        
        UploadModel();
        
    }
}
