using SoundEncoderDecoder.Encoding;
using SoundEncoderDecoder.Helpers;
using SoundEncoderDecoder.Modulation;
using SoundEncoderDecoder.WavFormat;
using System;
using System.Collections;
using System.IO;
using System.Linq;

namespace SoundEncoderDecoder {
    class Tests {

        public Tests() {

            var bytes = new byte[4096];

            for (int i = 0; i < bytes.Length; i++) {
                bytes[i] = (byte)((float)i / bytes.Length * bytes.Length);
            }

            var modulator = NzrModulator.Get_1_3150_BitDurationModulator();

            var envelope = Encoder.Encode(bytes);
            var sound = Wave.Compose(modulator, modulator.SampleRate, envelope.ToBitArray());

            using (FileStream fs = new FileStream($"NONFILTER.wav", FileMode.Create)) {
                BinaryWriter bw = new BinaryWriter(fs);
                bw.Write(sound.ToBytes());
            }

            sound = new WavFile(new FileInfo("NONFILTER.wav"));
            NzrDemodulator demodulator = NzrDemodulator.Get_1_3150_BitDurationDemodulator();

            BitArray bitArray = Wave.Decompose(demodulator, sound);
            if (bitArray != null) {

                Envelope decodedEnvelope = Encoder.Decode(bitArray);
                Console.WriteLine("1000");
                Console.WriteLine(Check(bytes, decodedEnvelope.Data.Take(decodedEnvelope.Data.Length - 2).ToArray()));
            }

            Console.WriteLine();
        }

        bool Check(byte[] bytesOrig, byte[] bytes) {
            if (bytesOrig.Length != bytes.Length) {
                return false;
            }
            bool success = true;
            int erorrsCount = 0;
            for (int i = 0; i < bytesOrig.Length; i++) {
                if (bytesOrig[i] != bytes[i]) {
                    Console.WriteLine($"POS: {i} / {bytesOrig.Length - 1}\r\n\tORIG: {new BitArray(new[] { bytesOrig[i] }).ToBitString()} ({bytesOrig[i]})\r\n\tREAD: {new BitArray(new[] { bytes[i] }).ToBitString()} ({bytes[i]})\r\n");
                    success = false;
                    erorrsCount++;
                }
            }

            Console.WriteLine($"Faults: {erorrsCount}");
            Console.WriteLine($"Success rate: {1.0 - ((float)erorrsCount / bytesOrig.Length)}");

            return success;
        }
    }
}
