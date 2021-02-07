using SoundEncoderDecoder.Encoding;
using SoundEncoderDecoder.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SoundEncoderDecoder.Modulation {
    public class WaveDecomposer {
        public IDemodulator Demodulator { get; }
        public double BitDuration => Demodulator.BitDuration;
        public int SampleRate => (int)Demodulator.SampleRate;

        public WaveDecomposer(IDemodulator demodulator) {
            Demodulator = demodulator;
        }

        public IEnumerable<DataSegment> FindDataSegments(short[] samples) {
            List<DataSegment> dataSegments = new List<DataSegment>();
            var headerLengthInSamples = Wave.CalculateLengthInSamples(Envelope.EnclosingSequence.Length - 16, BitDuration, SampleRate);
            var headerSequence = Envelope.EnclosingSequence.Skip(16);
            var footerSequence = Envelope.EnclosingSequence.SkipLast(16);

            for (int i = 0; i < samples.Length; i++) {
                var samplePosition = i;

                if (CheckIfHeaderFound(samples, headerLengthInSamples, headerSequence, samplePosition, out double onesTestValue, out double zerosTestValue)) {

                    var dataLengthInBytes = GetDataLength(samples, headerLengthInSamples, samplePosition, onesTestValue, zerosTestValue, out int headerSkip, out int intSizeInSamples);

                    if (dataLengthInBytes == null)
                        continue;

                    var dataLengthInSamples = Wave.CalculateLengthInSamples(dataLengthInBytes.Value * 8, BitDuration, SampleRate);

                    if (CheckIfFooterFound(samples, headerLengthInSamples, footerSequence, onesTestValue, zerosTestValue, headerSkip, intSizeInSamples, dataLengthInSamples, out short[] enclosingHeaderSamples)) {

                        var headerAndIntSkip = headerSkip + intSizeInSamples;
                        var dataSamples = samples.DeepCopy(headerAndIntSkip, dataLengthInSamples);

                        dataSegments.Add(new DataSegment {
                            DataSamples = dataSamples,
                            OnesValue = onesTestValue,
                            ZerosValue = zerosTestValue,
                            BytesToRead = dataLengthInBytes.Value
                        });

                        i = headerAndIntSkip + dataLengthInSamples;
                    }
                }
            }
            return dataSegments;
        }

        private bool CheckIfFooterFound(short[] samples, int headerLengthInSamples, BitArray footerSequence, double onesTestValue, double zerosTestValue, int headerSkip, int intSizeInSamples, int dataLengthInSamples, out short[] enclosingHeaderSamples) {
            var headerAndDataSkip = headerSkip + intSizeInSamples + dataLengthInSamples;
            enclosingHeaderSamples = samples.DeepCopy(headerAndDataSkip, headerLengthInSamples);
            BitArray enclosingHeaderBa = null;

            if (enclosingHeaderSamples != null) {
                enclosingHeaderBa = Demodulator.ReadBits(enclosingHeaderSamples, zerosTestValue, onesTestValue, footerSequence.Count / 8);
            }

            var compareToFooter = enclosingHeaderBa?.EqualTo(footerSequence);
            return compareToFooter == true;
        }

        private int? GetDataLength(short[] samples, int headerLengthInSamples, int samplePosition, double onesTestValue, double zerosTestValue, out int headerSkip, out int intSizeInSamples) {
            headerSkip = samplePosition + headerLengthInSamples;
            intSizeInSamples = Wave.CalculateLengthInSamples(sizeof(int) * 8, BitDuration, SampleRate);
            var dataLengthSamples = samples.DeepCopy(headerSkip, intSizeInSamples);

            BitArray dataLengthBa = Demodulator.ReadBits(dataLengthSamples, zerosTestValue, onesTestValue, sizeof(int));

            if (dataLengthBa == null)
                return null;

            var dataLengthInBytes = BitConverter.ToInt32(dataLengthBa.ToBytes());
            return dataLengthInBytes;
        }

        private bool CheckIfHeaderFound(short[] samples, int headerLengthInSamples, BitArray headerSequence, int samplePosition, out double onesTestValue, out double zerosTestValue) {
            var headerSamples = samples.DeepCopy(samplePosition, headerLengthInSamples);

            (onesTestValue, zerosTestValue) = CalculateRMSes(headerSequence, headerSamples);

            BitArray headerBa = Demodulator.ReadBits(headerSamples, zerosTestValue, onesTestValue, headerSequence.Count / 8);
            var compareToHeader = headerBa?.EqualTo(headerSequence);

            if (compareToHeader == true) {
                /*                Console.WriteLine(headerSequence.ToBitString());
                                Console.WriteLine(headerBa.ToBitString());
                                Console.WriteLine(headerBa.CompareTo(headerSequence));
                                Console.WriteLine();*/
            }

            return compareToHeader == true;
        }

        private (double onesTestValue, double zerosTestValues) CalculateRMSes(BitArray header, short[] headerSamples) {
            var headerOnesCount = header.ToBitString().Count(c => c == '1');
            var headerZerosCount = header.ToBitString().Count(c => c == '0');

            var bitLengthInSamples = Wave.CalculateLengthInSamples(1, BitDuration, SampleRate);
            double onesTestValue = 0, zerosTestValues = 0;

            for (int j = 0; j < header.Length; j++) {
                var headerBitSamples = headerSamples.DeepCopy(j * bitLengthInSamples, bitLengthInSamples);

                if (headerBitSamples == null)
                    continue;

                var headerBitTestValue = Demodulator.GetTestValue(headerBitSamples, header[j]);

                if (header[j])
                    onesTestValue += headerBitTestValue;
                else
                    zerosTestValues += headerBitTestValue;
            }

            onesTestValue /= headerOnesCount;
            zerosTestValues /= headerZerosCount;

            return (onesTestValue, zerosTestValues);
        }
    }
}
