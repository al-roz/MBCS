using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


public class DirectoryManager
{
    private StringBuilder userPath = new StringBuilder(@"F:\VS\MBCS\Users");

    public StringBuilder path = new StringBuilder(@"F:\VS\MBCS\Users");

    private bool CheckDirectoryInFolder(string checkedDirectory)
    {
        List<string> folders = GetFileAndDir();

        return folders.Any(i => i == checkedDirectory);
    }

    public bool CheckFilesInFolder(string checkedFile)
    {
        List<string> folders = GetFileAndDir();
        
        return folders.Any(i => i == checkedFile);
    }


    public DirectoryManager(string userLogin)
    {
        path.Append('\\' + userLogin);
        userPath.Append('\\' + userLogin);

        DirectoryInfo userDirectory = new DirectoryInfo(path.ToString());
        if (userDirectory.Exists == false)
        {
            userDirectory.Create();
        }
    }

    public void UpToTheParentDirectory()
    {
        List<string> folders = path.ToString().Split('\\').ToList();

        if (folders[^1] == "Users") return;
        folders.RemoveAt(Convert.ToInt32(folders.Count) - 1);
        string tmp = string.Join('\\', folders);
        path = new StringBuilder(tmp);
    }

    public void DownToTheDirectory(string directory)
    {
        if (!CheckDirectoryInFolder(directory)) return;
        List<string> folders = path.ToString().Split('\\').ToList();
        folders.Add(directory);
        path = new StringBuilder(string.Join('\\', folders));
    }

    public List<string> GetFileAndDir()
    {
        DirectoryInfo userDirectory = new DirectoryInfo(path.ToString());

        DirectoryInfo[] dir = userDirectory.GetDirectories();
        List<string> result = dir.Select(i => i.Name).ToList();

        FileInfo[] fails = userDirectory.GetFiles();
        result.AddRange(fails.Select(i => i.Name));

        if (result.Count == 0)
        {
            result.Add("Folder is empty");
        }


        return result;
    }

    public string ReadFile(string fileName)
    {
        StringBuilder tmpPath = new StringBuilder(path.ToString());
        tmpPath.Append('\\' + fileName);
        string result;
        using (StreamReader reader = new StreamReader(tmpPath.ToString()))
        {
            result = reader.ReadToEnd();
        }

        if (result == "")
            result = "File empty";
        return result;
    }

    public void WriteFile(string fileName, string text)
    {
        StringBuilder tmpPath = new StringBuilder(path.ToString());
        tmpPath.Append('\\' + fileName);
        using (StreamWriter writer = new StreamWriter(tmpPath.ToString(), true))
        {
            writer.Write(text);
        }
    }

    public void CreateDir(string dirName)
    {
        StringBuilder tmpPath = new StringBuilder(path.ToString());
        tmpPath.Append('\\' + dirName);
        DirectoryInfo userDirectory = new DirectoryInfo(tmpPath.ToString());
        if (userDirectory.Exists == false)
        {
            userDirectory.Create();
        }
    }


    public void BackToStartDirectory()
    {
        path = new StringBuilder(userPath.ToString());
    }
}