namespace AoC2024;

public class Day8
{
    private struct Point
    {
        public required int X { get; init; }
        public required int Y { get; init; }

        public static Point operator +(Point lhs, Point rhs) => new(){X = lhs.X + rhs.X, Y = lhs.Y + rhs.Y};
        public static Point operator -(Point lhs, Point rhs) => new(){X = lhs.X - rhs.X, Y = lhs.Y - rhs.Y};
        public static Point operator *(Point lhs, int scale) => new(){X = lhs.X * scale, Y = lhs.Y * scale};
    }

    public static void Solve1()
    {
        var field = new List<string>();
        while (true)
        {
            var line = Console.ReadLine();
            if (string.IsNullOrEmpty(line))
                break;
            else
                field.Add(line);
        }

        var antennaGroups = new Dictionary<char, List<Point>>();
        for (var y = 0; y < field.Count; y++)
        {
            for (var x = 0; x < field.Count; x++)
            {
                var tile = field[y][x];
                if (tile == '.')
                    continue;

                if (!antennaGroups.ContainsKey(tile))
                    antennaGroups[tile] = new List<Point>();

                antennaGroups[tile].Add(new Point {X = x, Y = y});
            }
        }

        var antiNodePoints = new HashSet<Point>();
        foreach (var (_, antennas) in antennaGroups)
        {
            for (var i = 0; i < antennas.Count; i++)
            {
                for (var k = 0; k < antennas.Count; k++)
                {
                    if (i == k)
                        continue;

                    var vec = antennas[k] - antennas[i];
                    var antiNodePoint = antennas[i] - vec;
                    if (0 <= antiNodePoint.X && antiNodePoint.X < field[0].Length &&
                        0 <= antiNodePoint.Y && antiNodePoint.Y < field.Count)
                        antiNodePoints.Add(antiNodePoint);
                }
            }
        }

        Console.WriteLine(antiNodePoints.Count);
    }

    public static void Solve2()
    {
        var field = new List<string>();
        while (true)
        {
            var line = Console.ReadLine();
            if (string.IsNullOrEmpty(line))
                break;
            else
                field.Add(line);
        }

        var antennaGroups = new Dictionary<char, List<Point>>();
        for (var y = 0; y < field.Count; y++)
        {
            for (var x = 0; x < field.Count; x++)
            {
                var tile = field[y][x];
                if (tile == '.')
                    continue;

                if (!antennaGroups.ContainsKey(tile))
                    antennaGroups[tile] = new List<Point>();

                antennaGroups[tile].Add(new Point {X = x, Y = y});
            }
        }

        var antiNodePoints = new HashSet<Point>();
        foreach (var (_, antennas) in antennaGroups)
        {
            for (var i = 0; i < antennas.Count; i++)
            {
                for (var k = 0; k < antennas.Count; k++)
                {
                    if (i == k)
                        continue;

                    var vec = antennas[k] - antennas[i];

                    // 横幅 = 縦幅で、x または y の傾きが 1 以上であることが保証される.
                    // なのでマップのサイズ分サンプリングすれば、マップ内の部直線上の全ての点を網羅できるはず
                    for (var x = 0; x < field[0].Length; x++)
                    {
                        var antiNodePoint = antennas[i] + vec * x;
                        if (0 <= antiNodePoint.X && antiNodePoint.X < field[0].Length &&
                            0 <= antiNodePoint.Y && antiNodePoint.Y < field.Count)
                            antiNodePoints.Add(antiNodePoint);
                    }
                }
            }
        }

        Console.WriteLine(antiNodePoints.Count);
    }
}