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

        public NzrDemodulator(double carrierFrequency, SampleRateType sampleRate, double bitDuration) {
            CarrierFrequency = carrierFrequency;
            SampleRate = sampleRate;
            BitDuration = bitDuration;
        }

        public BitArray ReadBits(short[] samples, double zerosRMS, double onesRMS, int bytesToRead) {
            var bitLengthInSamples = Wave.CalculateLengthInSamples(1, BitDuration, (int)SampleRate);
            var bitsToRead = bytesToRead * 8;
            //var bitCount = (int)Math.Ceiling((double)samples.Length / bitLengthInSamples);
            BitArray bitArray = new BitArray(bitsToRead);

            for (int i = 0; i < bitsToRead; i++) {
                var oneBitSamples = samples.DeepCopy(i * bitLengthInSamples, bitLengthInSamples);

                if (oneBitSamples == null)
                    return null;

                bitArray.Set(i, ReadBit(oneBitSamples, zerosRMS, onesRMS));
            }

            return bitArray;
        }

        public bool ReadBit(short[] oneBitSamples, double zerosAverage, double onesAverage) {
            //return oneBitSamples.Average(e=>e) > short.MaxValue / 4;
            var skip = (int)(oneBitSamples.Length / 4.0);

            oneBitSamples = oneBitSamples.DeepCopy(/*skip, oneBitSamples.Length - skip*/);
            //    var average = zerosAverage + onesAverage / 2.0;
            //return oneBitSamples.Max() > 0.5 * short.MaxValue;
            // var localRMS = Wave.RMS(oneBitSamples.ToArray());
            // return localRMS > average;
            //  return localRMS > 0.3266 * short.MaxValue;
            //var localRMS = Wave.RMS(new short[] { oneBitSamples.Min(), oneBitSamples.Max() });
            //return oneBitSamples.Average(e=>e) > short.MaxValue / 10;
            var modulator = new NzrModulator(CarrierFrequency, SampleRate, BitDuration);
            byte[] oneBytes;
            using (var ms = new MemoryStream()) {
                modulator.WriteOne(ms);
                oneBytes = ms.ToArray();
            }
            byte[] zeroBytes;
            using (var ms = new MemoryStream()) {
                modulator.WriteZero(ms);
                zeroBytes = ms.ToArray();
            }

            var oneSamples = oneBytes.ToShortArray();
            var zeroSamples = zeroBytes.ToShortArray();

            var diffOnes = 0.0;
            var diffZeros = 0.0;
            for (int i = 0; i < oneSamples.Length; i++) {

                diffOnes += oneSamples[i] * oneBitSamples[i];
                diffZeros += zeroSamples[i] * oneBitSamples[i];


            }
            diffOnes /= oneSamples.Length;
            diffZeros /= oneSamples.Length;

            return Math.Abs(diffOnes - onesAverage) <= Math.Abs(diffZeros - zerosAverage);
            //return diffOnes < diffZeros;
            // var oneSample = modulator.WriteOne
            return Math.Abs(diffOnes - onesAverage) > Math.Abs(diffOnes - zerosAverage);

            /*short peak = 0;
            var half = (short)(short.MaxValue / 2f + 0.1 * short.MaxValue);

            for (int i = 0; i < oneBitSamples.Length; i++) {
                var sample = oneBitSamples[i];

                if (sample > peak) {
                    peak = sample;
                }
            }

            return peak > half;*/
            //float peaksAverage = 0;
            //int peaksCount = 0;

            //short peak = -1;
            //short localAverage = (short)oneBitSamples.Average(e => e);

            //return oneBitSamples.Max() >= averageAbsAmplitude;

            //for (int i = 0; i < oneBitSamples.Length; i++) {
            //    var sample = oneBitSamples[i];
            //    if (sample > 100) {
            //        peaksAverage += sample;
            //        peaksCount++;

            //        if (sample > peak) {
            //            peak = sample;
            //        }
            //    }

            //}

            //if (peaksCount > 3) {
            //    peaksAverage /= peaksCount;
            //    return peaksAverage > averageAbsAmplitude;
            //    //return true;
            //}

            //short localAverage = (short)oneBitSamples.Average(e => e);
            //List<double> peakDeltas = new List<double>();
            //short peakValue = -1;
            //int peakPosition = -1;
            //int previousPeakPosition = 0;

            //for (int i = 0; i < oneBitSamples.Length - 1; i++) {
            //    var sample = oneBitSamples[i];
            //    var nextSample = oneBitSamples[i + 1];
            //    if (sample > localAverage) {
            //        if (sample > peakValue) {
            //            peakValue = sample;
            //            peakPosition = i;
            //        }
            //    } else {
            //        if (peakValue != -1) {
            //            peakDeltas.Add(peakPosition - previousPeakPosition);
            //            previousPeakPosition = peakPosition;
            //            peakValue = -1;
            //            peakPosition = -1;
            //        }
            //    }
            //}
            //if (peakDeltas.Count > 1) {
            //    double averagePeakDeltas = peakDeltas.Skip(1).Average();
            //    double averageFrequency = 1.0 / (averagePeakDeltas / (double)SampleRate) / 4.0 / Math.PI;

            //    return averageFrequency > 1000 / 2.0;
            //}

            return false;
        }
        public static NzrDemodulator Get_1_1000_BitDurationDemodulatorFREQ() {
            var demodulator = new NzrDemodulator(
                carrierFrequency: 800,
                sampleRate: SampleRateType._8000,
                bitDuration: 1.0 / 8
            );

            return demodulator;
        }

        public static NzrDemodulator Get_1_1000_BitDurationDemodulator() {
            var demodulator = new NzrDemodulator(
                carrierFrequency: 1000,
                sampleRate: SampleRateType._32000,
                bitDuration: 1.0 / 4000
            );

            return demodulator;
        }

        public static NzrDemodulator Get_1_3200_BitDurationDemodulator() {
            var demodulator = new NzrDemodulator(
                carrierFrequency: 3200,
                sampleRate: SampleRateType._32000,
                bitDuration: 1.0 / 3200
            );

            return demodulator;
        }

        public static NzrDemodulator Get_1_6400_BitDurationDemodulator() {
            var demodulator = new NzrDemodulator(
                carrierFrequency: 6400,
                sampleRate: SampleRateType._32000,
                bitDuration: 1.0 / 6400
            );

            return demodulator;
        }

        public static NzrDemodulator Get_1_4410_BitDurationDemodulator() {
            var demodulator = new NzrDemodulator(
                carrierFrequency: 3150,
                sampleRate: SampleRateType._44100,
                bitDuration: 1.0 / 3150
            );

            return demodulator;
        }
        public static NzrDemodulator Get_1_2205_BitDurationDemodulator() {
            var demodulator = new NzrDemodulator(
                carrierFrequency: 1000,
                sampleRate: SampleRateType._44100,
                bitDuration: 1.0 / 2205
            );

            return demodulator;
        }
        public static NzrDemodulator Get_1_6300_BitDurationDemodulator() {
            var demodulator = new NzrDemodulator(
                carrierFrequency: 6300,
                sampleRate: SampleRateType._44100,
                bitDuration: 1.0 / 6300
            );

            return demodulator;
        }

        public static NzrDemodulator Get_1_8820_BitDurationDemodulator() {
            var demodulator = new NzrDemodulator(
                carrierFrequency: 8820,
                sampleRate: SampleRateType._44100,
                bitDuration: 1.0 / 8820
            );

            return demodulator;
        }

        public static NzrDemodulator Get_1_9600_BitDurationDemodulator() {
            var demodulator = new NzrDemodulator(
                carrierFrequency: 9600,
                sampleRate: SampleRateType._96000,
                bitDuration: 1.0 / 9600
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
