using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using L2Script.Library;
using L2Script.Packets;
using L2Script.Packets.Game;

namespace L2Script.Plugins.R4Toolkit.InformationExtension
{
    public class InfoExt
    {
        public Character User;
        public Dictionary<Int32, Character> KnownUsers;
        public Dictionary<Int32, Character> KnownNPCs;
        public Character Targeted;

        public InfoExt()
        {
            User = new Character();
            KnownUsers = new Dictionary<Int32, Character>();
            KnownNPCs = new Dictionary<Int32, Character>();
        }

        public bool HandlePacket(PacketList.Server opcode, GameReader packet, GameData gameData, bool Handled, ExtensionHandler extensions)
        {
            switch (opcode)
            {
                case PacketList.Server.UserInfo:
                    User = Packets.UserInfo.parsePacket(packet);
                    break;
                case PacketList.Server.CharInfo:
                    Character temp = Packets.CharInfo.parsePacket(packet);
                    if(!KnownUsers.ContainsKey(temp.ObjectID))
                        KnownUsers.Add(temp.ObjectID, temp);
                    else
                        KnownUsers[temp.ObjectID] = temp;
                    break;
                case PacketList.Server.NpcInfo:
                    Character temp2 = Packets.NPCInfo.parsePacket(packet);
                    if(!KnownNPCs.ContainsKey(temp2.ObjectID))
                        KnownNPCs.Add(temp2.ObjectID, temp2);
                    else
                        KnownNPCs[temp2.ObjectID] = temp2;
                    break;
                case PacketList.Server.TargetSelectedPacket:
                    Int32 objectid = packet.readD();
                    if (KnownUsers.ContainsKey(objectid))
                    {
                        Targeted = KnownUsers[objectid];
                    }
                    else if (KnownNPCs.ContainsKey(objectid))
                    {
                        Targeted = KnownNPCs[objectid];
                    }
                    break;
                case PacketList.Server.TargetUnselectedPacket:
                    if(Targeted != null && Targeted.ObjectID == packet.readD())
                        Targeted = null;
                    break;
            }
            return false;
        }
    }
}
