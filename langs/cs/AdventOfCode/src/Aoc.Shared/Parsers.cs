using CommunityToolkit.Diagnostics;
using System.Drawing;

namespace Aoc.Shared;

public static class Parsers
{
    public static Point ToPoint(string input)
    {
        var (x, y) = ParseIntegers(input);
        var point = new Point(x, y);
        return point;
    }

    public static Size ToSize(string input)
    {
        var (width, height) = ParseIntegers(input);
        var size = new Size(width, height);
        return size;
    }

    private static (int, int) ParseIntegers(string input)
    {
        Guard.IsNotNull(input);

        return ParseIntegers(input.Split(','));
    }

    private static (int, int) ParseIntegers(string[] input)
    {
        Guard.IsNotNull(input);
        Guard.HasSizeEqualTo(input, 2);

        var value0 = int.Parse(input[0]);
        var value1 = int.Parse(input[1]);

        return (value0, value1);
    }
}
