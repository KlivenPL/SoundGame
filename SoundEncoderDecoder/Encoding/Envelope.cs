using SoundEncoderDecoder.Helpers;
using System;
using System.Collections;
using System.Linq;

namespace SoundEncoderDecoder.Encoding {
    public class Envelope {
        public static BitArray EnclosingSequence => BitArrayHelper.FromString("1111111100110011 0101010101010101 1111111100110011 0101010101010101 1010101010101010 1111000011111111");
        public byte[] Data { get; }
        public int DataLength => Data.Length;
        public DataType DataType => (DataType)Data.Last();

        /// <summary>
        /// Use only to decompose already encoded envelope
        /// </summary>
        /// <param name="data">Data with all necessary envelope data</param>
        public Envelope(byte[] data) {
            Data = data;
        }

        public Envelope(byte[] data, DataType dataType) {
            Data = data.Concat(new[] { (byte)0, (byte)dataType }).ToArray();
        }

        public BitArray ToBitArray() {
            var bitArray = EnclosingSequence
                .MergeWith(new BitArray(BitConverter.GetBytes(DataLength)))
                .MergeWith(new BitArray(Data))
                .MergeWith(EnclosingSequence);

            return bitArray;
        }
    }
}
