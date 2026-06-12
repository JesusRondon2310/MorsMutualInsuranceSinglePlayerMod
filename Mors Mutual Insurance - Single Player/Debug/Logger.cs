using MMI_SP.Helpers;
using System;
using System.IO;

namespace MMI_SP.Debug
{

    static class Logger
    {
        // ==========================================
        // BLOQUE 1: Datos
        // ==========================================
        private const string logFileName = "MMI-SP.log";

        // ==========================================
        // BLOQUE 2: Funciones
        // ==========================================
        public static void ResetLogFile()
        {
            File.Create(logFileName).Dispose();
        }

        public static void Debug(object message)
        {
            if (MMI_SP.MMI.IsDebug)
            {
                Log("Debug - " + Diagnostics.GetCurrentDiagnostics(Constants.ONE) + " " + message);
            }
        }

        public static void Info(object message)
        {
            Log("Info - " + message);
        }

        public static void Warning(object message)
        {
            Log("Warning - " + message);
        }

        public static void Error(object message)
        {
            Log("Error - " + Diagnostics.GetCurrentDiagnostics(Constants.ONE) + " " + message);
        }

        public static void Exception(Exception ex)
        {
            Log("Exception - " + ex.Message + "\r\n" + ex.StackTrace);
        }

        private static void Log(object message)
        {
            File.AppendAllText(logFileName, DateTime.Now + " : " + message + Environment.NewLine);
        }
    }
}