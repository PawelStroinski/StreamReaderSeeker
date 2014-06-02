using System.IO;
using System.Reflection;
using NUnit.Framework;

namespace StreamReaderSeeker
{
    public class StreamReaderSeekerTest
    {
        [Test]
        public void DiscardsBufferedDataWhenCharacterPositionIsNegative()
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("StreamReaderSeeker.input");
            var reader = new StreamReader(stream);
            var buffer = new char[30];
            reader.Seek(new StreamReaderSeeker.Position(1158, -431));
            reader.Read(buffer, 0, buffer.Length);
            var expected = "5350240105107934054\r\n210821277";
            var actual = new string(buffer);
            Assert.AreEqual(expected, actual);
        }
    }
}
