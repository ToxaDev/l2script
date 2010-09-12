using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using L2Script.Library;
using L2Script.Plugins;
using L2Script.Library.Networking;
using L2Script.Library.Encryption;
using L2Script.Packets.Game;
using L2Script.Packets;
using System.Threading;
/*C - 8bit integer (00)
H - 16bit integer (00 00)
D - 32bit integer (00 00 00 00)
Q - 64bit integer (00 00 00 00 00 00 00 00)
S - String
X/B - dunno*/
namespace L2Script
{
    public class GameServer : TCPClient
    {
        public byte ID;
        public IPAddress IP;
        public Int32 Port;
        public byte AgeLimit;
        public byte PvP;
        public Int16 CurrentPlayers;
        public Int16 MaxPlayers;
        public byte Online;
        public Int32 ExtraData;
        public byte DisplayBrackets;

        public Config UserConfig;
        public GameData gameData;
        public Plugin[] Plugins;
        public ExtensionHandler extensions;
        
        public GameServer(ref Config uc, ref GameData gd, ref Plugin[] plugs, ref ExtensionHandler ext)
        {
            UserConfig = uc;
            gameData = gd;
            Plugins = plugs;
            extensions = ext;
        }

        public void StartConnect()
        {
            Debug.Information("Transfered to GameServer Module.");
            Debug.Information("Connecting to GameServer.");
            try
            {
                Connect(IP.ToString(), (ushort)Port);
            }
            catch (Exception ex)
            {
                Debug.Exception("Error connecting to server.", ex);
                return;
            }
            Debug.Information("Connected to GameServer.");

            byte[] PVPacket = General.Hex("0B 01 0E 98 00 00 00 09 07 54 56 03 09 0B 01 07 02 54 54 56 07 00 02 55 56 00 51 00 53 57 04 07 55 08 54 01 07 01 53 00 56 55 56 01 06 05 04 51 03 08 51 08 51 56 04 54 06 55 08 02 09 51 56 01 53 06 55 04 53 00 56 56 53 01 09 02 09 01 51 54 51 09 55 56 09 03 04 07 05 55 04 06 55 04 06 09 04 51 01 08 08 06 05 52 06 04 01 07 54 03 06 52 55 06 55 55 51 01 02 04 54 03 55 54 01 57 51 55 05 52 05 54 07 51 51 55 07 02 53 53 00 52 05 52 07 01 54 00 03 05 05 08 06 05 05 06 03 00 0D 08 01 07 09 03 51 03 07 53 09 51 06 07 54 0A 50 56 02 52 04 05 55 51 02 53 00 08 54 04 52 56 06 02 09 00 08 03 53 56 01 05 00 55 06 08 56 04 0D 06 07 52 06 07 04 0A 06 01 04 54 04 00 05 02 04 54 00 09 52 53 05 04 01 04 05 05 01 52 51 52 0D 06 51 08 09 54 53 00 0D 01 02 03 54 53 01 05 03 08 56 54 07 02 54 0B 06 A6 23 F4 FE");
            Send(PVPacket);

            Thread sendThread = new Thread(new ThreadStart(sendTask));
            sendThread.Start();
        }

        public void sendTask()
        {
            while (Connected)
            {
                if (gameData.commands.outBuffer.Count > 0)
                {
                    byte[][] toSend = gameData.commands.outBuffer.ToArray();
                    gameData.commands.outBuffer.Clear();
                    for (int i = 0; i < toSend.Length; i++)
                        Send(toSend[i]);
                }
            }
        }

