using UnityEngine;

namespace ZundaTeller.Configuration
{
    public class Preferences
    {
        public static string OpenAIAPIKey
        {
            get
            {
                return PlayerPrefs.GetString("OpenAIAPIKey");
            }
            set
            {
                PlayerPrefs.SetString("OpenAIAPIKey", value);
            }
        }

        public static VoicevoxInregrationType VoicevoxInregrationType
        {
            get
            {
                return (VoicevoxInregrationType)PlayerPrefs.GetInt("VoicevoxInregrationType", 0);
            }
            set
            {
                PlayerPrefs.SetInt("VoicevoxInregrationType", (int)value);
            }
        }

        public static string VoicevoxEngineHost
        {
            get
            {
                return PlayerPrefs.GetString("VoicevoxEngineHost", "http://localhost:50021/");
            }
            set
            {
                PlayerPrefs.SetString("VoicevoxEngineHost", value);
            }
        }

        public static string VoicevoxWebAPIKey
        {
            get
            {
                return PlayerPrefs.GetString("VoicevoxWebAPIKey");
            }
            set
            {
                PlayerPrefs.SetString("VoicevoxWebAPIKey", value);
            }
        }

        public static bool IsRemoteServiceChecked
        {
            get
            {
                return PlayerPrefs.GetInt("IsRemoteServiceChecked", 0) == 1;
            }
            set
            {
                PlayerPrefs.SetInt("IsRemoteServiceChecked", value ? 1 : 0);
            }
        }
    }
}