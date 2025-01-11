using System.Text.RegularExpressions;

namespace AoC2024;

public class Day24
{
    public enum GateType
    {
        Or,
        And,
        Xor,
    }

    private class Gate(string inputA, string inputB, string output, GateType type)
    {
        public string InputA { get; } = inputA;
        public string InputB { get; } = inputB;
        public GateType Type { get; } = type;
        private string Output { get; } = output;

        public void Update(Dictionary<string, Wire> wireMap)
        {
            var inputA = wireMap[InputA].Value;
            var inputB = wireMap[InputB].Value;
            if (inputA == null || inputB == null)
                return;

            bool? value = Type switch
            {
                GateType.Or => inputA.Value || inputB.Value,
                GateType.And => inputA.Value && inputB.Value,
                GateType.Xor => inputA.Value ^ inputB.Value,
                _ => null
            };
            wireMap[Output].Value = value;
        }
    }

    private class Wire(bool? value)
    {
        public bool? Value { get; set; } = value;
    }

    public static void Solve1()
    {
        Dictionary<string, Wire> wireMap = new();
        List<Gate> gates = [];

        var wirePattern = new Regex(@"(\w+): (1|0)");
        while (true)
        {
            var line = Console.ReadLine();
            if (string.IsNullOrEmpty(line))
                break;

            var wireState = wirePattern.Match(line);
            wireMap[wireState.Groups[1].Value] = new Wire(wireState.Groups[2].Value == "1");
        }

        var gatePattern = new Regex(@"(\w+) (OR|AND|XOR) (\w+) -> (\w+)");
        while (true)
        {
            var line = Console.ReadLine();
            if (string.IsNullOrEmpty(line))
                break;

            var gateParam = gatePattern.Match(line);
            var gateType = gateParam.Groups[2].Value switch
            {
                "OR" => GateType.Or,
                "AND" => GateType.And,
                "XOR" => GateType.Xor,
                _ => throw new InvalidOperationException()
            };

            var inputA = gateParam.Groups[1].Value;
            var inputB = gateParam.Groups[3].Value;
            var output = gateParam.Groups[4].Value;
            if (!wireMap.ContainsKey(inputA))
                wireMap[inputA] = new Wire(null);
            if (!wireMap.ContainsKey(inputB))
                wireMap[inputB] = new Wire(null);
            if (!wireMap.ContainsKey(output))
                wireMap[output] = new Wire(null);

            var gate = new Gate(inputA, inputB, output, gateType);
            gates.Add(gate);
        }

        var resultNodes = wireMap.Where(kvp => kvp.Key[0] == 'z')
            .OrderByDescending(kvp => kvp.Key)
            .Select(kvp =>kvp.Value)
            .ToArray();

        while (resultNodes.Any(value => !value.Value.HasValue))
        {
            foreach (var gate in gates)
            {
                gate.Update(wireMap);
            }
        }

        var result = new string(resultNodes.Select(x => x.Value!.Value ? '1' : '0').ToArray());
        Console.WriteLine(Convert.ToInt64(result, 2));
    }

    public static void Solve2()
    {
        Dictionary<string, Gate> gates = [];

        // skip expected output section.
        while (!string.IsNullOrEmpty(Console.ReadLine()))
        {
        }

        var gatePattern = new Regex(@"(\w+) (OR|AND|XOR) (\w+) -> (\w+)");
        while (true)
        {
            var line = Console.ReadLine();
            if (string.IsNullOrEmpty(line))
                break;

            var gateParam = gatePattern.Match(line);
            var gateType = gateParam.Groups[2].Value switch
            {
                "OR" => GateType.Or,
                "AND" => GateType.And,
                "XOR" => GateType.Xor,
                _ => throw new InvalidOperationException()
            };

            var inputA = gateParam.Groups[1].Value;
            var inputB = gateParam.Groups[3].Value;
            var output = gateParam.Groups[4].Value;
            var gate = new Gate(inputA, inputB, output, gateType);
            gates[output] = gate;
        }

        var result = new List<string>();
        Simulate(gates, result);
        Console.WriteLine(string.Join(",", result.OrderBy(wire => wire)));
    }

    private static void Simulate(Dictionary<string, Gate> gates, List<string> result)
    {
        /*
           こういった構造の組み合わせが繋がっている. いわゆる全加算機.
           このセットを探して、この中で接続が成立するか調べる

           )> = or, |) = and, ))> = xor
           cin----------------
                             |
                             *---
           x---*---          |   ))> xor2(z)
               |   ))> xor1---*---
           y---*---        | |
             | |           |  ---
             | |           |     |) and2-
             | |           ------       |
             | |                        --
             | --                         )> or(cout) --
             |   |) and1------------------
             ----
         */
        var cin = FindOutput(gates, "x00", "y00", GateType.And)!;
        for (var i = 1; i < 45; i++)
        {
            var x = $"x{i:D2}";
            var y = $"y{i:D2}";
            var z = $"z{i:D2}";
            var xor1 = FindOutput(gates, x, y, GateType.Xor)!;
            var and1 = FindOutput(gates, x, y, GateType.And)!;
            var xor2 = FindOutput(gates, cin, xor1, GateType.Xor);
            var and2 = FindOutput(gates, cin, xor1, GateType.And);

            if (xor2 == null || and2 == null) // 1段目の xor や and に対応する出力ワイヤーが見つからない
            {
                (gates[xor1], gates[and1]) = (gates[and1], gates[xor1]);
                result.Add(xor1);
                result.Add(and1);
                Simulate(gates, result);
                return;
            }

            var carry = FindOutput(gates, and1, and2, GateType.Or);
            if (xor2 != z)  // 出力に相当する z が見つからない
            {
                (gates[z], gates[xor2]) = (gates[xor2], gates[z]);
                result.Add(xor2);
                result.Add(z);
                Simulate(gates, result);
                return;
            }

            // ここまでいったら加算機内に変更なし
            cin = carry!;
        }
        return;

        static string? FindOutput(Dictionary<string, Gate> gates, string x, string y, GateType type) =>
            gates.FirstOrDefault(pair =>
                (pair.Value.InputA == x && pair.Value.Type == type && pair.Value.InputB == y) ||
                (pair.Value.InputA == y && pair.Value.Type == type && pair.Value.InputB == x)
            ).Key;
    }
}