using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace L2Script.Library
{
    public enum LogType : byte
    {
        Error = 0x01,
        Warning = 0x02,
        Information = 0x03,
        Exception = 0x04,
        Unknown = 0xFF,
    }

    public static class Debug
    {
        public static void Exception(string Message, Exception ex) {
            _print(LogType.Exception, Message);
            _print(LogType.Exception, ex.ToString());
        }
        public static void Error(string Message)
        {
            _print(LogType.Error, Message);
        }
        public static void Information(string Message)
        {
            _print(LogType.Information, Message);
        }
        public static void Warning(string Message)
        {
            _print(LogType.Warning, Message);
        }
        public static void Unknown(string Message)
        {
            _print(LogType.Unknown, Message);
        }
        public static void Log(LogType Type, string Message)
        {
            _print(Type, Message);
        }

        public static void Blank()
        {
            Console.WriteLine();
            _toFile("\r\n");
        }

        private static void _toFile(string line)
        {
            try
            {
                string text = "";
                if (File.Exists("L2Script-debug.log"))
                    text = File.ReadAllText("L2Script-debug.log");

                File.WriteAllText("L2Script-debug.log", text + line);
            }
            catch (Exception) { }
        }
        private static void _print(LogType lt, string Message)
        {
            string code = "***";
            StringBuilder sb = new StringBuilder();
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("[");
            sb.Append("[");
            switch (lt)
            {
                case LogType.Error:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    code = "ERR";
                    break;
                case LogType.Warning:
                    Console.ForegroundColor = ConsoleColor.Blue;
                    code = "WRN";
                    break;
                case LogType.Information:
                    Console.ForegroundColor = ConsoleColor.Green;
                    code = "INF";
                    break;
                case LogType.Exception:
                    Console.ForegroundColor = ConsoleColor.Red;
                    code = "!E!";
                    break;
                case LogType.Unknown:
                    Console.ForegroundColor = ConsoleColor.White;
                    code = "UNK";
                    break;
            }
            Console.Write(code);
            sb.Append(code);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("][");
            sb.Append("][");
            string time = DateTime.Now.ToLongTimeString();
            Console.Write(time + "] ");
            sb.Append(time + "] ");
            Console.WriteLine(Message);
            sb.Append(Message + "\r\n");

            _toFile(sb.ToString());
        }
    }
}
