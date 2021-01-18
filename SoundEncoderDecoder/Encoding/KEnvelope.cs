using SoundEncoderDecoder.Headers;
using SoundEncoderDecoder.Helpers;
using System.Collections;

namespace SoundEncoderDecoder.Encoding {
    public class KEnvelope : IEnvelope<KHeader, byte[]> {
        public KHeader Header { get; private set; }

        public byte[] Data { get; private set; }

        public KEnvelope(byte[] data) {
            Data = data;
            Header = new KHeader(data.Length);
        }
        public BitArray ToBitArray() {
            return Header.ToBitArray().MergeWith(new BitArray(Data));
        }
    }
}
