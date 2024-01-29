using System;
using System.Buffers.Binary;
using System.Globalization;

public class TimeConverter
{
    public enum Encoding
    {
        ISO8601,
        Ticks,
        TicksBigEndian,
        TicksLittleEndian
    }

    public static readonly Dictionary<string, Encoding> Encodings = new()
    {
        { "Iso8601", Encoding.ISO8601 },
        { "Ticks", Encoding.Ticks },
        { "TicksBigEndian", Encoding.TicksBigEndian },
        { "TicksLittleEndian", Encoding.TicksLittleEndian }
    };

    public static DateTimeOffset Decode(string dateString, Encoding encoding)
    {
        return encoding switch
        {
            Encoding.ISO8601 => DateTimeOffsetFromISO8601(dateString),
            Encoding.Ticks => DateTimeOffsetFromTicks(Int64.Parse(dateString)),
            Encoding.TicksBigEndian => DateTimeOffsetFromBase64TicksBigEndian(dateString),
            Encoding.TicksLittleEndian => DateTimeOffsetFromBase64TicksLittleEndian(dateString),
            _ => throw new ArgumentException("Invalid time format", nameof(encoding)),
        };
    }

    public static DateTimeOffset DateTimeOffsetFromTicks(long ticks)
    {
        return new DateTimeOffset(ticks, TimeSpan.Zero);
    }

    public static DateTimeOffset DateTimeOffsetFromISO8601(string dateString)
    {
        return DateTimeOffset.ParseExact(dateString, "o", CultureInfo.InvariantCulture);
    }

    public static DateTimeOffset DateTimeOffsetFromBase64TicksLittleEndian(string ticksBase64)
    {
        long ticks = Int64FromBase64(ticksBase64);

        if (BitConverter.IsLittleEndian)
        {
            return DateTimeOffsetFromTicks(ticks);
        }
        else
        {
            return DateTimeOffsetFromTicks(BinaryPrimitives.ReverseEndianness(ticks));
        }
    }

    private static long Int64FromBase64(string base64)
    {
        return BitConverter.ToInt64(Convert.FromBase64String(base64));
    }

    public static DateTimeOffset DateTimeOffsetFromBase64TicksBigEndian(string ticksBase64)
    {
        long ticks = Int64FromBase64(ticksBase64);
        if (BitConverter.IsLittleEndian)
        {
            return DateTimeOffsetFromTicks(BinaryPrimitives.ReverseEndianness(ticks));
        }
        else
        {
            return DateTimeOffsetFromTicks(ticks);
        }
    }

    public static long TicksFromDateTimeOffset(DateTimeOffset dateTimeOffset)
    {
        return dateTimeOffset.Ticks;
    }

    public static long TicksFromISO8601(string dateString)
    {
        return DateTimeOffset.ParseExact(dateString, "o", CultureInfo.InvariantCulture).Ticks;
    }

    public static string ISO8601FromDateTimeOffset(DateTimeOffset dateTimeOffset)
    {
        return dateTimeOffset.ToString("o");
    }
}



//// String with date only
//dateString = "2023-06-03T12:57:27.6003807+00:00"; // ticks based on the ISO datetime string format: yyyy-MM-ddTHH:mm:ss.FFFFFFFzzz
//offsetDate = DateTimeOffset.ParseExact(dateString, "o", CultureInfo.InvariantCulture);
//Console.WriteLine(offsetDate.ToString());
//Console.WriteLine("638213938476003807"); // ticks long value string
//Console.WriteLine(offsetDate.Ticks);
////Console.WriteLine(DateTimeStyles.RoundtripKind);
//Console.WriteLine(BitConverter.IsLittleEndian);
//Console.WriteLine(BitConverter.ToInt64(Convert.FromBase64String("37GQFTJk2wg="))); // ticks long bytes (little-endian) Base64 string
//Console.WriteLine(BinaryPrimitives.ReverseEndianness(BitConverter.ToInt64(Convert.FromBase64String("CNtkMhWQsd8=")))); // ticks long bytes (big-endian) Base64 string