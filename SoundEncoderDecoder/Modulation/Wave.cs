using SoundEncoderDecoder.Encoding;
using SoundEncoderDecoder.Helpers;
using SoundEncoderDecoder.WavFormat;
using System;
using System.Collections;
using System.IO;
using System.Linq;

namespace SoundEncoderDecoder.Modulation {
    public static class Wave {
        public static WavFile Compose(IModulator modulator, SampleRateType sampleRate, BitArray data) {
            using MemoryStream ms = new MemoryStream();

            foreach (bool bit in data) {
                if (bit) {
                    modulator.WriteOne(ms);
                } else {
                    modulator.WriteZero(ms);
                }
            }

            var wavFile = new WavFile((int)sampleRate, ms.ToArray());
            return wavFile;
        }

        public static BitArray Decompose(IDemodulator demodulator, WavFile wavFile) {
            using (MemoryStream ms = new MemoryStream(wavFile.Data)) {
                BinaryReader br = new BinaryReader(ms);
                var averageAbsAmplitude = FindAverageAbsAmp(wavFile, br, ms);
                ms.Position = 0;

                var dataSegment = FindDataSegment(demodulator, wavFile, br, ms, averageAbsAmplitude);

                if (dataSegment != null) {
                    return ReadBits(demodulator, averageAbsAmplitude, dataSegment);
                }
            }

            return null;
        }

        private static short FindAverageAbsAmp(WavFile file, BinaryReader br, MemoryStream ms) {
            short peak = short.MinValue, low = short.MaxValue;

            for (int i = 0; i < file.Samples; i++) {
                var sample = (short)Math.Abs((int)br.ReadInt16());
                if (sample < 0)
                    sample = 0;

                if (sample > peak) {
                    peak = sample;
                } else if (sample < low) {
                    low = sample;
                }
            }

            return (short)((peak + low) / 2f);
        }

        private static byte[] FindDataSegment(IDemodulator demodulator, WavFile wavFile, BinaryReader br, MemoryStream ms, short averageAbsAmplitude) {
            short peakValue = -1;
            int peakPosition = -1;

            var headerLengthInWavFileBytes = CalculateLengthInWavFileBytes(Envelope.EnclosingSequence.Length, demodulator.BitDuration, wavFile);

            for (int i = 0; i < wavFile.Samples; i++) {
                var sample = (short)Math.Abs((int)br.ReadInt16());
                if (sample > averageAbsAmplitude) {
                    if (sample > peakValue) {
                        peakValue = sample;
                        peakPosition = i;
                    }
                } else {
                    if (peakValue != -1) {

                        // checking if header exists
                        var peakPositionInWavFileBytes = peakPosition * wavFile.BytesPerSample;
                        var headerBytes = wavFile.Data.Skip(peakPositionInWavFileBytes).Take(headerLengthInWavFileBytes).ToArray();

                        BitArray headerBa = ReadBits(demodulator, averageAbsAmplitude, headerBytes);

                        if (headerBa.EqualTo(Envelope.EnclosingSequence)) {
                            // getting real data length (with envelope data)

                            var headerSkip = peakPositionInWavFileBytes + headerLengthInWavFileBytes;
                            var intSizeInWavFileBytes = CalculateLengthInWavFileBytes(sizeof(int) * 8, demodulator.BitDuration, wavFile);
                            var dataLengthBytes = wavFile.Data.Skip(headerSkip).Take(intSizeInWavFileBytes).ToArray();

                            BitArray dataLengthBa = ReadBits(demodulator, averageAbsAmplitude, dataLengthBytes);

                            var dataLengthInBytes = BitConverter.ToInt32(dataLengthBa.ToBytes());
                            var dataLengthInWavFileBytes = CalculateLengthInWavFileBytes(dataLengthInBytes * 8, demodulator.BitDuration, wavFile);

                            // checking if enclosing header exists

                            var headerAndDataSkip = headerSkip + intSizeInWavFileBytes + dataLengthInWavFileBytes;
                            var enclosingHeaderBytes = wavFile.Data.Skip(headerAndDataSkip).Take(headerLengthInWavFileBytes).ToArray();

                            BitArray enclosingHeaderBa = ReadBits(demodulator, averageAbsAmplitude, enclosingHeaderBytes);

                            if (enclosingHeaderBa.EqualTo(Envelope.EnclosingSequence)) {
                                var headerAndIntSkip = headerSkip + intSizeInWavFileBytes;
                                var dataBytes = wavFile.Data.Skip(headerAndIntSkip).Take(dataLengthInWavFileBytes).ToArray();

                                return dataBytes;
                            }
                        }

                        peakValue = -1;
                        peakPosition = -1;
                    }
                }
            }
            return null;
        }

        private static int CalculateLengthInWavFileBytes(int bitsCount, float bitDuration, WavFile file) {
            var length = bitsCount * bitDuration * file.SampleRate * file.BytesPerSample;
            return (int)Math.Ceiling(length);
        }

        private static BitArray ReadBits(IDemodulator demodulator, short averageAbsAmplitude, byte[] fileBytes) {
            BitArray headerBa = null;

            using (MemoryStream ms = new MemoryStream(fileBytes)) {
                BinaryReader headerBr = new BinaryReader(ms);
                headerBa = demodulator.ReadBits(headerBr, ms, averageAbsAmplitude);
            }

            return headerBa;
        }
    }
}
