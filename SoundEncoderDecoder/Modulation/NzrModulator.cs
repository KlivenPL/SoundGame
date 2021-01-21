using SoundEncoderDecoder.WavFormat;
using System;
using System.IO;

namespace SoundEncoderDecoder.Modulation {
    public class NzrModulator : IModulator {
        public SampleRateType SampleRate { get; }
        public float CarrierFrequency { get; }
        public float BitDuration { get; }

        public NzrModulator(float carrierFrequency, SampleRateType sampleRate, float bitDuration) {
            CarrierFrequency = carrierFrequency;
            SampleRate = sampleRate;
            BitDuration = bitDuration;
        }

        public void WriteZero(MemoryStream ms) {
            WriteSound(GetZeroSample, BitDuration, (int)SampleRate, ms);
        }

        public void WriteOne(MemoryStream ms) {
            WriteSound(GetOneSample, BitDuration, (int)SampleRate, ms);
        }

        private short GenerateWaveSample(float t) {
            var wave = (short)(short.MaxValue * Math.Cos(2 * Math.PI * CarrierFrequency * t));
            return wave;
        }

        private short GetZeroSample(float t) {
            return 0;
        }

        private short GetOneSample(float t) {
            return GenerateWaveSample(t);
        }

        private void WriteSound(Func<float, short> soundFunc, float duration, int sampleRate, MemoryStream ms) {
            var totalSamples = duration * sampleRate;
            BinaryWriter bw = new BinaryWriter(ms);

            for (int sampleId = 0; sampleId < totalSamples; sampleId++) {
                short sample = soundFunc(sampleId / totalSamples * duration);
                bw.Write(sample);
            }
        }

        public NzrModulator OneThousandthBitDurationModulator() {
            var modulator = new NzrModulator(
                carrierFrequency: 1000,
                sampleRate: SampleRateType._32000,
                bitDuration: 1 / 1000f
            );

            return modulator;
        }
    }
}
