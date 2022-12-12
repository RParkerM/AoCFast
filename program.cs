using RT.Dijkstra;
using System.Collections.Immutable;
using System.Numerics;

var input = File.ReadAllLines("Input.txt");
var testInput = File.ReadAllLines("TestInput.txt");

List<List<int>> positions = new();

var run = input;

int startX = -1, startY = -1, endX = -1, endY = -1;

for (int y = 0; y < run.Length; y++)
{
    positions.Add(new List<int>());
    for (int x = 0; x < run[y].Length; x++)
    {
        if (startX == -1 && run[y][x] == 'S')
        {
            startX = x;
            startY = y;
            positions[^1].Add(0);
        }
        else if (endX == -1 & run[y][x] == 'E')
        {
            endX = x;
            endY = y;
            positions[^1].Add(25);
        }
        else
        {
            positions[^1].Add(run[y][x] - 'a');
        }
    }
    //Console.WriteLine(string.Join("", positions[^1].Select(x => $"[{x,3}]")));
}

Info.positions = positions;

int totalDistance;

//var route = DijkstrasAlgorithm.Run(
//    new GraphNode(endX, endY, startX, startY),
//    0,
//    (a, b) => a + b,
//    out totalDistance
//    );

//Console.WriteLine($"Length: {totalDistance}");

var route = DijkstrasAlgorithm.Run(
    new MapNode(endX, endY, (endX, endY)),
    0,
    (a, b) => a + b,
    out totalDistance
    );
Console.WriteLine($"Length: {totalDistance}");

public static class Info
{
    public static List<List<int>> positions = null!;
    public static bool CanMove(int curX, int curY, int newX, int newY)
    {
        if (newX < 0 || newY < 0 || newX >= positions[0].Count || newY >= positions.Count)
        {
            return false;
        }
        if (positions[curY][curX] < positions[newY][newX]) return true;
        if (Math.Abs(positions[curY][curX] - positions[newY][newX]) > 1)
        {
            return false;
        }
        return true;
    }

    public static readonly ImmutableList<(int x, int y)> search = new List<(int x, int y)>
    {
        (0, -1), (-1, 0), (1, 0), (0, 1)
    }.ToImmutableList();
}



public class MapNode : Node<int, int>
{
    public (int x, int y) position;
    public (int x, int y) dest;

    public MapNode(int x, int y, (int x, int y) dest)
    {
        position = (x, y);
        this.dest = dest;
    }

    public override bool Equals(Node<int, int> other)
    {
        var otherPos = ((MapNode)other).position;
        return position.x == otherPos.x && position.y == otherPos.y;
    }

    public override int GetHashCode() => position.GetHashCode();
    public override bool IsFinal => Info.positions[position.y][position.x] == 0;
    public override IEnumerable<Edge<int, int>> Edges
    {
        get
        {
            List<Edge<int, int>> adjacentNodes = new();
            var thisAltitude = Info.positions[position.y][position.x];
            foreach (var (addX, addY) in Info.search)
            {
                var (newX, newY) = (position.x + addX, position.y + addY);
                if (newX < 0 || newX >= Info.positions[0].Count || newY < 0 || newY >= Info.positions.Count) continue;
                int val = Info.positions[newY][newX];
                //if (val < 0 || val > thisAltitude + 1) continue;
                if (val < thisAltitude - 1) continue;
                adjacentNodes.Add(new Edge<int, int>(1, 0, new MapNode(newX, newY, dest)));
            }
            return adjacentNodes;
        }
    }
}

public sealed class GraphNode : Node<int, int>
{
    public readonly int x;
    public readonly int y;
    private readonly int endX;
    private readonly int endY;

    public GraphNode(int x, int y, int endX, int endY)
    {
        this.x = x;
        this.y = y;
        this.endX = endX;
        this.endY = endY;
    }

    public override bool Equals(Node<int, int> other)
    {
        return x.Equals(((GraphNode)other).x) && y.Equals(((GraphNode)other).y);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(x, y);
    }

    public override bool IsFinal { get => Info.positions[y][x] == 0; }

    public override IEnumerable<Edge<int, int>> Edges
    {
        get
        {
            List<Edge<int, int>> edges = new();

            if (Info.CanMove(x, y, x - 1, y))
            {
                edges.Add(new Edge<int, int>(1, 0, new GraphNode(x - 1, y, endX, endY)));
            }
            if (Info.CanMove(x, y, x + 1, y))
            {
                edges.Add(new Edge<int, int>(1, 0, new GraphNode(x + 1, y, endX, endY)));
            }
            if (Info.CanMove(x, y, x, y - 1))
            {
                edges.Add(new Edge<int, int>(1, 0, new GraphNode(x, y - 1, endX, endY)));
            }
            if (Info.CanMove(x, y, x, y + 1))
            {
                edges.Add(new Edge<int, int>(1, 0, new GraphNode(x, y + 1, endX, endY)));
            }

            return edges;
        }
    }
}