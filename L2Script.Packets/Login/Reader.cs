using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using L2Script.Library;
using L2Script.Library.Encryption;

namespace L2Script.Packets.Login
{
    public class LoginReader : PacketReader
    {
        private LoginCrypt crypt;

        public LoginReader(byte[] inpacket, GameData gameData)
        {
            packet = inpacket;
            packetBufferStream = new MemoryStream(packet);
            packetBuffer = new BinaryReader(packetBufferStream);
            crypt = new LoginCrypt(gameData.Blowfish_Key);
        }

        public void Decrypt()
        {
            packet = crypt.decrypt(packet, 2, packet.Length - 2);
            packetBufferStream = new MemoryStream(packet);
            packetBuffer = new BinaryReader(packetBufferStream);
        }
    }
}
