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
                cutoffFrequency: demodulator.CarrierFrequency / 2.0,
                sampleRate: demodulator.SampleRate
            );

            //   samples = hpFilter.FilterSamples(samples);

            samples = new Normalizer().PeakNormalize(samples);

            //   samples = new PhaseFixer().FixPhase(samples, (int)demodulator.SampleRate, 2);

            //

            var soundBytes = samples.ToByteArray();
            var processedSound = new WavFile(wavFile.SampleRate, soundBytes);

            using (FileStream fs = new FileStream($"PROCESSED.wav", FileMode.Create)) {
                BinaryWriter bw = new BinaryWriter(fs);
                bw.Write(processedSound.ToBytes());
            }

            //

            var waveDecomposer = new WaveDecomposer(demodulator);

            var dataSegments = waveDecomposer.FindDataSegments(samples);
            BitArray data = null;

            foreach (var segment in dataSegments) {

                var bits = demodulator.ReadBits(segment.DataSamples, segment.ZerosValue, segment.OnesValue, segment.BytesToRead);

                if (data == null)
                    data = new BitArray(bits);
                else
                    data = data.MergeWith(bits);
            }

            return data;
        }

        public static int CalculateLengthInSamples(int bitsCount, double bitDuration, int sampleRate) {
            var length = bitsCount * bitDuration * sampleRate;
            return (int)Math.Ceiling(length);
        }
    }
}
