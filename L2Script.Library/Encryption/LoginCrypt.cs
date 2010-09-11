using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace L2Script.Library.Encryption
{
    public class LoginCrypt
    {
        Blowfish bfdecrypt;
        Blowfish bfencrypt;

        public LoginCrypt(byte[] k)
        {
            setKey(k);
        }

        public void setKey(byte[] k)
        {
            bfdecrypt = new Blowfish();
            bfencrypt = new Blowfish();
            bfencrypt.Init(true, k);
            bfdecrypt.Init(false, k);
        }

        public byte[] decrypt(byte[] raw, int offset, int size)
        {
            byte[] result = new byte[size];

            int count = size / 8;
            for (int i = 0; i < count; i++)
            {
                bfdecrypt.ProcessBlock(raw, offset + i * 8, result, i * 8);
            }
            return result;
        }

        public byte[] crypt(byte[] raw, int offset, int size)
        {
            int count = size / 8;
            byte[] result = new byte[size];
            for (int i = 0; i < count; i++)
            {
                this.bfencrypt.ProcessBlock(raw, offset + i * 8, result, i * 8);
            }
            return result;
        }
        public bool verifyChecksum(byte[] raw, int offset, int size)
        {
            if ((size & 3) != 0 || size <= 4)
            {
                return false;
            }

            long chksum = 0;
            int count = size - 4;
            long check = -1;
            int i;

            for (i = offset; i < count; i += 4)
            {
                check = raw[i] & 0xff;
                check |= raw[i + 1] << 8 & 0xff00;
                check |= raw[i + 2] << 0x10 & 0xff0000;
                check |= raw[i + 3] << 0x18 & 0xff000000;

                chksum ^= check;
            }

            check = raw[i] & 0xff;
            check |= raw[i + 1] << 8 & 0xff00;
            check |= raw[i + 2] << 0x10 & 0xff0000;
            check |= raw[i + 3] << 0x18 & 0xff000000;

            return check == chksum;
        }
    }
}

