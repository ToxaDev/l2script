using System;
using System.Reflection;
using System.IO;

/**
 * This file is only meant for functions that cannot be defined elsewhere.
 * There are many functions that do not deserve a class to themselves,
 * and have no 'partner' functions to fill the class.
 * They are put here...
 **/

namespace L2Script.Library
{
    public class General
    {
        public static string[] ServerNames = new string[] { "Bartz", "Sieghardt", "Kain", "Lionna", "Erica", "Gustin", "Devianne", "Hindemith", "Teon(Euro)", "Franz", "Luna", "Kastien", "Airin", "Staris", "Ceriel", "Fehyshar", "Elhwynna", "Ellikia", "Shikken", "Scryde", "Frikios", "Ophylia", "Shakdun", "Tarziph", "Aria", "Esenn", "Elcardia", "Yiana", "Seresin", "Tarkai", "Khadia", "Roien", "Gallint", "Cedric", "Nerufa", "Asterios", "Orfen", "Mitrael", "Thifiel", "Lithra", "Lockirin", "Kakai", "Cadmus", "Athebaldt", "Blackbird", "Ramsheart", "Esthus", "Vasper", "Lancer", "Ashton", "Waytrel", "Waltner", "Tahnford", "Hunter", "Dewell", "Rodemaye", "Ken Rauhel", "Ken Abigail", "Ken Orwen", "Van Holter", "Desperion", "Einhovant", "Schuneimann", "Faris", "Tor", "Carneiar", "Dwyllios", "Baium", "Hallate", "Zaken", "Core" };

        public static string GetCurrentExecutingDirectory(Assembly assembly)
        {
            string filePath = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
            return Path.GetDirectoryName(filePath);
        }

        public static byte[] Hex(String hex)
        {
            hex = hex.Replace(" ", "");
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        public static string Hex(byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", " ");
        }

        public static string Hex(byte bytes)
        {
            return BitConverter.ToString(new byte[] { bytes }).Replace("-", " ");
        }

        static public string Sanitize(string inp)
        {
            inp = inp.Replace("\\", "");
            inp = inp.Replace("\"", "");
            inp = inp.Replace("'", "");
            inp = inp.Replace(".", "");
            inp = inp.Replace("/", "");
            inp = inp.Replace("\n", "");
            inp = inp.Replace(":", "");

            return inp;
        }

        static public ulong CheckSum(byte[] raw, int count)
        {
            ulong chksum = 0;
            ulong ecx = 0;
            int i = 0;

            for (i = 0; i < count; i += 4)
            {
                ecx = ((ulong)raw[i] & 0xff);
                ecx |= ((ulong)raw[i + 1] << 8 & 0xff00);
                ecx |= ((ulong)raw[i + 2] << 0x10 & 0xff0000);
                ecx |= ((ulong)raw[i + 3] << 0x18 & 0xff000000);

                chksum = chksum ^ ecx;
            }
            /*
                        ecx = raw[i] &0xff;
                        ecx |= raw[i+1] << 8 &0xff00;
                        ecx |= raw[i+2] << 0x10 &0xff0000;
                        ecx |= raw[i+3] << 0x18 &0xff000000;
            */
            raw[i] = (byte)(chksum & 0xff);
            raw[i + 1] = (byte)(chksum >> 0x08 & 0xff);
            raw[i + 2] = (byte)(chksum >> 0x10 & 0xff);
            raw[i + 3] = (byte)(chksum >> 0x18 & 0xff);

            //store the checksum in the last 4 bytes
            return chksum;
        }

        static public uint HexToUInt(string str)
        {
            uint val = 0;
            try
            {
                val = uint.Parse(str, System.Globalization.NumberStyles.AllowHexSpecifier);
            }
            catch
            {
                //oops
            }

            return val;
        }

        public static void Read_String(ref string source, ref string outs)
        {
            int pipe = source.IndexOf("\r\n");
            if (pipe == -1)
            {
                outs = source;
                source = "";
            }
            else
            {
                outs = source.Substring(0, pipe);
                source = source.Remove(0, pipe + 2);
            }
        }

        public static System.Collections.ArrayList GetArray(string inp)
        {
            System.Collections.ArrayList val = new System.Collections.ArrayList();

            int pipe;

            while (inp.Length > 0)
            {
                pipe = inp.IndexOf(';');
                if (pipe == -1)
                {
                    val.Add(inp);
                    inp = "";
                }
                else
                {
                    val.Add(inp.Substring(0, pipe));
                }
                inp = inp.Remove(0, pipe + 1);
            }

            return val;
        }

        public static int MAX(int a, int b)
        {
            if (a > b)
                return a;
            return b;
        }

        public static double MAX(double a, double b)
        {
            if (a > b)
                return a;
            return b;
        }

        public static int MIN(int a, int b)
        {
            if (a < b)
                return a;
            return b;
        }

        public static double MIN(double a, double b)
        {
            if (a < b)
                return a;
            return b;
        }

        public static string MD5(string input)
        {
            // step 1, calculate MD5 hash from input
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }
    }//end of class
}
