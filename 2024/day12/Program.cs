using System.Diagnostics;
using System.Reflection.Metadata;

const char EMPTY_PLANT = '\0';
(int, int) EMPTY_REGION = (0, 0);
(int, int) BOUNDARY_FENCE = (0, 1);


string FormatCurrency(int value) => $"${value:#,0}";

string FormatFencePrice(int area, int perimeter, int price)
{
    return $"{area,3} x {perimeter,3} = {FormatCurrency(price):7}";
}

char[][] CreateGardenPlotMap(char[][] gardenMap)
{
    var gardenPlotMap = new char[gardenMap.Length][];

    for (int i = 0; i < gardenMap.Length; i++)
        gardenPlotMap[i] = new char[gardenMap.Length];

    return gardenPlotMap;
}

bool AddGardenPlot(char[][] gardenPlotMap, int row, int col, char plant)
{
    if (HasGardenPlot(gardenPlotMap, row, col))
        return false;

    gardenPlotMap[row][col] = plant;
    return true;
}

bool HasGardenPlot(char[][] gardenPlotMap, int row, int col)
{
    return gardenPlotMap[row][col] != EMPTY_PLANT;
}

char[][] ReadGardenMap(string inputPath)
{
    var input = File.ReadAllLines(inputPath);
    var gardenMap = new char[input.Length][];
    
    for ( int i = 0; i < input.Length; i++ )
        gardenMap[i] = input[i].ToCharArray();

    return gardenMap;
}

(int Area, int Perimeter) LookRegion(char[][] gardenPlotMap, char[][] gardenMap, int row, int col, char requestedPlant)
{
    if (row < 0 || row >= gardenMap.Length)
        return BOUNDARY_FENCE;

    if (col < 0 || col >= gardenMap[0].Length)
        return BOUNDARY_FENCE;

    var plant = gardenMap[row][col];

    if (plant != requestedPlant)
        return BOUNDARY_FENCE;

    if (AddGardenPlot(gardenPlotMap, row, col, plant) == false)
        return EMPTY_REGION;

    var regionArea = 1;
    var regionPerimeter = 0;

    void AddValues(int rowOffset, int colOffset)
    {
        var (area, perimeter) = LookRegion(gardenPlotMap, gardenMap, row + rowOffset, col + colOffset, requestedPlant);
        regionArea += area;
        regionPerimeter += perimeter;
    }

    AddValues(-1, 0);
    AddValues(+1, 0);
    AddValues(0, -1);
    AddValues(0, +1);

    return (regionArea, regionPerimeter);
}

int GetRegion(char[][] gardenPlotMap, char[][] gardenMap, int row, int col)
{
    if (HasGardenPlot(gardenPlotMap, row, col))
        return 0;

    var plant = gardenMap[row][col];
    var (area, perimeter) = LookRegion(gardenPlotMap, gardenMap, row, col, plant);
    var price = area * perimeter;

    Console.Error.WriteLine($"Region of {plant} plants with price: {FormatFencePrice(area, perimeter, price)}");

    return price;
}

int GetTotalPrice(char[][] gardenMap)
{
    var gardenPlotMap = CreateGardenPlotMap(gardenMap);
    var totalPrice = 0;

    for (var r = 0; r < gardenMap.Length; r++)
    {
        var row = gardenMap[r];

        for (var c = 0; c < row.Length; c++)
            totalPrice += GetRegion(gardenPlotMap, gardenMap, r, c);
    }

    return totalPrice;
}

void InnerMain()
{
    var inputPath = args[0];
    var gardenMap = ReadGardenMap(inputPath);
    var totalPrice = GetTotalPrice(gardenMap);

    Console.WriteLine($"Total price: {FormatCurrency(totalPrice)}");
}

InnerMain();