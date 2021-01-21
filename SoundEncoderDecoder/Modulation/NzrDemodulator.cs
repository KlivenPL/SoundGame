using SoundEncoderDecoder.WavFormat;
using System;
using System.Collections;
using System.IO;

namespace SoundEncoderDecoder.Modulation {
    public class NzrDemodulator : IDemodulator {
        public SampleRateType SampleRate { get; }
        public float BitDuration { get; }

        public NzrDemodulator(SampleRateType sampleRate, float bitDuration) {
            SampleRate = sampleRate;
            BitDuration = bitDuration;
        }

        public BitArray ReadBits(BinaryReader br, MemoryStream ms, short averageAbsAmplitude) {
            int samplesPerBit = (int)(BitDuration * (int)SampleRate);
            short[] oneBitSamples = new short[samplesPerBit];

            var bitCount = (int)Math.Ceiling(ms.Length / 2f / samplesPerBit);
            BitArray bitArray = new BitArray(bitCount);

            for (int i = 0; i < bitCount; i++) {
                for (int sampleId = 0; sampleId < samplesPerBit; sampleId++) {
                    var sample = (short)Math.Abs((int)br.ReadInt16());
                    oneBitSamples[sampleId] = sample;
                }

                bitArray.Set(i, ReadBit(oneBitSamples, averageAbsAmplitude));
            }

            return bitArray;
        }

        public bool ReadBit(short[] oneBitSamples, short averageAbsAmplitude) {
            short peak = 0;

            for (int i = 0; i < oneBitSamples.Length; i++) {
                var sample = oneBitSamples[i];

                if (sample > peak) {
                    peak = sample;
                }
            }
            return peak > averageAbsAmplitude;
        }

        public NzrDemodulator OneThousandthBitDurationDemodulator() {
            var demodulator = new NzrDemodulator(
                sampleRate: SampleRateType._32000,
                bitDuration: 1 / 1000f
            );

            return demodulator;
        }
    }
}
