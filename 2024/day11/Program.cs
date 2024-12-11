
ulong Add(ulong value1, ulong value2)
{
    checked { return value1 + value2; }
}

ulong Mul(ulong value1, ulong value2)
{
    checked { return value1 * value2; }
}


Dictionary<ulong, ulong> CreateStones(IEnumerable<ulong> numbers)
{
    var stones = numbers.ToDictionary(n => n, n => 1ul);
    return stones;
}

void AddStone(Dictionary<ulong, ulong> stones, ulong number, ulong count)
{
    stones.TryGetValue(number, out ulong currentCount);
    stones[number] = Add(currentCount, count);
}

bool Split(ulong number, out ulong high, out ulong low)
{
    high = 0;
    low = 0;

    var divisor1 = 10ul;
    var divisor2 = 1ul;
    var countDigits = 1;
    var isEven = false;

    while (number / divisor1 > 0)
    {
        divisor1 = Mul(divisor1, 10);

        countDigits++;
        isEven = !isEven;

        if (isEven)
            divisor2 = Mul(divisor2, 10);
    }

    if (countDigits % 2 > 0)
    {
        return false;
    }

    high = number / divisor2;
    low = number % divisor2;

    return true;
}

Dictionary<ulong, ulong> Blink(Dictionary<ulong, ulong> stones)
{
    var newStones = new Dictionary<ulong, ulong>();

    foreach (var pair in stones)
    {
        var number = pair.Key;
        var count = pair.Value;

        if (number == 0)
        {
            AddStone(newStones, 1, count);
        }

        else if (Split(number, out var high, out var low))
        {
            AddStone(newStones, high, count);
            AddStone(newStones, low, count);
        }

        else if (number >= 1)
        {
            var newNumber = Mul(number, 2024);
            AddStone(newStones, newNumber, count);
        }
    }

    return newStones;
}

ulong BlinkMany(int count, Dictionary<ulong, ulong> stones)
{
    if (count <= 0)
        return 0;

    var current = stones;

    for (var i = 0; i < count; i++)
    {
        current = Blink(current);
    }

    var totalStones = 0ul;

    foreach (var stonesCount in current.Values)
    {
        totalStones = Add(totalStones, stonesCount);
    }

    return totalStones;
}

void InnerMain()
{
    var blinkCount = int.Parse(args[0]);
    var numbers = args.Skip(1).Select(ulong.Parse);
    var stones = CreateStones(numbers);
    var count = BlinkMany(blinkCount, stones);

    Console.WriteLine($"Stones: {count}");
}

InnerMain();