using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using L2Script.Library;

namespace L2Script.Packets.Game
{
    public class GameReader : PacketReader
    {
        public GameReader(byte[] inpacket, GameData gameData)
        {
            byte[] tmp = new byte[inpacket.Length - 2];
            for (int i = 2; i < inpacket.Length; i++)
                tmp[i - 2] = inpacket[i];
            packet = tmp;
            packetBufferStream = new MemoryStream(packet);
            packetBuffer = new BinaryReader(packetBufferStream);
            GameDataS = gameData;
        }

        public void Decrypt()
        {
            packet = GameDataS.CryptIn.decrypt(packet);
            packetBufferStream = new MemoryStream(packet);
            packetBuffer = new BinaryReader(packetBufferStream);
        }
    }
}
