using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using L2Script.Library;
using L2Script.Packets.Game;
using L2Script.Packets;

namespace L2Script.Plugins
{
    public class Commands
    {
        GameData gameData;
        public Stack<byte[]> outBuffer = new Stack<byte[]>();

        public Commands(GameData gd)
        {
            gameData = gd;
        }

        public void SendMessage(ChatType type, Character Destination, string message)
        {
            GameWriter gw = new GameWriter(gameData);
            gw.writeB(PacketList.Client.Say2);
            gw.writeS(message);
            gw.writeD((byte)type);

            if(type == ChatType.Private)
                gw.writeS(Destination.Name);
            gw.Encrypt();
            outBuffer.Push(gw.Finalize());
        }
        public void SendMessage(ChatType type, string message)
        {
            if (type == ChatType.Private)
                return;

            SendMessage(type, new Character(), message);
        }
    }
}