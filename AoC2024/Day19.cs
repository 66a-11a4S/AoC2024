namespace AoC2024;

public class Day19
{
    public static void Solve1()
    {
        var usablePatterns = Console.ReadLine()!.Split(", ");

        // skip empty line.
        Console.ReadLine();

        var result = 0;
        var knownPatterns = new Dictionary<string, long>();
        while (true)
        {
            var desiredPattern = Console.ReadLine();
            if (string .IsNullOrEmpty(desiredPattern))
                break;

            if (0 < GetBuildPatternCount(desiredPattern, usablePatterns, knownPatterns))
                result++;
        }

        Console.WriteLine(result);
    }

    public static void Solve2()
    {
        var usablePatterns = Console.ReadLine()!.Split(", ");

        // skip empty line.
        Console.ReadLine();

        var result = 0L;
        var knownPatterns = new Dictionary<string, long>();
        while (true)
        {
            var desiredPattern = Console.ReadLine();
            if (string .IsNullOrEmpty(desiredPattern))
                break;

            result += GetBuildPatternCount(desiredPattern, usablePatterns, knownPatterns);
        }

        Console.WriteLine(result);
    }

    private static long GetBuildPatternCount(string desiredPattern, string[] usablePatterns,
        Dictionary<string, long> knownPatterns)
    {
        // 再帰呼び出しで全ての文字列を消費した = パターンを組めた
        if (string.IsNullOrEmpty(desiredPattern))
            return 1;

        // すでに試行済みのパターンならその結果を使う
        if (knownPatterns.TryGetValue(desiredPattern, out var count))
            return count;

        var result = 0L;
        foreach (var pattern in usablePatterns)
        {
            // パターンをはめられるサイズがない
            if (desiredPattern.Length < pattern.Length)
                continue;

            // パターンが合わない
            if (desiredPattern[..(pattern.Length)] != pattern)
                continue;

            // パターンが一致した
            result += GetBuildPatternCount(desiredPattern[(pattern.Length)..], usablePatterns, knownPatterns);
        }

        // 一致するパターンがなかったらその結果を保存しておく
        knownPatterns[desiredPattern] = result;
        return result;
    }
}