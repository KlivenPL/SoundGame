using SoundEncoderDecoder.Encoding;
using SoundEncoderDecoder.Helpers;
using SoundEncoderDecoder.Modulation;
using SoundEncoderDecoder.WavFormat;
using System.Collections;
using System.IO;
using System.Linq;

namespace SoundEncoderDecoder {
    class Tests {

        public Tests() {

            var bytes = new byte[1024];

            for (int i = 0; i < bytes.Length; i++) {
                bytes[i] = (byte)((float)i / bytes.Length * bytes.Length);
            }

            //GenerateTestFiles(bytes);
            // DecodeTestBytes(bytes);
            var modulator = NzrModulator.Get_1_4410_BitDurationModulator();

            var envelope = Encoder.Encode(bytes);
            var sound = Wave.Compose(modulator, modulator.SampleRate, envelope.ToBitArray());
            using (FileStream fs = new FileStream($"NONFILTER.wav", FileMode.Create)) {
                BinaryWriter bw = new BinaryWriter(fs);
                bw.Write(sound.ToBytes());
            }

            sound = new WavFile(new FileInfo("NONFILTER.wav"));

            Envelope decodedEnvelope = null;
            NzrDemodulator demodulator;
            // WavFile sound;
            BitArray bitArray;

            demodulator = NzrDemodulator.Get_1_4410_BitDurationDemodulator();
            bitArray = Wave.Decompose(demodulator, sound);
            if (bitArray != null) {

                decodedEnvelope = Encoder.Decode(bitArray);
                System.Console.WriteLine("1000");
                System.Console.WriteLine(check(bytes, decodedEnvelope.Data.Take(decodedEnvelope.Data.Length - 2).ToArray()));
            }

            System.Console.WriteLine();

            /*
                        HighPassFilter hpf = new HighPassFilter {
                            CutoffFrequency = 2000,
                            SampleRate = SampleRateType._44100
                        };

                        short[] shorts = new short[(int)Math.Ceiling(sound.Data.Length / 2.0)];
                        Buffer.BlockCopy(sound.Data, 0, shorts, 0, sound.Data.Length);

                        var xd = hpf.FilterSamples(shorts);
                        xd = hpf.FilterSamples(xd);
                        xd = hpf.FilterSamples(xd);

                        xd = new Normalizer().PeakNormalize(xd);

                        byte[] filtered = new byte[(int)Math.Ceiling(xd.Length * 2.0)];
                        Buffer.BlockCopy(xd, 0, filtered, 0, xd.Length * 2);

                        var filteredWav = new WavFile((int)SampleRateType._44100, filtered);


                        using (FileStream fs = new FileStream($"HPFILTER{modulator.CarrierFrequency}.wav", FileMode.Create)) {
                            BinaryWriter bw = new BinaryWriter(fs);
                            bw.Write(filteredWav.ToBytes());
                        }*/
        }

        void GenerateTestFiles(byte[] bytes) {
            var envelope = Encoder.Encode(bytes);
            var modulator = NzrModulator.Get_1_1000_BitDurationModulatorFREQ();
            var sound = Wave.Compose(modulator, modulator.SampleRate, envelope.ToBitArray());
            using (FileStream fs = new FileStream($"100bSINtest_{modulator.CarrierFrequency}.wav", FileMode.Create)) {
                BinaryWriter bw = new BinaryWriter(fs);
                bw.Write(sound.ToBytes());
            }

            return;

            modulator = NzrModulator.Get_1_3200_BitDurationModulator();
            sound = Wave.Compose(modulator, modulator.SampleRate, envelope.ToBitArray());
            using (FileStream fs = new FileStream($"10kbtest_{modulator.CarrierFrequency}.wav", FileMode.Create)) {
                BinaryWriter bw = new BinaryWriter(fs);
                bw.Write(sound.ToBytes());
            }

            modulator = NzrModulator.Get_1_6300_BitDurationModulator();
            sound = Wave.Compose(modulator, modulator.SampleRate, envelope.ToBitArray());
            using (FileStream fs = new FileStream($"10kbtest_{modulator.CarrierFrequency}.wav", FileMode.Create)) {
                BinaryWriter bw = new BinaryWriter(fs);
                bw.Write(sound.ToBytes());
            }

            modulator = NzrModulator.Get_1_6400_BitDurationModulator();
            sound = Wave.Compose(modulator, modulator.SampleRate, envelope.ToBitArray());
            using (FileStream fs = new FileStream($"10kbtest_{modulator.CarrierFrequency}.wav", FileMode.Create)) {
                BinaryWriter bw = new BinaryWriter(fs);
                bw.Write(sound.ToBytes());
            }

            modulator = NzrModulator.Get_1_8820_BitDurationModulator();
            sound = Wave.Compose(modulator, modulator.SampleRate, envelope.ToBitArray());
            using (FileStream fs = new FileStream($"10kbtest_{modulator.CarrierFrequency}.wav", FileMode.Create)) {
                BinaryWriter bw = new BinaryWriter(fs);
                bw.Write(sound.ToBytes());
            }

            modulator = NzrModulator.Get_1_9600_BitDurationModulator();
            sound = Wave.Compose(modulator, modulator.SampleRate, envelope.ToBitArray());
            using (FileStream fs = new FileStream($"10kbtest_{modulator.CarrierFrequency}.wav", FileMode.Create)) {
                BinaryWriter bw = new BinaryWriter(fs);
                bw.Write(sound.ToBytes());
            }
        }

