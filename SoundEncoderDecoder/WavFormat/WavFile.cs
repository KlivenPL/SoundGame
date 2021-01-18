using System;
using System.IO;

namespace SoundEncoderDecoder.WavFormat {
    public class WavFile {

        public int Samples => Data.Length / BytesPerSample;
        public int BytesPerSample { get; set; } = 2;
        public char[] ChunkId { get; set; } = "RIFF".ToCharArray();
        public int ChunkSize { get; set; }
        public char[] Format { get; set; } = "WAVE".ToCharArray();
        public char[] Subchunk1Id { get; set; } = "fmt ".ToCharArray();
        public int Subchunk1Size { get; set; } = 16;
        public short AudioFormat { get; set; } = 1;
        public short NumChannels { get; set; } = 1;
        public int SampleRate { get; set; } = 8000;
        public int ByteRate { get; set; }
        public short BlockAlign { get; set; }
        public short BitsPerSample { get; set; }
        public char[] Subchunk2Id { get; set; } = "data".ToCharArray();
        public int Subchunk2Size { get; set; }
        public byte[] Data { get; set; }

        public WavFile(FileInfo fileInfo) : this(File.ReadAllBytes(fileInfo.FullName)) { }

        public WavFile(byte[] readBytes) {
            using MemoryStream ms = new MemoryStream(readBytes);
            BinaryReader br = new BinaryReader(ms);

            var chunkId = br.ReadChars(4);
            if (new string(chunkId) != new string(ChunkId)) {
                throw new NotSupportedException();
            }

            ChunkSize = br.ReadInt32();
            var format = br.ReadChars(4);
            var subchunk1Id = br.ReadChars(4);

            if (new string(format) != new string(Format) || new string(subchunk1Id) != new string(Subchunk1Id)) {
                throw new NotSupportedException();
            }

            Subchunk1Size = br.ReadInt32();
            AudioFormat = br.ReadInt16();
            NumChannels = br.ReadInt16();
            SampleRate = br.ReadInt32();
            ByteRate = br.ReadInt32();
            BlockAlign = br.ReadInt16();
            BitsPerSample = br.ReadInt16();

            var subchunk2Id = br.ReadChars(4);

            if (new string(subchunk2Id) != new string(Subchunk2Id)) {
                throw new NotSupportedException();
            }

            Subchunk2Size = br.ReadInt32();
            Data = br.ReadBytes(Subchunk2Size);
        }

        public WavFile(int sampleRate, byte[] data) {
            SampleRate = sampleRate;
            Data = data;

            ChunkSize = 36 + BytesPerSample * Samples;
            ByteRate = sampleRate * NumChannels * BytesPerSample;
            BlockAlign = (short)(NumChannels * BytesPerSample);
            BitsPerSample = (short)(BytesPerSample * 8);
            Subchunk2Size = Samples * NumChannels * BytesPerSample;
        }

        public byte[] ToBytes() {
            byte[] bytes;

            using (var ms = new MemoryStream()) {
                BinaryWriter bw = new BinaryWriter(ms);

                bw.Write(ChunkId);
                bw.Write(ChunkSize);
                bw.Write(Format);

                bw.Write(Subchunk1Id);
                bw.Write(Subchunk1Size);
                bw.Write(AudioFormat);
                bw.Write(NumChannels);
                bw.Write(SampleRate);
                bw.Write(ByteRate);
                bw.Write(BlockAlign);
                bw.Write(BitsPerSample);

                bw.Write(Subchunk2Id);
                bw.Write(Subchunk2Size);

                bw.Write(Data);

                bytes = ms.ToArray();
            }

            return bytes;
        }
    }
}
