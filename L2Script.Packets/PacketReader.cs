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

        public string readS()
        {
            StringBuilder sb = new StringBuilder();
            byte currentChar;
            byte lastChar = 0xFF;
            bool stop = false;
            while (stop == false)
            {
                currentChar = packetBuffer.ReadByte();

                if (currentChar != 0x00)
                    sb.Append((char)currentChar);

                if (currentChar == 0x00 && lastChar == 0x00)
                    stop = true;
                else
                    lastChar = currentChar;
            }
            try
            {
                packetBuffer.ReadByte();
            }
            catch (Exception) { }
            if (sb.ToString() == "Reporter")
                currentChar = 0x00;
            return sb.ToString();
        }
    }
}
