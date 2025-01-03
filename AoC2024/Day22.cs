namespace AoC2024;

public class Day22
{
    public static void Solve1()
    {
        var result = 0L;
        while (true)
        {
            var initialNumber = Console.ReadLine();
            if (string.IsNullOrEmpty(initialNumber))
                break;

            var secretNumber = long.Parse(initialNumber);
            for (var i = 0; i < 2000; i++)
            {
                var next = Update(secretNumber);
                secretNumber = next;
            }

            result += secretNumber;
        }

        Console.WriteLine(result);
    }

    public static void Solve2()
    {
        var benefitsByPriceChangePatterns = new Dictionary<(long, long, long, long), long>();
        while (true)
        {
            var initialNumber = Console.ReadLine();
            if (string.IsNullOrEmpty(initialNumber))
                break;

            var secretNumber = long.Parse(initialNumber);
            var priceChangePatterns = GetPriceChangePatterns(secretNumber);
            foreach (var pattern in priceChangePatterns.Keys)
            {
                benefitsByPriceChangePatterns[pattern] = benefitsByPriceChangePatterns.GetValueOrDefault(pattern) + priceChangePatterns[pattern];
            }
        }

        Console.WriteLine(benefitsByPriceChangePatterns.Values.Max());
    }

    private static Dictionary<(long, long, long, long), long> GetPriceChangePatterns(long initialNumber)
    {
        var prices = new List<long>(2000);
        var secretNumber = initialNumber;
        for (var i = 0; i < 2000; i++)
        {
            var next = Update(secretNumber);
            secretNumber = next;
            prices.Add(secretNumber % 10);
        }

        var priceChanges = prices.Zip(prices.Skip(1), (from, to) => to - from).ToArray();

        // 直近4回の差分と、最後の差分で得られる price を記録する).
        // 問題の性質的に 5,4,4,6 と 3,2,2,4 のような同じ周期だが price が違うパターンはない(はず)
        var priceChangePatterns = new Dictionary<(long, long, long, long), long>();
        for (var i = 0; i < prices.Count - 4; i++)
        {
            var sequence = (priceChanges[i], priceChanges[i + 1], priceChanges[i + 2], priceChanges[i + 3]);
            if (!priceChangePatterns.ContainsKey(sequence))
                priceChangePatterns[sequence] = prices[i + 4];
        }

        return priceChangePatterns;
    }

    private static long Update(long original)
    {
        var secretNumber = Mix(original * 64, original);
        secretNumber = Prune(secretNumber);

        secretNumber = Mix(secretNumber / 32, secretNumber);
        secretNumber = Prune(secretNumber);

        secretNumber = Mix(secretNumber * 2048, secretNumber);
        secretNumber = Prune(secretNumber);
        return secretNumber;

        static long Mix(long arg, long secretNumber) => arg ^ secretNumber;
        static long Prune(long secretNumber) => secretNumber % 16777216;
    }
}