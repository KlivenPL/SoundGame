using System;

namespace SoundEncoderDecoder.Helpers {
    public static class ByteHelper {

        public static T[] ToArray<T>(this byte[] bytes, int sizeofT, int skip, int take) {
            T[] tArr = new T[take / sizeofT];
            Buffer.BlockCopy(bytes, skip * sizeofT, tArr, 0, take);

            return tArr;
        }

        public static T[] ToArray<T>(this byte[] bytes, int sizeofT) {
            return bytes.ToArray<T>(sizeofT, 0, bytes.Length);
        }

        public static short[] ToShortArray(this byte[] bytes, int skip, int take) {
            return bytes.ToArray<short>(sizeof(short), skip, take);
        }

        public static short[] ToShortArray(this byte[] bytes) {
            return bytes.ToShortArray(0, bytes.Length);
        }

        public static byte[] DeepCopy(this byte[] bytes, int skip, int take) {
            return bytes.ToArray<byte>(sizeof(byte), skip, take);
        }

        public static byte[] DeepCopy(this byte[] bytes) {
            return bytes.DeepCopy(0, bytes.Length);
        }
    }
}
