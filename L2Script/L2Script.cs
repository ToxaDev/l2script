using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using L2Script.Library;

namespace L2Script
{
    class L2Script
    {

        public static string version_string = "0.1b (Oxygen)";
        public static double version_int = 0.1;

        public static string version_script_string = "21 (NetPlusOne)";
        public static int version_script_int = 21;

        static void Main(string[] args)
        {
            Console.Title = "[Loading...] L2Script - version " + version_string;
            Debug.Blank();
            Debug.Information("L2Script - version " + version_string);
            Debug.Information("Application Started: " + DateTime.Now.ToString());
            Debug.Blank();
            Debug.Information("THIS IS BETA SOFTWARE. NO RESPONSABILITY IS ACCEPTED FOR");
            Debug.Information("ANYTHING THAT HAPPENS TO YOUR PC, LINEAGE ACCOUNT OR ANYTHING");
            Debug.Information("ELSE. YOU HAVE BEEN WARNED. ENJOY THE BETA!");
            Debug.Blank();

            LoginServer Application;
            if (args.Length >= 1)
                Application = new LoginServer(args[0]);
            else
                Application = new LoginServer("default");
        }
    }
}
