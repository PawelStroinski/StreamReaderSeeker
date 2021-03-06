﻿using System.IO;
using System.Reflection;
using System.Text;
using NUnit.Framework;

namespace StreamUtils
{
    public class StreamReaderSeekerTest
    {
        [Test]
        public void DiscardsBufferedDataWhenCharacterPositionIsNegative()
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("StreamUtils.input");
            var reader = new StreamReader(stream);
            var buffer = new char[30];
            reader.Seek(new StreamReaderSeeker.Position(1158, -431));
            reader.Read(buffer, 0, buffer.Length);
            var expected = "5350240105107934054\r\n210821277";
            var actual = new string(buffer);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetPosition()
        {
            var reader = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes("text")));
            reader.ReadToEnd();
            var actual = reader.GetPosition();
            Assert.AreEqual("streamPosition=4, characterPosition=0", actual.ToString());
        }

        [Test]
        public void SimpleExample()
        {
            Program.SimpleExample();
        }
    }
}
