using System.Text;

namespace AoC2024;

public class Day15
{
    private enum Tile
    {
        None,
        Box,
        Block,
        Robot
    }

    private struct Vec2(int x, int y)
    {
        public int X { get; set; } = x;
        public int Y { get; set; } = y;

        public static bool operator ==(Vec2 lhs, Vec2 rhs) => lhs.X == rhs.X && lhs.Y == rhs.Y;
        public static bool operator !=(Vec2 lhs, Vec2 rhs) => !(lhs == rhs);
        public static Vec2 operator +(Vec2 lhs, Vec2 rhs) => new(lhs.X + rhs.X, lhs.Y + rhs.Y);
        public static Vec2 operator -(Vec2 lhs, Vec2 rhs) => new(lhs.X - rhs.X, lhs.Y - rhs.Y);

        public bool Equals(Vec2 other) => X == other.X && Y == other.Y;
        public override bool Equals(object? obj) => obj is Vec2 other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(X, Y);
    }

    public static void Solve1()
    {
        var robotPosition = new Vec2();
        List<List<Tile>> field = [];
        while (true)
        {
            var line = Console.ReadLine();
            if (string.IsNullOrEmpty(line))
                break;

            var segment = new List<Tile>();
            foreach (var (c, x) in line.Select((c, idx) => (c, idx)))
            {
                switch (c)
                {
                    case '.':
                        segment.Add(Tile.None);
                        break;
                    case '#':
                        segment.Add(Tile.Block);
                        break;
                    case 'O':
                        segment.Add(Tile.Box);
                        break;
                    case '@':
                        segment.Add(Tile.Robot);
                        robotPosition = new Vec2(x, field.Count);
                        break;
                }
            }
            field.Add(segment);
        }

        var moveRequests = GetMoveRequests();
        var width = field[0].Count;
        var height = field.Count;

        foreach (var move in moveRequests)
        {
            robotPosition = MoveRobot(robotPosition, move, width, height, field);
            // PrintState();
        }

        var result = field.Select((line, y) =>
            line.Select((tile, x) => tile == Tile.Box ? 100 * y + x : 0L).Sum()).Sum();
        Console.WriteLine(result);
        return;

        void PrintState()
        {
            var sb = new StringBuilder();
            foreach (var (line, y) in field.Select((line, y) => (line, y)))
            {
                foreach (var (tile,  x) in line.Select((tile, x) => (tile, x)))
                {
                    var c = tile switch
                    {
                        Tile.Block => '#',
                        Tile.Box => 'O',
                        Tile.None => '.',
                        Tile.Robot => '@',
                        _ => throw new InvalidOperationException()
                    };
                    sb.Append(c);
                }
                sb.Append('\n');
            }

            Console.WriteLine(sb.ToString());
        }

        static Vec2 MoveRobot(Vec2 robotPosition, Vec2 vec, int width, int height, List<List<Tile>> field)
        {
            // ロボットの位置から vec 方向に走査
            var pushRequests = new Stack<(Vec2, bool isBox)>();
            pushRequests.Push((robotPosition, isBox: false));

            var pos = robotPosition;
            while (true)
            {
                pos += vec;
                if (pos.X < 0 || width <= pos.X ||
                    pos.Y < 0 || height <= pos.Y)
                    break;

                // 箱のマスがあったら押せ右日圧縮ものに加えておく
                if (field[pos.Y][pos.X] == Tile.Box)
                {
                    pushRequests.Push((pos, isBox: true));
                }

                // 何もないなら、そこまで連なった箱を押す
                if (field[pos.Y][pos.X] == Tile.None)
                {
                    break;
                }

                // 壁があったらその方向には押せない
                if (field[pos.Y][pos.X] == Tile.Block)
                {
                    pushRequests.Clear();
                    break;
                }
            }

            while (pushRequests.Count != 0)
            {
                var (pushFrom, isBox) = pushRequests.Pop();
                var pushTo = pushFrom + vec;
                field[pushFrom.Y][pushFrom.X] = Tile.None;
                field[pushTo.Y][pushTo.X] = isBox ? Tile.Box : Tile.Robot;

                if (!isBox)
                    robotPosition = pushTo;
            }

            return robotPosition;
        }
    }

    private interface IMovable
    {
        void Move(Vec2 move);
    }

    private class Robot(Vec2 position) : IMovable
    {
        public Vec2 Position { get; private set; } = position;

        public void Move(Vec2 move) => Position += move;

        public bool ContainsPosition(Vec2 pos) => Position == pos;
    }

