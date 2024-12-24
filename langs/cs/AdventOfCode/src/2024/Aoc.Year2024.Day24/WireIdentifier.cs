using CommunityToolkit.Diagnostics;

namespace Aoc.Year2024.Day24;

internal class WireIdentifier
{
    public const int IdCount = 1_000;
    public const int FirstX = 0;
    public const int FirstY = FirstX + IdCount;
    public const int FirstZ = FirstY + IdCount;
    public const int FirstI = FirstZ + IdCount;

    private readonly Dictionary<string, Wire> wires = new Dictionary<string, Wire>();
    private int intermediateCount = 0;

    public static int GetFirstId(string wireName)
    {
        Guard.IsNotNullOrWhiteSpace(wireName);

        return GetFirstId(wireName[0]);
    }

    public static int GetFirstId(char wireType)
    {
        return wireType switch
        {
            'x' => FirstX,
            'y' => FirstY,
            'z' => FirstZ,
            _ => FirstI,
        };
    }

    public static IEnumerable<Wire> FilterWires(char wireType, IEnumerable<Wire> wires)
    {
        var firstId = GetFirstId(wireType);

        return FilterWires(firstId, wires);
    }

    public static IEnumerable<Wire> FilterWires(int firstId, IEnumerable<Wire> wires)
    {
        return
            from wire in wires
            where FilterWire(wire, firstId)
            select wire;
    }

    public static bool FilterWire(Wire wire, int firstId)
    {
        return wire.Id >= firstId && wire.Id < (firstId + IdCount);
    }

    public Wire GetWire(string wireName)
    {
        if (wires.TryGetValue(wireName, out var wire))
            return wire;

        wire = CreateWire(wireName);
        wires[wireName] = wire;
        return wire;
    }

    public Wire CreateWire(string wireName)
    {
        Guard.IsNotNullOrWhiteSpace(wireName);

        var firstId = GetFirstId(wireName);
        var wire = firstId < FirstI
            ? CreateBitWire(firstId, wireName)
            : new Wire(FirstI + intermediateCount++, wireName);

        return wire;
    }

    private Wire CreateBitWire(int firstId, string wireName)
    {
        var offset = int.Parse(wireName.Substring(1));
        return new Wire(firstId + offset, wireName);
    }
}
