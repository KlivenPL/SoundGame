using SoundEncoderDecoder.Helpers;
using System;
using System.Linq;

namespace SoundEncoderDecoder.AudioProcessing {
    public class Trimmer {
        public short[] Trim(short[] samples, short minAbsValue = short.MaxValue / 10) {
            var query = samples
                .Select((value, position) => new { value, position })
                .Where(x => Math.Abs((int)x.value) > minAbsValue)
                .Select(x => x.position);

            var firstPos = query.FirstOrDefault();
            var lastPos = query.LastOrDefault();

            var count = (lastPos - firstPos);

            if (count <= 0)
                return null;

            return samples.DeepCopy(firstPos, count);
        }
    }
}
