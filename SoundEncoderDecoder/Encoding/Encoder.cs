using System.Collections;
using TextEncoding = System.Text.Encoding;
namespace SoundEncoderDecoder.Encoding {
    public static class Encoder {

        public static BitArray Encode(byte[] bytes) {
            KEnvelope kEnvelope = new KEnvelope(bytes);
            return kEnvelope.ToBitArray();
        }

        public static BitArray Encode(string utf8Text) {
            KEnvelope kEnvelope = new KEnvelope(TextEncoding.UTF8.GetBytes(utf8Text));
            return kEnvelope.ToBitArray();
        }

    }
}
