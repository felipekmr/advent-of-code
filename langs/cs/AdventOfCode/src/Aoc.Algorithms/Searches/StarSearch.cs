//using CommunityToolkit.Diagnostics;
//using System.Drawing;

//namespace Aoc.Algorithms.Searches;

//// A* Search
//public class StarSearch
//{
//    private const int NoVisited = int.MaxValue;

//    private readonly Rectangle area;
//    private readonly Func<Point, bool> isBlocked;
//    private readonly HashSet<Point> closed;
//    private readonly Queue<Point> opened;
//    private readonly Dictionary<Point, Cell> cells;

//    public StarSearch(Size size, Point start, Point end, Func<Point, bool> isBlocked)
//    {
//        Guard.IsGreaterThan(size.Width, 0);
//        Guard.IsGreaterThan(size.Height, 0);

//        area = new Rectangle(new Point(0, 0), size);
//        Guard.IsTrue(area.Contains(start));
//        Guard.IsTrue(area.Contains(end));

//        Size = size;
//        Start = start;
//        End = end;
//        this.isBlocked = isBlocked;

//        opened = new Queue<Point>();
//        closed = new HashSet<Point>();

//        AddOpen(start, 0);
//    }

//    public Size Size { get; }

//    public Point Start { get; }

//    public Point End { get; }

//    public bool IsClosed(Point point) => closed.Contains(point);

//    public int Execute()
//    {
//        // TODO: initialize

//        PrintMap("output-0000.txt");
//        var count = 0;

//        while (ExecuteOne())
//        {
//            PrintMap($"output-{++count:0000}.txt");
//        }

//        return values[End];
//    }

//    public void PrintMap(string path)
//    {
//        return;

//        var map = new char[Size.Height][];

//        for (int y = 0; y < Size.Height; y++)
//        {
//            map[y] = new string('.', Size.Width).ToCharArray();

//            for (int x = 0; x < Size.Width; x++)
//            {
//                var point = new Point(x, y);

//                if (isBlocked(point))
//                    map[y][x] = '#';
//            }
//        }

//        TraceMap(map, Start);

//        File.WriteAllLines(path, map.Select(r => new string(r)));
//    }

//    public void TraceMap(char[][] map, Point startPoint)
//    {
//        var points = new Queue<Point>([startPoint]);

//        while (points.Count > 0)
//        {
//            var point = points.Dequeue();

//            MarkMap(map, point);

//            foreach (var p in NextPoints(map, point))
//                points.Enqueue(p);
//        }
//    }

//    public void MarkMap(char[][] map, Point point)
//    {
//        map[point.Y][point.X] = (char)((values[point] % 10) + '0');
//    }

//    public IEnumerable<Point> NextPoints(char[][] map, Point point)
//    {
//        var value = values[point];
//        var nextValue = value + 1;

//        var dirs = new[]
//        {
//            Direction.Up,
//            Direction.Down,
//            Direction.Left,
//            Direction.Right,
//        };

//        return
//            from dir in dirs
//            let p = point + dir
//            where Size.Contains(p) && isBlocked(p) == false
//            let v = values[p]
//            where v < NoVisited
//            where v == nextValue
//            select p;
//    }

//    public bool ExecuteOne()
//    {
//        // TODO: test if initialized

//        if (opened.Count == 0)
//            return false;

//        var point = opened.Dequeue();

//        if (IsClosed(point))
//            return false;

//        closed.Add(point);

//        var value = values[point];
//        var newValue = value + 1;

//        var found
//            = UpdateNext(point + Direction.Right, newValue)
//            || UpdateNext(point + Direction.Down, newValue)
//            || UpdateNext(point + Direction.Left, newValue)
//            || UpdateNext(point + Direction.Up, newValue)
//            ;

//        return !found && opened.Count > 0;
//    }

//    private bool UpdateNext(Point nextPoint, int newValue)
//    {
//        if (Size.Contains(nextPoint) == false)
//            return false;

//        if (isBlocked(nextPoint))
//            return false;

//        var nextValue = values[nextPoint];

//        if (nextValue == NoVisited || nextValue > newValue)
//        {
//            AddOpen(nextPoint, newValue);
//        }

//        return nextPoint == End;
//    }

//    private void AddOpen(Point point, int value)
//    {
//        values[point] = value;
//        opened.Enqueue(point);
//    }

//    private struct Cell
//    {
//        public Cell(int factorG, int factorH)
//        {
//            FactorG = factorG;
//            FactorH = factorH;
//        }

//        public readonly int FactorF => FactorG + FactorH;

//        public int FactorG { get; }

//        public int FactorH { get; }

//}
