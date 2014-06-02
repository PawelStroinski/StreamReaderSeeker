using System;
using System.IO;
using System.Linq;
using System.Text;

namespace StreamUtils
{
    class Program
    {
        static void Main(string[] args)
        {
            SimpleExample();
            LongExample();
        }

        public static void SimpleExample()
        {
            var fromOneToTenInArmenian = "մեկ երկու երեք չորս հինգ վեց յոթ ութ ինը տասը";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(fromOneToTenInArmenian));
            var reader = new StreamReader(stream);
            var buffer = new char[5];
            reader.Read(buffer, 0, buffer.Length);
            var positionAfterReadingFiveChars = reader.GetPosition();
            var positionAfterReadingThreeChars = positionAfterReadingFiveChars.ShiftByCharacters(-2);
            reader.Seek(positionAfterReadingThreeChars);
            reader.Read(buffer, 0, buffer.Length);
            var expected = fromOneToTenInArmenian.Substring(3, buffer.Length);
            var actual = new string(buffer);
            System.Diagnostics.Contracts.Contract.Assert(expected == actual);
        }

        public static void LongExample()
        {
            var builder = new StringBuilder();
            var random = new Random();
            char[] value = new char[20];
            char[] stored = new char[value.Length];
            var loopCount = 10;
            for (int i = 1; i <= value.Length * loopCount; i++)
                builder.Append(Char.ConvertFromUtf32(random.Next(100)));
            var stream = new MemoryStream(Encoding.UTF7.GetBytes(builder.ToString()));
            Console.Write("stream length:       ");
            Console.WriteLine(stream.Length);
            Console.WriteLine();
            var reader = new StreamReader(stream, Encoding.UTF7);
            StreamReaderSeeker.Position position = null;
            for (int i = 1; i <= loopCount; i++)
            {
                reader.Read(value, 0, value.Length);
                Console.Write("value:               ");
                Console.WriteLine(value);
                Console.Write("stream position:     ");
                Console.WriteLine(stream.Position);
                Console.Write("calculated position: ");
                Console.WriteLine(GetCalculatedPosition(reader));
                if (i == loopCount / 3)
                {
                    position = reader.GetPosition();
                    Console.WriteLine("POSITION STORED");
                    Console.WriteLine("The next value should be same as what we will read later after seeking");
                }
                if (i == loopCount / 3 + 1)
                    value.CopyTo(stored, 0);
                Console.WriteLine();
            }
            reader.Seek(position);
            Console.WriteLine("We are back at the stored position");
            Console.Write("calculated position: ");
            Console.WriteLine(GetCalculatedPosition(reader));
            reader.Read(value, 0, value.Length);
            Console.Write("value:               ");
            Console.WriteLine(value);
            Console.Write("stored value same:   ");
            var same = stored.SequenceEqual(value);
            Console.WriteLine(same);
            System.Diagnostics.Contracts.Contract.Assert(same);
            Console.WriteLine();
            reader.ReadToEnd();
            Console.WriteLine("We are back at the end of stream, let's go backwards by 5 characters");
            var ending = reader.GetPosition().ShiftByCharacters(-5);
            reader.Seek(ending);
            Console.Write("calculated position: ");
            Console.WriteLine(GetCalculatedPosition(reader));
            Console.Write("ending:              ");
            Console.WriteLine(reader.ReadToEnd());
            Console.Write("calculated position: ");
            Console.WriteLine(GetCalculatedPosition(reader));
        }

        public static long GetCalculatedPosition(StreamReader reader) // http://stackoverflow.com/a/17457085
        {
            // The current buffer of decoded characters
            char[] charBuffer = (char[])reader.GetType().InvokeMember("charBuffer"
                , System.Reflection.BindingFlags.DeclaredOnly | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.GetField
                , null, reader, null);

            // The current position in the buffer of decoded characters
            int charPos = (int)reader.GetType().InvokeMember("charPos"
                , System.Reflection.BindingFlags.DeclaredOnly | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.GetField
                , null, reader, null);

            // The number of bytes that the already-read characters need when encoded.
            int numReadBytes = reader.CurrentEncoding.GetByteCount(charBuffer, 0, charPos);

            // The number of encoded bytes that are in the current buffer
            int byteLen = (int)reader.GetType().InvokeMember("byteLen"
                , System.Reflection.BindingFlags.DeclaredOnly | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.GetField
                , null, reader, null);

            return reader.BaseStream.Position - byteLen + numReadBytes;
        }
    }
}