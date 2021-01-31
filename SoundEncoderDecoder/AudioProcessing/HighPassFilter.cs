using SoundEncoderDecoder.WavFormat;
using System;

namespace SoundEncoderDecoder.AudioProcessing {
    public class HighPassFilter {

        /*        public double CutoffFrequency { get; set; }
                public SampleRateType SampleRate { get; set; }*/

        private HighpassFilterButterworthImplementation filter;

        public HighPassFilter(double cutoffFrequency, SampleRateType sampleRate) {
            filter = new HighpassFilterButterworthImplementation(cutoffFrequency, 1, (double)sampleRate);
        }

        public short[] FilterSamples(short[] samples) {
            short[] filtered = new short[samples.Length];
            for (int i = 0; i < samples.Length; i++) {
                filtered[i] = (short)filter.compute(samples[i]);
            }
            return filtered;
        }

        /* public short[] FilterSamples(short[] samples) {
             short[] filtered = new short[samples.Length];
             double dt = 1.0 / (double)SampleRate;
             double RC = 1.0 / (2 * Math.PI * CutoffFrequency);
             double alpha = RC / (RC + dt);
             filtered[0] = samples[0];

             for (int i = 1; i < samples.Length; i++) {
                 var filter = alpha * filtered[i - 1] + alpha * (samples[i] - samples[i - 1]);
                 filtered[i] = (short)filter;
             }

             return filtered;
         }*/
    }
    public class HighpassFilterButterworthImplementation {
        protected HighpassFilterButterworthSection[] section;

        public HighpassFilterButterworthImplementation
        (double cutoffFrequencyHz, int numSections, double Fs) {
            this.section = new HighpassFilterButterworthSection[numSections];
            for (int i = 0; i < numSections; i++) {
                this.section[i] = new HighpassFilterButterworthSection
                (cutoffFrequencyHz, i + 1, numSections * 2, Fs);
            }
        }
        public double compute(double input) {
            double output = input;
            for (int i = 0; i < this.section.Length; i++) {
                output = this.section[i].compute(output);
            }
            return output;
        }
    }

    public class HighpassFilterButterworthSection {
        protected FIRFilterImplementation firFilter = new FIRFilterImplementation(3);
        protected IIRFilterImplementation iirFilter = new IIRFilterImplementation(2);

        protected double[] a = new double[3];
        protected double[] b = new double[2];
        protected double gain;

        public HighpassFilterButterworthSection(double cutoffFrequencyHz, double k, double n, double Fs) {
            // pre-warp omegac and invert it
            double omegac = 1.0 / (2.0 * Fs * Math.Tan(Math.PI * cutoffFrequencyHz / Fs));

            // compute zeta
            double zeta = -Math.Cos(Math.PI * (2.0 * k + n - 1.0) / (2.0 * n));

            // fir section
            this.a[0] = 4.0 * Fs * Fs;
            this.a[1] = -8.0 * Fs * Fs;
            this.a[2] = 4.0 * Fs * Fs;

            //iir section
            //normalize coefficients so that b0 = 1
            //and higher-order coefficients are scaled and negated
            double b0 = (4.0 * Fs * Fs) + (4.0 * Fs * zeta / omegac) + (1.0 / (omegac * omegac));
            this.b[0] = ((2.0 / (omegac * omegac)) - (8.0 * Fs * Fs)) / (-b0);
            this.b[1] = ((4.0 * Fs * Fs)
                       - (4.0 * Fs * zeta / omegac) + (1.0 / (omegac * omegac))) / (-b0);
            this.gain = 1.0 / b0;
        }

        public double compute(double input) {
            // compute the result as the cascade of the fir and iir filters
            return this.iirFilter.compute
                (this.firFilter.compute(this.gain * input, this.a), this.b);
        }
        public class FIRFilterImplementation {
            protected double[] z;
            public FIRFilterImplementation(int order) {
                this.z = new double[order];
            }

            public double compute(double input, double[] a) {
                // computes y(t) = a0*x(t) + a1*x(t-1) + a2*x(t-2) + ... an*x(t-n)
                double result = 0;

                for (int t = a.Length - 1; t >= 0; t--) {
                    if (t > 0) {
                        this.z[t] = this.z[t - 1];
                    } else {
                        this.z[t] = input;
                    }
                    result += a[t] * this.z[t];
                }
                return result;
            }
        }
        public class IIRFilterImplementation {
            protected double[] z;
            public IIRFilterImplementation(int order) {
                this.z = new double[order];
            }

            public double compute(double input, double[] a) {
                // computes y(t) = x(t) + a1*y(t-1) + a2*y(t-2) + ... an*y(t-n)
                // z-transform: H(z) = 1 / (1 - sum(1 to n) [an * y(t-n)])
                // a0 is assumed to be 1
                // y(t) is not stored, so y(t-1) is stored at z[0], 
                // and a1 is stored as coefficient[0]

                double result = input;

                for (int t = 0; t < a.Length; t++) {
                    result += a[t] * this.z[t];
                }
                for (int t = a.Length - 1; t >= 0; t--) {
                    if (t > 0) {
                        this.z[t] = this.z[t - 1];
                    } else {
                        this.z[t] = result;
                    }
                }
                return result;
            }
        }
    }
}