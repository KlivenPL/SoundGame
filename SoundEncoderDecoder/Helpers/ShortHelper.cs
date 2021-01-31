using System;

namespace SoundEncoderDecoder.Helpers {
    public static class ShortHelper {

        public static byte[] ToByteArray(this short[] shorts, int skip, int take) {
            byte[] byteArr = new byte[(int)Math.Ceiling(take * 2.0)];
            Buffer.BlockCopy(shorts, skip * 2, byteArr, 0, take * 2);

            return byteArr;
        }

        public static byte[] ToByteArray(this short[] shorts) {
            byte[] byteArr = new byte[(int)Math.Ceiling(shorts.Length * 2.0)];
            Buffer.BlockCopy(shorts, 0, byteArr, 0, shorts.Length * 2);

            return byteArr;
        }

        public static short[] DeepCopy(this short[] shorts, int skip, int take) {
            if (shorts == null || take > shorts.Length - skip)
                return null;

            short[] newArr = new short[take];
            Array.Copy(shorts, skip, newArr, 0, take);
            return newArr;
        }

        public static short[] DeepCopy(this short[] shorts) {
            return shorts.DeepCopy(0, shorts.Length);
        }
    }
}
