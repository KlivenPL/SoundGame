using SoundEncoderDecoder.WavFormat;
using System;
using System.Collections;
using System.IO;
using System.Linq;

namespace SoundEncoderDecoder.Modulation {
    public class NzrDemodulator : IDemodulator {
        public SampleRateType SampleRate { get; }
        public double BitDuration { get; }

        public NzrDemodulator(SampleRateType sampleRate, double bitDuration) {
            SampleRate = sampleRate;
            BitDuration = bitDuration;
        }

        public BitArray ReadBits(BinaryReader br, MemoryStream ms, short zerosAverage, short onesAverage) {
            int samplesPerBit = (int)Math.Ceiling(BitDuration * (int)SampleRate);
            short[] oneBitSamples = new short[samplesPerBit];

            var bitCount = (int)Math.Ceiling(ms.Length / 2.0 / samplesPerBit);
            BitArray bitArray = new BitArray(bitCount);

            for (int i = 0; i < bitCount; i++) {
                for (int sampleId = 0; sampleId < samplesPerBit; sampleId++) {
                    var sample = (short)Math.Abs((int)br.ReadInt16());
                    oneBitSamples[sampleId] = sample;
                }

                bitArray.Set(i, ReadBit(oneBitSamples, zerosAverage, onesAverage));
            }

            return bitArray;
        }

        public bool ReadBit(short[] oneBitSamples, short zerosAverage, short onesAverage) {
            short localAverage = (short)oneBitSamples.Average(e => e);
            return Math.Abs(localAverage - onesAverage) < Math.Abs(localAverage - zerosAverage);

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

        public static NzrDemodulator Get_1_1000_BitDurationDemodulator() {
            var demodulator = new NzrDemodulator(
                sampleRate: SampleRateType._32000,
                bitDuration: 1.0 / 1000
            );

            return demodulator;
        }

        public static NzrDemodulator Get_1_3200_BitDurationDemodulator() {
            var demodulator = new NzrDemodulator(
                sampleRate: SampleRateType._32000,
                bitDuration: 1.0 / 3200
            );

            return demodulator;
        }

        public static NzrDemodulator Get_1_6400_BitDurationDemodulator() {
            var demodulator = new NzrDemodulator(
                sampleRate: SampleRateType._32000,
                bitDuration: 1.0 / 6400
            );

            return demodulator;
        }

        public static NzrDemodulator Get_1_6300_BitDurationDemodulator() {
            var demodulator = new NzrDemodulator(
                sampleRate: SampleRateType._44100,
                bitDuration: 1.0 / 6300
            );

            return demodulator;
        }

        public static NzrDemodulator Get_1_8820_BitDurationDemodulator() {
            var demodulator = new NzrDemodulator(
                sampleRate: SampleRateType._44100,
                bitDuration: 1.0 / 8820
            );

            return demodulator;
        }

        public static NzrDemodulator Get_1_9600_BitDurationDemodulator() {
            var demodulator = new NzrDemodulator(
                sampleRate: SampleRateType._96000,
                bitDuration: 1.0 / 9600
            );

            return demodulator;
        }

        public static NzrDemodulator GetCustomBitDurationDemodulator(SampleRateType sampleRate, double bitDuration) {
            var demodulator = new NzrDemodulator(
                sampleRate: sampleRate,
                bitDuration: bitDuration
            );

            return demodulator;
        }
    }
}
