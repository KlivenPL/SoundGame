using SoundEncoderDecoder.Encoding;
using SoundEncoderDecoder.Helpers;
using System;
using System.Collections;
using System.Linq;

namespace SoundEncoderDecoder.Modulation {
    public class WaveDecomposer {
        public IDemodulator Demodulator { get; }
        public double BitDuration => Demodulator.BitDuration;
        public int SampleRate => (int)Demodulator.SampleRate;

        public WaveDecomposer(IDemodulator demodulator) {
            Demodulator = demodulator;
        }

        public short[] FindDataSegments(short[] samples, out double zerosRMS, out double onesRMS, out int bytesToRead) {
            var headerLengthInSamples = Wave.CalculateLengthInSamples(Envelope.EnclosingSequence.Length - 16, BitDuration, SampleRate);
            var headerSequence = Envelope.EnclosingSequence.Skip(16);
            var footerSequence = Envelope.EnclosingSequence.SkipLast(16);
            //var sineOffset = (int)((double)SampleRate / (4.0 * demodulator.CarrierFrequency));

            for (int i = 0; i < samples.Length; i++) {
                var sample = samples[i];
                if (sample < 0)
                    continue;

                /* try {*/
                var samplePosition = i;
                var headerSamples = samples.DeepCopy(samplePosition, headerLengthInSamples);

                (onesRMS, zerosRMS) = CalculateRMSes(headerSequence, headerSamples);

                // if (onesRMS < zerosRMS)
                //    continue;

                BitArray headerBa = Demodulator.ReadBits(headerSamples, zerosRMS, onesRMS, headerSequence.Count / 8);

                Console.WriteLine(headerSequence.ToBitString());
                Console.WriteLine(headerBa.ToBitString());
                Console.WriteLine(headerBa.CompareTo(headerSequence));
                Console.WriteLine();

                var compareToHeader = headerBa?.CompareTo(headerSequence);
                if (compareToHeader >= 0.9) {


                    // getting real data length (with envelope data)

                    var headerSkip = samplePosition + headerLengthInSamples;
                    var intSizeInSamples = Wave.CalculateLengthInSamples(sizeof(int) * 8, BitDuration, SampleRate);
                    var dataLengthSamples = samples.DeepCopy(headerSkip, intSizeInSamples);

                    BitArray dataLengthBa = Demodulator.ReadBits(dataLengthSamples, zerosRMS, onesRMS, sizeof(int));

                    var dataLengthInBytes = BitConverter.ToInt32(dataLengthBa.ToBytes());

                    if (dataLengthInBytes < 0)
                        continue;

                    var dataLengthInSamples = Wave.CalculateLengthInSamples(dataLengthInBytes * 8, BitDuration, SampleRate);

                    // checking if enclosing header exists

                    var headerAndDataSkip = headerSkip + intSizeInSamples + dataLengthInSamples;
                    var enclosingHeaderSamples = samples.DeepCopy(headerAndDataSkip, headerLengthInSamples);
                    BitArray enclosingHeaderBa = null;
                    if (enclosingHeaderSamples != null) {
                        enclosingHeaderBa = Demodulator.ReadBits(enclosingHeaderSamples, zerosRMS, onesRMS, footerSequence.Count / 8);
                    }

                    var compareToFooter = enclosingHeaderBa?.CompareTo(footerSequence);
                    if (compareToFooter >= 0.9) {
                        (double onesRMS2, double zerosRMS2) = CalculateRMSes(footerSequence, enclosingHeaderSamples);

                        onesRMS = (onesRMS + onesRMS2) / 2.0;
                        zerosRMS = (zerosRMS + zerosRMS2) / 2.0;

                        var headerAndIntSkip = headerSkip + intSizeInSamples;
                        var dataBytes = samples.DeepCopy(headerAndIntSkip, dataLengthInSamples);

                        bytesToRead = dataLengthInBytes;
                        return dataBytes;
                    }
                }
                /*} catch {

                }*/
            }
            bytesToRead = 0;
            zerosRMS = 0;
            onesRMS = 0;
            return null;
        }

        private (double onesRMS, double zerosRMS) CalculateRMSes(BitArray header, short[] headerSamples) {
            var headerOnesCount = header.ToBitString().Count(c => c == '1');
            var headerZerosCount = header.ToBitString().Count(c => c == '0');

            var bitLengthInSamples = Wave.CalculateLengthInSamples(1, BitDuration, SampleRate);
            double onesRMS = 0, zerosRMS = 0;

            for (int j = 0; j < header.Length; j++) {
                var headerBitSamples = headerSamples.DeepCopy(j * bitLengthInSamples, bitLengthInSamples);

                if (headerBitSamples == null)
                    continue;

                // getting only middle samples
                var skip = (int)(headerBitSamples.Length / 4.0);
                headerBitSamples = headerBitSamples.DeepCopy(/*skip, headerBitSamples.Length - skip*/);

                var headerBitRMS = Wave.RMS(headerBitSamples, header[j]);

                if (header[j])
                    onesRMS += headerBitRMS;
                else
                    zerosRMS += headerBitRMS;
            }

            onesRMS = onesRMS / headerOnesCount;
            zerosRMS = zerosRMS / headerZerosCount;

            return (onesRMS, zerosRMS);
        }
    }
}
