//using Aoc.Shared;
//using Aoc.Support;
//using System.Drawing;

//namespace Aoc.Year2024.Day18;

//internal class Part1 : IAdvent
//{
//    public const char CorruptedByte = '#';

//    public object Execute(string[] args)
//    {
//        var i = 0;
//        var count = int.Parse(args[i++]);
//        var size = Parsers.ToSize(args[i++]);
//        var inputPath = args[i++];

//        var grid = new Grid<char>(size);

//        var corruptedBytes = File
//            .ReadLines(inputPath)
//            .Take(count)
//            .Select(Point.Parse);

//        foreach (var point in corruptedBytes)
//            grid[point] = CorruptedByte;

//        var start = new Point(0, 0);
//        var end = new Point(size.Width - 1, size.Height - 1);
//        var astart = new AStartSearch(size, start, end, p => grid[p] == CorruptedByte);

//        var steps = astart.Execute();

//        return steps;
//    }
