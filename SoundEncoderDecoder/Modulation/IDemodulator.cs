using SoundEncoderDecoder.WavFormat;
using System.Collections;
using System.IO;

namespace SoundEncoderDecoder.Modulation {
    public interface IDemodulator {
        public SampleRateType SampleRate { get; }
        public double BitDuration { get; }

        bool ReadBit(short[] oneBitSamples, short zerosAverage, short onesAverage);
        BitArray ReadBits(BinaryReader br, MemoryStream ms, short zerosAverage, short onesAverage);
    }
}
