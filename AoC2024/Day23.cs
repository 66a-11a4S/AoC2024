namespace AoC2024;

public class Day23
{
    public static void Solve1()
    {
        var graph = BuildGraph();
        var islands = new HashSet<(string A, string B, string C)>();

        foreach (var a in graph)
        {
            foreach (var b in a.Value)
            {
                foreach (var c in a.Value)
                {
                    if (b == c)
                        continue;

                    if (graph[b].Contains(c) || graph[c].Contains(b))
                    {
                        var sequence = new[]{a.Key, b, c}.OrderBy(str => str).ToArray();
                        islands.Add((sequence[0], sequence[1], sequence[2]));
                    }
                }
            }
        }

        var result = islands.Count(g => g.A[0] == 't' || g.B[0] == 't' || g.C[0] == 't');
        Console.WriteLine(result);
    }

    public static void Solve2()
    {
        var graph = BuildGraph();
        var islands = graph.Keys.ToHashSet();

        // 問題の性質から最大の集合は1種類になるはず
        while (islands.Count != 1)
        {
            islands = Update(islands, graph);
            // Console.WriteLine(string.Join('\n', islands));
            // Console.WriteLine(islands.Count);
        }
        Console.WriteLine(islands.Single());
        return;

        static HashSet<string> Update(IEnumerable<string> islands, Dictionary<string, HashSet<string>> graph)
        {
            return islands.Select(island =>
                {
                    // 集合内の頂点から繋がる頂点のうち
                    var members = island.Split(',').ToArray();
                    var nextMembers = members.SelectMany(m => graph[m]).Distinct();

                    // まだ集合に追加されていない & 全ての集合から繋がる頂点を抽出.
                    var extended = nextMembers
                        .Where(next => !members.Contains(next) && members.All(m => graph[next].Contains(m)))
                        .Select(next =>
                        {
                            // 集合に追加して重複排除のためアルファベット順に並べる
                            return string.Join(",", members.Append(next).OrderBy(name => name));
                        });
                    // Console.WriteLine(string.Join('\n', extended));
                    return extended;
                })
                .SelectMany(island => island) // 空配列(=拡大できなかったケース)は flatten で除去
                .ToHashSet();
        }
    }

    private static Dictionary<string, HashSet<string>> BuildGraph()
    {
        var vertexes = new Dictionary<string, HashSet<string>>();
        while (true)
        {
            var input = Console.ReadLine();
            if (string.IsNullOrEmpty(input))
                break;

            var connection = input.Split('-');
            var from = connection[0];
            var to = connection[1];
            if (!vertexes.ContainsKey(from))
                vertexes[from] = [];
            if (!vertexes.ContainsKey(to))
                vertexes[to] = [];
            vertexes[from].Add(to);
            vertexes[to].Add(from);
        }

        return vertexes;
    }
}