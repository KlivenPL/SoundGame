using SoundEncoderDecoder.WavFormat;
using System.Collections;
using System.IO;

namespace SoundEncoderDecoder.Modulation {
    public interface IDemodulator {
        public SampleRateType SampleRate { get; }
        public float BitDuration { get; }

        bool ReadBit(short[] oneBitSamples, short averageAbsAmplitude);
        BitArray ReadBits(BinaryReader br, MemoryStream ms, short averageAbsAmplitude);
    }
}
