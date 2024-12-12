namespace AoC2024;

public class Day11
{
    public static void Solve1()
    {
        var state = Console.ReadLine()!.Split(' ');
        for (var i = 0; i < 25; i++)
        {
            state = Blink(state);
        }

        Console.WriteLine(state.Length);
        return;

        static string[] Blink(string[] stones)
        {
            var result = new List<string>();
            foreach (var stone in stones)
            {
                result.AddRange(UpdateStone(stone));
            }

            return result.ToArray();
        }

        static string[] UpdateStone(string stone)
        {
            if (stone == "0")
                return ["1"];

            if (stone.Length % 2 == 0)
            {
                var firstPart = stone[..(stone.Length / 2)];
                var secondPart = stone[(stone.Length / 2)..];
                return
                [
                    firstPart,
                    long.Parse(secondPart).ToString() // NOTE: 先頭の 0 を省略するために一旦 long に変換する
                ];
            }

            var stoneValue = long.Parse(stone);
            return [(stoneValue * 2024).ToString()];
        }
    }

    public static void Solve2()
    {
        var stones = Console.ReadLine()!.Split(' ').Select(long.Parse);
        var cache = new Dictionary<(long, int), long>();
        var result = stones.Select(stone => UpdateStoneCount(stone, 75, cache)).Sum();
        Console.WriteLine(result);
        return;

        /*
        * 答える必要があるのは石の「個数」
        * 石は瞬きのたびに増え続ける
        * 同じ石が何度も現れている
        * x という石を n 回瞬きすると何個になっているかをメモ化する

        0 -> 1 -> 2024 -> 20 24
                               -> 2 0
                                  2   -> 4048 -> 40 48 -> 4 0 4 8
                                  0   -> 1 -> 2024 -> 20 24
                                                            -> 2 0
                                                            -> 2 4
        */
        static long UpdateStoneCount(long stoneValue, int blinks, Dictionary<(long state, int blink), long> cache)
        {
            var key = (stoneValue, blinks);
            if (cache.TryGetValue(key, out var value))
                return value;

            var count = GetStoneCount();
            cache[key] = count;
            return count;

            long GetStoneCount()
            {
                if (key.blinks == 0)
                    return 1;

                if (stoneValue == 0)
                    return UpdateStoneCount(1, blinks - 1, cache);

                var stoneStr = stoneValue.ToString();
                if (stoneStr.Length % 2 == 0)
                {
                    var firstPartStone = long.Parse(stoneStr[..(stoneStr.Length / 2)]);
                    var firstPart = UpdateStoneCount(firstPartStone, blinks - 1, cache);
                    var secondPartStone = long.Parse(stoneStr[(stoneStr.Length / 2)..]);
                    var secondPart = UpdateStoneCount(secondPartStone, blinks - 1, cache);
                    return firstPart + secondPart;
                }

                return UpdateStoneCount(2024 * stoneValue, blinks - 1, cache);
            }
        }
    }
}