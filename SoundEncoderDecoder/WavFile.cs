using System;
using System.IO;

namespace SoundEncoderDecoder {
    public class WavFile {

        public int Samples { get; set; }
        public int BytesPerSample { get; set; }
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

        public WavFile(int samples, int bytesPerSample, char[] chunkId, int chunkSize, char[] format, char[] subchunk1Id, int subchunk1Size, short audioFormat, short numChannels, int sampleRate, int byteRate, short blockAlign, short bitsPerSample, char[] subchunk2Id, int subchunk2Size, byte[] data) {
            Samples = samples;
            BytesPerSample = bytesPerSample;
            ChunkId = chunkId;
            ChunkSize = chunkSize;
            Format = format;
            Subchunk1Id = subchunk1Id;
            Subchunk1Size = subchunk1Size;
            AudioFormat = audioFormat;
            NumChannels = numChannels;
            SampleRate = sampleRate;
            ByteRate = byteRate;
            BlockAlign = blockAlign;
            BitsPerSample = bitsPerSample;
            Subchunk2Id = subchunk2Id;
            Subchunk2Size = subchunk2Size;
            Data = data;
        }

        public WavFile(FileInfo fileInfo) {
            using (FileStream fs = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read)) {
                BinaryReader br = new BinaryReader(fs);

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
        }
    }
}
