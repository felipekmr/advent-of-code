using CommunityToolkit.Diagnostics;

namespace Aoc.Year2024.Day24;

internal class CircuitResolver
{
    public CircuitResolver(Wires wires, Gates gates)
    {
        Guard.IsNotNull(wires);
        Guard.IsNotNull(gates);

        Wires = wires;
        Gates = gates;
    }

    public Wires Wires { get; }
    public Gates Gates { get; }

    public static bool GetOutputValue(Gate gate, bool input0, bool input1)
    {
        Guard.IsNotNull(gate);

        return GetOutputValue(gate.Type, input0, input1);
    }

    public static bool GetOutputValue(string gateType, bool input0, bool input1)
    {
        Guard.IsNotNullOrWhiteSpace(gateType);

        var outputValue = gateType switch
        {
            "AND" => input0 && input1,
            "OR" => input0 || input1,
            "XOR" => input0 ^ input1,
            _ => throw new InvalidOperationException($"Invalid gate type '{gateType}'"),
        };

        return outputValue;
    }

    public void Execute()
    {
        foreach (var gate in Gates.Values)
            Wires.Add(gate.Output, GetOutputWireValue(gate));
    }

    public bool GetOutputWireValue(Gate gate)
    {
        Guard.IsNotNull(gate);

        if (Wires.TryGetValue(gate.Output, out var wireValue))
            return wireValue;

        return GetOutputValue(gate);
    }


    public bool GetOutputValue(Gate gate)
    {
        var input0 = GetWireValue(gate.Input0);
        var input1 = GetWireValue(gate.Input1);
        var output = GetOutputValue(gate, input0, input1);
        return output;
    }

    public bool GetWireValue(Wire wire)
    {
        if (Wires.TryGetValue(wire, out var wireValue))
            return wireValue;

        var gate = Gates[wire];
        return GetOutputValue(gate);
    }
}
