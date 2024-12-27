using System.Text.RegularExpressions;

namespace AoC2024;

public class Day13
{
    public static void Solve1()
    {
        var buttonPattern = new Regex(@"Button [AB]: X\+(\d+), Y\+(\d+)");
        var prizePattern = new Regex(@"Prize: X=(\d+), Y=(\d+)");
        var result = 0L;
        while (true)
        {
            var buttonA = buttonPattern.Match(Console.ReadLine()!);
            var buttonB = buttonPattern.Match(Console.ReadLine()!);
            var prize = prizePattern.Match(Console.ReadLine()!);

            var tryResult = CalculateCost(int.Parse(buttonA.Groups[1].Value),
                int.Parse(buttonA.Groups[2].Value),
                int.Parse(buttonB.Groups[1].Value),
                int.Parse(buttonB.Groups[2].Value),
                int.Parse(prize.Groups[1].Value),
                int.Parse(prize.Groups[2].Value),
                100);

            if (tryResult.HasValue)
            {
                result += tryResult.Value;
                Console.WriteLine(tryResult.Value);
            }

            var separator = Console.ReadLine();
            if (separator == "#")
                break;
        }

        Console.WriteLine(result);
    }

    private static long? CalculateCost(int aMoveX, int aMoveY, int bMoveX, int bMoveY, int prizeX, int prizeY,
        int tryCount)
    {
        var minCost = (long?)null;
        for (var a = 0; a <= tryCount; a++)
        {
            for (var b = 0; b <= tryCount; b++)
            {
                if (aMoveX * a + bMoveX * b == prizeX &&
                    aMoveY * a + bMoveY * b == prizeY)
                {
                    long cost = a * 3 + b;
                    minCost = minCost == null ? cost : Math.Min(minCost.Value, cost);
                }
            }
        }

        return minCost;
    }

    public static void Solve2()
    {
        var buttonPattern = new Regex(@"Button [AB]: X\+(\d+), Y\+(\d+)");
        var prizePattern = new Regex(@"Prize: X=(\d+), Y=(\d+)");
        var result = 0L;
        while (true)
        {
            var buttonA = buttonPattern.Match(Console.ReadLine()!);
            var buttonB = buttonPattern.Match(Console.ReadLine()!);
            var prize = prizePattern.Match(Console.ReadLine()!);

            var tryResult = CalculateCost2(long.Parse(buttonA.Groups[1].Value),
                long.Parse(buttonA.Groups[2].Value),
                long.Parse(buttonB.Groups[1].Value),
                long.Parse(buttonB.Groups[2].Value),
                long.Parse(prize.Groups[1].Value) + 1_000_000_0000_000L,
                long.Parse(prize.Groups[2].Value) + 1_000_000_0000_000L);

            if (tryResult.HasValue)
            {
                result += tryResult.Value;
                Console.WriteLine(tryResult.Value);
            }

            var separator = Console.ReadLine();
            if (separator == "#")
                break;
        }

        Console.WriteLine(result);
    }

    private static long? CalculateCost2(long aMoveX, long aMoveY, long bMoveX, long bMoveY, long pX, long pY)
    {
        // クラメルの公式: https://manabitimes.jp/math/994, https://shikousakugo.wordpress.com/2015/11/22/cramers-rule/
        // 連立一次方程式が成り立つとき、係数を行列を使って求められる
        // aMoveX * a + bMoveX * b = pX
        // aMoveY * a + bMoveY * b = pY

        // det が 0 のときクラメルの方程式は成り立たない
        var det = aMoveX * bMoveY - bMoveX * aMoveY;
        if (det == 0)
            return null;

        // 押す回数がマイナスはありえないケース
        var a = (pX * bMoveY - bMoveX * pY) / det;
        var b = (aMoveX * pY - pX * aMoveY) / det;
        if (a < 0 && b < 0)
            return null;

        if (aMoveX * a + bMoveX * b == pX &&
            aMoveY * a + bMoveY * b == pY)
            return 3 * a + b;

        return null;
    }
}