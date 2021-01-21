using SoundEncoderDecoder.Helpers;
using System;
using System.Collections;
using System.Linq;

namespace SoundEncoderDecoder.Encoding {
    public class Envelope {
        public static BitArray EnclosingSequence => BitArrayHelper.FromString("1001 1010 0101 1100").MergeWith(new BitArray(new[] { 21, 37 }));
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
            Data = data.Concat(new[] { (byte)dataType }).ToArray();
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
