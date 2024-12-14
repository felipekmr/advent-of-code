const int POS_X_IDX = 0;
const int POS_Y_IDX = 1;
const int VEL_X_IDX = 2;
const int VEL_Y_IDX = 3;

int[] CreateRobot() => new int[4];

int[] ParseCoord(string input)
{
    return input.Trim().Split('=')[1].Split(",").Select(int.Parse).ToArray();
}

int[] ParseRobot(string input)
{
    var robot = CreateRobot();
    var split = input.Split(' ');

    Array.Copy(ParseCoord(split[0]), 0, robot, POS_X_IDX, 2);
    Array.Copy(ParseCoord(split[1]), 0, robot, VEL_X_IDX, 2);

    return robot;
}

int[][] ParseRobots(params string[] inputs)
{
    var robots = new int[inputs.Length][];

    for (int i = 0; i < inputs.Length; i++)
        robots[i] = ParseRobot(inputs[i]);

    return robots;
}

int[][] ReadRobots(string filePath)
{
    return ParseRobots(File.ReadAllLines(filePath));
}

void WalkRobot(int width, int height, int[] robot)
{
    var x = robot[POS_X_IDX] + robot[VEL_X_IDX];
    var y = robot[POS_Y_IDX] + robot[VEL_Y_IDX];

    if (x < 0) x = width + x;
    if (y < 0) y = height + y;
    if (x >= width) x -= width;
    if (y >= height) y -= height;

    robot[POS_X_IDX] = x;
    robot[POS_Y_IDX ] = y;
}

void Run(int secconds, int width, int height, int[][] robots)
{
    for (int i = 0; i < secconds; i++)
        foreach (var robot in robots)
            WalkRobot(width, height, robot);
}

int CalculateSafetyFactor(int width, int height, int[][] robots)
{
    var quadrants = new int[4];
    var halfWidth = width / 2;
    var halfHeight = height / 2;
    var q0x = halfWidth;
    var q0y = halfHeight;
    var q1x = width;
    var q1y = halfHeight;
    var q2x = halfWidth;
    var q2y = height;

    foreach (var robot in robots)
    {
        var x = robot[POS_X_IDX];
        var y = robot[POS_Y_IDX];

        if (x == q0x || y == q0y)
            continue;

        var idx = quadrants.Length - 1;
        if (x < q0x && y < q0y) idx = 0; else
        if (x < q1x && y < q1y) idx = 1; else
        if (x < q2x && y < q2y) idx = 2;

        quadrants[idx]++;
    }

    return quadrants.Aggregate(1, (a, b) => a * b);
}

(int Secconds, int Width, int Height, int[][] Robots) ReadArguments()
{
    var i = 0;
    var secconds = int.Parse(args[i++]);
    var width = int.Parse(args[i++]);
    var height = int.Parse(args[i++]);
    var robots = ReadRobots(args[i++]);
    return (secconds, width, height, robots);
}

void InnerMain()
{
    var (secconds, width, height, robots) = ReadArguments();
    Run(secconds, width, height, robots);

    var safetyFactor = CalculateSafetyFactor(width, height, robots);

    Console.WriteLine($"Safety factor: {safetyFactor}");
}

InnerMain();