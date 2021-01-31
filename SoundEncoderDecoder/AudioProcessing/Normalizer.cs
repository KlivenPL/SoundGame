using System.Linq;

namespace SoundEncoderDecoder.AudioProcessing {
    public class Normalizer {
        public short[] PeakNormalize(short[] samples) {
            var peak = samples.Max();
            double a = short.MaxValue / (double)peak;
            var normalized = samples.Select(s => (short)(a * s)).ToArray();
            return normalized;
        }
    }
}
