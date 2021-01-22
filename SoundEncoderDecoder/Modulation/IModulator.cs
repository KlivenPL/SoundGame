using SoundEncoderDecoder.WavFormat;
using System.IO;

namespace SoundEncoderDecoder.Modulation {
    public interface IModulator {
        public SampleRateType SampleRate { get; }
        public double CarrierFrequency { get; }
        public double BitDuration { get; }
        void WriteZero(MemoryStream ms);
        void WriteOne(MemoryStream ms);
    }
}
