namespace AoC2024;

public class Day16
{
    private struct Vec2(int x, int y)
    {
        public static readonly Vec2 North = new(0, -1);
        public static readonly Vec2 East = new(1, 0);
        public static readonly Vec2 South = new(0, 1);
        public static readonly Vec2 West = new(-1, 0);
        public static readonly Vec2[] Dirs = [North, East, South, West];

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
    }

    private class Node(Vec2 position)
    {
        public Vec2 Position { get; } = position;
        private Dictionary<Vec2, Node> Connection { get; } = new();
        private Dictionary<Vec2, Node> InverseConnection { get; } = new();

        public void Connect(Node connectTo)
        {
            var diff = connectTo.Position - Position;
            Connection[diff] = connectTo;
            connectTo.Connection[-diff] = this;
            InverseConnection[-diff] = connectTo;
            connectTo.InverseConnection[diff] = this;
        }

        public IReadOnlyCollection<(Node next, Vec2 dir, int cost)> NextStep(Vec2 currentDir, bool moveForward)
        {
            return Vec2.Dirs
                .Select(dir =>
                {
                    if (dir == currentDir)
                    {
                        var connection = moveForward ? Connection : InverseConnection;
                        return (connection.GetValueOrDefault(dir)!, dir, 1);
                    }

                    return dir == -currentDir ? (this, dir, 2000) : (this, dir, 1000);
                })
                .Where(t => t.Item1 != null)
                .ToArray();
        }
    }

    public static void Solve1()
    {
        var (nodes, start, end) = Setup();
        var costMap = Dijkstra(nodes[start.Y][start.X], Vec2.East);
        var endCost = costMap.Where(kvp => kvp.Key.Node.Position == end).Select(kvp => kvp.Value);
        Console.WriteLine(endCost.Min());
        return;

        static Dictionary<(Node Node, Vec2 Dir), int> Dijkstra(Node start, Vec2 startDir)
        {
            var queue = new PriorityQueue<(Node node, Vec2 dir), int>();
            var costMap = new Dictionary<(Node, Vec2), int>();

            queue.Enqueue((start, startDir), 0);

            while (queue.TryDequeue(out var current, out var totalCost))
            {
                var costs = current.node.NextStep(current.dir, moveForward: true);
                foreach (var (next, dir, cost) in costs)
                {
                    var nextCost = cost + totalCost;
                    var nextState = (next, dir);
                    if (nextCost < costMap.GetValueOrDefault(nextState, int.MaxValue))
                    {
                        costMap[nextState] = nextCost;
                        queue.Enqueue(nextState, nextCost);
                    }
                }
            }

            // 各状態への最短経路を返す
            return costMap;
        }
    }

    public static void Solve2()
    {
        var (nodes, start, end) = Setup();

        // まず end -> start へのコストを調べる.
        // 戻り値として各マスからゴールまでの最短距離を取得する
        var costFromGoalMap = BuildCostFromGoalMap(end, nodes);

        // 次に start -> end に向けてコストを調べる
        var bestSpots = FindBestSpots(nodes[start.Y][start.X], startDir: Vec2.East, costFromGoalMap);
        Console.WriteLine(bestSpots.DistinctBy(p => p.Node).Count());
        return;

        static Dictionary<(Node Node, Vec2 Dir), int> BuildCostFromGoalMap(Vec2 end, List<List<Node>> nodes)
        {
            var queue = new PriorityQueue<(Node node, Vec2 dir), int>();
            var costMap = new Dictionary<(Node, Vec2), int>();

            // 初期状態を手動で入れる
            foreach (var dir in Vec2.Dirs)
            {
                queue.Enqueue((nodes[end.Y][end.X], dir), 0);
                costMap[(nodes[end.Y][end.X], dir)] = 0;
            }

            while (queue.TryDequeue(out var current, out var totalCost))
            {
                var costs = current.node.NextStep(current.dir, moveForward: false);
                foreach (var (next, dir, cost) in costs)
                {
                    var nextCost = cost + totalCost;
                    var nextState = (next, dir);
                    if (nextCost < costMap.GetValueOrDefault(nextState, int.MaxValue))
                    {
                        costMap[nextState] = nextCost;
                        queue.Enqueue(nextState, nextCost);
                    }
                }
            }

            // Dijkstra 法により各状態への最短経路が記録されている
            return costMap;
        }

        static HashSet<(Node Node, Vec2 Dir)> FindBestSpots(Node start, Vec2 startDir,
            Dictionary<(Node Node, Vec2 Dir), int> costFromGoalMap)
        {
            var queue = new PriorityQueue<(Node node, Vec2 dir), int>();
            queue.Enqueue((start, startDir), 0);
            var bestSpots = new HashSet<(Node Node, Vec2)>{(start, startDir)};
            var maxCost = costFromGoalMap[(start, startDir)];

            while (queue.TryDequeue(out var current, out var totalCost))
            {
                foreach (var (next, dir, cost) in current.node.NextStep(current.dir, moveForward: true))
                {
                    var nextCost = cost + totalCost;
                    var nextState = (next, dir);
                    var remainingConst = maxCost - costFromGoalMap[nextState];

                    // 始点からの最短距離 と 終点からの最短距離が同じなら、最短距離の別ルートと考えられる
                    if (nextCost == remainingConst && !bestSpots.Contains(nextState))
                    {
                        queue.Enqueue(nextState, nextCost);
                        bestSpots.Add(nextState);
                    }
                }
            }

            // 各状態への最短経路を返す
            return bestSpots;
        }
    }

    private static (List<List<Node>>, Vec2, Vec2) Setup()
    {
        Vec2 start = new();
        Vec2 end = new();

        var nodes = new List<List<Node>>();
        var field = new List<string>();
        while (true)
        {
            var line = Console.ReadLine();
            if (string.IsNullOrEmpty(line))
                break;

            if (line.Contains('S'))
            {
                start.X = line.IndexOf('S');
                start.Y = field.Count;
            }
            else if (line.Contains('E'))
            {
                end.X = line.IndexOf('E');
                end.Y = field.Count;
            }

            var nodeLine = line.Select((c, idx) => (c, idx))
                .Select(t => t.c == '#' ? null : new Node(new Vec2(t.idx, field.Count)))
                .ToList();

            field.Add(line);
            nodes.Add(nodeLine!);
        }

        for (var y = 0; y < field.Count; y++)
        {
            for (var x = 0; x < field[0].Length; x++)
            {
                if (field[y][x] == '#')
                    continue;

                if (x < field[0].Length && field[y][x + 1] != '#')
                    nodes[y][x].Connect(nodes[y][x + 1]);

                if (y < field.Count && field[y + 1][x] != '#')
                    nodes[y][x].Connect(nodes[y + 1][x]);
            }
        }

        return (nodes, start, end);
    }
}