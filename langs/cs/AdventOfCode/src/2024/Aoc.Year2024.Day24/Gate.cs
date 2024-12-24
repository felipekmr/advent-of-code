using System.Diagnostics;

namespace Aoc.Year2024.Day24;

internal record Gate(string Type, Wire Input0, Wire Input1, Wire Output)
{
    public override string ToString() => $"{Input0} {Type} {Input1} -> {Output}";
}
