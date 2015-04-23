using System;
using System.IO;

namespace StreamUtils
{
    public static class StreamReaderSeeker
    {
        const int maximumBytesPerCodeUnit = 5;

        public static Position GetPosition(this StreamReader reader)
        {
            var runningOnMono = Type.GetType("Mono.Runtime") != null;
            int byteBufferSize = GetField(reader, runningOnMono ? "buffer_size" : "byteLen");
            int characterPosition = GetField(reader, runningOnMono ? "pos" : "charPos");
            return new Position(streamPosition: reader.BaseStream.Position - byteBufferSize,
                characterPosition: characterPosition);
        }

        public static void Seek(this StreamReader reader, Position position)
        {
            position.Seek(reader);
        }

        static int GetField(object instance, string field)
        {
            return (int)instance.GetType().InvokeMember(field,
                System.Reflection.BindingFlags.DeclaredOnly | System.Reflection.BindingFlags.NonPublic
                | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.GetField,
                null, instance, null);
        }

        static void Seek(this StreamReader reader, long streamPosition, int characterPosition)
        {
            reader.DiscardBufferedData();
            reader.BaseStream.Position = streamPosition;
            if (characterPosition > 0)
            {
                var buffer = new char[characterPosition];
                reader.Read(buffer, 0, characterPosition);
            }
            else
                if (characterPosition < 0)
                {
                    var bufferSizeInBytes = Math.Abs(characterPosition) * maximumBytesPerCodeUnit;
                    if (bufferSizeInBytes > streamPosition)
                        bufferSizeInBytes = (int)streamPosition;
                    reader.BaseStream.Position = streamPosition - bufferSizeInBytes;
                    var buffer = new char[bufferSizeInBytes];
                    var bufferSizeInCharacters = reader.Read(buffer, 0, bufferSizeInBytes);
                    var positiveCharacterPosition = bufferSizeInCharacters - Math.Abs(characterPosition);
                    reader.DiscardBufferedData();
                    reader.BaseStream.Position = streamPosition - bufferSizeInBytes;
                    reader.Read(buffer, 0, positiveCharacterPosition);
                }
        }

        public class Position
        {
            readonly long streamPosition;
            readonly int characterPosition;

            public Position(long streamPosition, int characterPosition)
            {
                this.streamPosition = streamPosition;
                this.characterPosition = characterPosition;
            }

            public void Seek(StreamReader reader)
            {
                reader.Seek(streamPosition: streamPosition, characterPosition: characterPosition);
            }

            public Position ShiftByCharacters(int characters)
            {
                return new Position(streamPosition: streamPosition, characterPosition: characterPosition + characters);
            }

            public override string ToString()
            {
                return string.Format("streamPosition={0}, characterPosition={1}", streamPosition, characterPosition);
            }
        }
    }
}