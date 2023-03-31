using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace ZundaTeller.Test.ExternalServiceIntegrationTest
{
    public class Env
    {
        static Dictionary<string, string> envs = null;

        static void LoadIfNeeds()
        {
            if (envs != null) return;
            envs = new Dictionary<string, string>();
            var envPath = Path.Combine(Application.dataPath.Replace("/Assets", ""), ".env");
            foreach (var line in File.ReadLines(envPath))
            {
                var entry = line.Split('=');
                envs[entry[0]] = entry[1];
            }
        }

        public static string Get(string key)
        {
            LoadIfNeeds();
            if (!envs.ContainsKey(key)) return null;
            return envs[key];
        }
    }
}