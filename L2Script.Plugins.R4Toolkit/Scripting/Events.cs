using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using L2Script.Library;
using L2Script.Packets;
using L2Script.Packets.Game;
using L2Script.Plugins.R4Toolkit.InformationExtension;

namespace L2Script.Plugins.R4Toolkit.Scripting
{
    public class Events
    {
        public delegate void ChatReceivedDelegate(ChatType type, Character sender, string message, GameReader packet, GameData gameData, ExtensionHandler extensions);
        public delegate void SelfDieDelegate(bool hideout, bool castle, bool siege, bool fortress, GameReader packet, GameData gameData, ExtensionHandler extensions);
        public delegate void OtherDieDelegate(bool hideout, bool castle, bool siege, bool fortress, bool sweepable, bool fixedrespawn, GameReader packet, GameData gameData, ExtensionHandler extensions);

        public event ChatReceivedDelegate ChatReceived;
        public event SelfDieDelegate SelfDie;
        public event OtherDieDelegate OtherDie;

        public bool HandlePacket(PacketList.Server opcode, GameReader packet, GameData gameData, bool Handled, ExtensionHandler extensions)
        {
            InfoExt infoExt = (InfoExt)extensions.Get("InfoExt");
            switch (opcode)
            {
                case PacketList.Server.CreatureSay:
                    Character toon = new Character();
                    toon.ObjectID = packet.readD();
                    ChatType type = (ChatType)packet.readD();
                    toon.Name = packet.readS();
                    ChatReceived(type, toon, packet.readS(), packet, gameData, extensions);
                    break;
                case PacketList.Server.Die:
                    Character toon2 = new Character();
                    toon2.ObjectID = packet.readD();
                    bool hideout = (packet.readD() == 0x00) ? false : true;
                    bool castle = (packet.readD() == 0x00) ? false : true;
                    bool siege = (packet.readD() == 0x00) ? false : true;
                    bool sweep = (packet.readD() == 0x00) ? false : true;
                    bool fixedresp = (packet.readD() == 0x00) ? false : true;
                    bool fortress = (packet.readD() == 0x00) ? false : true;
                    if (toon2.ObjectID == infoExt.User.ObjectID)
                        SelfDie(hideout, castle, siege, fortress, packet, gameData, extensions);
                    else
                        OtherDie(hideout, castle, siege, fortress, sweep, fixedresp, packet, gameData, extensions);
                    break;
            }
            return false;
        }
    }
}
