using System.Text;

namespace AoC2024;

public class Day9
{
    private class Segment
    {
        public int Id { get; }
        public int Length { get; }
        public const int SpaceId = -1;
        public bool IsFile => Id != SpaceId;
        public IReadOnlyList<int> Ids => _ids;

        private readonly int[] _ids;
        private int _currentIdx = 0;

        public Segment(int length, int id)
        {
            Id = id;
            Length = length;

            _ids = Enumerable.Repeat(id, length).ToArray();
            _currentIdx = IsFile ? Length - 1 : 0;
        }

        public bool Empty() => _ids.All(id => id == SpaceId);

        public bool IsFull() => _ids.All(id => id != SpaceId);

        public int GetVacantSize() => _ids.Count(id => id == SpaceId);

        public void PushBack(int id)
        {
            _ids[_currentIdx] = id;
            _currentIdx = Math.Min(Length - 1, _currentIdx + 1);
        }

        public int PopBack()
        {
            var result = _ids[_currentIdx];
            _ids[_currentIdx] = SpaceId;
            _currentIdx = Math.Max(0, _currentIdx - 1);
            return result;
        }

        public void Clear()
        {
            for (var idx = 0; idx < Length; idx++)
                PopBack();
        }

        public void Fill(int id, int length)
        {
            for (var idx = 0; idx < length; idx++)
                PushBack(id);
        }

        public override string ToString()
        {
            return string.Concat(_ids.Select(id => id == SpaceId ? "." : id.ToString()));
        }
    }

    public static void Solve1()
    {
        var segments = ParseToSegments(GetInput());
        Process(segments);

        var line = segments.SelectMany(s => s.Ids);
        Console.WriteLine(CalcCheckSum(line));
        return;

        static void Process(List<Segment> segments)
        {
            var currentSpaceIndex = FindIndex(0, isFile: false, searchForward: true, segments)!.Value;
            var currentFileIndex = FindIndex(segments.Count - 1, isFile: true, searchForward: false, segments)!.Value;

            while (currentSpaceIndex < segments.Count && 0 <= currentFileIndex && currentSpaceIndex < currentFileIndex)
            {
                var file = segments[currentFileIndex];
                var space = segments[currentSpaceIndex];

                // サイズが 0 の file や space に操作しようとしていたらスキップ
                if (!file.Empty() && !space.IsFull())
                    space.PushBack(file.PopBack());

                // File が空になったら前の file へ
                if (file.Empty())
                {
                    var nextIdx = FindIndex(currentFileIndex - 1, isFile: true, searchForward: false, segments);
                    if (nextIdx == null)
                        break;

                    currentFileIndex = nextIdx.Value;
                }

                // 数値を詰めたあと空きがなくなったら次の space へ
                if (space.IsFull())
                {
                    var nextIdx = FindIndex(currentSpaceIndex + 1, isFile: false, searchForward: true, segments);
                    if (nextIdx == null)
                        break;

                    currentSpaceIndex = nextIdx.Value;
                }
            }

            return;

            static int? FindIndex(int startIndex, bool isFile, bool searchForward, IReadOnlyList<Segment> segments)
            {
                if (searchForward)
                {
                    for (var idx = startIndex; idx < segments.Count; idx++)
                    {
                        if (segments[idx].IsFile == isFile)
                            return idx;
                    }
                }
                else
                {
                    for (var idx = startIndex; 0 <= idx; idx--)
                    {
                        if (segments[idx].IsFile == isFile)
                            return idx;
                    }
                }

                return null;
            }
        }
    }

    public static void Solve2()
    {
        var segments = ParseToSegments(GetInput());
        Process(segments);

        var line = segments.SelectMany(s => s.Ids);
        Console.WriteLine(CalcCheckSum(line));
        return;

        static void Process(List<Segment> segments)
        {
            var files = segments.Where(segment => segment.IsFile).Select((file, fileIdx) => (file, fileIdx)).Reverse()
                .ToArray();
            var spaces = segments.Where(segment => !segment.IsFile).Select((space, spaceIdx) => (space, spaceIdx))
                .ToArray();
            foreach (var (file, fileIdx) in files)
            {
                var (vacantSpace, _) = spaces.FirstOrDefault(t =>
                {
                    var (space, spaceIdx) = t;
                    return spaceIdx < fileIdx && file.Length <= space.GetVacantSize();
                });

                if (vacantSpace == null)
                    continue;

                vacantSpace.Fill(file.Id, file.Length);
                file.Clear();
            }
        }
    }

    private static string GetInput()
    {
        using var fs = new FileStream("PuzzleInputs/Day9_Input.txt", FileMode.Open);
        using var reader = new StreamReader(fs, Encoding.UTF8);
        return reader.ReadToEnd();
        // return Console.ReadLine()!;
    }

    private static List<Segment> ParseToSegments(string input)
    {
        var isBlock = false;
        var fileId = 0;
        var segments = new List<Segment>();
        foreach (var c in input)
        {
            var length = int.Parse(c.ToString());
            var id = isBlock ? Segment.SpaceId : fileId;
            segments.Add(new Segment(length, id));

            if (!isBlock)
                fileId++;

            isBlock = !isBlock;
        }

        return segments;
    }

    private static long CalcCheckSum(IEnumerable<int> input)
    {
        var result = 0L;
        foreach (var (c, idx) in input.Select((c, idx) => (c, idx)))
        {
            if (c == Segment.SpaceId)
                continue;
            else
                result += (long)c * idx;
        }

        return result;
    }
}