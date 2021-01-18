using System.Collections;

namespace SoundEncoderDecoder.Encoding {
    public interface IEnvelope<THeader, TData> {
        THeader Header { get; }
        TData Data { get; }
        BitArray ToBitArray();
    }
}
