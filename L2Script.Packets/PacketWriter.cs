using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using L2Script.Library;
using System.IO;

namespace L2Script.Packets
{
    public abstract class PacketWriter
    {
        public GameData GameDataS;
        public byte[] packet;
        public MemoryStream packetBufferStream;
        public BinaryWriter packetBuffer;

        public void writeB(byte data)
        {
            packetBuffer.Write(data);
            packetBuffer.Flush();
        }

        public void writeF(double data)
        {
            packetBuffer.Write(data);
            packetBuffer.Flush();
        }

        public void writeH(int data)
        {
            packetBuffer.Write((short)data);
            packetBuffer.Flush();
        }

        public void writeD(int data)
        {
            packetBuffer.Write((int)data);
            packetBuffer.Flush();
        }

        public void writeQ(long data)
        {
            packetBuffer.Write((long)data);
            packetBuffer.Flush();
        }

        public void writeB(byte[] data)
        {
            packetBuffer.Write(data);
            packetBuffer.Flush();
        }

        public void writeB(PacketList.Client data)
        {
            writeB((byte)data);
        }

        public void writeB(PacketList.Server data)
        {
            writeB((byte)data);
        }

        public void writeS(string data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                packetBuffer.Write(data[i]);
                packetBuffer.Write((byte)0x00);
            }
            packetBuffer.Write((byte)0x00);
            packetBuffer.Flush();
        }
    }
}
