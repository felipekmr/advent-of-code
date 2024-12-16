using System.Text;

const bool DEBUG = false;

const char WALL = '#';
const char EMPTY_SPACE = '.';
const char BOX = 'O';
const char BOX_L = '[';
const char BOX_R = ']';
const char ROBOT = '@';

const string WH2_EMPTY_SPACE = "..";
const string WH2_BOX = "[]";
const string WH2_ROBOT = "@.";
const string WH2_WALL = "##";

const char DIR_UP = '^';
const char DIR_DOWN = 'v';
const char DIR_LEFT = '<';
const char DIR_RIGHT = '>';

const int POS_SHIFT = 1_000;

Exception CreateExceptionInvalidSpace(char space) => new InvalidOperationException($"Invalid space '{space}'");

Exception CreateExceptionInvalidDirection(char direction) => new InvalidOperationException($"Invalid direction '{direction}'");

int JoinPosition(int x, int y) => x * POS_SHIFT + y;

(int X, int Y) ExtractPosition(int position) => (position / POS_SHIFT, position % POS_SHIFT);

(int X, int Y) AddCoord(int x, int y, int value, bool isVertical = false)
{
    if (isVertical)
        return (x, y + value);

    else
        return (x + value, y);
}

IEnumerable<int> OrderByY(IEnumerable<int> positions)
{
    return
        from pos in positions
        orderby pos % POS_SHIFT
        select pos;
}

IEnumerable<int> OrderByYDesc(IEnumerable<int> positions)
{
    return
        from pos in positions
        orderby pos % POS_SHIFT descending
        select pos;
}

char GetSpace(char[][] map, int x, int y) => map[y][x];

void SetSpace(char[][] map, int x, int y, char newSpace) => map[y][x] = newSpace;

void Swap(char[][] map, int x1, int y1, int x2, int y2)
{
    var space1 = GetSpace(map, x1, y1);
    var space2 = GetSpace(map, x2, y2);
    SetSpace(map, x2, y2, space1);
    SetSpace(map, x1, y1, space2);
}

(int X, int Y) NextPosition(char direction, int x, int y)
{
    return direction switch
    {
        DIR_UP      => (x, y - 1),
        DIR_DOWN    => (x, y + 1),
        DIR_LEFT    => (x - 1, y),
        DIR_RIGHT   => (x + 1, y),
        _           => throw CreateExceptionInvalidDirection(direction),
    };
}

string RelayoutSpace(char space)
{
    return space switch
    {
        EMPTY_SPACE => WH2_EMPTY_SPACE,
        WALL        => WH2_WALL,
        BOX         => WH2_BOX,
        ROBOT       => WH2_ROBOT,
        _           => throw CreateExceptionInvalidSpace(space),
    };
}

(int OffsetLeft, int OffsetRight) GetBox2VerticalOffset(char space)
{
    return space switch
    {
        BOX_L   => (0, +1),
        BOX_R   => (-1, 0),
        _       => throw CreateExceptionInvalidSpace(space),
    };
}

bool IsBox(char space) => space switch
{
    BOX => true,
    BOX_L => true,
    BOX_R => true,
    _ => false,
};

bool CanMoveBox2Vertical(char[][] map, int x, int y, int inc, ICollection<int> positions)
{
    if (positions.Contains(JoinPosition(x, y)))
        return true;

    var space = GetSpace(map, x, y);

    switch (space)
    {
        case WALL:
            return false;

        case EMPTY_SPACE:
            return true;
    }

    var (offsetL, offsetR) = GetBox2VerticalOffset(space);
    var x1 = x + offsetL;
    var x2 = x + offsetR;

    positions.Add(JoinPosition(x1, y));
    positions.Add(JoinPosition(x2, y));

    return CanMoveBox2Vertical(map, x1, y + inc, inc, positions)
        && CanMoveBox2Vertical(map, x2, y + inc, inc, positions);
}

bool TryMoveBoxVertical(char[][] map, int x, int y, char space, int inc)
{
    switch (space)
    {
        case BOX:
            return MoveBox1(map, x, y, inc, isVertical: true);

        case BOX_L:
        case BOX_R:
            var positions = new HashSet<int>();
            if (CanMoveBox2Vertical(map, x, y, inc, positions) == false)
                return false;

            MoveBoxVertical2(map, inc, positions);
            return true;

        default:
            throw CreateExceptionInvalidSpace(space);
    }
}

