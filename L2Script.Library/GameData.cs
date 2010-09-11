using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using L2Script.Library.Encryption;

namespace L2Script.Library
{
    public class GameData
    {
        public byte[] Blowfish_Key = new byte[0];
        public byte[] RSA_Key = new byte[0];
        public byte[] Session = new byte[0];
        public byte[] LoginOK = new byte[0];
        public byte[] PlayOK = new byte[0];

        public GameCrypt CryptIn;
        public GameCrypt CryptOut;
    }
}
