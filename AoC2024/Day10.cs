namespace AoC2024;

public class Day10
{
    private enum Direction
    {
        Up,
        Right,
        Down,
        Left
    }

    private const int MinHeight = 0;
    private const int MaxHeight = 9;
    private const int Invalid = -1;

    public static void Solve1()
    {
        var field = new List<List<int>>();

        while (true)
        {
            var line = Console.ReadLine();
            if (string.IsNullOrEmpty(line))
                break;

            var values = line.Select(c =>
            {
                if (c == '.')
                    return Invalid;
                return c - '0';
            }).ToList();
            field.Add(values);
        }

        var peekPoints = new List<(int X, int Y)>();
        for (var y = 0; y < field.Count; y++)
        {
            for (var x = 0; x < field[0].Count; x++)
            {
                if (field[y][x] == MaxHeight)
                    peekPoints.Add((x, y));
            }
        }

        var result = 0;
        for (var y = 0; y < field.Count; y++)
        {
            for (var x = 0; x < field[0].Count; x++)
            {
                if (field[y][x] == MinHeight)
                {
                    var seenTable = CreateSeenTable(field[0].Count, field.Count);
                    DFS(x, y, MinHeight, prevDirection: null, field, seenTable);
                    result += peekPoints.Count(p => seenTable[p.Y][p.X]);
                }
            }
        }

        Console.WriteLine(result);
        return;

        static List<List<bool>> CreateSeenTable(int width, int height)
        {
            var result = new List<List<bool>>();
            for (var y = 0; y < height; y++)
            {
                result.Add(Enumerable.Repeat(false, width).ToList());
            }

            return result;

            // これは最初に作った配列の参照を repeat するので NG
            // return Enumerable.Repeat(Enumerable.Repeat(0, width).ToList(), height).ToList():
        }

        static void DFS(int x, int y, int height, Direction? prevDirection, List<List<int>> field,
            List<List<bool>> seenTable)
        {
            if (x < 0 || field[0].Count <= x ||
                y < 0 || field.Count <= y)
                return;

            if (field[y][x] != height)
                return;

            seenTable[y][x] = true;

            if (field[y][x] == MaxHeight)
                return;

            // 上
            if (prevDirection != Direction.Down)
                DFS(x, y - 1, height + 1, Direction.Up, field, seenTable);
            // 右
            if (prevDirection != Direction.Left)
                DFS(x + 1, y, height + 1, Direction.Right, field, seenTable);
            // 下
            if (prevDirection != Direction.Up)
                DFS(x, y + 1, height + 1, Direction.Down, field, seenTable);
            // 左
            if (prevDirection != Direction.Right)
                DFS(x - 1, y, height + 1, Direction.Left, field, seenTable);
        }
    }

    public static void Solve2()
    {
        var field = new List<List<int>>();

        while (true)
        {
            var line = Console.ReadLine();
            if (string.IsNullOrEmpty(line))
                break;

            var values = line.Select(c =>
            {
                if (c == '.')
                    return Invalid;
                return c - '0';
            }).ToList();
            field.Add(values);
        }

        var peekPoints = new List<(int X, int Y)>();
        for (var y = 0; y < field.Count; y++)
        {
            for (var x = 0; x < field[0].Count; x++)
            {
                if (field[y][x] == MaxHeight)
                    peekPoints.Add((x, y));
            }
        }

        var result = 0;
        for (var y = 0; y < field.Count; y++)
        {
            for (var x = 0; x < field[0].Count; x++)
            {
                if (field[y][x] == MinHeight)
                {
                    result += DFS(x, y, MinHeight, prevDirection: null, field);
                }
            }
        }

        Console.WriteLine(result);
        return;

        static int DFS(int x, int y, int height, Direction? prevDirection, List<List<int>> field)
        {
            if (x < 0 || field[0].Count <= x ||
                y < 0 || field.Count <= y)
                return 0;

            if (field[y][x] != height)
                return 0;

            if (field[y][x] == MaxHeight)
                return 1;

            var result = 0;
            // 上
            if (prevDirection != Direction.Down)
                result += DFS(x, y - 1, height + 1, Direction.Up, field);
            // 右
            if (prevDirection != Direction.Left)
                result += DFS(x + 1, y, height + 1, Direction.Right, field);
            // 下
            if (prevDirection != Direction.Up)
                result += DFS(x, y + 1, height + 1, Direction.Down, field);
            // 左
            if (prevDirection != Direction.Right)
                result += DFS(x - 1, y, height + 1, Direction.Left, field);

            return result;
        }
    }
}