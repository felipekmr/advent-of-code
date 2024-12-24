using Aoc.Support;

namespace Aoc.Year2024.Day24;

internal class Part1 : IAdvent
{
    public object Execute(string[] args)
    {
        var inputPath = args[0];
        var (wires, gates) = Advent24.ReadInputFile(inputPath);
        var outputValue = Advent24.CalculateOutput(wires, gates);

        return outputValue;
    }
}
