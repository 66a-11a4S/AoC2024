namespace AoC2024;

public class Day20
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

    private class Node(Vec2 position)
    {
        public Vec2 Position { get; } = position;
        public HashSet<Node> Children { get; } = [];
        public int StepFromGoal { get; set; } = int.MaxValue;

        public void Connect(Node connectTo)
        {
            Children.Add(connectTo);
            connectTo.Children.Add(this);
        }
    }

    public static void Solve1()
    {
        var (field, nodes, start, end) = Setup();
        var width = field[0].Length;
        var height = field.Count;

        // ゴールまでノードを繋ぐ
        var path = GetPath(start, end, nodes);

        // ゴールから何ステップで到達するかを設定する
        var stepFromGoal = path.Count - 1;
        foreach (var node in path)
        {
            node.StepFromGoal = stepFromGoal;
            stepFromGoal--;
        }

        var shortcutGroup = new Dictionary<int, int>();
        foreach (var node in path)
        {
            var pos = node.Position;

            // 上
            {
                var length = GetShortcutLength(pos, pos + new Vec2(0, -2));
                if (0 < length && !shortcutGroup.TryAdd(length, 1))
                    shortcutGroup[length]++;
            }
            // 右
            {
                var length = GetShortcutLength(pos, pos + new Vec2(2, 0));
                if (0 < length && !shortcutGroup.TryAdd(length, 1))
                    shortcutGroup[length]++;
            }
            // 下
            {
                var length = GetShortcutLength(pos, pos + new Vec2(0, 2));
                if (0 < length && !shortcutGroup.TryAdd(length, 1))
                    shortcutGroup[length]++;
            }
            // 左
            {
                var length = GetShortcutLength(pos, pos + new Vec2(-2, 0));
                if (0 < length && !shortcutGroup.TryAdd(length, 1))
                    shortcutGroup[length]++;
            }
        }

        var result = 0;
        foreach (var group in shortcutGroup.Select(kvp => kvp).OrderBy(kvp => kvp.Key))
        {
            if (100 <= group.Key)
                result += group.Value;
            // Console.WriteLine($"shortcut time: {group.Key}, patterns {group.Value}");
        }

        Console.WriteLine(result);
        return;

        int GetShortcutLength(Vec2 from, Vec2 to)
        {
            if (to.X < 0 || width <= to.X ||
                to.Y < 0 || height <= to.Y)
                return 0;

            if (field[to.Y][to.X] == '#')
                return 0;

            var fromNode = nodes[from.Y][from.X];
            var toNode = nodes[to.Y][to.X];
            return fromNode.StepFromGoal - toNode.StepFromGoal - 2; // ショートカットした経路を2秒かけて進む. (つまり2秒より多く縮んでないと意味がない)
        }
    }

    public static void Solve2()
    {
        var (_, nodes, start, end) = Setup();
        const int maxStep = 20;

        // ゴールまでノードを繋ぐ
        var path = GetPath(start, end, nodes);

        // ゴールから何ステップで到達するかを設定する
        var stepFromGoal = path.Count - 1;
        foreach (var node in path)
        {
            node.StepFromGoal = stepFromGoal;
            stepFromGoal--;
        }

        var shortcutGroup = new Dictionary<int, int>();
        var pathLength = path.Count;
        // 現在の位置から maxStep 内で到達できる全ての組み合わせをリーグ表形式で調べる
        for (var shortCutFrom = 0; shortCutFrom < pathLength; shortCutFrom++)
        {
            for (var shortCutTo = 0; shortCutTo < shortCutFrom; shortCutTo++)
            {
                var from = path[shortCutFrom];
                var to = path[shortCutTo];
                var shortcutDistance = from.Position.ManhattanDistance(to.Position);
                if (maxStep < shortcutDistance)
                    continue;

                var length = GetShortcutLength(from, to, step: shortcutDistance);
                if (0 < length && !shortcutGroup.TryAdd(length, 1))
                    shortcutGroup[length]++;
            }
        }

        var result = 0;
        foreach (var group in shortcutGroup.Select(kvp => kvp).OrderBy(kvp => kvp.Key))
        {
            if (100 <= group.Key)
            {
                result += group.Value;
                // Console.WriteLine($"shortcut time: {group.Key}, patterns {group.Value}");
            }
        }

        Console.WriteLine(result);
        return;

        static int GetShortcutLength(Node from, Node to, int step) =>  to.StepFromGoal - from.StepFromGoal - step;
    }

    private static List<Node> GetPath(Vec2 start, Vec2 end, List<List<Node>> nodes)
    {
        var startNode = nodes[start.Y][start.X];
        var endNode = nodes[end.Y][end.X];
        var path = new List<Node>{startNode};
        var current = startNode;
        Node? previous = null;
        while (current != endNode)
        {
            var next = current.Children.Single(next => next != previous);
            path.Add(next);
            previous = current;
            current = next;
        }

        return path;
    }

    private static (List<string> field, List<List<Node>>, Vec2, Vec2) Setup()
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

        return (field, nodes, start, end);
    }
}