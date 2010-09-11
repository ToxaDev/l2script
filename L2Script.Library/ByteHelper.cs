namespace L2Script.Library
{
    public class ByteHelper
    {
        public static int ByteInt(byte[] ko, int off)
        {
            int edx = (ko[off] & 0xFF);
            edx |= (ko[1 + off] & 0xFF) << 8;
            edx |= (ko[2 + off] & 0xFF) << 16;
            edx |= (ko[3 + off] & 0xFF) << 24;
            return edx;

        }
        public static int ByteIntR(byte[] ko, int off)
        {
            int edx = (ko[off + 3] & 0xFF);
            edx |= (ko[off + 2] & 0xFF) << 8;
            edx |= (ko[off + 1] & 0xFF) << 16;
            edx |= (ko[off] & 0xFF) << 24;
            return edx;
        }
        public static int Vuelta(int edx)
        {
            byte[] uy = new byte[4];
            uy[0] = (byte)(edx & 0xFF);
            uy[1] = (byte)(edx >> 8 & 0xFF);
            uy[2] = (byte)(edx >> 16 & 0xFF);
            uy[3] = (byte)(edx >> 24 & 0xFF);
            return ByteIntR(uy, 0);
        }
        public static byte[] IntByte(int[] ji)
        {
            byte[] jo = new byte[ji.Length * 4];

            for (int g = 0; g < ji.Length; g++)
            {
                jo[(g * 4)] = (byte)(ji[g] & 0xFF);
                jo[(g * 4) + 1] = (byte)(ji[g] >> 8 & 0xFF);
                jo[(g * 4) + 2] = (byte)(ji[g] >> 16 & 0xFF);
                jo[(g * 4) + 3] = (byte)(ji[g] >> 24 & 0xFF);
            }
            byte[] ja = new byte[jo.Length];

            for (int g = 0; g < jo.Length; g++)
            {
                ja[g] = jo[jo.Length - g - 1];
            }

            return ja;
        }

    }
}
