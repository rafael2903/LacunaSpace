using System.Buffers.Binary;
using System.Globalization;

namespace Converters;

public enum Encoding
{
    ISO8601,
    Ticks,
    TicksBigEndian,
    TicksLittleEndian
}

public class TimeConverter
{

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

    public static string Encode(DateTimeOffset dateTimeOffset, Encoding encoding)
    {
        return encoding switch
        {
            Encoding.ISO8601 => ISO8601FromDateTimeOffset(dateTimeOffset),
            Encoding.Ticks => dateTimeOffset.Ticks.ToString(),
            Encoding.TicksBigEndian => Base64TicksBigEndianFromDateTimeOffset(dateTimeOffset),
            Encoding.TicksLittleEndian => Base64TicksLittleEndianFromDateTimeOffset(dateTimeOffset),
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

    public static string ISO8601FromDateTimeOffset(DateTimeOffset dateTimeOffset)
    {
        return dateTimeOffset.ToString("o");
    }

    private static long Int64FromBase64(string base64)
    {
        return BitConverter.ToInt64(Convert.FromBase64String(base64));
    }

    private static string Base64FromInt64(long value)
    {
        return Convert.ToBase64String(BitConverter.GetBytes(value));
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

    public static string Base64TicksLittleEndianFromDateTimeOffset(DateTimeOffset dateTimeOffset)
    {
        if (BitConverter.IsLittleEndian)
        {
            return Base64FromInt64(dateTimeOffset.Ticks);
        }
        else
        {
            return Base64FromInt64(BinaryPrimitives.ReverseEndianness(dateTimeOffset.Ticks));
        }
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

    public static string Base64TicksBigEndianFromDateTimeOffset(DateTimeOffset dateTimeOffset)
    {
        if (BitConverter.IsLittleEndian)
        {
            return Base64FromInt64(BinaryPrimitives.ReverseEndianness(dateTimeOffset.Ticks));
        }
        else
        {
            return Base64FromInt64(dateTimeOffset.Ticks);
        }
    } 
}