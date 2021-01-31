using SoundEncoderDecoder.WavFormat;
using System.Collections;

namespace SoundEncoderDecoder.Modulation {
    public interface IDemodulator {
        public SampleRateType SampleRate { get; }
        public double CarrierFrequency { get; }
        public double BitDuration { get; }

        bool ReadBit(short[] oneBitSamples, double zerosAverage, double onesAverage);
        BitArray ReadBits(short[] samples, double zerosRMS, double onesRMS, int bitsToRead);
    }
}
