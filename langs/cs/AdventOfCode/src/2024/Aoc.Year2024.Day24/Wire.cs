namespace Aoc.Year2024.Day24;

internal record Wire(int Id, string Name)
{
    public override string ToString() => $"{Name}({Id})";
}
