namespace AoC2024;

public class Day7
{
    public static void Solve1()
    {
        var result = 0L;
        while (true)
        {
            var line = Console.ReadLine();
            if (string.IsNullOrEmpty(line))
                break;

            // parse line
            var input = line.Split(':');
            var expected = long.Parse(input[0]);
            var values = input[1]
                .Split(' ')
                .Where(value => !string.IsNullOrEmpty(value))
                .Select(value => long.Parse(value.Replace(" ", string.Empty)))
                .ToArray();

            // validate
            if (Dfs(current: values[0], idx: 1, values, expected))
                result += expected;
        }

        Console.WriteLine(result);
        return;

        static bool Dfs(long current, int idx, long[] values, long expected)
        {
            if (idx == values.Length)
                return current == expected;

            var added = current + values[idx];
            if (Dfs(added, idx + 1, values, expected))
                return true;

            var multiplied = current * values[idx];
            if (Dfs(multiplied, idx + 1, values, expected))
                return true;

            return false;
        }
    }

    public static void Solve2()
    {
        var result = 0L;
        while (true)
        {
            var line = Console.ReadLine();
            if (string.IsNullOrEmpty(line))
                break;

            // parse line
            var input = line.Split(':');
            var expected = input[0];
            var values = input[1]
                .Split(' ')
                .Where(value => !string.IsNullOrEmpty(value))
                .Select(value => value.Replace(" ", string.Empty))
                .ToArray();

            // validate
            if (Dfs(current: values[0], idx: 1, values, expected))
                result += long.Parse(expected);
        }

        Console.WriteLine(result);
        return;

        static bool Dfs(string current, int idx, string[] values, string expected)
        {
            if (idx == values.Length)
                return current == expected;

            var added = long.Parse(current) + long.Parse(values[idx]);
            if (Dfs(added.ToString(), idx + 1, values, expected))
                return true;

            var multiplied = long.Parse(current) * long.Parse(values[idx]);
            if (Dfs(multiplied.ToString(), idx + 1, values, expected))
                return true;

            // 12 combine 345 -> 12 * 100 + 345 = 12345
            // NOTE: 数値型のまま結合しようとすると途中で桁落ちが発生して正しい結果を取得できない
            // var digits = Math.Ceiling(Math.Log10(values[idx]));
            // var combined = current * (long)Math.Pow(10, digits) + values[idx];
            var combined = current + values[idx];
            if (Dfs(combined, idx + 1, values, expected))
                return true;

            return false;
        }
    }
}