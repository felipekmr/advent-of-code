const int LOWEST_HEIGHT = 0;
const int HIGHEST_HEIGHT = 9;

int JoinPosition(int row, int col) => (row << 16) + col;

HashSet<int> CreateTrailTrack() => [];

bool AddPosition(HashSet<int> track, int row, int col)
{
    var pos = JoinPosition(row, col);
    return track.Add(pos);
}

int[][] ReadMap(string inputPath)
{
    var input = File.ReadAllLines(inputPath);
    var map = new int[input.Length][];

    for (int i = 0; i < input.Length; i++)
    {
        var line = input[i];
        var row = new int[line.Length];

        for (int j = 0; j < line.Length; j++)
        {
            row[j] = line[j] - '0';
        }

        map[i] = row;
    }

    return map;
}

int MeasureTrailHead(int[][] map, Func<int, int, bool> continueOnTrack, int row, int col, int lastHeight)
{
    if (row < 0 || row >= map.Length)
        return 0;

    if (col < 0 || col >= map[row].Length)
        return 0;

    var height = map[row][col];

    if (height != (lastHeight + 1))
        return 0;

    if (continueOnTrack(row, col) == false)
        return 0;

    if (height == HIGHEST_HEIGHT)
        return 1;

    var score = 0;
    score += MeasureTrailHead(map, continueOnTrack, row - 1, col, height);
    score += MeasureTrailHead(map, continueOnTrack, row + 1, col, height);
    score += MeasureTrailHead(map, continueOnTrack, row, col - 1, height);
    score += MeasureTrailHead(map, continueOnTrack, row, col + 1, height);

    return score;
}

int MeasureTrailHeads(int[][] map, bool useMeasureByScore)
{
    var totalScore = 0;

    for (int r = 0; r < map.Length; r++)
    {
        var row = map[r];

        for (int c = 0; c < row.Length; c++)
        {
            var height = row[c];

            if (height != LOWEST_HEIGHT)
                continue;

            var track = CreateTrailTrack();

            // Determines whether should continue in the trail.
            bool ContinueOnTrack(int row, int col)
            {
                return useMeasureByScore == false || AddPosition(track, row, col);
            }

            totalScore += MeasureTrailHead(map, ContinueOnTrack, r, c, -1);
        }
    }

    return totalScore;
}

void InnerMain()
{
    var inputPath = args[0];
    var map = ReadMap(inputPath);

    var trailheadsScore = MeasureTrailHeads(map, useMeasureByScore: true);
    var trailheadsRating = MeasureTrailHeads(map, useMeasureByScore: false);

    Console.WriteLine($"Trailheads score sum  : {trailheadsScore}");
    Console.WriteLine($"Trailheads rating sum : {trailheadsRating}");
}

InnerMain();
