using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using L2Script.Library;
using System.IO;

namespace L2Script.Packets
{
    public abstract class PacketReader
    {
        public GameData GameDataS;
        public byte[] packet;
        public MemoryStream packetBufferStream;
        public BinaryReader packetBuffer;

        public byte readB()
        {
            return packetBuffer.ReadByte();
        }

        public byte readC()
        {
            return packetBuffer.ReadByte();
        }

        public Int16 readH()
        {
            return packetBuffer.ReadInt16();
        }

        public Int32 readD()
        {
            return packetBuffer.ReadInt32();
        }

        public Int64 readQ()
        {
            return packetBuffer.ReadInt64();
        }

        public double readF()
        {
            return packetBuffer.ReadDouble();
        }

        public string ReadS()
        {
            StringBuilder sb = new StringBuilder();
            byte currentChar;
            byte nextChar;
            do
            {
                currentChar = packetBuffer.ReadByte();

                if (currentChar != 0x00)
                    sb.Append((char)currentChar);

                nextChar = (byte)packetBuffer.PeekChar();
            } while (nextChar != 0x00 || currentChar != 0x00);
            packetBuffer.ReadByte();
            packetBuffer.ReadByte();
            return sb.ToString();
        }
    }
}
