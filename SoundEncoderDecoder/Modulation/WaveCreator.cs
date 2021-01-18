using SoundEncoderDecoder.WavFormat;
using System.Collections;
using System.IO;

namespace SoundEncoderDecoder.Modulation {
    public static class WaveCreator {

        public static WavFile CreateWave(IModulator modulator, SampleRateType sampleRate, BitArray data) {
            using MemoryStream ms = new MemoryStream();

            foreach (bool bit in data) {
                if (bit) {
                    modulator.WriteOne(ms);
                } else {
                    modulator.WriteZero(ms);
                }
            }

            var wavFile = new WavFile((int)sampleRate, ms.ToArray());
            return wavFile;
        }
    }
}
