using System;
namespace PlayString
{
    class Progra
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("STR2WAVE - Convert BASIC play command string into an 8 bit 44100hz mono WAV file.\r\n");
                Console.WriteLine("Usage: STR2WAV [string] [file]\r\n");
                Console.WriteLine("Example:");
                Console.WriteLine("STR2WAV T240L8CDEFGAB test.wav");
            }
            STR2WAV.GenerateWAVFile(args[0], args[1]);
        }
    }
}
