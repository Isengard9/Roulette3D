using System;
using UnityEngine;

namespace NCGames.Managers
{
    public static class LogManager
    {
        public enum LogLevel
        {
            Log,
            Warning,
            Error,
            Development
        }
    
        public static void Log(string message, LogLevel level = LogLevel.Log, GameObject context = null)
        {
            string formattedMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";
            
            
            switch (level)
            {
                case LogLevel.Log:
                    Debug.Log(formattedMessage, context);
                    break;
                case LogLevel.Warning:
                    Debug.LogWarning(formattedMessage,context);
                    break;
                case LogLevel.Error:
                    Debug.LogError(formattedMessage,context);
                    break;
                case LogLevel.Development:
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                    Debug.Log($"<color=cyan>[DEV] {formattedMessage}</color>",context);
#endif
                    break;
            }
        }
    }
}