        void DecodeTestBytes(byte[] bytes) {
            Envelope decodedEnvelope = null;
            NzrDemodulator demodulator;
            // WavFile sound;
            BitArray bitArray;

            demodulator = NzrDemodulator.GetCustomBitDurationDemodulator(800, SampleRateType._8000, 0.01);
            var sound = new WavFile(new FileInfo("100bSINtest_800.wav"));
            bitArray = Wave.Decompose(demodulator, sound);
            if (bitArray != null) {

                decodedEnvelope = Encoder.Decode(bitArray);
                System.Console.WriteLine("1000");
                System.Console.WriteLine(check(bytes, decodedEnvelope.Data.Take(decodedEnvelope.Data.Length - 2).ToArray()));
            }

            System.Console.WriteLine();

            return;

            demodulator = NzrDemodulator.Get_1_3200_BitDurationDemodulator();
            sound = new WavFile(new FileInfo("10kbtest_3200.wav"));
            bitArray = Wave.Decompose(demodulator, sound);
            if (bitArray != null) {

                decodedEnvelope = Encoder.Decode(bitArray);
                System.Console.WriteLine("3200");
                System.Console.WriteLine(check(bytes, decodedEnvelope.Data.Take(decodedEnvelope.Data.Length - 2).ToArray()));
            }

            return;
            System.Console.WriteLine();

            demodulator = NzrDemodulator.Get_1_6300_BitDurationDemodulator();
            sound = new WavFile(new FileInfo("10kbtest_6300.wav"));
            bitArray = Wave.Decompose(demodulator, sound);
            if (bitArray != null) {

                decodedEnvelope = Encoder.Decode(bitArray);
                System.Console.WriteLine("6300");
                System.Console.WriteLine(check(bytes, decodedEnvelope.Data.Take(decodedEnvelope.Data.Length - 2).ToArray()));
            }

            System.Console.WriteLine();

            demodulator = NzrDemodulator.Get_1_6400_BitDurationDemodulator();
            sound = new WavFile(new FileInfo("10kbtest_6400.wav"));
            bitArray = Wave.Decompose(demodulator, sound);
            if (bitArray != null) {

                decodedEnvelope = Encoder.Decode(bitArray);
                System.Console.WriteLine("6400");
                System.Console.WriteLine(check(bytes, decodedEnvelope.Data.Take(decodedEnvelope.Data.Length - 2).ToArray()));
            }

            System.Console.WriteLine();

            demodulator = NzrDemodulator.Get_1_8820_BitDurationDemodulator();
            sound = new WavFile(new FileInfo("10kbtest_8820.wav"));
            bitArray = Wave.Decompose(demodulator, sound);
            if (bitArray != null) {

                decodedEnvelope = Encoder.Decode(bitArray);
                System.Console.WriteLine("8820");
                System.Console.WriteLine(check(bytes, decodedEnvelope.Data.Take(decodedEnvelope.Data.Length - 2).ToArray()));
            }

            System.Console.WriteLine();

            demodulator = NzrDemodulator.Get_1_9600_BitDurationDemodulator();
            sound = new WavFile(new FileInfo("10kbtest_9600.wav"));
            bitArray = Wave.Decompose(demodulator, sound);
            if (bitArray != null) {

                decodedEnvelope = Encoder.Decode(bitArray);
                System.Console.WriteLine("9600");
                System.Console.WriteLine(check(bytes, decodedEnvelope.Data.Take(decodedEnvelope.Data.Length - 2).ToArray()));
            }

            System.Console.WriteLine();
        }

        bool check(byte[] bytesOrig, byte[] bytes) {
            if (bytesOrig.Length != bytes.Length) {
                return false;
            }
            bool success = true;
            int erorrsCount = 0;
            for (int i = 0; i < bytesOrig.Length; i++) {
                if (bytesOrig[i] != bytes[i]) {
                    System.Console.WriteLine($"POS: {i}\r\n\tORIG: {new BitArray(new[] { bytesOrig[i] }).ToBitString()} ({bytesOrig[i]})\r\n\tREAD: {new BitArray(new[] { bytes[i] }).ToBitString()} ({bytes[i]})\r\n");
                    success = false;
                    erorrsCount++;
                }
            }

            System.Console.WriteLine($"Faults: {erorrsCount}");
            System.Console.WriteLine($"Success rate: {1.0 - ((float)erorrsCount / bytesOrig.Length)}");

            //System.Console.WriteLine("SucessRate: " + new );

            return success;
        }
    }
}
