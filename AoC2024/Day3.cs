using System.Text.RegularExpressions;

namespace AoC2024;

public class Day3
{
    public static void Solve1()
    {
        var mulPattern = new Regex(@$"mul\((\d+),(\d+)\)");
        var result = 0;
        while (true)
        {
            var line = Console.ReadLine();
            if (string.IsNullOrEmpty(line))
                break;

            var matches = mulPattern.Matches(line);
            foreach (var group in matches.Select(match => match.Groups))
            {
                var lhs = int.Parse(group[1].Value);
                var rhs = int.Parse(group[2].Value);
                result += lhs * rhs;
            }
        }

        Console.WriteLine(result);
    }

    public static void Solve2()
    {
        var mulPattern = new Regex(@$"mul\((\d+),(\d+)\)|do\(\)|don't\(\)");
        var result = 0;
        var doing = true;
        while (true)
        {
            var line = Console.ReadLine();
            if (string.IsNullOrEmpty(line))
                break;

            var matches = mulPattern.Matches(line);
            foreach (var group in matches.Select(match => match.Groups))
            {
                if (group[0].Value == "do()")
                    doing = true;
                else if (group[0].Value == "don't()")
                    doing = false;
                else
                {
                    if (!doing)
                        continue;

                    var lhs = int.Parse(group[1].Value);
                    var rhs = int.Parse(group[2].Value);
                    result += lhs * rhs;
                }
            }
        }

        Console.WriteLine(result);
    }
}