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
        checked
        {
            divisor1 *= 10;
        }

        countDigits++;
        isEven = !isEven;

        if (isEven)
            divisor2 *= 10;
    }

    if (countDigits % 2 > 0)
    {
        return false;
    }

    high = number / divisor2;
    low = number % divisor2;

    return true;
}

IEnumerable<ulong> Blink(IEnumerable<ulong> source)
{
    foreach (var item in source)
    {
        if (item == 0)
        {
            yield return 1;
        }

        else if (Split(item, out var high, out var low))
        {
            yield return high;
            yield return low;
        }

        else if (item >= 1)
        {
            checked
            {
                yield return item * 2024;
            }
        }
    }
}

int BlinkMany(int count, IEnumerable<ulong> source)
{
    if (count <= 0)
        return 0;

    var current = source;

    for (var i = 0; i < count; i++)
    {
        current = Blink(current);
    }

    return current.Count();
}

void InnerMain()
{
    var blinkCount = int.Parse(args[0]);
    var intput = args.Skip(1).Select(ulong.Parse);
    var count = BlinkMany(blinkCount, intput);

    Console.WriteLine($"Stones: {count}");
}

InnerMain();