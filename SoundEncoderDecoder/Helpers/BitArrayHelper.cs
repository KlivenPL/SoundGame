using System;
using System.Collections;
using System.IO;

namespace SoundEncoderDecoder.Helpers {
    public static class BitArrayHelper {

        public static BitArray MergeWith(this BitArray bitArray, BitArray otherBitArray) {
            if (!bitArray.Check8BitRule()) {
                throw new Exception("Bit array does not obey 8 bit rule");
            }

            if (!otherBitArray.Check8BitRule()) {
                throw new Exception("Other bit array does not obey 8 bit rule");
            }

            byte[] mergedBytes;

            using (MemoryStream ms = new MemoryStream()) {
                bitArray.WriteBytesToMemStream(ms);
                otherBitArray.WriteBytesToMemStream(ms);
                mergedBytes = ms.ToArray();
            }
            return new BitArray(mergedBytes);
        }

        public static byte[] ToBytes(this BitArray bitArray) {
            if (!bitArray.Check8BitRule()) {
                throw new Exception("Bit array does not obey 8 bit rule");
            }

            byte[] array = new byte[bitArray.Count / 8];
            bitArray.CopyTo(array, 0);

            //byte[] result;

            //using (MemoryStream ms = new MemoryStream()) {
            //    bitArray.WriteBytesToMemStream(ms);
            //    result = ms.ToArray();
            //}
            return array;
        }

        public static void WriteBytesToMemStream(this BitArray bitArray, MemoryStream ms) {
            if (!bitArray.Check8BitRule()) {
                throw new Exception("Bit array does not obey 8 bit rule");
            }

            BinaryWriter bw = new BinaryWriter(ms);
            bw.Write(bitArray.ToBytes());

            //StringBuilder sb = new StringBuilder(8);

            //for (int i = 0; i < bitArray.Count / 8; i++) {
            //    for (int j = 0; j < 8; j++) {
            //        sb.Append(bitArray[i] ? "1" : "0");
            //    }
            //    bw.Write(Convert.ToByte(sb.ToString(), 2));
            //    sb.Clear();
            //}
        }

        public static bool Check8BitRule(this BitArray bitArray) {
            return bitArray.Length % 8 == 0;
        }
    }
}
