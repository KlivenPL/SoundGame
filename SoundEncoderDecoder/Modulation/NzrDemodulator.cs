using SoundEncoderDecoder.Helpers;
using SoundEncoderDecoder.WavFormat;
using System;
using System.Collections;
using System.IO;

namespace SoundEncoderDecoder.Modulation {
    public class NzrDemodulator : IDemodulator {
        public SampleRateType SampleRate { get; }
        public double CarrierFrequency { get; }
        public double BitDuration { get; }

        private short[] zeroSamples = null;
        private short[] ZeroSamples {
            get {
                if (zeroSamples == null) {
                    var modulator = new NzrModulator(CarrierFrequency, SampleRate, BitDuration);

                    byte[] zeroBytes;
                    using (var ms = new MemoryStream()) {
                        modulator.WriteZero(ms);
                        zeroBytes = ms.ToArray();
                    }
                    zeroSamples = zeroBytes.ToShortArray();
                }

                return zeroSamples;
            }
        }

        private short[] oneSamples = null;
        private short[] OneSamples {
            get {
                if (oneSamples == null) {
                    var modulator = new NzrModulator(CarrierFrequency, SampleRate, BitDuration);

                    byte[] oneBytes;
                    using (var ms = new MemoryStream()) {
                        modulator.WriteOne(ms);
                        oneBytes = ms.ToArray();
                    }
                    oneSamples = oneBytes.ToShortArray();
                }

                return oneSamples;
            }
        }

        public NzrDemodulator(double carrierFrequency, SampleRateType sampleRate, double bitDuration) {
            CarrierFrequency = carrierFrequency;
            SampleRate = sampleRate;
            BitDuration = bitDuration;
        }

        public BitArray ReadBits(short[] samples, double zerosTestValue, double onesTestValue, int bytesToRead) {
            var bitLengthInSamples = Wave.CalculateLengthInSamples(1, BitDuration, (int)SampleRate);
            var bitsToRead = bytesToRead * 8;

            BitArray bitArray = new BitArray(bitsToRead);

            for (int i = 0; i < bitsToRead; i++) {
                var oneBitSamples = samples.DeepCopy(i * bitLengthInSamples, bitLengthInSamples);

                if (oneBitSamples == null)
                    return null;

                bitArray.Set(i, ReadBit(oneBitSamples, zerosTestValue, onesTestValue));
            }

            return bitArray;
        }

        public bool ReadBit(short[] oneBitSamples, double zerosTestValue, double onesTestValue) {
            var diffOnes = 0.0;
            var diffZeros = 0.0;

            for (int i = 0; i < OneSamples.Length; i++) {

                diffOnes += OneSamples[i] * oneBitSamples[i];
                diffZeros += ZeroSamples[i] * oneBitSamples[i];
            }

            diffOnes /= OneSamples.Length;
            diffZeros /= OneSamples.Length;

            return Math.Abs(diffOnes - onesTestValue) <= Math.Abs(diffZeros - zerosTestValue);
        }

        public double GetTestValue(short[] arr, bool forOne) {
            var testSamples = forOne ? OneSamples : ZeroSamples;

            var testValue = 0.0;
            for (int i = 0; i < testSamples.Length; i++) {
                testValue += testSamples[i] * arr[i];
            }

            testValue /= arr.Length;
            return testValue;
        }

        public static NzrDemodulator Get_1_3150_BitDurationDemodulator() {
            var demodulator = new NzrDemodulator(
                carrierFrequency: 3150,
                sampleRate: SampleRateType._44100,
                bitDuration: 1.0 / 3150
            );

            return demodulator;
        }
        public static NzrDemodulator GetCustomBitDurationDemodulator(double carrierFrequency, SampleRateType sampleRate, double bitDuration) {
            var demodulator = new NzrDemodulator(
                carrierFrequency: carrierFrequency,
                sampleRate: sampleRate,
                bitDuration: bitDuration
            );

            return demodulator;
        }
    }
}
