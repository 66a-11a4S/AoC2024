namespace AoC2024;

public class Day25
{
    private const int Height = 7;
    private const int MaxGap = Height - 2;

    public static void Solve1()
    {
        var keys = new List<int[]>();
        var locks = new List<int[]>();

        while (true)
        {
            var shape = ReadShape();
            if (shape == null)
                break;

            var (pattern, isLock) = ParsePattern(shape);
            (isLock ? locks : keys).Add(pattern);

            // skip empty line
            Console.ReadLine();
        }

        var result = new HashSet<(int[], int[])>();
        foreach (var key in keys)
        {
            foreach (var @lock in locks)
            {
                if (CanUse(key, @lock))
                {
                    result.Add((key, @lock));
                    // Console.WriteLine($"OK : k {string.Join(',', key)} vs l {string.Join(',', @lock)}");
                }
            }
        }

        Console.WriteLine(result.Count);
    }

    private static bool CanUse(int[] key, int[] @lock)
    {
        var width = key.Length;
        for (var k = 0; k < width; k++)
        {
            if (MaxGap < key[k] + @lock[k])
                return false;
        }

        return true;
    }

    private static List<string>? ReadShape()
    {
        var shape = new List<string>();
        for (var row = 0; row < Height; row++)
        {
            var line = Console.ReadLine();
            if (string.IsNullOrEmpty(line))
                return null;
            shape.Add(line);
        }

        return shape;
    }

    private static (int[] Pattern, bool IsLock) ParsePattern(IReadOnlyList<string> shape)
    {
        var pattern = Enumerable.Repeat(-1, shape[0].Length).ToArray();
        foreach (var row in shape)
        {
            foreach (var (c, idx) in row.Select((c, idx) => (c, idx)))
            {
                if (c == '#')
                    pattern[idx]++;
            }
        }
        var isLock = shape[0].Any(c => c == '#');
        return (pattern, isLock);
    }
}