void MoveBoxVertical2(char[][] map, int inc, IEnumerable<int> positions)
{
    var orderedPos = inc < 0
        ? OrderByY(positions)
        : OrderByYDesc(positions);

    foreach (var pos in orderedPos)
    {
        var (x, y) = ExtractPosition(pos);

        Swap(map, x, y, x, y - -inc);
    }
}

bool MoveBox1(char[][] map, int x, int y, int inc, bool isVertical = false)
{
    var (x2, y2) = AddCoord(x, y, 0, isVertical);
    var count = 0;
    var space = '\0';

    while (space != EMPTY_SPACE)
    {
        (x2, y2) = AddCoord(x2, y2, inc, isVertical);
        space = GetSpace(map, x2, y2);
        count++;

        if (space == WALL)
            return false;
    }

    for (int i = 0; i < count; i++)
    {
        var (x1, y1) = AddCoord(x2, y2, -inc, isVertical);
        Swap(map, x1, y1, x2, y2);

        x2 = x1;
        y2 = y1;
    }

    return true;
}

bool MoveBox(char[][] map, char direction, int x, int y, char space)
{
    if (IsBox(space) == false)
        return true;

    return direction switch
    {
        DIR_UP      => TryMoveBoxVertical(map, x, y, space, -1),
        DIR_DOWN    => TryMoveBoxVertical(map, x, y, space, +1),
        DIR_LEFT    => MoveBox1(map, x, y, -1),
        DIR_RIGHT   => MoveBox1(map, x, y, +1),
        _           => throw CreateExceptionInvalidDirection(direction),
    };
}

void MoveRobot(char[][] map, char robotDir, ref int robotPosition)
{
    var (startX, startY) = ExtractPosition(robotPosition);
    var (nextX, nextY) = NextPosition(robotDir, startX, startY);
    var nextSpace = GetSpace(map, nextX, nextY);

    if (nextSpace == WALL)
        return;

    if (MoveBox(map, robotDir, nextX, nextY, nextSpace) == false)
        return;

    Swap(map, startX, startY, nextX, nextY);
    robotPosition = JoinPosition(nextX, nextY);
}

int Run(char[][] map, int startRobotPosition, string robotMoves)
{
    var robotPosition = startRobotPosition;
    SaveMapOnDebug($"output-0000.txt", map);

    for (int i = 0; i < robotMoves.Length; i++)
    {
        var robotDir = robotMoves[i];
        MoveRobot(map, robotDir, ref robotPosition);
        SaveMapOnDebug($"output-{i + 1:0000}.txt", map);
        SaveMapOnDebug($"output-last.txt", map);
    }

    return SumGpsCoordinates(map);
}

int SumGpsCoordinates(char[][] map)
{
    var sum = 0;

    for (int r = 0; r < map.Length; r++)
    {
        var row = map[r];

        for (int c = 0; c < map[r].Length; c++)
        {
            sum += map[r][c] switch
            {
                BOX
                or BOX_L    => r * 100 + c,
                _           => 0,
            };
        }
    }

    return sum;
}

void SaveMapOnDebug(string outputPath, char[][] map)
{
    if (DEBUG == false)
        return;

    SaveMap(outputPath, map);
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

(char[][] Map, int RobotPosition) RelayoutToWarehouse2(char[][] map1, int robotPosition1)
{
    var map2 = new char[map1.Length][];

    for (int i = 0; i < map1.Length; i++)
    {
        map2[i] = new char[map1[i].Length * 2];

        for (int j = 0; j < map1[i].Length; j++)
        {
            var newSpace = RelayoutSpace(map1[i][j]);
            var k = j * 2;
            map2[i][k + 0] = newSpace[0]; 
            map2[i][k + 1] = newSpace[1];
        }
    }

    var (x1, y1) = ExtractPosition(robotPosition1);
    var robotPosition2 = JoinPosition(x1 * 2, y1);
    return (map2, robotPosition2);
}

void InnerMain()
{
    var (map1, robotPos1, robotMoves) = ReadInput(args[0]);
    var (map2, robotPos2) = RelayoutToWarehouse2(map1, robotPos1);

    var sum1 = Run(map1, robotPos1, robotMoves);
    var sum2 = Run(map2, robotPos2, robotMoves);

    Console.WriteLine($"The sum of all boxes' GPS coordinates from warehouse 1 is {sum1}");
    Console.WriteLine($"The sum of all boxes' GPS coordinates from warehouse 2 is {sum2}");

    SaveMap("output-warehouse1.txt", map1);
    SaveMap("output-warehouse2.txt", map2);
}

InnerMain();