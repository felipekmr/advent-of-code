using CommunityToolkit.Diagnostics;

namespace Aoc.Support;

public static class DayRunner
{
    public static int Execute<TPart1, TPart2>(int day, string[] args)
        where TPart1 : IAdvent, new()
        where TPart2 : IAdvent, new()
    {
        Guard.IsGreaterThanOrEqualTo(day, 1);
        Guard.IsNotNull(args);

        try
        {
            if (args.Length == 0)
                throw new ApplicationException($"Too few arguments");

            if (int.TryParse(args[0], out var partNumber) == false)
                throw new ApplicationException($"{args[0]}: Is not a integer");

            return Execute<TPart1, TPart2>(day, partNumber, args.Skip(1).ToArray());
        }
        catch (ApplicationException ex)
        {
            Console.Error.WriteLine($"Error: {ex.Message}");
            return 1;
        }
    }
    public static int Execute<TPart1, TPart2>(int day, int partNumber, string[] args)
        where TPart1 : IAdvent, new()
        where TPart2 : IAdvent, new()
    {
        Guard.IsGreaterThanOrEqualTo(day, 1);
        Guard.IsNotNull(args);

        IAdvent advent = partNumber switch
        {
            1 => new TPart1(),
            2 => new TPart2(),
            _ => throw new ApplicationException($"{partNumber}: Invalid part number"),
        };

        var answer = advent.Execute(args);

        Console.WriteLine($"Answer to day {day} is {answer}");

        return 0;
    }
}
