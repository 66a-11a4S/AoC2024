using System.Text.RegularExpressions;

namespace AoC2024;
using LengthTable = Dictionary<(char currentKey, char nextKey, int depth), long>;

public class Day21
{
    private struct Vec2(int x, int y)
    {
        public int X { get; set; } = x;
        public int Y { get; set; } = y;

        public static bool operator ==(Vec2 lhs, Vec2 rhs) => lhs.X == rhs.X && lhs.Y == rhs.Y;
        public static bool operator !=(Vec2 lhs, Vec2 rhs) => !(lhs == rhs);
        public static Vec2 operator +(Vec2 lhs, Vec2 rhs) => new(lhs.X + rhs.X, lhs.Y + rhs.Y);
        public static Vec2 operator -(Vec2 lhs, Vec2 rhs) => new(lhs.X - rhs.X, lhs.Y - rhs.Y);

        public bool Equals(Vec2 other) => X == other.X && Y == other.Y;
        public override bool Equals(object? obj) => obj is Vec2 other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(X, Y);
    }

    private class Layout(Dictionary<char, Vec2> buttons, Vec2?[][] layout)
    {
        public Vec2? Get(int x, int y) => layout[y][x];
        public Vec2 Get(char button) => buttons[button];
    }

    public static void Solve1()
    {
        var keypad = BuildKeypadLayout();
        var controller = BuildControllerLayout();
        var layouts = new[]
        {
            keypad,
            controller,
            controller
        };

        var cache = new LengthTable();
        var result = 0L;
        var valuePattern = new Regex(@"\d+");
        while (true)
        {
            var code = Console.ReadLine();
            if (string.IsNullOrEmpty(code))
                break;

            var value = long.Parse(valuePattern.Match(code).Value);
            result += value * OutputMovements(code, layouts, cache);
        }

        Console.WriteLine(result);
    }

    public static void Solve2()
    {
        var keypad = BuildKeypadLayout();
        var controller = BuildControllerLayout();
        var layouts = Enumerable.Repeat(controller, 25).Prepend(keypad).ToArray();

        var cache = new LengthTable();
        var result = 0L;
        var valuePattern = new Regex(@"\d+");
        while (true)
        {
            var code = Console.ReadLine();
            if (string.IsNullOrEmpty(code))
                break;

            var value = long.Parse(valuePattern.Match(code).Value);
            result += value * OutputMovements(code, layouts, cache);
        }

        Console.WriteLine(result);
    }

    private static long OutputMovements(string code, Layout[] layouts, LengthTable cache)
    {
        if (layouts.Length == 0)
            return code.Length;

        var from = 'A';
        var result = 0L;
        foreach (var to in code)
        {
            result += Move(from, to, layouts, cache);
            from = to;
        }

        return result;
    }

    private static long Move(char from, char to, Layout[] layouts, LengthTable cache)
    {
        var state = (from, to, layouts.Length);
        if (!cache.TryGetValue(state, out var value))
        {
            value = MoveImpl(from, to, layouts, cache);
            cache[state] = value;
        }

        return value;

        static long MoveImpl(char from, char to, Layout[] layouts, LengthTable cache)
        {
            var layout = layouts.First();
            var start = layout.Get(from);
            var goal = layout.Get(to);

            var diffX = goal.X - start.X;
            var horizontalMove = new string(diffX < 0 ? '<' : '>', Math.Abs(diffX));
            var diffY = goal.Y - start.Y;
            var verticalMove = new string(diffY < 0 ? '^' : 'v', Math.Abs(diffY));

            var result = long.MaxValue;
            if (layout.Get(start.X, goal.Y) != null) // 縦軸に gap がなければ 縦 -> 横 に移動して A を押す
            {
                var commandVh = OutputMovements(verticalMove + horizontalMove + "A", layouts[1..], cache);
                result = commandVh;
            }

            if (layout.Get(goal.X, start.Y) != null) // 縦軸に gap がなければ 横 → 縦 に移動して A を押す
            {
                var commandHv = OutputMovements(horizontalMove + verticalMove + "A", layouts[1..], cache);
                result = Math.Min(commandHv, result);
            }

            return result;
        }
    }

    private static Layout BuildKeypadLayout()
    {
        /* layout:
           +---+---+---+
           | 7 | 8 | 9 |
           +---+---+---+
           | 4 | 5 | 6 |
           +---+---+---+
           | 1 | 2 | 3 |
           +---+---+---+
               | 0 | A |
               +---+---+
         */
        var buttons = new Dictionary<char, Vec2>
        {
            {'A', new Vec2(2, 3)},
            {'0', new Vec2(1, 3)},
            {'1', new Vec2(0, 2)},
            {'2', new Vec2(1, 2)},
            {'3', new Vec2(2, 2)},
            {'4', new Vec2(0, 1)},
            {'5', new Vec2(1, 1)},
            {'6', new Vec2(2, 1)},
            {'7', new Vec2(0, 0)},
            {'8', new Vec2(1, 0)},
            {'9', new Vec2(2, 0)}
        };

        var layout = new[]
        {
            new Vec2?[] { buttons['7'], buttons['8'], buttons['9'] },
            new Vec2?[] { buttons['4'], buttons['5'], buttons['6'] },
            new Vec2?[] { buttons['1'], buttons['2'], buttons['3'] },
            new Vec2?[] { null, buttons['0'], buttons['A'] },
        };

        return new Layout(buttons, layout);
    }

    private static Layout BuildControllerLayout()
    {
        /*
               +---+---+
               | ^ | A |
           +---+---+---+
           | < | v | > |
           +---+---+---+
         */

        var buttons = new Dictionary<char, Vec2>
        {
            {'A', new Vec2(2, 0)},
            {'^', new Vec2(1, 0)},
            {'>', new Vec2(2, 1)},
            {'v', new Vec2(1, 1)},
            {'<', new Vec2(0, 1)},
        };

        var layout = new[]
        {
            new Vec2?[] {null, buttons['^'], buttons['A'] },
            new Vec2?[] { buttons['<'], buttons['v'], buttons['>'] },
        };
        return new Layout(buttons, layout);
    }
}