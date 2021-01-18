using SoundEncoderDecoder.WavFormat;
using System;
using System.Collections;
using System.IO;

namespace SoundEncoderDecoder.Modulation {
    public class NzrDemodulator : IDemodulator {
        public SampleRateType SampleRate { get; }
        public float BitDuration { get; }
        public int DataLength { get; }

        public NzrDemodulator(SampleRateType sampleRate, float bitDuration, int dataLength) {
            SampleRate = sampleRate;
            BitDuration = bitDuration;
            DataLength = dataLength;
        }

        public BitArray ReadBits(BinaryReader br, MemoryStream ms) {
            int samplesPerBit = (int)(BitDuration * (int)SampleRate);
            short[] oneBitSamples = new short[samplesPerBit];

            var bitCount = DataLength * 8;
            BitArray bitArray = new BitArray(bitCount);

            for (int i = 0; i < bitCount; i++) {
                for (int sampleId = 0; sampleId < samplesPerBit; sampleId++) {
                    var sample = (short)Math.Abs((int)br.ReadInt16());
                    oneBitSamples[sampleId] = sample;
                }

                bitArray.Set(i, ReadBit(oneBitSamples));
            }

            return bitArray;
        }

        public bool ReadBit(short[] oneBitSamples) {
            short peak = 0;
            var half = (short)(short.MaxValue / 2f + 0.1 * short.MaxValue);

            for (int i = 0; i < oneBitSamples.Length; i++) {
                var sample = oneBitSamples[i];

                if (sample > peak) {
                    peak = sample;
                }
            }

            return peak > half;
        }
    }
}
