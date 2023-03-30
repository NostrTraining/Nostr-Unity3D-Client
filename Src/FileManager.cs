using UnityEngine;
using System.IO;
public static class FileManager {




    public static string[] LoadKeys(string path)
    {
        return File.ReadAllLines(path);
    }
    public static void SaveKey(string path, string[] keys)
    {
        File.WriteAllLines(path, keys);
    }
   
}
