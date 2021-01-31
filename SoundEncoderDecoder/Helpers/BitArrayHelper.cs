using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;

namespace SoundEncoderDecoder.Helpers {
    public static class BitArrayHelper {

        public static BitArray MergeWith(this BitArray bitArray, BitArray otherBitArray) {
            if (!bitArray.Check16BitRule()) {
                throw new Exception("Bit array does not obey 16 bit rule");
            }

            if (!otherBitArray.Check16BitRule()) {
                throw new Exception("Other bit array does not obey 16 bit rule");
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
            if (!bitArray.Check16BitRule()) {
                throw new Exception("Bit array does not obey 16 bit rule");
            }

            byte[] array = new byte[bitArray.Count / 8];
            bitArray.CopyTo(array, 0);

            return array;
        }

        public static void WriteBytesToMemStream(this BitArray bitArray, MemoryStream ms) {
            if (!bitArray.Check16BitRule()) {
                throw new Exception("Bit array does not obey 16 bit rule");
            }

            BinaryWriter bw = new BinaryWriter(ms);
            bw.Write(bitArray.ToBytes());
        }

        public static bool EqualTo(this BitArray bitArray, BitArray other) {
            if (bitArray.Count != other.Count) {
                return false;
            }

            for (int i = 0; i < bitArray.Count; i++) {
                if (bitArray[i] != other[i]) {
                    return false;
                }
            }

            return true;
        }

        public static BitArray FromString(string str) {
            str = str.Replace(" ", "");

            if (str.Length % 16 != 0) {
                throw new Exception("Bit array does not obey 16 bit rule");
            }

            if (str.Any(c => c != '0' && c != '1')) {
                throw new Exception("This string does not represent valid bit array");
            }

            return new BitArray(str.Select(c => c == '1').ToArray());
        }

        public static string ToBitString(this BitArray bitArray) {
            if (bitArray == null)
                return null;
            StringBuilder sb = new StringBuilder(bitArray.Length);
            for (int i = 0; i < bitArray.Length; i++) {
                sb.Append(bitArray[i] ? "1" : "0");
            }

            return sb.ToString();
        }

        public static bool Check16BitRule(this BitArray bitArray) {
            if (bitArray == null)
                return false;
            return bitArray.Length % 16 == 0;
        }

        public static double CompareTo(this BitArray bitArray, BitArray other) {
            if (bitArray == null || other == null)
                return 0;

            if (bitArray.Count != other.Count) {
                return 0;
            }

            int sameBits = 0;

            for (int i = 0; i < bitArray.Count; i++) {
                if (bitArray[i] == other[i]) {
                    sameBits++;
                }
            }

            return (double)sameBits / bitArray.Length;
        }

        public static BitArray Skip(this BitArray bitArray, int skip) {
            return FromString(bitArray.ToBitString().Substring(skip));
        }

        public static BitArray SkipLast(this BitArray bitArray, int skip) {
            return FromString(bitArray.ToBitString().Substring(0, bitArray.Count - skip));
        }
    }
}
