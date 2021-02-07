using SoundEncoderDecoder.WavFormat;
using System.Collections;

namespace SoundEncoderDecoder.Modulation {
    public interface IDemodulator {
        public SampleRateType SampleRate { get; }
        public double CarrierFrequency { get; }
        public double BitDuration { get; }
        double GetTestValue(short[] arr, bool forOne);
        bool ReadBit(short[] oneBitSamples, double zerosTestValue, double onesTestValue);
        BitArray ReadBits(short[] samples, double zerosTestValue, double onesTestValue, int bitsToRead);
    }
}
