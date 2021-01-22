using SoundEncoderDecoder.Encoding;
using SoundEncoderDecoder.Modulation;
using SoundEncoderDecoder.WavFormat;
using System.Collections;
using System.IO;
using System.Linq;

namespace SoundEncoderDecoder {
    class Tests {

        public Tests() {

            var bytes = new byte[100];

            for (int i = 0; i < bytes.Length; i++) {
                bytes[i] = (byte)((float)i / bytes.Length * 255);
            }

/*            var envelope = Encoder.Encode(bytes);
            var modulator = NzrModulator.GetCustomBitDurationModulator(SampleRateType._8000, 0.01, 800);
            var sound = Wave.Compose(modulator, modulator.SampleRate, envelope.ToBitArray());
            using (FileStream fs = new FileStream($"100bSINtest_{modulator.CarrierFrequency}.wav", FileMode.Create)) {
                BinaryWriter bw = new BinaryWriter(fs);
                bw.Write(sound.ToBytes());
            }*/

            //modulator = NzrModulator.Get_1_3200_BitDurationModulator();
            //sound = Wave.Compose(modulator, modulator.SampleRate, envelope.ToBitArray());
            //using (FileStream fs = new FileStream($"10kbtest_{modulator.CarrierFrequency}.wav", FileMode.Create)) {
            //    BinaryWriter bw = new BinaryWriter(fs);
            //    bw.Write(sound.ToBytes());
            //}

            //modulator = NzrModulator.Get_1_6300_BitDurationModulator();
            //sound = Wave.Compose(modulator, modulator.SampleRate, envelope.ToBitArray());
            //using (FileStream fs = new FileStream($"10kbtest_{modulator.CarrierFrequency}.wav", FileMode.Create)) {
            //    BinaryWriter bw = new BinaryWriter(fs);
            //    bw.Write(sound.ToBytes());
            //}

            //modulator = NzrModulator.Get_1_6400_BitDurationModulator();
            //sound = Wave.Compose(modulator, modulator.SampleRate, envelope.ToBitArray());
            //using (FileStream fs = new FileStream($"10kbtest_{modulator.CarrierFrequency}.wav", FileMode.Create)) {
            //    BinaryWriter bw = new BinaryWriter(fs);
            //    bw.Write(sound.ToBytes());
            //}

            //modulator = NzrModulator.Get_1_8820_BitDurationModulator();
            //sound = Wave.Compose(modulator, modulator.SampleRate, envelope.ToBitArray());
            //using (FileStream fs = new FileStream($"10kbtest_{modulator.CarrierFrequency}.wav", FileMode.Create)) {
            //    BinaryWriter bw = new BinaryWriter(fs);
            //    bw.Write(sound.ToBytes());
            //}

            //modulator = NzrModulator.Get_1_9600_BitDurationModulator();
            //sound = Wave.Compose(modulator, modulator.SampleRate, envelope.ToBitArray());
            //using (FileStream fs = new FileStream($"10kbtest_{modulator.CarrierFrequency}.wav", FileMode.Create)) {
            //    BinaryWriter bw = new BinaryWriter(fs);
            //    bw.Write(sound.ToBytes());
            //}
            Envelope decodedEnvelope = null;
            NzrDemodulator demodulator;
            // WavFile sound;
            BitArray bitArray;

            demodulator = NzrDemodulator.GetCustomBitDurationDemodulator(SampleRateType._8000, 0.01);
            var sound = new WavFile(new FileInfo("100bSINtest_800.wav"));
            bitArray = Wave.Decompose(demodulator, sound);
            if (bitArray != null) {

                decodedEnvelope = Encoder.Decode(bitArray);
                System.Console.WriteLine("1000");
                System.Console.WriteLine(check(bytes, decodedEnvelope.Data.Take(decodedEnvelope.Data.Length - 1).ToArray()));
            }

            System.Console.WriteLine();

            return;

            demodulator = NzrDemodulator.Get_1_3200_BitDurationDemodulator();
            sound = new WavFile(new FileInfo("10kbtest_3200.wav"));
            bitArray = Wave.Decompose(demodulator, sound);
            if (bitArray != null) {

                decodedEnvelope = Encoder.Decode(bitArray);
                System.Console.WriteLine("3200");
                System.Console.WriteLine(check(bytes, decodedEnvelope.Data.Take(decodedEnvelope.Data.Length - 1).ToArray()));
            }

            return;
            System.Console.WriteLine();

            demodulator = NzrDemodulator.Get_1_6300_BitDurationDemodulator();
            sound = new WavFile(new FileInfo("10kbtest_6300.wav"));
            bitArray = Wave.Decompose(demodulator, sound);
            if (bitArray != null) {

                decodedEnvelope = Encoder.Decode(bitArray);
                System.Console.WriteLine("6300");
                System.Console.WriteLine(check(bytes, decodedEnvelope.Data.Take(decodedEnvelope.Data.Length - 1).ToArray()));
            }

            System.Console.WriteLine();

            demodulator = NzrDemodulator.Get_1_6400_BitDurationDemodulator();
            sound = new WavFile(new FileInfo("10kbtest_6400.wav"));
            bitArray = Wave.Decompose(demodulator, sound);
            if (bitArray != null) {

                decodedEnvelope = Encoder.Decode(bitArray);
                System.Console.WriteLine("6400");
                System.Console.WriteLine(check(bytes, decodedEnvelope.Data.Take(decodedEnvelope.Data.Length - 1).ToArray()));
            }

            System.Console.WriteLine();

            demodulator = NzrDemodulator.Get_1_8820_BitDurationDemodulator();
            sound = new WavFile(new FileInfo("10kbtest_8820.wav"));
            bitArray = Wave.Decompose(demodulator, sound);
            if (bitArray != null) {

                decodedEnvelope = Encoder.Decode(bitArray);
                System.Console.WriteLine("8820");
                System.Console.WriteLine(check(bytes, decodedEnvelope.Data.Take(decodedEnvelope.Data.Length - 1).ToArray()));
            }

            System.Console.WriteLine();

            demodulator = NzrDemodulator.Get_1_9600_BitDurationDemodulator();
            sound = new WavFile(new FileInfo("10kbtest_9600.wav"));
            bitArray = Wave.Decompose(demodulator, sound);
            if (bitArray != null) {

                decodedEnvelope = Encoder.Decode(bitArray);
                System.Console.WriteLine("9600");
                System.Console.WriteLine(check(bytes, decodedEnvelope.Data.Take(decodedEnvelope.Data.Length - 1).ToArray()));
            }

            System.Console.WriteLine();
        }

        bool check(byte[] bytesOrig, byte[] bytes) {
            if (bytesOrig.Length != bytes.Length) {
                return false;
            }

            for (int i = 0; i < bytesOrig.Length; i++) {
                if (bytesOrig[i] != bytes[i])
                    return false;
            }

            return true;
        }
    }
}
