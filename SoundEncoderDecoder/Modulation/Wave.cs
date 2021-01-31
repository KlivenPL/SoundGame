using SoundEncoderDecoder.AudioProcessing;
using SoundEncoderDecoder.Helpers;
using SoundEncoderDecoder.WavFormat;
using System;
using System.Collections;
using System.IO;

namespace SoundEncoderDecoder.Modulation {
    public static class Wave {
        public static WavFile Compose(IModulator modulator, SampleRateType sampleRate, BitArray data) {
            using MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);

            bw.Write(new byte[(int)(0.5 * 2 * (int)sampleRate)]);

            foreach (bool bit in data) {
                if (bit) {
                    modulator.WriteOne(ms);
                } else {
                    modulator.WriteZero(ms);
                }
            }

            bw.Write(new byte[(int)(0.5 * 2 * (int)sampleRate)]);

            var wavFile = new WavFile((int)sampleRate, ms.ToArray());
            return wavFile;
        }

        public static BitArray Decompose(IDemodulator demodulator, WavFile wavFile) {
            var samples = wavFile.Data.ToShortArray();
            samples = new Trimmer().Trim(samples);

            var hpFilter = new HighPassFilter(
                cutoffFrequency: demodulator.CarrierFrequency,
                sampleRate: demodulator.SampleRate
            );

            //    samples = hpFilter.FilterSamples(samples);

            samples = new Normalizer().PeakNormalize(samples);

            samples = new PhaseFixer().FixPhase(samples, (int)demodulator.SampleRate, 1);

            //

            var soundBytes = samples.ToByteArray();
            var processedSound = new WavFile(wavFile.SampleRate, soundBytes);

            using (FileStream fs = new FileStream($"PROCESSED.wav", FileMode.Create)) {
                BinaryWriter bw = new BinaryWriter(fs);
                bw.Write(processedSound.ToBytes());
            }

            //

            var waveDecomposer = new WaveDecomposer(demodulator);

            var data = waveDecomposer.FindDataSegments(samples, out double zerosRMS, out double onesRMS, out int bytesToRead);

            if (data != null) {
                return demodulator.ReadBits(data, zerosRMS, onesRMS, bytesToRead);
            }

            return null;
        }

        public static double RMS(short[] arr, bool forOne) {
            /*  double square = 0;
              double mean;

              var average = arr.Average(e => Math.Abs((double)e));
              var count = 0;

              for (int i = 0; i < arr.Length; i++) {
                  var abs = Math.Abs((double)arr[i]);

                  if (forOne && abs > average || !forOne && abs < average) {
                      square += Math.Pow(arr[i] / (double)short.MaxValue, 2);
                      count++;
                  }
              }

              mean = square / count;

              double root = Math.Sqrt(mean);

              return root;*/
            var modulator = new NzrModulator(3150, SampleRateType._44100, 1.0 / 3150);
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
                diffOnes += oneSamples[i] * arr[i];
                diffZeros += zeroSamples[i] * arr[i];

            }
            diffOnes /= arr.Length;
            diffZeros /= arr.Length;

            if (forOne)
                return diffOnes;

            return diffZeros;
        }

        /* public static double RMS(short[] arr) {
             /* double square = 0;
              double mean;

              for (int i = 0; i < arr.Length; i++) {
                  square += Math.Pow(arr[i] / (double)short.MaxValue, 2);
              }

              mean = square / arr.Length;

              double root = Math.Sqrt(mean);

              return root;*/
        /*   var ups = arr.Where(e => e >= 0).Sum(e => e / (double)short.MaxValue);
           var downs = arr.Where(e => e < 0).Sum(e => e / (double)short.MaxValue);
           return Math.Abs(ups - downs);

           var sum = 0.0;
           for (int i = 0; i < arr.Length; i++) {
               var val = arr[i] / (double)short.MaxValue;
               sum += val * val;
           }
           return sum / arr.Length;*/

        // }

        public static int CalculateLengthInSamples(int bitsCount, double bitDuration, int sampleRate) {
            var length = bitsCount * bitDuration * sampleRate;
            return (int)Math.Ceiling(length);
        }
    }
}
