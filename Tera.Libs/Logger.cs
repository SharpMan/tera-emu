using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using System.IO;
using System.Timers;
using System.Runtime.CompilerServices;

namespace Tera.Libs
{
    public class Logger
    {
        public static bool canDebug = true;
        private static StreamWriter ErrWriter;
        public static object Locker = new object();
        
        public static void setStreamWriter(StreamWriter sw)
        {
            ErrWriter = sw;
        }

        public static string GetFormattedDate
        {
            get
            {
                return DateTime.Now.ToString("HH:mm:ss");
            }
        }

        public static void Init()
        {
            if (!Directory.Exists("Logs"))
            {
                Directory.CreateDirectory("Logs");
            }
            ErrWriter = new StreamWriter(@"logs\err_log_" + DateTime.Now.ToString("dd-MM-yyyy") + ".txt", true);
        }

        public static void Debug(String m)
        {
            if (canDebug)
            {
                Append("[Debug]", m, ConsoleColor.Magenta, true , false);
            }
        }
        
        public static void Info(String m)
        {
            Append("[Infos]", m, ConsoleColor.Green);
        }

        public static void Load(String m)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("INFO - " + string.Format("{0} - {1}", DateTime.Now.ToString("HH:mm:ss"), m));
        }

        public static void Loaded(String m)
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine(" "+m);
        }


        public static void Warn(String m)
        {
            Append("[Warning]", m, ConsoleColor.Yellow);
        }

        public static void Error(String e,bool a = true)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            String Format = "ERROR - " + string.Format("{0} - {1}", DateTime.Now.ToString("HH:mm:ss"), e);
            if (a) 
                Append("[Error]", e, ConsoleColor.Red);
            ErrWriter.WriteLine(Format);
            ErrWriter.Flush();
        }

        public static void Error(Exception e)
        {
            Error(e.ToString());
        }

        public static void Stage(string stage)
        {
            Console.Write("\n\n");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("                 ================ ");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(stage);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(" ================ ");
            Console.Write("\n");
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void Append(string header, string message, ConsoleColor headcolor, bool line = true, bool colore = true)
        {
                if (line)
                    Console.Write("\n");

                Console.ForegroundColor = headcolor;
                Console.Write(header);
                Console.Write(" ");
                Console.ForegroundColor = ConsoleColor.Gray;
                string toPrint = GetFormattedDate+" - "+message;
                if(colore && toPrint.Contains("@")){
                    foreach(string str in (toPrint).Split('@')){
                        if (Console.ForegroundColor == ConsoleColor.Gray)
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Gray;
                        }
                        Console.Write(str);
                    }
                }else{
                    Console.Write(toPrint);
                }
        }

        public static string CurrentLoadingSymbol = "|";
        public static Timer LoadingSymbolTimer = new Timer(50);

        public static void InitConsole()
        {
            LoadingSymbolTimer.Elapsed += new ElapsedEventHandler(LoadingSymbolTimer_Elapsed);
        }

        public static void EnableLoadingSymbol()
        {
            LoadingSymbolTimer.Enabled = true;
            LoadingSymbolTimer.Start();
            Console.Write(" " + CurrentLoadingSymbol);
        }

        public static void DisabledLoadingSymbol()
        {
            try
            {
                LoadingSymbolTimer.Enabled = false;
                LoadingSymbolTimer.Stop();
                LoadingSymbolTimer.Close();
                Console.CursorLeft -= 1;
                Console.Write(" ");
            }
            catch { }
        }

        private static void LoadingSymbolTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                Console.CursorLeft -= 1;
                switch (CurrentLoadingSymbol)
                {
                    case "|":
                        CurrentLoadingSymbol = "/";
                        break;

                    case "/":
                        CurrentLoadingSymbol = "-";
                        break;

                    case "-":
                        CurrentLoadingSymbol = "\\";
                        break;

                    case "\\":
                        CurrentLoadingSymbol = "|";
                        break;
                }
                Console.Write(CurrentLoadingSymbol);
            }
            catch { }
        }


    }
}
