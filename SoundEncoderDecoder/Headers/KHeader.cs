using SoundEncoderDecoder.Helpers;
using System;
using System.Collections;

namespace SoundEncoderDecoder.Headers {
    public class KHeader {
        public BitArray FormatSequence => new BitArray(new[] { true, true, false, true, false, true, true, false });
        public int DataLength { get; private set; }

        public KHeader(int dataLength) {
            DataLength = dataLength;
        }

        public BitArray ToBitArray() {
            return FormatSequence.MergeWith(new BitArray(BitConverter.GetBytes(DataLength))).MergeWith(FormatSequence);
        }
    }
}
