using SoundEncoderDecoder.Helpers;
using System.Linq;

namespace SoundEncoderDecoder.AudioProcessing {
    public class PhaseFixer {
        public short[] FixPhase(short[] samples, int sampleRate, int skipsPerSecond) {
            short[] fixedSamples = null;

            for (int i = 0; i < samples.Length; i += sampleRate + skipsPerSecond) {
                int take = sampleRate;
                if (i + take > samples.Length) {
                    take = samples.Length - i;
                }
                if (fixedSamples == null)
                    fixedSamples = samples.DeepCopy(i, take);
                else
                    fixedSamples = fixedSamples.Concat(samples.DeepCopy(i, take)).ToArray();
            }

            return fixedSamples;
        }
    }
}
