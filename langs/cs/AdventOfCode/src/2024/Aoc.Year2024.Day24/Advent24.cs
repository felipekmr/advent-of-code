using System.Diagnostics;

namespace Aoc.Year2024.Day24;

internal static class Advent24
{
    public static (Wires Wires, Gates Gates) ReadInputFile(string inputFile)
    {
        var wirer = new WireIdentifier();
        var wires = new Wires();
        var gates = new Gates();

        var lines = File.ReadLines(inputFile);
        var linetor = lines.GetEnumerator();

        while (linetor.MoveNext())
        {
            if (string.IsNullOrEmpty(linetor.Current))
                break;

            var split = linetor.Current.Split(": ");
            var wireName = split[0];
            var wireValue = int.Parse(split[1]) > 0;
            var wire = wirer.GetWire(wireName);
            wires.Add(wire, wireValue);
        }

        while (linetor.MoveNext())
        {
            var split = linetor.Current.Split(' ');
            var input0 = wirer.GetWire(split[0]);
            var input1 = wirer.GetWire(split[2]);
            var output = wirer.GetWire(split[4]);
            var type = split[1];
            var gate = new Gate(type, input0, input1, output);
            gates.Add(output, gate);
        }

        return (wires, gates);
    }

    public static long CalculateOutput(Wires wires, Gates gates)
    {
        CalculateWires(wires, gates);

        return GetNumber('z', wires);
    }

    public static void CalculateWires(Wires wires, Gates gates)
    {
        new CircuitResolver(wires, gates).Execute(); ;
    }

    public static long GetNumber(char wireType, Wires wires)
    {
        var number = 0L;
        var firstId = WireIdentifier.GetFirstId(wireType);

        foreach (var pair in wires)
        {
            var wire = pair.Key;
            var wireValue = pair.Value;

            if (wireValue == false)
                continue;

             if (WireIdentifier.FilterWire(wire, firstId) == false)
                continue;

            var wireBit = wire.Id - firstId;
            number += 1L << wireBit;
        }

        return number;
    }
}
