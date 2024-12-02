using System.Text.RegularExpressions;

namespace AoC2024;

public static class Day1
{
    public static void Solve1()
    {
        var regex = new Regex(@"(\d+)(\s+)(\d+)");
        var leftList = new List<long>();
        var rightList = new List<long>();
        while (true)
        {
            var input = Console.ReadLine();
            if (string.IsNullOrEmpty(input))
                break;

            var match = regex.Match(input);
            leftList.Add(long.Parse(match.Groups[1].Value));
            rightList.Add(long.Parse(match.Groups[3].Value));
        }

        leftList.Sort();
        rightList.Sort();

        var result = leftList.Zip(rightList, (left, right) => Math.Abs(left - right)).Sum();
        Console.WriteLine(result);
    }

    public static void Solve2()
    {
        var regex = new Regex(@"(\d+)(\s+)(\d+)");
        var leftList = new List<long>();
        var rightList = new Dictionary<long, int>();
        while (true)
        {
            var input = Console.ReadLine();
            if (string.IsNullOrEmpty(input))
                break;

            var match = regex.Match(input);
            leftList.Add(long.Parse(match.Groups[1].Value));

            var rightValue = long.Parse(match.Groups[3].Value);
            if (!rightList.TryAdd(rightValue, 1))
                rightList[rightValue] += 1;
        }

        long result = 0;
        foreach (var left in leftList)
        {
            result += left * rightList.GetValueOrDefault(left);
        }

        Console.WriteLine(result);
    }
}