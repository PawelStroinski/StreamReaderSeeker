StreamReaderSeeker
==================
Two extension methods of `StreamReader` allowing to `GetPosition` and then to `Seek` back to that position. Furthermore, position can be fine-tuned by number of characters with help of `Position.ShiftByCharacters` method, if needed.

### Example

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

There is also too long example in Program.cs.

### How it works

When you invoke `GetPosition`, position is stored as pair of absolute byte position (cannot be amended) and relative character position (can be amended). This character position is necessary because `StreamReader` reads data in chunks decoded to series of characters, not byte-by-byte. So position needs to reflect that.

When you `Seek` to position, this first sets stream position. This is easy. But how to set character position? Well, this is easy too. It just reads that number of characters. So you want to seek by relatively small number of characters but you can seek by infinitely large number of bytes. If you are not using `ShiftByCharacters` you don't have to worry about it, because character position returned from `GetPosition` is never bigger than `StreamReader`'s buffer what is by default not terribly large number.

It works with Unicode also when used encoding allows multiple representations of the same source string.

Don't try to use this with other TextReaders. It wouldn't work.

### Caveat

I only use it in hobby project. So test yourself before you put it into something large.
