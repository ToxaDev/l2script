using System;
using System.Net;
using L2Script.Library;
using L2Script.Library.Encryption;
using L2Script.Library.Networking;
using L2Script.Packets;
using L2Script.Packets.Login;
using L2Script.Plugins;

namespace L2Script
{
    class LoginServer : TCPClient
    {
        public Plugin[] Plugins = new Plugin[0];
        public Config UserConfig;
        public GameData gameData = new GameData();
        public GameServer[] Servers;
        public ExtensionHandler Extensions = new ExtensionHandler();
        int selectedServer = -1;

        public LoginServer(string configFile)
        {
            UserConfig = new Config(configFile + ".xml");
            Console.Title = "[" + UserConfig.Username + "] L2Script - version " + L2Script.version_string;

            Debug.Information("Loading plugins...");
            Plugins = Manager.GetPlugins(this.GetType().Assembly);
            Debug.Information("Sucessfully loaded " + Plugins.Length.ToString() + " plugins.");

            Debug.Information("Loading Extensions from plugins...");
            int largeCount = 0;
            for (int i = 0; i < Plugins.Length; i++)
            {
                PluginInfo pi = Plugins[i].GetInfo();
                int count = 0;
                try
                {
                    Extension[] ext = Plugins[i].GetExtensions();

                    for (int x = 0; x < ext.Length; x++)
                    {
                        if (ext[x].ShortName == "Events" && Plugins[i].GetInfo().Author == "Peter Corcoran (R4000)")
                            for (int z = 0; z < Plugins.Length; z++)
                                Plugins[z].RegisterEvents(ext[x].Resource);
                        Extensions.Add(ext[x]);
                    }

                    count = ext.Length;
                    largeCount += count;
                }
                catch (Exception ex)
                {
                    Debug.Exception(" - Error loading extensions from '" + pi.ShortName + "'.", ex);
                    continue;
                }
                Debug.Information(" - Loaded " + count + " extensions from '" + pi.ShortName + "'.");
            }
            Debug.Information("Loaded " + largeCount + " extensions from all plugins.");

            Debug.Information("Connecting to login server...");
            try
            {
                Connect(UserConfig.LoginIP, ushort.Parse(UserConfig.LoginPort));
            }
            catch (Exception ex)
            {
                Debug.Exception("Error connecting to server.", ex);
                return;
            }
            Debug.Information("Connected to login server.");

            gameData.commands = new Commands(gameData);
        }

