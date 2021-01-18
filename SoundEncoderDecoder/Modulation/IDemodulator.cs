using SoundEncoderDecoder.WavFormat;
using System.Collections;
using System.IO;

namespace SoundEncoderDecoder.Modulation {
    public interface IDemodulator {
        public SampleRateType SampleRate { get; }
        public float BitDuration { get; }
        public int DataLength { get; }

        bool ReadBit(short[] oneBitSamples);
        BitArray ReadBits(BinaryReader br, MemoryStream ms);
    }
}
