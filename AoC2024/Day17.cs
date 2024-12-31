using System.Text.RegularExpressions;

namespace AoC2024;

public class Day17
{
    public static void Solve1()
    {
        var valuePattern = new Regex(@"\d+");
        var aStr = Console.ReadLine()!;
        var A = long.Parse(valuePattern.Match(aStr).Value);
        var bStr = Console.ReadLine()!;
        var B = long.Parse(valuePattern.Match(bStr).Value);
        var cStr = Console.ReadLine()!;
        var C = long.Parse(valuePattern.Match(cStr).Value);

        // 空行は読み飛ばす
        Console.ReadLine();

        var programStr = Console.ReadLine()!;
        var program = programStr["Program: ".Length..].Split(',').Select(long.Parse).ToArray();
        var outputs = Run(program, A, B, C);
        Console.WriteLine(string.Join(',', outputs));
    }

    public static void Solve2()
    {
        // A, B, C の入力を読み飛ばす
        Console.ReadLine();
        Console.ReadLine();
        Console.ReadLine();

        // 空行は読み飛ばす
        Console.ReadLine();

        var programStr = Console.ReadLine()!;
        var program = programStr["Program: ".Length..].Split(',').Select(long.Parse).ToArray();
        var candidates = GetACandidates(program, program);
        Console.WriteLine(candidates.Min());
    }

    private static IReadOnlyList<long> GetACandidates(long[] program, long[] output)
    {
        // 再帰関数の終了条件
        if (output.Length == 0)
            return new[] { 0L };

        // output の最後から再帰的に、 A の候補となる値を探す
        var prevCandidates = GetACandidates(program, output[1..]).ToArray();

        // 見つかった候補をもとに output と同じになる候補を探す
        var candidates = new List<long>();
        foreach (var candidate in prevCandidates)
        {
            // 下位3bitは推定できないので総当たりで動かす
            for (var literal = 0; literal < 8; literal++)
            {
                // mod 8 は最下位の8進数を1桁だけ残す処理. 8進数を1つ左シフトして候補を探す
                var A = candidate * 8 + literal;

                // A に値が設定されたものとして動かして結果が output と同じになるか調べる
                var result = Run(program, A, 0, 0);
                if (result.SequenceEqual(output))
                    candidates.Add(A);
            }
        }

        return candidates;
    }

    private static List<long> Run(long[] program, long initialA, long initialB, long initialC)
    {
        var A = initialA;
        var B = initialB;
        var C = initialC;
        var outputs = new List<long>();
        var pointer = 0L;

        while (pointer < program.Length)
        {
            var instruction = program[pointer];
            var input = program[pointer + 1];
            switch (instruction)
            {
                case 0:
                    Adv(input);
                    break;
                case 1:
                    Bxl(input);
                    break;
                case 2:
                    Bst(input);
                    break;
                case 3:
                    Jnz(input);
                    break;
                case 4:
                    Bxc(input);
                    break;
                case 5:
                    Out(input);
                    break;
                case 6:
                    Bdv(input);
                    break;
                case 7:
                    Cdv(input);
                    break;

            }

            // jnz 以外か、 jnz のとき A が 0 だった
            if (instruction != 3 || A == 0)
                pointer += 2;
        }

        return outputs;

        void Adv(long input)
        {
            var operand = ComboOperand(input);
            var result = A / (long)Math.Pow(2, operand);
            A = result;
        }

        void Bxl(long input)
        {
            var result = B ^ input;
            B = result;
        }

        void Bst(long input)
        {
            var operand = ComboOperand(input);
            var result = operand % 8;
            B = result;
        }

        void Jnz(long input)
        {
            if (A == 0)
                return;

            pointer = input;
        }

        void Bxc(long _)
        {
            var result = B ^ C;
            B = result;
        }

        void Out(long input)
        {
            var operand = ComboOperand(input);
            var result = operand % 8;
            outputs.Add(result);
        }

        void Bdv(long input)
        {
            var operand = ComboOperand(input);
            var result = A / (long)Math.Pow(2, operand);
            B = result;
        }

        void Cdv(long input)
        {
            var operand = ComboOperand(input);
            var result = A / (long)Math.Pow(2, operand);
            C = result;
        }

        long ComboOperand(long input)
        {
            return input switch
            {
                <= 3 => input,
                4 => A,
                5 => B,
                6 => C,
                _ => throw new InvalidOperationException()
            };
        }
    }
}