        public override void DataReceived(ref byte[] inputPacket)
        {
            if (gameData.CryptIn == null)
            {
                #region GameServer Auth stuff
                byte[] game_key = new byte[16];
                game_key[0] = inputPacket[4];
                game_key[1] = inputPacket[5];
                game_key[2] = inputPacket[6];
                game_key[3] = inputPacket[7];
                game_key[4] = inputPacket[8];
                game_key[5] = inputPacket[9];
                game_key[6] = inputPacket[10];
                game_key[7] = inputPacket[11];
                game_key[8] = 0xc8;
                game_key[9] = 0x27;
                game_key[10] = 0x93;
                game_key[11] = 0x01;
                game_key[12] = 0xa1;
                game_key[13] = 0x6c;
                game_key[14] = 0x31;
                game_key[15] = 0x97;
                gameData.CryptIn = new GameCrypt();
                gameData.CryptOut = new GameCrypt();
                gameData.CryptIn.setKey(game_key);
                gameData.CryptOut.setKey(game_key);

                int offset = 0;
                byte[] buff = new byte[1024];

                buff[offset++] = 0x2B;
                for (int i = 0; i < UserConfig.Username.Length; i++)
                {
                    buff[offset++] = (byte)UserConfig.Username[i];
                    buff[offset++] = 0x00;
                }
                buff[offset++] = 0x00;
                buff[offset++] = 0x00;
                buff[offset++] = gameData.PlayOK[4];
                buff[offset++] = gameData.PlayOK[5];
                buff[offset++] = gameData.PlayOK[6];
                buff[offset++] = gameData.PlayOK[7];
                buff[offset++] = gameData.PlayOK[0];
                buff[offset++] = gameData.PlayOK[1];
                buff[offset++] = gameData.PlayOK[2];
                buff[offset++] = gameData.PlayOK[3];
                buff[offset++] = gameData.LoginOK[0];
                buff[offset++] = gameData.LoginOK[1];
                buff[offset++] = gameData.LoginOK[2];
                buff[offset++] = gameData.LoginOK[3];
                buff[offset++] = gameData.LoginOK[4];
                buff[offset++] = gameData.LoginOK[5];
                buff[offset++] = gameData.LoginOK[6];
                buff[offset++] = gameData.LoginOK[7];
                buff[offset++] = 0x01;
                buff[offset++] = 0x00;
                buff[offset++] = 0x00;
                buff[offset++] = 0x00;
                buff[offset++] = 0x30;
                buff[offset++] = 0x01;
                buff[offset++] = 0x00;
                buff[offset++] = 0x00;
                for (int x = 0; x < 10; x++)
                    buff[offset++] = 0x00;

                byte[] realBuffer = new byte[offset];
                for (int i = 0; i < realBuffer.Length; i++)
                    realBuffer[i] = buff[i];

                GameWriter pck = new GameWriter(realBuffer, gameData);
                pck.Encrypt();
                byte[] bytes = pck.Finalize();
                Send(bytes);
                Debug.Information("Authenticating with the GameServer.");
                #endregion
            }
            else
            {
                GameReader packet = new GameReader(inputPacket, gameData);
                packet.Decrypt();
                PacketList.Server opcode = (PacketList.Server)packet.readB();

                if (opcode == PacketList.Server.DummyPacket)
                {
                    opcode = (PacketList.Server)BitConverter.ToUInt16(new byte[] {(byte)opcode, packet.readB()}, 0);
                }

                switch (opcode)
                {
                    case PacketList.Server.CharacterSelectionInfo:
                        if (!CallPlugin_HandlePacket(opcode, packet))
                        {
                            Debug.Information("Selecting the character specified in the config.");
                            GameWriter pw = new GameWriter(gameData);
                            pw.writeB(PacketList.Client.CharacterSelect);
                            pw.writeD(Int32.Parse(UserConfig.Toon) - 1);
                            pw.writeB(General.Hex("00 00 00 00 00 00 00 00 00 00 00 00 00 00"));
                            pw.Encrypt();
                            Send(pw.Finalize());
                        }
                        else
                        {
                            Debug.Information("The 'CharacterSelectionInfo' packet was overridden by a plugin.");
                        }
                        break;
                    case PacketList.Server.CharacterSelectedPacket:
                        if (!CallPlugin_HandlePacket(opcode, packet))
                        {
                            byte[] EnterWorldPacket = General.Hex(PacketList.EnterWorld);
                            GameWriter gw = new GameWriter(EnterWorldPacket, gameData);
                            gw.Encrypt();
                            Send(gw.Finalize());
                            Debug.Information("Character selected, entering world...");
                            CallPlugin_HandlePacket(opcode, packet);
                        }
                        else
                        {
                            Debug.Information("The 'CharacterSelectedPacket' packet was overridden by a plugin.");
                        }
                        break;
                    default:
                        CallPlugin_HandlePacket(opcode, packet);
                        break;
                }
            }
        }

        public bool CallPlugin_HandlePacket(PacketList.Server opcode, GameReader packet)
        {
            bool SimpleHandled = false;
            for (int i = 0; i < Plugins.Length; i++)
            {
                if (Plugins[i].HandlePacket(opcode, packet, gameData, SimpleHandled, extensions))
                    SimpleHandled = true;
            }
            return SimpleHandled;
        }
    }
}