        public override void DataReceived(ref byte[] inputPacket)
        {
            if (gameData.Blowfish_Key.Length > 0)
            {
                LoginReader packet = new LoginReader(inputPacket, gameData);
                packet.Decrypt();
                byte opcode = packet.readB();
                switch (opcode)
                {
                    case 0x01:
                        #region Failed Login.
                        byte reason = packet.readB();
                        string message = "";
                        switch (reason)
                        {
                            case 0x01:
                                message = "System Error.";
                                break;
                            case 0x02:
                                message = "Password is incorrect.";
                                break;
                            case 0x03:
                                message = "Username or password incorrect.";
                                break;
                            case 0x04:
                                message = "Access Failed.";
                                break;
                            case 0x07:
                                message = "Account in use.";
                                break;
                            case 0x0f:
                                message = "Server overloaded.";
                                break;
                            case 0x10:
                                message = "Server undergoing maintenance.";
                                break;
                            case 0x11:
                                message = "Temporary password has expired.";
                                break;
                            case 0x23:
                                message = "Dual box detected.";
                                break;
                        }
                        Debug.Error("Unable to login, message from server: " + message);
                        Close();
                        break;
                        #endregion
                    case 0x0B:
                        #region GameGuard Response.
                        string name = UserConfig.Username;
                        string pw = UserConfig.Password;
                        byte[] login_info = new byte[128];
                        login_info[0x5B] = 0x24;
                        for (int i = 0; i < name.Length; i++)
                        {
                            login_info[0x5E + i] = (byte)name[i];
                        }
                        for (int i = 0; i < pw.Length; i++)
                        {
                            login_info[0x6C + i] = (byte)pw[i];
                        }
                        byte[] Exponent = { 1, 0, 1 };
                        System.Security.Cryptography.RSAParameters RSAKeyInfo = new System.Security.Cryptography.RSAParameters();

                        //Set RSAKeyInfo to the public key values. 
                        RSAKeyInfo.Modulus = gameData.RSA_Key;
                        RSAKeyInfo.Exponent = Exponent;

                        RSA poo = new RSA();
                        poo.ImportParameters(RSAKeyInfo);

                        byte[] outb = new byte[128];

                        outb = poo.EncryptValue(login_info);
                        byte[] login_send = new byte[176];
                        byte[] login_sende = new byte[176];
                        outb.CopyTo(login_send, 1);
                        login_send[129] = gameData.Session[0];
                        login_send[130] = gameData.Session[1];
                        login_send[131] = gameData.Session[2];
                        login_send[132] = gameData.Session[3];
                        login_send[133] = 0x23;//gameguard reply start
                        login_send[134] = 0x01;
                        login_send[135] = 0x00;
                        login_send[136] = 0x00;
                        login_send[137] = 0x67;//
                        login_send[138] = 0x45;
                        login_send[139] = 0x00;
                        login_send[140] = 0x00;
                        login_send[141] = 0xAB;//
                        login_send[142] = 0x89;
                        login_send[143] = 0x00;
                        login_send[144] = 0x00;
                        login_send[145] = 0xEF;//
                        login_send[146] = 0xCD;
                        login_send[147] = 0x00;
                        login_send[148] = 0x00;//game guard reply stop
                        login_send[149] = 0x08;//
                        login_send[150] = 0x00;
                        login_send[151] = 0x00;
                        login_send[152] = 0x00;
                        login_send[153] = 0x00;//
                        login_send[154] = 0x00;
                        login_send[155] = 0x00;
                        login_send[156] = 0x00;
                        login_send[157] = 0x00;//
                        login_send[158] = 0x00;
                        login_send[159] = 0x00;

                        ulong chk = General.CheckSum(login_send, 160);

                        login_send[160] = (byte)(chk & 0xff);
                        login_send[161] = (byte)(chk >> 0x08 & 0xff);
                        login_send[163] = (byte)(chk >> 0x10 & 0xff);
                        login_send[163] = (byte)(chk >> 0x18 & 0xff);

                        LoginWriter pck = new LoginWriter(login_send, gameData);
                        pck.Encrypt();
                        Send(pck.Finalize());
                        Debug.Information("Sending login information.");
                        break;
                        #endregion
                    case 0x03:
                        #region LoginOK
                        byte[] send = new byte[32];

                        send[0] = 0x05;
                        send[1] = packet.readB();
                        send[2] = packet.readB();
                        send[3] = packet.readB();
                        send[4] = packet.readB();
                        send[5] = packet.readB();
                        send[6] = packet.readB();
                        send[7] = packet.readB();
                        send[8] = packet.readB();
                        send[9] = 0x04;
                        send[10] = 0x00;
                        send[11] = 0x00;
                        send[12] = 0x00;
                        send[13] = 0x00;
                        send[14] = 0x00;
                        send[15] = 0x00;

                        gameData.LoginOK = new byte[8];
                        gameData.LoginOK[0] = send[1];
                        gameData.LoginOK[1] = send[2];
                        gameData.LoginOK[2] = send[3];
                        gameData.LoginOK[3] = send[4];
                        gameData.LoginOK[4] = send[5];
                        gameData.LoginOK[5] = send[6];
                        gameData.LoginOK[6] = send[7];
                        gameData.LoginOK[7] = send[8];

                        chk = General.CheckSum(send, 16);

                        send[16] = (byte)(chk & 0xff);
                        send[17] = (byte)(chk >> 0x08 & 0xff);
                        send[18] = (byte)(chk >> 0x10 & 0xff);
                        send[19] = (byte)(chk >> 0x18 & 0xff);

                        pck = new LoginWriter(send, gameData);
                        pck.Encrypt();
                        Send(pck.Finalize());
                        break;
                        #endregion
                    case 0x04:
                        #region Server List
                        int serv_num = packet.readB();
                        Servers = new GameServer[serv_num];
                        packet.readB(); // Unknown byte
                        for (int i = 0; i < serv_num; i++)
                        {
                            GameServer server = new GameServer(ref UserConfig, ref gameData, ref Plugins, ref Extensions);
                            server.ID = packet.packetBuffer.ReadByte();
                            server.IP = new IPAddress(BitConverter.GetBytes(packet.packetBuffer.ReadInt32()));
                            server.Port = packet.packetBuffer.ReadInt32();
                            server.AgeLimit = packet.packetBuffer.ReadByte();
                            server.PvP = packet.packetBuffer.ReadByte();
                            server.CurrentPlayers = packet.packetBuffer.ReadInt16();
                            server.MaxPlayers = packet.packetBuffer.ReadInt16();
                            server.Online = packet.packetBuffer.ReadByte();
                            server.ExtraData = packet.packetBuffer.ReadInt32();
                            server.DisplayBrackets = packet.packetBuffer.ReadByte();
                            Servers[i] = server;
                        }
                        try {
                            selectedServer = Int32.Parse(UserConfig.GameServer) - 1;
                        } catch (Exception ex) {
                            Debug.Exception("Unable to parse which gameserver to use.", ex);
                            return;
                        }
                        Debug.Information("Logging into: " + General.ServerNames[selectedServer]);
                        byte[] buff = new byte[32];

                            buff[0] = 0x02;
                            buff[1] = gameData.LoginOK[0];
                            buff[2] = gameData.LoginOK[1];
                            buff[3] = gameData.LoginOK[2];
                            buff[4] = gameData.LoginOK[3];
                            buff[5] = gameData.LoginOK[4];
                            buff[6] = gameData.LoginOK[5];
                            buff[7] = gameData.LoginOK[6];
                            buff[8] = gameData.LoginOK[7];
                            buff[9] = Servers[selectedServer].ID;
                            buff[10] = 0x00;
                            buff[11] = 0x00;
                            buff[12] = 0x00;
                            buff[13] = 0x00;
                            buff[14] = 0x00;
                            buff[15] = 0x00;

                            chk = General.CheckSum(buff, 16);

                            buff[16] = (byte)(chk & 0xff);
                            buff[17] = (byte)(chk >> 0x08 & 0xff);
                            buff[18] = (byte)(chk >> 0x10 & 0xff);
                            buff[19] = (byte)(chk >> 0x18 & 0xff);

                            pck = new LoginWriter(buff, gameData);
                            pck.Encrypt();
                            Send(pck.Finalize());
                        break;
                        #endregion
                    case 0x06:
                        #region PlayFailed
                        message = "";
                        switch (packet.packetBuffer.ReadByte())
                        {
                            case 0x01:
                                message = "System Error.";
                                break;
                            case 0x02:
                                message = "Username or password is wrong.";
                                break;
                            case 0x03:
                                message = "Reason 3.";
                                break;
                            case 0x04:
                                message = "Reason 4.";
                                break;
                            case 0x0F:
                                message = "Too many players connected.";
                                break;
                        }
                        Debug.Error("Unable to connect to the game server, message: " + message);
                        break;
                        #endregion
                    case 0x07:
                        #region PlayOK
                        gameData.PlayOK = new byte[] { packet.readB(), packet.readB(), packet.readB(), packet.readB(), packet.readB(), packet.readB(), packet.readB(), packet.readB() };
                        Debug.Information("Transfering to the GameServer Module.");
                        Close();
                        Servers[selectedServer].StartConnect();
                        break;
                        #endregion
                    default:
                        Debug.Warning("Unknown packet: " + General.Hex(opcode));
                        break;
                }
            }
            else
            {
                Blowfish blowfish = new Blowfish();
                LoginCrypt crypt = new LoginCrypt(General.Hex(UserConfig.BlowfishKey));

                #region Blowfish key stuff
                Debug.Information("Attempting to get Blowfish key.");
                try
                {
                    int size = 180;
                    byte[] raw = inputPacket;
                    raw = crypt.decrypt(raw, 2, size + 4);
                    int key = ByteHelper.ByteIntR(raw, size - 4);

                    int Bloques = (size / 4) - 1;

                    int[] r = new int[Bloques];
                    int t;
                    for (t = Bloques - 1; t > 0; t--)
                    {
                        int p = ByteHelper.ByteIntR(raw, (4 * t)) ^ key;
                        r[Bloques - t - 1] = p;
                        key = ByteHelper.Vuelta(ByteHelper.Vuelta(key) - ByteHelper.Vuelta(p));
                    }
                    r[Bloques - 1] = ByteHelper.ByteIntR(raw, 0);
                    inputPacket = ByteHelper.IntByte(r);

                    int ia;
                    byte[] t2 = new byte[16];
                    for (ia = 0; ia < 16; ia++)
                    {
                        t2[ia] = inputPacket[153 + ia];
                    }

                    gameData.Blowfish_Key = t2;

                    byte[] session = new byte[4];
                    session[0] = inputPacket[1];
                    session[1] = inputPacket[2];
                    session[2] = inputPacket[3];
                    session[3] = inputPacket[4];

                    gameData.Session = session;

                    crypt = new LoginCrypt(gameData.Blowfish_Key);
                }
                catch (Exception ex)
                {
                    Debug.Exception("Error getting Blowfish key", ex);
                    return;
                }
                Debug.Information("Successfully got Blowfish key:");
                Debug.Information(" - " + General.Hex(gameData.Blowfish_Key));
                #endregion

                #region RSA key stuff
                Debug.Information("Attempting to get RSA key.");
                try
                {
                    byte[] enckey = new byte[128];
                    for (int i = 0; i < 128; i++)
                        enckey[i] = inputPacket[9 + i];

                    for (int i = 0; i < 0x40; i++)
                        enckey[0x40 + i] = (byte)(enckey[0x40 + i] ^ enckey[i]);

                    for (int i = 0; i < 4; i++)
                        enckey[0x0d + i] = (byte)(enckey[0x0d + i] ^ enckey[0x34 + i]);

                    for (int i = 0; i < 0x40; i++)
                        enckey[i] = (byte)(enckey[i] ^ enckey[0x40 + i]);

                    for (int i = 0; i < 4; i++)
                    {
                        byte temp = enckey[0x00 + i];
                        enckey[0x00 + i] = enckey[0x4d + i];
                        enckey[0x4d + i] = temp;
                    }

                    gameData.RSA_Key = enckey;
                }
                catch (Exception ex)
                {
                    Debug.Exception("Error getting RSA key", ex);
                    return;
                }
                Debug.Information("Successfully got RSA key:");
                Debug.Information(" - " + General.Hex(gameData.RSA_Key));
                #endregion

                #region GameGuard Login stuff
                try
                {
                    byte[] send = new byte[40];
                    byte[] sende = new byte[40];

                    send[00] = 0x07;
                    send[01] = inputPacket[1];
                    send[02] = inputPacket[2];
                    send[03] = inputPacket[3];
                    send[04] = inputPacket[4];

                    ulong chk = General.CheckSum(send, 24);

                    send[24] = (byte)(chk & 0xff);
                    send[25] = (byte)(chk >> 0x08 & 0xff);
                    send[26] = (byte)(chk >> 0x10 & 0xff);
                    send[27] = (byte)(chk >> 0x18 & 0xff);

                    LoginWriter lw = new LoginWriter(send, gameData);
                    lw.Encrypt();
                    Send(lw.Finalize());
                }
                catch (Exception ex)
                {
                    Debug.Exception("Error sending GameGuard packet.", ex);
                    return;
                }
                Debug.Information("Successfully sent GameGuard packet.");
                #endregion
            }
        }
    }
}
