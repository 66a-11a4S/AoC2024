namespace AoC2024;

public class Day5
{
    // NOTE: 入力例のルールを繋げてみると順番が循環している
    // - 42 < 24 < 94 < 61 < 49 < 42
    // - 全てのルールの依存関係が重要ではない
    // - 2つずつ比較すれば済む
    public static void Solve1()
    {
        // page ordering rules
        var orderingRules = new Dictionary<string, List<string>>();
        while (true)
        {
            var line = Console.ReadLine();
            if (string.IsNullOrEmpty(line))
                break;

            var values = line.Split('|');
            var from = values[0];
            var to = values[1];
            if (!orderingRules.ContainsKey(from))
                orderingRules[from] = new List<string>();

            orderingRules[from].Add(to);
        }

        var result = 0;

        // pages to produce in each update
        while (true)
        {
            var line = Console.ReadLine();
            if (string.IsNullOrEmpty(line))
                break;

            var values = line.Split(',');
            if (IsHit(values, orderingRules))
                result += int.Parse(values[values.Length / 2]);
        }

        Console.WriteLine(result);
        return;

        static bool IsHit(IReadOnlyList<string> values, IReadOnlyDictionary<string, List<string>> orderingRules)
        {
            for (var i = 0; i < values.Count - 1; i++)
            {
                for (var k = i + 1; k < values.Count; k++)
                {
                    var lhs = values[i];
                    var rhs = values[k];

                    // 左辺がルールにない
                    if (!orderingRules.TryGetValue(lhs, out var rule))
                        return false;

                    // 左辺のルールの中に右辺がない
                    if (!rule.Contains(rhs))
                        return false;
                }
            }

            return true;
        }
    }

    public static void Solve2()
    {
        // page ordering rules
        var orderingRules = new Dictionary<string, List<string>>();
        while (true)
        {
            var line = Console.ReadLine();
            if (string.IsNullOrEmpty(line))
                break;

            var values = line.Split('|');
            var from = values[0];
            var to = values[1];
            if (!orderingRules.ContainsKey(from))
                orderingRules[from] = new List<string>();

            orderingRules[from].Add(to);
        }

        var result = 0;

        // pages to produce in each update
        while (true)
        {
            var line = Console.ReadLine();
            if (string.IsNullOrEmpty(line))
                break;

            var values = line.Split(',');
            var orderedValues = Sort2(values.ToList(), orderingRules);
            if (!values.SequenceEqual(orderedValues))
                result += int.Parse(orderedValues[values.Length / 2]);
        }

        Console.WriteLine(result);
        return;

        static IReadOnlyList<string> Sort2(List<string> values,
            IReadOnlyDictionary<string, List<string>> orderingRules)
        {
            for (var i = 0; i < values.Count - 1; i++)
            {
                for (var k = i + 1; k < values.Count; k++)
                {
                    var lhs = values[i];
                    var rhs = values[k];
                    // lhs が左辺のルールがあり、その中に rhs がある
                    if (orderingRules.TryGetValue(lhs, out var rule) && rule.Contains(rhs))
                        continue;

                    // rhs が左辺のルールがあり、その中に lhs がある
                    if (orderingRules.TryGetValue(rhs, out var inverseRule) && inverseRule.Contains(lhs))
                        (values[k], values[i]) = (values[i], values[k]);
                }
            }

            return values;
        }
    }
}
