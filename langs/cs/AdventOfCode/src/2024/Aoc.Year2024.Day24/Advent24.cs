using System.Diagnostics;

namespace Aoc.Year2024.Day24;

internal static class Advent24
{
    public static (Wires Wires, Gates Gates) ReadInputFile(string inputFile)
    {
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
            wires.Add(wireName, wireValue);
        }

        while (linetor.MoveNext())
        {
            var split = linetor.Current.Split(' ');
            var input0 = split[0];
            var input1 = split[2];
            var output = split[4];
            var type = split[1];
            var gate = new Gate(type, output, [input0, input1]);
            gates.Add(output, gate);
        }

        return (wires, gates);
    }

    public static long CalculateOutput(Wires wires, Gates gates)
    {
        var outputValue = 0L;

        foreach (var gate in gates.Values)
            if (TryGetOutputValue(gate, wires, gates, out var gateOutputValue))
                checked { outputValue += gateOutputValue; }

        return outputValue;
    }

    public static bool TryGetOutputValue(Gate gate, Wires wires, Gates gates, out long outputValue)
    {
        outputValue = 0;

        if (gate.Output.StartsWith('z') == false)
            return false;

        var wireValue = CalculateWire(gate.Output, wires, gates);

        if (wireValue == false)
            return false;

        var outputBit = int.Parse(gate.Output.Substring(1));
        
        Debug.Assert(outputBit < 64);

        outputValue = 1L << outputBit;
        return true;
    }

    public static void CalculateWires(Wires wires, Gates gates)
    {
        foreach (var gate in gates.Values)
            if (TryCalculateWire(gate, wires, gates, out var wireValue))
                wires.Add(gate.Output, wireValue);
    }

    public static bool TryCalculateWire(Gate gate, Wires wires, Gates gates, out bool outputValue)
    {
        var wireName = gate.Output;

        if (wires.TryGetValue(wireName, out outputValue))
            return true;

        return CalculateWire(gate, wires, gates);
    }

    public static bool CalculateWire(Gate gate, Wires wires, Gates gates)
    {
        var input0 = CalculateWire(gate.Inputs[0], wires, gates);
        var input1 = CalculateWire(gate.Inputs[1], wires, gates);

        var gateType = gate.Type;
        var wireValue = gateType switch
        {
            "AND" => input0 && input1,
            "OR" => input0 || input1,
            "XOR" => input0 ^ input1,
            _ => throw new InvalidOperationException($"Invalid gate type '{gateType}'"),
        };

        return wireValue;
    }

    public static bool CalculateWire(string wireName, Wires wires, Gates gates)
    {
        if (wires.TryGetValue(wireName, out var wireValue))
            return wireValue;

        var gate = gates[wireName];
        return CalculateWire(gate, wires, gates);
    }
}
