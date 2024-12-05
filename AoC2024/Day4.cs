namespace AoC2024;

public class Day4
{
    public static void Solve1()
    {
        var result = 0;
        List<string> valueTable = new();
        while (true)
        {
            var line = Console.ReadLine();
            if (string.IsNullOrEmpty(line))
                break;

            valueTable.Add(line);
        }

        var width = valueTable[0].Length;
        var height = valueTable.Count;
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                result += CountXMAS(valueTable, x, y);
            }
        }

        Console.WriteLine(result);
        return;

        static int CountXMAS(List<string> valueTable, int startX, int startY)
        {
            var width = valueTable[0].Length;
            var height = valueTable.Count;

            if (valueTable[startY][startX] != 'X')
                return 0;

            var result = 0;
            // 左に走査
            if (3 <= startX)
            {
                var substr = new List<char>();
                for (var i = 0; i < 4; i++)
                {
                    substr.Add(valueTable[startY][startX - i]);
                }

                var str = new string(substr.ToArray());
                if (str == "XMAS")
                    result++;
            }

            // 右に走査
            if (startX < width - 3)
            {
                var substr = new List<char>();
                for (var i = 0; i < 4; i++)
                {
                    substr.Add(valueTable[startY][startX + i]);
                }

                var str = new string(substr.ToArray());
                if (str == "XMAS")
                    result++;
            }

            // 上に走査
            if (3 <= startY)
            {
                var substr = new List<char>();
                for (var i = 0; i < 4; i++)
                {
                    substr.Add(valueTable[startY - i][startX]);
                }

                var str = new string(substr.ToArray());
                if (str == "XMAS")
                    result++;
            }

            // 下に走査
            if (startY < height - 3)
            {
                var substr = new List<char>();
                for (var i = 0; i < 4; i++)
                {
                    substr.Add(valueTable[startY + i][startX]);
                }

                var str = new string(substr.ToArray());
                if (str == "XMAS")
                    result++;
            }

            // 左上
            if (3 <= startX && 3 <= startY)
            {
                var substr = new List<char>();
                for (var i = 0; i < 4; i++)
                {
                    substr.Add(valueTable[startY - i][startX - i]);
                }

                var str = new string(substr.ToArray());
                if (str == "XMAS")
                    result++;
            }

            // 右上
            if (startX < width - 3 && 3 <= startY)
            {
                var substr = new List<char>();
                for (var i = 0; i < 4; i++)
                {
                    substr.Add(valueTable[startY - i][startX + i]);
                }

                var str = new string(substr.ToArray());
                if (str == "XMAS")
                    result++;
            }

            // 左下
            if (3 <= startX && startY < height - 3)
            {
                var substr = new List<char>();
                for (var i = 0; i < 4; i++)
                {
                    substr.Add(valueTable[startY + i][startX - i]);
                }

                var str = new string(substr.ToArray());
                if (str == "XMAS")
                    result++;
            }


            // 右下
            if (startX < width - 3 && startY < height - 3)
            {
                var substr = new List<char>();
                for (var i = 0; i < 4; i++)
                {
                    substr.Add(valueTable[startY + i][startX + i]);
                }

                var str = new string(substr.ToArray());
                if (str == "XMAS")
                    result++;
            }

            return result;
        }
    }

    static readonly string[][] Patterns =
    [
        [
            "M.M",
            ".A.",
            "S.S",
        ],
        [
            "S.M",
            ".A.",
            "S.M"
        ],
        [
            "S.S",
            ".A.",
            "M.M"
        ],
        [
            "M.S",
            ".A.",
            "M.S"
        ]
    ];


    public static void Solve2()
    {
        var result = 0;
        List<string> valueTable = new();
        while (true)
        {
            var line = Console.ReadLine();
            if (string.IsNullOrEmpty(line))
                break;

            valueTable.Add(line.Replace('X','.'));
        }

        var width = valueTable[0].Length;
        var height = valueTable.Count;
        for (var y = 1; y < height - 1; y++)
        {
            for (var x = 1; x < width - 1; x++)
            {
                result += CountX_MAS(valueTable, x, y);
            }
        }

        Console.WriteLine(result);
        return;

        static int CountX_MAS(List<string> valueTable, int startX, int startY)
        {
            if (valueTable[startY][startX] != 'A')
                return 0;

            var fromX = startX - 1;
            var toX = startX + 2;
            var l1 = valueTable[startY - 1][fromX..toX];
            var l2 = valueTable[startY][fromX..toX];
            var l3 = valueTable[startY + 1][fromX..toX];
            var grid = new[] { l1, l2, l3 };
            return Patterns.Count(pattern => IsMatch(pattern, grid));
        }

        static bool IsMatch(string[] pattern, string[] input)
        {
            var match = true;
            for (var i = 0; i < 3; i++)
            {
                for (var k = 0; k < 3; k++)
                {
                    if (pattern[i][k] == '.')
                        continue;

                    if (pattern[i][k] == input[i][k])
                        continue;

                    match = false;
                    break;
                }
            }

            return match;
        }
    }
}