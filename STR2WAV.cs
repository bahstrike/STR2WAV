using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PlayString
{
    class STR2WAV
    {
        public static byte[] GeneratePCMBuffer(string snd)
        {
            STR2WAV s = new STR2WAV(snd);
            return s.generateFinalDataBuffer();
        }

        public static void GenerateWAVFile(string snd, string outpath)
        {
            STR2WAV s = new STR2WAV(snd);

            FileStream f = new FileStream(outpath, FileMode.Create);
            generateWAV(s.generateFinalDataBuffer(), f);
            f.Close();
        }
        

        private List<byte[]> data = new List<byte[]>();
        private static double[] FREQ = new double[] {
            // C, C#, D, D#, E,F, F#, G, G#, A, A#, H
            16.35, 17.32, 18.35, 19.45, 20.60, 21.83, 23.12, 24.50, 25.96, 27.50, 29.14, 30.87, // Octave 0
            32.70, 34.65, 36.71, 38.89, 41.20, 43.65, 46.25, 49.00, 51.91,  55.00, 58.27, 61.74, // Octave 1
            65.41, 69.30, 73.42, 77.78, 82.41, 87.31, 92.50, 98.00, 103.83, 110.00, 116.54, 123.47, // Octave 2
            130.81, 138.59, 146.83, 155.56, 164.81, 174.61, 185.00, 196.00, 207.65, 220.00, 233.08, 246.94, // Octave 3
            261.63, 277.18, 293.66, 311.13, 329.63, 349.23, 369.99, 392.00, 415.30, 440.00, 466.16, 493.88, // Octave 4
            523.25,  554.37, 587.33, 622.25, 659.25, 698.46,  739.99, 783.99, 830.61, 880.00, 932.33, 987.77, // Octave 5
            1046.50, 1108.73, 1174.66, 1244.51, 1318.51, 1396.91, 1479.98, 1567.98, 1661.22, 1760.00, 1864.66, 1975.53, // Octave 6
            2093.00, 2217.46, 2349.32, 2489.02, 2637.02, 2793.83, 2959.96, 3135.96, 3322.44, 3520.00, 3729.31, 3951.07, // Octave 7
            4186.01,  4434.92, 4698.63, 4978.03, 5274.04, 5587.65, 5919.91, 6271.93, 6644.88, 7040.00, 7458.62, 7902.13 // Octave 8
        };
        enum NoteDurations { Normal, Legato, Staccato };

        int notelength = 4;
        int octave = 4;
        NoteDurations NoteDuration = NoteDurations.Normal;
        int tempo = 120;

        /*
Ln     Sets the duration (length) of the notes. The variable n does not indicate an actual duration
       amount but rather a note type; L1 - whole note, L2 - half note, L4 - quarter note, etc.
       (L8, L16, L32, L64, ...). By default, n = 4.
       For triplets and quintets, use L3, L6, L12, ... and L5, L10, L20, ... series respectively.
       The shorthand notation of length is also provided for a note. For example, "L4 CDE L8 FG L4 AB"
       can be shortened to "L4 CDE F8G8 AB". F and G play as eighth notes while others play as quarter notes.
On     Sets the current octave. Valid values for n are 0 through 6. An octave begins with C and ends with B.
       Remember that C- is equivalent to B. 
< >    Changes the current octave respectively down or up one level.
Nn     Plays a specified note in the seven-octave range. Valid values are from 0 to 84. (0 is a pause.)
       Cannot use with sharp and flat. Cannot use with the shorthand notation neither.
MN     Stand for Music Normal. Note duration is 7/8ths of the length indicated by Ln. It is the default mode.
ML     Stand for Music Legato. Note duration is full length of that indicated by Ln.
MS     Stand for Music Staccato. Note duration is 3/4ths of the length indicated by Ln.
Pn     Causes a silence (pause) for the length of note indicated (same as Ln). 
Tn     Sets the number of "L4"s per minute (tempo). Valid values are from 32 to 255. The default value is T120. 
.      When placed after a note, it causes the duration of the note to be 3/2 of the set duration.
       This is how to get "dotted" notes. "L4 C#." would play C sharp as a dotted quarter note.
       It can be used for a pause as well.
MB MF  Stand for Music Background and Music Foreground. MB places a maximum of 32 notes in the music buffer
       and plays them while executing other statements. Works very well for games.
       MF switches the PLAY mode back to normal. Default is MF.
         */
        protected STR2WAV(string snd)
        {
            // dude just get rid of all the spaces
            for(; ;)
            {
                int i = snd.IndexOf(' ');
                if (i == -1)
                    break;
                snd = snd.Remove(i, 1);
            }

            snd = snd.ToUpper();
            for(int i=0; i<snd.Length; i++)
            {
                char c = snd[i];
                switch (c)
                {
                    //case ' ':
                        //continue;

                    case 'L':
                        {
                            i++;
                            string val = "";
                            while (i < snd.Length && char.IsDigit(snd[i]))
                            {
                                val += snd[i];
                                i++;
                            }
                            notelength = int.Parse(val);
                            i--;
                        }
                        break;
                    case 'T':
                        {
                            i++;
                            string val = "";
                            while (i < snd.Length && char.IsDigit(snd[i]))
                            {
                                val += snd[i];
                                i++;
                            }
                            tempo = int.Parse(val);
                            i--;
                        }
                        break;
                    case 'M':
                        {
                            i++;
                            switch (snd[i])
                            {
                                case 'L':
                                    NoteDuration = NoteDurations.Legato;
                                    break;
                                case 'N':
                                    NoteDuration = NoteDurations.Normal;
                                    break;
                                case 'S':
                                    NoteDuration = NoteDurations.Staccato;
                                    break;
                                case 'B':
                                    // nothing
                                    break;
                                case 'F':
                                    // nothing
                                    break;
                                default:
                                    throw new Exception("Invalid duration " + snd[i] + "!");
                            }
                        }
                        break;
                    case 'A':
                    case 'B':
                    case 'C':
                    case 'D':
                    case 'E':
                    case 'F':
                    case 'G':
                        {
                            bool flat = false;
                            bool sharp = false;
                            bool dotted = false;
                            bool doubledotted = false;
                            PlayNoteInfo n = new PlayNoteInfo();
                            i++;
                            if (i < snd.Length && snd[i] == '+') { sharp = true; i++; }
                            if (i < snd.Length && snd[i] == '-') { flat = true; i++; }
                            if (i < snd.Length && snd[i] == '.') { dotted = true; i++;
                                if (snd[i] == '.')
                                {
                                    doubledotted = true;
                                    i++;
                                }
                            }

                            if (i < snd.Length && char.IsDigit(snd[i]))
                            {
                                string val = "";
                                while (i < snd.Length && char.IsDigit(snd[i]))
                                {
                                    val += snd[i];
                                    i++;
                                }
                                int duration = int.Parse(val);

                                if (i < snd.Length && snd[i] == '+') { sharp = true; i++; }
                                if (i < snd.Length && snd[i] == '-') { flat = true; i++; }
                                if (i < snd.Length && snd[i] == '.') { dotted = true; i++;
                                    if (snd[i] == '.')
                                    {
                                        doubledotted = true;
                                        i++;
                                    }
                                }

                                n.Note = c;
                                n.Length = duration;
                            }
                            else
                                n.Note = c;
                            n.Flat = flat;
                            n.Sharp = sharp;
                            n.Dotted = dotted;
                            n.DoubleDotted = doubledotted;
                            playnote(n);
                            i--;

                        }
                        break;
                    case 'O':
                        {
                            i++;
                            string val = "";
                            while (i < snd.Length && char.IsDigit(snd[i]))
                            {
                                val += snd[i];
                                i++;
                            }
                            octave = int.Parse(val);
                            i--;
                        }
                        break;
                    case '>':
                        octave++;
                        break;
                    case '<':
                        octave--;
                        break;
                    case 'P':
                        {
                            i++;
                            string val = "";
                            while (i < snd.Length && char.IsDigit(snd[i]))
                            {
                                val += snd[i];
                                i++;
                            }
                            int pause = int.Parse(val);

                            double totalseconds = (1.0 / (tempo / 60.0)) * (4.0 / pause);
                            Tone(0, totalseconds);

                            i--;
                        }
                        break;
                    default:
                        throw new Exception("Command " + c + " not implemented!");
                }
            }
        }

        class PlayNoteInfo
        {
            public char Note;
            public int Length;
            public bool Dotted;
            public bool DoubleDotted;
            public bool Sharp;
            public bool Flat;

            public PlayNoteInfo(char note)
            {
                Note = note;
                Length = -1; // Note length is set by the Ln command 
            }
            public PlayNoteInfo(char note, int length)
            {
                Note = note;
                Length = length;
            }

            public PlayNoteInfo()
            {

            }

            public int getNumber()
            {

                // C, C#, D, D#, E,F, F#, G, G#, A, A#, H
                int number = 0;
                switch (Note)
                {
                    case 'C': number = 0; break;
                    case 'D': number = 2; break;
                    case 'E': number = 4; break;
                    case 'F': number = 5; break;
                    case 'G': number = 7; break;
                    case 'A': number = 9; break;
                    case 'B': number = 11; break;
                }
                if (Sharp)
                    number++;
                if (Flat)
                    number--;
                return number;
            }

        }

        private void playnote(PlayNoteInfo n)
        {
            int noteno = n.getNumber();
            double herz = FREQ[(12 * (octave + 2)) + noteno];
            int thisnotelength = notelength;
            if (n.Length > 0)
                thisnotelength = n.Length;
            double totalseconds = (1.0 / (tempo / 60.0)) * (4.0 / thisnotelength);
            double noteseconds = totalseconds;
            double pause = 0.0;
            if (NoteDuration == NoteDurations.Normal)
            {
                noteseconds = noteseconds * (7.0 / 8.0);
                pause = totalseconds - noteseconds;
            }
            else if (NoteDuration == NoteDurations.Staccato)
            {
                noteseconds = noteseconds * (3.0 / 4.0);
                pause = totalseconds - noteseconds;
            }
            if (n.DoubleDotted)
                noteseconds += (noteseconds / 2.0) + (noteseconds / 4.0);
            else if (n.Dotted)
                noteseconds += (noteseconds / 2.0);
            Tone(herz, noteseconds);
            if (pause > 0.0)
                Tone(0, pause);
        }


        protected void Tone(double hz, double seconds)
        {
            double sampleRate = 44100;
            int duration = (int)(sampleRate * seconds);
            byte[] d = new byte[duration];
            double amplitude = 127;
            for (int n = 0; n < duration; n++)
            {
                int value = (int)(amplitude * Math.Sin((2.0 * Math.PI * n * hz) / sampleRate));
                if (value > 4)
                    value = 127;
                else
                if (value < -4)
                    value = -127;
                else value = 0;
                d[n] = (byte)(128 - value);
            }
            data.Add(d);
        }

        protected byte[] generateFinalDataBuffer()
        {
            int totalbytes = 0;
            foreach (byte[] d in data)
                totalbytes += d.Length;

            byte[] ret = new byte[totalbytes];

            int pos = 0;
            foreach (byte[] d in data)
            {
                Array.Copy(d, 0, ret, pos, d.Length);
                pos += d.Length;
            }

            return ret;
        }

        static protected void generateWAV(byte[] pcm, Stream f)
        {
            UInt32 SubChunk2Size = (uint)pcm.Length;
            UInt16 AudioFormat = 1; // PCM
            UInt16 NumChannels = 1; // Mono
            UInt16 BitsPerSample = 8; // 8 bit
            UInt32 SampleRate = 44100;
            UInt32 ByteRate = (UInt32)(SampleRate * NumChannels * (BitsPerSample / 8));
            UInt32 SubChunk1Size = 16;
            UInt32 ChunkSize = 36 + SubChunk2Size;
            UInt16 BlockAlign = (UInt16)(NumChannels * (BitsPerSample / 8));
            BinaryWriter wr = new BinaryWriter(f);

            wr.Write(Encoding.ASCII.GetBytes("RIFF"));
            wr.Write(ChunkSize);
            wr.Write(Encoding.ASCII.GetBytes("WAVE"));
            wr.Write(Encoding.ASCII.GetBytes("fmt "));
            wr.Write(SubChunk1Size);
            wr.Write(AudioFormat);
            wr.Write(NumChannels);
            wr.Write(SampleRate);
            wr.Write(ByteRate);
            wr.Write(BlockAlign);
            wr.Write(BitsPerSample);
            wr.Write(Encoding.ASCII.GetBytes("data"));
            wr.Write(pcm);
        }
    }
}
