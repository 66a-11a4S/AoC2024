using System.Text;
using System.Text.RegularExpressions;

namespace AoC2024;

public class Day14
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
    }

    private record Robot(Vec2 position, Vec2 velocity)
    {
        public Vec2 Position { get; set; } = position;
        public Vec2 Velocity => velocity;
    }

    public static void Solve1()
    {
        const int width = 101;
        const int height = 103;
        var robots = GenerateRobots();
        for (var step = 0; step < 100; step++)
        {
            Update(robots, width, height);
        }

        var scores = new long[4];
        foreach (var robot in robots)
        {
            var pos = robot.Position;
            if (pos.X == width / 2 || pos.Y == height / 2)
                continue;

            var dimX = pos.X < width / 2 ? 0 : 1;
            var dimY = pos.Y < height / 2 ? 0 : 1;
            var dim = dimX + dimY * 2;
            scores[dim]++;
        }

        var result = scores.Aggregate(1L, (score, dimScore) => score * dimScore);
        Console.WriteLine(result);
    }

    public static void Solve2()
    {
        const int width = 101;
        const int height = 103;
        var robots = GenerateRobots();

        long step = 0;
        while (true)
        {
            Update(robots, width, height);
            step++;

            var sb = new StringBuilder(width * height);
            var placement = OutputRobotPlacements(sb);

            // クリスマスツリー状なので横の直線・縦の直線・斜線があるはず.
            // ひたすらループをまわし、ツリー状の配置になることを確認したら手動で止める
            if (placement.Contains("#####"))
            {
                Console.WriteLine(step);
                Console.WriteLine(placement);
            }

            sb.Clear();
        }

        string OutputRobotPlacements(StringBuilder sb)
        {
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var count = robots.Count(robot => robot.Position.X == x && robot.Position.Y == y);
                    sb.Append(count == 0 ? "." : "#");
                }
                sb.Append('\n');
            }

            return sb.ToString();
        }
    }

    private static IReadOnlyCollection<Robot> GenerateRobots()
    {
        var robots = new List<Robot>();
        var valuePattern = new Regex(@"(-?\d+)");
        while (true)
        {
            var line = Console.ReadLine();
            if (string.IsNullOrEmpty(line))
                break;

            var values = valuePattern.Matches(line);
            var robot = new Robot(
                new Vec2(int.Parse(values[0].Value), int.Parse(values[1].Value)),
                new Vec2(int.Parse(values[2].Value), int.Parse(values[3].Value))
            );
            robots.Add(robot);
        }

        return robots;
    }

    private static void Update(IReadOnlyCollection<Robot> robots, int width, int height)
    {
        foreach (var robot in robots)
        {
            var pos = robot.Position;
            pos += robot.Velocity;
            pos.X = (pos.X + width) % width;
            pos.Y = (pos.Y + height) % height;
            robot.Position = pos;
        }
    }
}