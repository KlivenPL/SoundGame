using SoundEncoderDecoder.Helpers;
using System.Collections;
using TextEncoding = System.Text.Encoding;
namespace SoundEncoderDecoder.Encoding {
    public static class Encoder {

        public static Envelope Encode(byte[] bytes, DataType dataType) {
            if (bytes == null || bytes.Length == 0) {
                throw new System.Exception("Cannot encode null or empty byte array");
            }

            Envelope envelope = new Envelope(bytes, dataType);
            return envelope;
        }

        public static Envelope Encode(byte[] bytes) {
            return Encode(bytes, DataType.Binary);
        }

        public static Envelope Encode(string utf8Text) {
            if (utf8Text == null || utf8Text.Length == 0) {
                throw new System.Exception("Cannot encode null or empty bit string");
            }

            var encodedText = TextEncoding.UTF8.GetBytes(utf8Text);
            return Encode(encodedText, DataType.Text);
        }

        public static Envelope Decode(BitArray bitArray) {
            if (bitArray == null || bitArray.Length == 0) {
                throw new System.Exception("Cannot decode null or empty bit array");
            }

            return new Envelope(bitArray.ToBytes());
        }
    }
}
