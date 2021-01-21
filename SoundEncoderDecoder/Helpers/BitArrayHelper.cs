using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;

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

            return array;
        }

        public static void WriteBytesToMemStream(this BitArray bitArray, MemoryStream ms) {
            if (!bitArray.Check8BitRule()) {
                throw new Exception("Bit array does not obey 8 bit rule");
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

            if (str.Length % 8 != 0) {
                throw new Exception("Bit array does not obey 8 bit rule");
            }

            if (str.Any(c => c != '0' && c != '1')) {
                throw new Exception("This string does not represent valid bit array");
            }

            return new BitArray(str.Select(c => c == '1').ToArray());
        }

        public static string ToString(this BitArray bitArray) {
            StringBuilder sb = new StringBuilder(bitArray.Length);
            for (int i = 0; i < bitArray.Length; i++) {
                sb.Append(bitArray[i] ? "1" : "0");
            }

            return sb.ToString();
        }

        public static bool Check8BitRule(this BitArray bitArray) {
            return bitArray.Length % 8 == 0;
        }
    }
}
