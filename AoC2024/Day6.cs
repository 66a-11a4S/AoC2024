namespace AoC2024;

public class Day6
{
    private enum Direction
    {
        Up,
        Right,
        Down,
        Left
    }

    public static void Solve1()
    {
        var x = 0;
        var y = 0;
        var table = new List<string>();
        while (true)
        {
            var line = Console.ReadLine();
            if (string.IsNullOrEmpty(line))
                break;

            if (line.Contains('^'))
            {
                x = line.IndexOf('^');
                y = table.Count;
                line = line.Replace('^', '.');
            }

            table.Add(line);
        }

        var direction = Direction.Up;
        var seenTable = new HashSet<(int, int)>();
        while (true)
        {
            var (offsetX, offsetY) = GetForward(direction);
            if (x + offsetX < 0 || table[0].Length <= x + offsetX)
                break;
            if (y + offsetY < 0 || table.Count <= y + offsetY)
                break;

            if (table[y + offsetY][x + offsetX] == '#')
            {
                direction = NextDirection(direction);
                continue;
            }

            x += offsetX;
            y += offsetY;
            seenTable.Add((x, y));
        }

        Console.WriteLine(seenTable.Count);
    }

    public static void Solve2()
    {
        var startX = 0;
        var startY = 0;
        var table = new List<char[]>();
        while (true)
        {
            var line = Console.ReadLine();
            if (string.IsNullOrEmpty(line))
                break;

            if (line.Contains('^'))
            {
                startX = line.IndexOf('^');
                startY = table.Count;
                line = line.Replace('^', '.');
            }

            table.Add(line.ToCharArray());
        }

        var result = 0;
        for (var y = 0; y < table.Count; y++)
        {
            for (var x = 0; x < table[0].Length; x++)
            {
                if (x == startX && y == startY)
                    continue;

                if (table[y][x] == '#')
                    continue;

                table[y][x] = '#';
                if (WillBeLoop(startX, startY, Direction.Up, table))
                    result++;
                table[y][x] = '.';
            }
        }

        Console.WriteLine(result);
        return;

        static bool WillBeLoop(int startX, int startY, Direction startDirection, IReadOnlyList<char[]> table)
        {
            var x = startX;
            var y = startY;
            var direction = startDirection;
            var seenTable = new HashSet<(int, int, Direction)>();

            while (true)
            {
                if (seenTable.Contains((x, y, direction)))
                    return true;
                else
                    seenTable.Add((x, y, direction));

                var (offsetX, offsetY) = GetForward(direction);

                // マップ外に移動するなら終了
                var nextX = x + offsetX;
                var nextY = y + offsetY;
                if (nextX < 0 || table[0].Length <= nextX)
                    break;
                if (nextY < 0 || table.Count <= nextY)
                    break;

                // 正面が障害物なら現在地から右を向く
                if (table[nextY][nextX] == '#')
                {
                    direction = NextDirection(direction);
                    continue;
                }

                x = nextX;
                y = nextY;
            }

            return false;
        }
    }

    private static Direction NextDirection(Direction current) => (Direction)(((int)current + 1) % 4);

    private static (int X, int Y) GetForward(Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                return (0, -1);
            case Direction.Right:
                return (1, 0);
            case Direction.Down:
                return (0, 1);
            case Direction.Left:
                return (-1, 0);
            default:
                throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
        }
    }
}