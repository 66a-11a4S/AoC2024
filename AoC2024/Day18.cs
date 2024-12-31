namespace AoC2024;

public class Day18
{
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

        public int ManhattanDistance(Vec2 point)
        {
            var diff = this - point;
            return Math.Abs(diff.X) + Math.Abs(diff.Y);
        }
    }

    private class AStarNode
    {
        public const int MaxScore = int.MaxValue;
        public int TotalCost { get; private set; }
        public int ActualCost { get; private set; }
        public int HeuristicCost { get; private set; }

        public AStarNode? Parent { get; private set; }
        public HashSet<AStarNode> Children { get; } = new();

        public Vec2 Position { get; }

        public enum NodeState
        {
            Open,
            Ready,
            Closed,
        }

        public NodeState State { get; private set; }

        public AStarNode(Vec2 position)
        {
            Position = position;

            State = NodeState.Ready;
            ActualCost = MaxScore;
            HeuristicCost = MaxScore;
            TotalCost = MaxScore;
        }

        public void Connect(AStarNode node)
        {
            Children.Add(node);
        }

        public void SetParent(AStarNode? parent)
        {
            Parent = parent;
            ActualCost = parent == null ? 0 : CalculateCost(Parent, this);
        }

        public void Open(AStarNode? parentNode, AStarNode destination)
        {
            SetParent(parentNode);

            // 実コスト
            ActualCost = Parent == null ? 0 : CalculateCost(Parent, this);

            // 推定コスト
            HeuristicCost = Position.ManhattanDistance(destination.Position);

            // 最終的なコスト
            TotalCost = ActualCost + HeuristicCost;

            State = NodeState.Open;
        }

        public void Close() => State = NodeState.Closed;

        public void Reset()
        {
            Children.Clear();
            State = NodeState.Ready;
            ActualCost = MaxScore;
            HeuristicCost = MaxScore;
            TotalCost = MaxScore;
        }

        private static int CalculateCost(AStarNode from, AStarNode to)
        {
            var cost = from.Position.ManhattanDistance(to.Position);
            return from.ActualCost + cost;
        }
    }

    public static void Solve1()
    {
        const int width = 71;
        const int height = 71;
        var (field, nodes) = Setup(width, height);

        const int inputSize = 1024;
        for (var i = 0 ; i < inputSize; i++)
        {
            var line = Console.ReadLine()!;
            var values = line.Split(',').Select(int.Parse).ToArray();
            field[values[1]][values[0]] = '#';
        }

        for (var y = 0; y < field.Count; y++)
        {
            for (var x = 0; x < field[0].Count; x++)
            {
                if (field[y][x] == '#')
                    continue;

                if (x < width - 1 && field[y][x + 1] != '#')
                {
                    nodes[y][x].Connect(nodes[y][x + 1]);
                    nodes[y][x + 1].Connect(nodes[y][x]);
                }

                if (y < height - 1 && field[y + 1][x] != '#')
                {
                    nodes[y][x].Connect(nodes[y + 1][x]);
                    nodes[y + 1][x].Connect(nodes[y][x]);
                }
            }
        }

        // 左上が開始、右下が終了
        var start = nodes.First().First();
        var end = nodes.Last().Last();
        var path = AStarPathFinder.Execute(start, end, nodes.SelectMany(nodeLine => nodeLine).ToArray());
        Console.WriteLine(path.Length - 1);
    }

    public static void Solve2()
    {
        const int width = 71;
        const int height = 71;
        var (field, nodes) = Setup(width, height);

        while (true)
        {
            var line = Console.ReadLine()!;
            if (string.IsNullOrEmpty(line))
                break;

            var values = line.Split(',').Select(int.Parse).ToArray();
            field[values[1]][values[0]] = '#';

            foreach (var node in nodes.SelectMany(nodeLine => nodeLine))
            {
                node.Reset();
            }

            for (var y = 0; y < field.Count; y++)
            {
                for (var x = 0; x < field[0].Count; x++)
                {
                    if (field[y][x] == '#')
                        continue;

                    if (x < width - 1 && field[y][x + 1] != '#')
                    {
                        nodes[y][x].Connect(nodes[y][x + 1]);
                        nodes[y][x + 1].Connect(nodes[y][x]);
                    }

                    if (y < height - 1 && field[y + 1][x] != '#')
                    {
                        nodes[y][x].Connect(nodes[y + 1][x]);
                        nodes[y + 1][x].Connect(nodes[y][x]);
                    }
                }
            }

            var start = nodes.First().First();
            var end = nodes.Last().Last();
            var path = AStarPathFinder.Execute(start, end, nodes.SelectMany(nodeLine => nodeLine).ToArray());
            if (path.Last() != end)
            {
                Console.WriteLine(line);
                break;
            }
        }
    }

    private static (List<List<char>>, List<List<AStarNode>>) Setup(int width, int height)
    {
        var field = new List<List<char>>();
        var nodes = new List<List<AStarNode>>();
        for (var y = 0; y < height; y++)
        {
            var line = new List<char>();
            var nodeLine = new List<AStarNode>();
            for (var x = 0; x < width; x++)
            {
                line.Add('.');
                nodeLine.Add(new AStarNode(new Vec2(x, y)));
            }
            field.Add(line);
            nodes.Add(nodeLine);
        }

        return (field, nodes);
    }

    private class AStarPathFinder
    {
        public static AStarNode[] Execute(AStarNode start, AStarNode end, IReadOnlyList<AStarNode> nodes)
        {
            // 目的地から始まる経路が帰ってくるので reverse
            return SearchPath(start, end, nodes).Reverse().ToArray();
        }

        private static IEnumerable<AStarNode> SearchPath(AStarNode start, AStarNode end,
            IReadOnlyList<AStarNode> allNode)
        {
            if (start == end)
                return new[] { end };

            // 最初だけは手動でOpen
            start.Open(null, end);

            var current = start;
            var clippingCost = int.MaxValue;
            while (true)
            {
                // 周りをOpen
                if (current.ActualCost < clippingCost)
                    OpenAround(current, end);

                // 自身はClose
                current.Close();

                // コストが既知のマスの中で最もスコアが小さいマスを選ぶ
                AStarNode? next = null;
                var minTotalCost = AStarNode.MaxScore;
                var minActualCost = AStarNode.MaxScore;

                // ノードが生成された順("Z" の書き順)に線形探索
                foreach (var node in allNode.Where(x => x.State == AStarNode.NodeState.Open))
                {
                    if (minTotalCost < node.TotalCost)
                        continue;

                    // コストが同じときは実コストだけで比較
                    if (node.TotalCost == minTotalCost && minActualCost <= node.ActualCost)
                        continue;

                    next = node;
                    minTotalCost = node.TotalCost;
                    minActualCost = node.ActualCost;
                }

                // open がない = 袋小路にたどり着いたら終了
                if (next == null)
                    break;

                current = next;

                // 目的のノードの範囲内にたどり着いたらより短い経路の可能性だけ調べる
                if (current == end)
                    clippingCost = current.ActualCost;
            }

            var destination = FindNearestDestinationNode(allNode, AStarNode.NodeState.Closed);
            return GetPath(destination);
        }

        private static void OpenAround(AStarNode node, AStarNode targetNode)
        {
            foreach (var nextNode in node.Children.Where(x => x.State == AStarNode.NodeState.Ready))
            {
                if (nextNode.State == AStarNode.NodeState.Ready)
                {
                    nextNode.Open(node, targetNode);
                    continue;
                }

                // 探索済みの場合でもより近いルートになり得るならルートを差し替える
                var cost = node.Position.ManhattanDistance(nextNode.Position);
                if (node.ActualCost + cost < nextNode.ActualCost)
                    nextNode.SetParent(node);
            }
        }

        private static IEnumerable<AStarNode> GetPath(AStarNode destination)
        {
            var node = destination;
            while (node != null)
            {
                yield return node;
                node = node.Parent;
            }
        }

        private static AStarNode FindNearestDestinationNode(IEnumerable<AStarNode> nodes, AStarNode.NodeState state)
        {
            return nodes.Where(x => x.State == state)
                .OrderBy(x => x.HeuristicCost)
                .First();
        }
    }
}