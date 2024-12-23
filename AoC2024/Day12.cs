namespace AoC2024;

public class Day12
{
    private struct Vec2(int x, int y)
    {
        public static readonly Vec2 Up = new(0, -1);
        public static readonly Vec2 Right = new(1, 0);
        public static readonly Vec2 Down = new(0, 1);
        public static readonly Vec2 Left = new(-1, 0);
        public static readonly Vec2[] Dirs = [Up, Right, Down, Left];

        public int X { get; set; } = x;
        public int Y { get; set; } = y;

        public static bool operator ==(Vec2 lhs, Vec2 rhs) => lhs.X == rhs.X && lhs.Y == rhs.Y;
        public static bool operator !=(Vec2 lhs, Vec2 rhs) => !(lhs == rhs);
        public static Vec2 operator +(Vec2 lhs, Vec2 rhs) => new(lhs.X + rhs.X, lhs.Y + rhs.Y);
        public static Vec2 operator -(Vec2 lhs, Vec2 rhs) => new(lhs.X - rhs.X, lhs.Y - rhs.Y);
        public static Vec2 operator -(Vec2 a) => new(-a.X, -a.Y);

        public bool Equals(Vec2 other) => X == other.X && Y == other.Y;
        public override bool Equals(object? obj) => obj is Vec2 other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(X, Y);
        public override string ToString() => $"({X}, {Y})";
    }

    private class Node(Vec2 pos, char tile) : IEquatable<Node>
    {
        public Vec2 Position { get; } = pos;
        public char Tile { get; } = tile;

        public HashSet<Node>? Region { get; private set; }
        private HashSet<Node> Connection { get; } = [];

        private Node? Up { get; set; }
        private Node? Right { get; set; }
        private Node? Down { get; set; }
        private Node? Left { get; set; }

        public void BuildConnection(Node[][] nodes)
        {
            // 上
            if (0 < Position.Y && Tile == nodes[Position.Y - 1][Position.X].Tile)
                Connect(nodes[Position.Y - 1][Position.X]);

            // 右
            if (Position.X < nodes[0].Length - 1 && Tile == nodes[Position.Y][Position.X + 1].Tile)
                Connect(nodes[Position.Y][Position.X + 1]);

            // 下
            if (Position.Y < nodes.Length - 1 && Tile == nodes[Position.Y + 1][Position.X].Tile)
                Connect(nodes[Position.Y + 1][Position.X]);

            // 左
            if (0 < Position.X && Tile == nodes[Position.Y][Position.X - 1].Tile)
                Connect(nodes[Position.Y][Position.X - 1]);

            return;

            void Connect(Node opponent)
            {
                Connection.Add(opponent);
                opponent.Connection.Add(this);

                var diffX = opponent.Position.X - Position.X;
                if (diffX == 1)
                    Right = opponent;
                else if (diffX == -1)
                    Left = opponent;

                var diffY = opponent.Position.Y - Position.Y;
                if (diffY == 1)
                    Down = opponent;
                else if (diffY == -1)
                    Up = opponent;
            }
        }

        public void BuildRegion()
        {
            if (Region != null)
                return;

            Region = new HashSet<Node>{this};
            Dfs(this, parent: null);
            return;

            static void Dfs(Node current, Node? parent)
            {
                if (parent != null)
                {
                    if (current.Region != null)
                        return;

                    current.Region = parent.Region;
                    current.Region!.Add(current);
                }

                foreach (var child in current.Connection)
                {
                    if (!child.Equals(parent))
                        Dfs(child, current);
                }
            }
        }

        public bool ContainsNext(Vec2 dir)
        {
            if (dir == Vec2.Up)
                return Up != null;
            if (dir == Vec2.Right)
                return Right != null;
            if (dir == Vec2.Down)
                return Down != null;
            if (dir == Vec2.Left)
                return Left != null;

            return false;
        }

        public bool Equals(Node? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Position == other.Position && Tile == other.Tile;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Node)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Position, Tile);
        }
    }

    public static void Solve1()
    {
        var regions = GetRegions();
        var result = regions.Sum(region => GetFences(region).Count * region.Count);
        Console.WriteLine(result);
    }

    public static void Solve2()
    {
        var regions = GetRegions();
        var result = 0;
        foreach (var region in regions)
        {
            var fences = GetFences(region);
            var edge = CountEdges(fences);
            result += edge * region.Count;
        }
        Console.WriteLine(result);
    }

    private static HashSet<(Vec2 Pos, Vec2 Normal)> GetFences(IReadOnlyCollection<Node> region)
    {
        var result = new HashSet<(Vec2, Vec2)>();
        foreach (var node in region)
        {
            foreach (var dir in Vec2.Dirs)
            {
                if (!node.ContainsNext(dir))
                    result.Add((node.Position, dir));
            }
        }

        return result;
    }

    private static int CountEdges(HashSet<(Vec2 Pos, Vec2 Normal)> fences)
    {
        var edges = 0;
        var seenState = new HashSet<(Vec2 Pos, Vec2 Normal)>();
        foreach (var fence in fences)
        {
            if (seenState.Contains(fence))
                continue;

            seenState.Add(fence);

            // 法線の右手側に進む
            var current = fence;
            var progressDir = GetClockwiseDir(current.Normal, counter: false);
            while (true)
            {
                var next = (current.Pos + progressDir, current.Normal);
                if (!fences.Contains(next) || seenState.Contains(next))
                    break;

                seenState.Add(next);
                current = next;
            }

            current = fence;
            // 法線の左手側に進む
            var counterProgressDir = GetClockwiseDir(current.Normal, counter: true);
            while (true)
            {
                var next = (current.Pos + counterProgressDir, current.Normal);
                if (!fences.Contains(next) || seenState.Contains(next))
                    break;

                seenState.Add(next);
                current = next;
            }

            edges++;
        }

        /*
        foreach (var state in seenState)
        {
            Console.WriteLine($"Tile: {state.Item3} pos: {state.Item1}, dir: {state.Item2}");
        }
        */

        return edges;

        static Vec2 GetClockwiseDir(Vec2 baseVec, bool counter)
        {
            if (baseVec == Vec2.Up)
                return counter ? Vec2.Left : Vec2.Right;
            if (baseVec == Vec2.Right)
                return counter ? Vec2.Up : Vec2.Down;
            if (baseVec == Vec2.Down)
                return counter ? Vec2.Right : Vec2.Left;
            if (baseVec == Vec2.Left)
                return counter ? Vec2.Down : Vec2.Up;

            throw new InvalidOperationException();
        }
    }

    private static HashSet<HashSet<Node>> GetRegions()
    {
        var tiles = new List<string>();
        while (true)
        {
            var line = Console.ReadLine();
            if (string.IsNullOrEmpty(line))
                break;

            tiles.Add(line);
        }

        var width = tiles[0].Length;
        var height = tiles.Count;
        var nodes = Enumerable.Range(0, height).Select(_ => new Node[width]).ToArray();
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                nodes[y][x] = new Node(new Vec2(x, y), tiles[y][x]);
            }
        }

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                nodes[y][x].BuildConnection(nodes);
            }
        }

        var regions = new HashSet<HashSet<Node>>();
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                nodes[y][x].BuildRegion();
                regions.Add(nodes[y][x].Region!);
            }
        }

        return regions;
    }
}