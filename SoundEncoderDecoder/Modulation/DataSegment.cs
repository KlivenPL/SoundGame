namespace SoundEncoderDecoder.Modulation {
    public class DataSegment {
        public double OnesValue { get; set; }
        public double ZerosValue { get; set; }
        public int BytesToRead { get; set; }
        public short[] DataSamples { get; set; }
    }
}
