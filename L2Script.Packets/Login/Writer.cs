using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using L2Script.Library;
using L2Script.Library.Encryption;

namespace L2Script.Packets.Login
{
    public class LoginWriter : PacketWriter
    {
        private LoginCrypt crypt;
        byte[] packet;
        MemoryStream packetBufferStream;
        public BinaryWriter packetBuffer;

        public LoginWriter(GameData gameData)
        {
            packetBufferStream = new MemoryStream();
            packetBuffer = new BinaryWriter(packetBufferStream);
            crypt = new LoginCrypt(gameData.Blowfish_Key);
        }

        public LoginWriter(byte[] data, GameData gameData)
        {
            packetBufferStream = new MemoryStream(data);
            packetBuffer = new BinaryWriter(packetBufferStream);
            crypt = new LoginCrypt(gameData.Blowfish_Key);
        }

        public void Encrypt()
        {
            packetBuffer.Flush();
            byte[] output = packetBufferStream.ToArray();
            packet = crypt.crypt(output, 0, output.Length);
        }

        public byte[] Finalize()
        {
            MemoryStream finalBufferStream = new MemoryStream();
            BinaryWriter finalBuffer = new BinaryWriter(finalBufferStream);
            finalBuffer.Write(UInt16.Parse((packet.Length + 2).ToString()));
            finalBuffer.Flush();

            byte[] memstream = finalBufferStream.ToArray();
            byte[] pack_out = new byte[packet.Length + 2];
            pack_out[0] = memstream[0];
            pack_out[1] = memstream[1];
            packet.CopyTo(pack_out, 2);
            return pack_out;
        }
    }
}
