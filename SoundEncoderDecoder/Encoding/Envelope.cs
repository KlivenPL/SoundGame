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
        public const int PacketSize = 528;

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
            var bitArray = new BitArray(GetPacketedData());

            return bitArray;
        }

        private byte[] GetPacketedData() {
            byte[] data = null;
            for (int i = 0; i < DataLength; i += PacketSize) {
                if (data == null) {
                    data = EnclosingSequence.ToBytes();
                } else {
                    data = data.Concat(EnclosingSequence.ToBytes()).ToArray();
                }

                int take = PacketSize;
                if (take + i > DataLength)
                    take = DataLength - i;

                data = data.Concat(BitConverter.GetBytes(take)).ToArray();
                data = data.Concat(Data.DeepCopy(i, take)).ToArray();
            }
            data = data.Concat(EnclosingSequence.ToBytes()).ToArray();
            return data;
        }
    }
}
