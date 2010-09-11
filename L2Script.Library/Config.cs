using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace L2Script.Library
{
    public class Config
    {
        public string LoginIP = "127.0.0.1";
        public string LoginPort = "2106";
        public string Username = "";
        public string Password = "";
        public string GameServer = "1";
        public string Toon = "1";
        public string BlowfishKey = "6B60CB5B82CE90B1CC2B6C556C6C6C6C";

        public Config(string configFile)
        {
            Debug.Information("Loading configuration file '" + configFile + "'.");

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(configFile);

                LoginIP = GetConfigValue(doc, "LoginIP");
                LoginPort = GetConfigValue(doc, "LoginPort");
                Username = GetConfigValue(doc, "Username");
                Password = GetConfigValue(doc, "Password");
                GameServer = GetConfigValue(doc, "GameServer");
                Toon = GetConfigValue(doc, "Toon");
                BlowfishKey = GetConfigValue(doc, "BlowfishKey");
            }
            catch (Exception ex)
            {
                Debug.Exception("Error loading configuration file.", ex);
            }

            Debug.Information("Successfully loaded configuration file.");
        }

        private string GetConfigValue(XmlDocument doc, string key)
        {
            try
            {
                XmlNodeList nl = doc.GetElementsByTagName(key);
                XmlElement e = (XmlElement)nl[0];
                return e.InnerText;
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}
