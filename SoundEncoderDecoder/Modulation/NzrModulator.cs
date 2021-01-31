using SoundEncoderDecoder.WavFormat;
using System;
using System.IO;

namespace SoundEncoderDecoder.Modulation {
    public class NzrModulator : IModulator {
        public SampleRateType SampleRate { get; }
        public double CarrierFrequency { get; }
        public double BitDuration { get; }

        public NzrModulator(double carrierFrequency, SampleRateType sampleRate, double bitDuration) {
            CarrierFrequency = carrierFrequency;
            SampleRate = sampleRate;
            BitDuration = bitDuration;
        }

        public void WriteZero(MemoryStream ms) {
            WriteSound(GetZeroSample, ms);
        }

        public void WriteOne(MemoryStream ms) {
            WriteSound(GetOneSample, ms);
        }

        private short GenerateWaveSample(double frequency, double t) {
            var wave = (short)(short.MaxValue * Math.Sin(2 * Math.PI * frequency * t));
            return wave;
        }

        /*        private short GetZeroSample(double t) {
                    return 0;
                }

                private short GetOneSample(double t) {
                    return GenerateWaveSample(CarrierFrequency, t);
                }*/

        private short GetZeroSample(double t) {
            return GenerateWaveSample(-CarrierFrequency, t);
        }

        private short GetOneSample(double t) {
            return GenerateWaveSample(CarrierFrequency, t);
        }


        private void WriteSound(Func<double, short> soundFunc, MemoryStream ms) {
            var totalSamples = (int)(BitDuration * (int)SampleRate);
            BinaryWriter bw = new BinaryWriter(ms);

            for (int sampleId = 0; sampleId < totalSamples; sampleId++) {
                short sample = soundFunc(ms.Position / 2.0 / (double)SampleRate);
                bw.Write(sample);
            }
        }

        public static NzrModulator Get_1_1000_BitDurationModulatorFREQ() {
            var modulator = new NzrModulator(
                carrierFrequency: 800,
                sampleRate: SampleRateType._8000,
                bitDuration: 1.0 / 8
            );

            return modulator;
        }

        public static NzrModulator Get_1_1000_BitDurationModulator() {
            var modulator = new NzrModulator(
                carrierFrequency: 1000,
                sampleRate: SampleRateType._32000,
                bitDuration: 1.0 / 4000
            );

            return modulator;
        }

        public static NzrModulator Get_1_3200_BitDurationModulator() {
            var modulator = new NzrModulator(
                carrierFrequency: 3200,
                sampleRate: SampleRateType._32000,
                bitDuration: 1.0 / 3200
            );

            return modulator;
        }

        public static NzrModulator Get_1_6400_BitDurationModulator() {
            var modulator = new NzrModulator(
                carrierFrequency: 6400,
                sampleRate: SampleRateType._32000,
                bitDuration: 1.0 / 6400
            );

            return modulator;
        }

        public static NzrModulator Get_1_4410_BitDurationModulator() {
            var modulator = new NzrModulator(
                carrierFrequency: 3150,
                sampleRate: SampleRateType._44100,
                bitDuration: 1.0 / 3150//4900
            );

            return modulator;
        }

        public static NzrModulator Get_1_2205_BitDurationModulator() {
            var modulator = new NzrModulator(
                carrierFrequency: 1000,
                sampleRate: SampleRateType._44100,
                bitDuration: 1.0 / 2205
            );

            return modulator;
        }

        public static NzrModulator Get_1_6300_BitDurationModulator() {
            var modulator = new NzrModulator(
                carrierFrequency: 6300,
                sampleRate: SampleRateType._44100,
                bitDuration: 1.0 / 6300
            );

            return modulator;
        }

        public static NzrModulator Get_1_8820_BitDurationModulator() {
            var modulator = new NzrModulator(
                carrierFrequency: 8820,
                sampleRate: SampleRateType._44100,
                bitDuration: 1.0 / 8820
            );

            return modulator;
        }

        public static NzrModulator Get_1_9600_BitDurationModulator() {
            var modulator = new NzrModulator(
                carrierFrequency: 9600,
                sampleRate: SampleRateType._96000,
                bitDuration: 1.0 / 9600
            );

            return modulator;
        }

        public static NzrModulator GetCustomBitDurationModulator(SampleRateType sampleRate, double bitDuration, double carrierFrequency) {
            var modulator = new NzrModulator(
                carrierFrequency: carrierFrequency,
                sampleRate: sampleRate,
                bitDuration: bitDuration
            );

            return modulator;
        }
    }
}
