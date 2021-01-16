using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;

namespace SoundEncoderDecoder {
    class Tests {

        const float cNote = 261.6256f;
        const float dNote = 293.6648f;
        const float eNote = 329.6276f;
        const float fNote = 349.2282f;
        const float gNote = 391.9954f;

        const float carrierFreq = 1000;

        public Tests() {
            //StaticTest();
            ReadTest();
        }

        void WriteHeader(int samples, MemoryStream ms) {
            int bytesPerSample = 2;

            var chunkId = "RIFF".ToCharArray();
            int chunkSize = 36 + bytesPerSample * samples;
            var format = "WAVE".ToCharArray();

            var subchunk1Id = "fmt ".ToCharArray();
            int subchunk1Size = 16;
            short audioFormat = 1;
            short numChannels = 1;
            int sampleRate = 8000;
            int byteRate = sampleRate * numChannels * bytesPerSample;
            short blockAlign = (short)(numChannels * bytesPerSample);
            short bitsPerSample = (short)(bytesPerSample * 8);

            char[] subchunk2Id = "data".ToCharArray();
            int subchunk2Size = samples * numChannels * bytesPerSample;


            byte[] header;

            BinaryWriter bw = new BinaryWriter(ms);

            bw.Write(chunkId);
            bw.Write(chunkSize);
            bw.Write(format);

            bw.Write(subchunk1Id);
            bw.Write(subchunk1Size);
            bw.Write(audioFormat);
            bw.Write(numChannels);
            bw.Write(sampleRate);
            bw.Write(byteRate);
            bw.Write(blockAlign);
            bw.Write(bitsPerSample);

            bw.Write(subchunk2Id);
            bw.Write(subchunk2Size);

        }

        short GenerateWaveSample(float frequency, float t) {
            var wave = (short)(short.MaxValue * Math.Sin(2 * Math.PI * frequency * t));
            return wave;
        }

        void WriteTimeOf(Func<float, float, short> soundFunc, float frequency, float duration, int sampleRate, MemoryStream ms) {
            var totalSamples = duration * sampleRate;
            BinaryWriter bw = new BinaryWriter(ms);

            for (int sampleId = 0; sampleId < totalSamples; sampleId++) {
                short sample = soundFunc(frequency, sampleId / totalSamples * duration);
                bw.Write(sample);
            }
        }

        short WriteZero(float frequency, float t) {
            //return (short)(0.1f * GenerateWaveSample(cNote, t));
            return 0;
        }

        short WriteOne(float frequency, float t) {
            return GenerateWaveSample(cNote, t);
        }

        void WriteSound(int samples, MemoryStream ms) {
            Random random = new Random();

            BinaryWriter bw = new BinaryWriter(ms);


            //WriteTimeOf(GenerateWaveSample, gNote, 0.5f, 0, 8000, ms);
            //WriteTimeOf(GenerateWaveSample, eNote, 0.5f, 0.5f, 8000, ms);
            //WriteTimeOf(GenerateWaveSample, eNote, 0.5f, 1f, 8000, ms);
            //WriteTimeOf(GenerateWaveSample, fNote, 0.5f, 1.5f, 8000, ms);
            //WriteTimeOf(GenerateWaveSample, dNote, 0.5f, 2f, 8000, ms);
            //WriteTimeOf(GenerateWaveSample, dNote, 0.5f, 2.5f, 8000, ms);
            //WriteTimeOf(GenerateWaveSample, cNote, 0.5f, 3f, 8000, ms);
            //WriteTimeOf(GenerateWaveSample, eNote, 0.5f, 3.5f, 8000, ms);
            //WriteTimeOf(GenerateWaveSample, gNote, 0.5f, 4f, 8000, ms);

            //bool write1 = false;
            //float bitDuration = 0.005f;

            //for (float i = 0; i < 0.5f / bitDuration; i += bitDuration) {
            //    if (write1) {
            //        WriteTimeOf(WriteOne, freq1, bitDuration, 8000, ms);

            //    } else {
            //        WriteTimeOf(WriteZero, freq1, bitDuration, 8000, ms);
            //    }
            //    write1 = !write1;
            //}

            WriteBytes(0.005f, ms);

        }

        void WriteBytes(float bitDuration, MemoryStream ms) {
            var bytes = ToBits(Encoding.ASCII.GetBytes("Wlazl kotek na plotek"));

            foreach (var byt in bytes) {
                if (byt == 1) {
                    WriteTimeOf(WriteOne, carrierFreq, bitDuration, 8000, ms);
                } else {

                    WriteTimeOf(WriteZero, carrierFreq, bitDuration, 8000, ms);
                }
            }
        }



        void StaticTest() {
            using (MemoryStream ms = new MemoryStream()) {
                WriteHeader((int)(8000 * 168 * 0.005f), ms);
                WriteSound((int)(8000 * 168 * 0.005f), ms);

                using (FileStream fs = new FileStream("noise.wav", FileMode.Create)) {
                    fs.Write(ms.ToArray());
                }
            }
        }

        byte[] ToBits(byte[] bytes) {
            byte[] res;
            using (MemoryStream ms = new MemoryStream()) {
                BinaryWriter br = new BinaryWriter(ms);

                BitArray bitArray = new BitArray(bytes);

                foreach (bool bit in bitArray) {
                    br.Write((byte)(bit ? 1 : 0));
                }

                res = ms.ToArray();
            }

            return res;
        }

        byte[] ToBytes(byte[] bits) {
            byte[] res;

            using (MemoryStream ms = new MemoryStream()) {
                BinaryWriter br = new BinaryWriter(ms);

                MemoryStream msw = new MemoryStream(bits);
                BinaryReader bw = new BinaryReader(msw);

                for (int i = 0; i < bits.Length / 8; i++) {
                    string bin = new string(string.Join("", bw.ReadBytes(8)).Reverse().ToArray());
                    var length = bin.Length;
                    var @byte = Convert.ToByte(bin, 2);
                    br.Write(@byte);
                }

                res = ms.ToArray();
            }
            return res;
        }

        void ReadTest() {
            var WavFile = new WavFile(new FileInfo("noise.wav"));

            float bitDuration = 0.005f;
            int samplesPerBit = (int)(bitDuration * WavFile.SampleRate);
            var half = (short)(short.MaxValue / 2f + 0.1 * short.MaxValue);

            using (MemoryStream dems = new MemoryStream()) {
                BinaryWriter debw = new BinaryWriter(dems);

                using (MemoryStream ms = new MemoryStream(WavFile.Data)) {
                    BinaryReader br = new BinaryReader(ms);

                    var sampleCount = WavFile.Subchunk2Size / (WavFile.BitsPerSample / 8) / samplesPerBit;

                    for (int i = 0; i < sampleCount; i++) {

                        short peak = 0;

                        for (int bitSample = 0; bitSample < samplesPerBit; bitSample++) {
                            var sample = (short)Math.Abs((int)br.ReadInt16());

                            if (sample > peak) {
                                peak = sample;
                            }
                        }

                        if (peak > half) {
                            debw.Write((byte)1);
                        } else {
                            debw.Write((byte)0);
                        }
                    }
                }

                var bits = dems.ToArray();

                var result = ToBytes(bits);

                var resultText = System.Text.Encoding.ASCII.GetString(result);
            }
        }
    }
}
