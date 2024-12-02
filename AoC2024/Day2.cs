namespace AoC2024;

public static class Day2
{
    public static void Solve1()
    {
        var result = 0;
        while (true)
        {
            var line = Console.ReadLine();
            if (string.IsNullOrEmpty(line))
                break;

            var values = line.Split(' ').Select(int.Parse).ToArray();
            if (IsSafe(values))
                result++;
        }

        Console.WriteLine(result);
    }

    public static void Solve2()
    {
        var result = 0;
        while (true)
        {
            var line = Console.ReadLine();
            if (string.IsNullOrEmpty(line))
                break;

            var values = line.Split(' ').Select(int.Parse).ToArray();
            if (IsSafe(values))
            {
                result++;
                continue;
            }

            for (var i = 0; i < values.Length; i++)
            {
                var toleratedValues = values.Where((_, idx) => idx != i).ToArray();
                if (IsSafe(toleratedValues))
                {
                    result++;
                    break;
                }
            }
        }

        Console.WriteLine(result);
    }

    private static bool IsSafe(IReadOnlyCollection<int> values)
    {
        var pairs = values.Take(values.Count - 1)
            .Zip(values.Skip(1), (before, after) => (before, after))
            .ToArray();

        var isDescending = pairs.First().after < pairs.First().before;
        foreach (var (pair, idx) in pairs.Select((pair, idx) => (pair, idx)))
        {
            var diff = pair.after - pair.before;
            if (isDescending)
            {
                if (0 < diff)
                    return false;
            }
            else
            {
                if (diff < 0)
                    return false;
            }

            if (1 <= Math.Abs(diff) && Math.Abs(diff) <= 3)
                continue;

            return false;
        }

        return true;
    }
}