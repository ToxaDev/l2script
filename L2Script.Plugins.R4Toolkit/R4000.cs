using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using L2Script.Library;
using L2Script.Packets;
using L2Script.Packets.Game;
using L2Script.Plugins.R4Toolkit;
using L2Script.Plugins.R4Toolkit.InformationExtension;
using L2Script.Plugins.R4Toolkit.Scripting;

namespace L2Script.Plugins.R4000
{
    public class R4Toolkit : Plugin
    {
        PluginInfo pluginInfo;

        public R4Toolkit()
        {
            pluginInfo = new PluginInfo();
            pluginInfo.Name = "L2Script Toolkit";
            pluginInfo.ShortName = "R4Toolkit";
            pluginInfo.Description = "A large selection of tools and extensions for L2Script.";
            pluginInfo.Author = "Peter Corcoran (R4000)";
        }

        public void RegisterEvents(object evnt)
        {
            Events events = (Events)evnt;
            events.ChatReceived += new Events.ChatReceivedDelegate(ChatToDebug);
        }

        public void ChatToDebug(ChatType type, Character sender, string message, GameReader packet, GameData gameData, ExtensionHandler extensions)
        {
            if (type == ChatType.Announcement)
                sender.Name = "";
            Debug.Information(type.ToString() + ":" + sender.Name + ": " + message);

            if (type == ChatType.Private && sender.Name == "Reporter")
            {
                gameData.commands.SendMessage(ChatType.Private, sender, "ECHO: " + message);
            }
        }

        public PluginInfo GetInfo()
        {
            return pluginInfo;
        }

        public bool HandlePacket(PacketList.Server opcode, GameReader packet, GameData gameData, bool Handled, ExtensionHandler extensions)
        {
            InfoExt infoExt = (InfoExt)extensions.Get("InfoExt");
            if (infoExt.HandlePacket(opcode, packet, gameData, Handled, extensions))
                Handled = true;

            Events events = (Events)extensions.Get("Events");
            if (events.HandlePacket(opcode, packet, gameData, Handled, extensions))
                Handled = true;

            return Handled;
        }

        public Extension[] GetExtensions()
        {
            Stack<Extension> extStack = new Stack<Extension>();
            extStack.Push(new Extension("InfoExt", "Information Extension", "Provides key information", new InfoExt()));
            extStack.Push(new Extension("Events", "Event Extension", "Provides access to C# Event driven stuff.", new Events()));
            return extStack.ToArray();
        }
    }
}
