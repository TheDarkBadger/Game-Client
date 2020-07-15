using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public static class Config
{
    private static readonly string configPath = "";

    private static Dictionary<string, string> settings = new Dictionary<string, string>();

    static Config()
    {
        configPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/My Games/RPGMMO/config.cfg";
        ReadConfig();
    }

    public static string GetSetting(string key)
    {
        if (settings.ContainsKey(key))
        {
            return settings[key];
        }
        else
        {
            Debug.LogError($"ERROR: Could not find setting '{key}'");
            return "NULL";
        }
    }

    public static void SetSetting(string key, string value)
    {
        if (settings.ContainsKey(key))
        {
            settings[key] = value;
        }
        else
        {
            Debug.LogError($"ERROR: Could not find setting '{key}'");
        }
    }

    private static void ReadConfig()
    {
        if (!File.Exists(configPath))
        {
            CreateDefaultConfig();
            return;
        }

        using (StreamReader sr = File.OpenText(configPath))
        {
            string s = "";
            while ((s = sr.ReadLine()) != null)
            {
                string[] read = s.Split('=');
                settings.Add(read[0], read[1]);
            }
        }
    }

    private static void CreateDefaultConfig()
    {
        SetDefaultConfigValues();
        UpdateConfigFile();
    }

    public static void UpdateConfigFile()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(configPath));
        using (FileStream fs = File.Create(configPath))
        {
            foreach (string key in settings.Keys)
            {
                byte[] toWrite = new UTF8Encoding(true).GetBytes($"{key}={settings[key]}\n");
                fs.Write(toWrite, 0, toWrite.Length);
            }
        }
    }

    private static void SetDefaultConfigValues()
    {
        settings.Add("Username", "TheDarkBadger");
        settings.Add("Password", "TheDarkBadger");
        settings.Add("RememberLogin", "False");

    }
}
