using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using L2Script.Library;

namespace L2Script.Packets.Game
{
    public class GameWriter : PacketWriter
    {
        public GameWriter(GameData gameData)
        {
            packetBufferStream = new MemoryStream();
            packetBuffer = new BinaryWriter(packetBufferStream);
            GameDataS = gameData;
        }

        public GameWriter(byte[] data, GameData gameData)
        {
            packetBufferStream = new MemoryStream(data);
            packetBuffer = new BinaryWriter(packetBufferStream);
            GameDataS = gameData;
        }

        public void Encrypt()
        {
            packetBuffer.Flush();
            byte[] output = packetBufferStream.ToArray();
            packet = GameDataS.CryptOut.encrypt(output);
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
