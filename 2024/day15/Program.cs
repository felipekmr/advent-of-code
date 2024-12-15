using System.Text;

const char WALL = '#';
const char EMPTY_SPACE = '.';
const char BOX = 'O';
const char ROBOT = '@';

const char DIR_UP = '^';
const char DIR_DOWN = 'v';
const char DIR_LEFT = '<';
const char DIR_RIGHT = '>';

const int POS_SHIFT = 1_000;

int JoinPosition(int x, int y) => x * POS_SHIFT + y;

(int X, int Y) ExtractPosition(int position) => (position / POS_SHIFT, position % POS_SHIFT);

char GetSpace(char[][] map, int x, int y) => map[y][x];

void Swap(char[][] map, int x1, int y1, int x2, int y2)
{
    var space1 = GetSpace(map, x1, y1);
    var space2 = GetSpace(map, x2, y2);
    map[y2][x2] = space1;
    map[y1][x1] = space2;
}

(int X, int Y) NextPosition(char direction, int x, int y)
{
    return direction switch
    {
        DIR_UP      => (x, y - 1),
        DIR_DOWN    => (x, y + 1),
        DIR_LEFT    => (x - 1, y),
        DIR_RIGHT   => (x + 1, y),
        _           => throw new NotImplementedException($"Invalid direction '{direction}'"),
    };
}

bool CanMove(char[][] map, char direction, int x, int y)
{
    var space = map[y][x];

    return space switch
    {
        EMPTY_SPACE     => true,
        WALL            => false,
        BOX             => MoveBox(direction, x, y, map),
        _               => throw new InvalidOperationException($"Invalid space '{space}'"),
    };
}

bool MoveBox(char direction, int x, int y, char[][] map)
{
    var (nextX, nextY) = NextPosition(direction, x, y);

    if (CanMove(map, direction, nextX, nextY) == false)
        return false;

    Swap(map, x, y, nextX, nextY);
    return true;
}

bool MoveRobot(char[][] map, char robotDir, ref int robotPosition)
{
    var (startX, startY) = ExtractPosition(robotPosition);
    var (nextX, nextY) = NextPosition(robotDir, startX, startY);

    if (CanMove(map, robotDir, nextX, nextY) == false)
        return false;

    Swap(map, startX, startY, nextX, nextY);
    robotPosition = JoinPosition(nextX, nextY);
    return true;
}

void Run(char[][] map, int startRobotPosition, string robotMoves)
{
    var robotPosition = startRobotPosition;

    for (int i = 0; i < robotMoves.Length; i++)
    {
        MoveRobot(map, robotMoves[i], ref robotPosition);
    }
}

int SumGpsCoordinates(char[][] map)
{
    var sum = 0;

    for (int r = 0; r < map.Length; r++)
    {
        var row = map[r];

        for (int c = 0; c < map[r].Length; c++)
        {
            if (map[r][c] != BOX)
                continue;

            sum += r * 100 + c;
        }
    }

    return sum;
}

void SaveMap(string outputPath, char[][] map)
{
    var content = map.Select(a => new string(a));

    File.WriteAllLines(outputPath, content);
}

(char[][] Map, int StartRobotPosition, string RobotMoves) ReadInput(string inputPath)
{
    var lines = File.ReadAllLines(inputPath);
    var i = 0;
    var x = -1;
    var y = -1;

    for (i = 0; i < lines.Length; i++)
    {
        var line = lines[i];

        if (string.IsNullOrEmpty(line))
            break;

        var j = line.IndexOf(ROBOT);
        if (j > -1)
        {
            x = j;
            y = i;
        }
    }

    if (i == lines.Length)
        throw new InvalidOperationException("No found the empty line");

    if (x < 0 || y < 0)
        throw new InvalidOperationException("No found robot");

    var map = new char[i][];
    for (int j = 0; j < map.Length; j++)
        map[j] = lines[j].ToCharArray();

    var builder = new StringBuilder();
    for (int j = i + 1; j < lines.Length; j++)
        builder.Append(lines[j]);

    var position = new[] { x, y };
    var moves = builder.ToString();

    return (map, JoinPosition(x, y), moves);
}

void InnerMain()
{
    var (map, robotPos, robotMoves) = ReadInput(args[0]);
    Run(map, robotPos, robotMoves);

    var sum = SumGpsCoordinates(map);

    Console.WriteLine($"The sum of all boxes' GPS coordinates is {sum}");

    SaveMap($"output.txt", map);
}

InnerMain();