    private class Box(Vec2 position) : IMovable
    {
        public Vec2[] Positions { get; } = [position, position + new Vec2(1, 0)];

        public void Move(Vec2 move)
        {
            for (var idx = 0; idx < Positions.Length; idx++)
            {
                Positions[idx] += move;
            }
        }

        public bool ContainsPosition(Vec2 pos) => Positions.Contains(pos);
    }

    public static void Solve2()
    {
        var robotPosition = new Vec2();
        var boxes = new List<Box>();
        List<List<Tile>> field = [];
        while (true)
        {
            var line = Console.ReadLine();
            if (string.IsNullOrEmpty(line))
                break;

            var segment = new List<Tile>();
            foreach (var (c, x) in line.Select((c, idx) => (c, idx)))
            {
                switch (c)
                {
                    case '.':
                        segment.Add(Tile.None);
                        segment.Add(Tile.None);
                        break;
                    case '#':
                        segment.Add(Tile.Block);
                        segment.Add(Tile.Block);
                        break;
                    case 'O':
                        boxes.Add(new Box(new Vec2(x * 2, field.Count)));
                        segment.Add(Tile.None);
                        segment.Add(Tile.None);
                        break;
                    case '@':
                        segment.Add(Tile.None);
                        segment.Add(Tile.None);
                        robotPosition = new Vec2(x * 2, field.Count);
                        break;
                }
            }
            field.Add(segment);
        }
        // PrintState();

        var robot = new Robot(robotPosition);
        var moveRequests = GetMoveRequests();
        foreach (var move in moveRequests)
        {
            Step(robot, move, boxes, field);
            // PrintState();
        }

        var result = boxes.Select(box => box.Positions[0].Y * 100 + box.Positions[0].X).Sum();
        Console.WriteLine(result);
        return;

        void PrintState()
        {
            var sb = new StringBuilder();
            foreach (var (line, y) in field.Select((line, y) => (line, y)))
            {
                foreach (var (tile,  x) in line.Select((tile, x) => (tile, x)))
                {
                    var pos = new Vec2(x, y);
                    if (robot.ContainsPosition(pos))
                    {
                        sb.Append('@');
                        continue;
                    }

                    if (boxes.Any(box => box.ContainsPosition(pos)))
                    {
                        sb.Append('O');
                        continue;
                    }

                    var c = tile switch
                    {
                        Tile.Block => '#',
                        Tile.None => '.',
                        _ => throw new InvalidOperationException()
                    };
                    sb.Append(c);
                }
                sb.Append('\n');
            }

            Console.WriteLine(sb.ToString());
        }

        static void Step(Robot robot, Vec2 vec, IReadOnlyList<Box> boxes, List<List<Tile>> field)
        {
            var boxIsland = GetIsland(robot);
            if (!boxIsland.Any())
                return;

            foreach (var movable in boxIsland)
            {
                movable.Move(vec);
            }
            return;

            HashSet<IMovable> GetIsland(Robot robot)
            {
                var movablePoints = new Queue<Vec2>();
                movablePoints.Enqueue(robot.Position);

                var canPush = true;
                var movableIsland = new HashSet<IMovable> { robot };

                while (movablePoints.Any())
                {
                    // 押す先の位置を調べる
                    var point = movablePoints.Dequeue();
                    point += vec;

                    // 壁だったら押せないものとする
                    if (field[point.Y][point.X] == Tile.Block)
                    {
                        canPush = false;
                        break;
                    }

                    // 他の箱がその位置にあれば一緒に押せるものとみなす
                    var box = boxes.FirstOrDefault(box => box.ContainsPosition(point));
                    if (box == null || !movableIsland.Add(box))
                        continue;

                    foreach (var boxPoint in box.Positions)
                    {
                        movablePoints.Enqueue(boxPoint);
                    }
                }

                // 押せないと判明したら何も押さないものとして扱う
                if (!canPush)
                   movableIsland.Clear();

                return movableIsland;
            }
        }
    }

    private static IReadOnlyCollection<Vec2> GetMoveRequests()
    {
        var moveRequests = new List<Vec2>();
        while (true)
        {
            var line = Console.ReadLine();
            if (string.IsNullOrEmpty(line))
                break;

            var moves = line.Select(c =>
            {
                return c switch
                {
                    '^' => new Vec2(0, -1),
                    '>' => new Vec2(1, 0),
                    'v' => new Vec2(0, 1),
                    '<' => new Vec2(-1, 0),
                    _ => throw new InvalidOperationException()
                };
            });
            moveRequests.AddRange(moves);
        }

        return moveRequests;
    }
}