using Assets.Scripts.MapGeneration.Enumerations;
using Assets.Scripts.MapGeneration.Types;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    public SquareGrid SquareGrid;
    public MeshFilter Walls;
    public MeshCollider WallCollider;
    public Transform Floor;

    private List<Vector3> _vertices;
    private List<int> _triangles;

    private int _wallHeight;
    private int _squareSize;

    private Dictionary<int, List<Triangle>> _triangleDictionary = new Dictionary<int, List<Triangle>>();
    private HashSet<int> _checkedVertices = new HashSet<int>();
    private List<List<int>> _outlines = new List<List<int>>();

    public void GenerateMesh(TileType[,] map, int wallHeight, int squareSize)
    {
        _wallHeight = wallHeight;
        _squareSize = squareSize;
        Floor.position = new Vector3(Floor.position.x, wallHeight * -squareSize, Floor.position.z);
        _triangleDictionary.Clear();
        _outlines.Clear();
        _checkedVertices.Clear();

        SquareGrid = new SquareGrid(map, squareSize);
        Floor.localScale = new Vector3(map.GetLength(0) / 8 * squareSize, 1, map.GetLength(1) / 8 * squareSize);

        _vertices = new List<Vector3>();
        _triangles = new List<int>();

        for (int x = 0; x < SquareGrid.Squares.GetLength(0); x++)
            for (int y = 0; y < SquareGrid.Squares.GetLength(1); y++)
            {
                var square = SquareGrid.Squares[x, y];
                if (square == null || square.TileType != TileType.Wall) continue;
                if (square.TileType == TileType.Wall)
                {
                    MeshFromPoints(square.TopLeft, square.TopRight, square.BottomRight, square.BottomLeft);
                }
            }

        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        mesh.vertices = _vertices.ToArray();
        mesh.triangles = _triangles.ToArray();
        mesh.RecalculateNormals();

        CreateWallMesh();
    }

    private void MeshFromPoints(params Node[] points)
    {
        AssignVertices(points);

        CreateTriangle(points[1], points[3], points[0]);
        CreateTriangle(points[1], points[2], points[3]);
    }

    private void AssignVertices(Node[] points)
    {
        for (int i = 0; i < points.Length; i++)
        {
            if (points[i].VertexIndex == -1)
            {
                points[i].VertexIndex = _vertices.Count;
                _vertices.Add(points[i].Position);
            }
        }
    }

    private void CreateTriangle(Node a, Node b, Node c)
    {
        _triangles.Add(a.VertexIndex);
        _triangles.Add(b.VertexIndex);
        _triangles.Add(c.VertexIndex);
    }

    private void CreateWallMesh()
    {
        List<Vector3> wallVertices = new List<Vector3>();
        List<int> wallTriangles = new List<int>();
        Mesh wallmesh = new Mesh();
        List<Vector2> uvs = new List<Vector2>();

        foreach (var square in SquareGrid.Squares)
        {
            if (square == null) continue;
            AddActiveWalls(wallVertices, wallTriangles, uvs, square);
        }

        wallmesh.vertices = wallVertices.ToArray();
        wallmesh.triangles = wallTriangles.ToArray();
        wallmesh.RecalculateNormals();

        wallmesh.uv = uvs.ToArray();
        Walls.mesh = wallmesh;

        WallCollider.sharedMesh = wallmesh;
    }

    private void AddActiveWalls(List<Vector3> wallVertices, List<int> wallTriangles, List<Vector2> uvs, Square square)
    {
        if (square.BottomEdgeActive)
        {
            AddWallHeight(square.BottomLeft.VertexIndex, square.BottomRight.VertexIndex, wallVertices, uvs, wallTriangles);
        }
        if (square.LeftEdgeActive)
        {
            AddWallHeight(square.TopLeft.VertexIndex, square.BottomLeft.VertexIndex, wallVertices, uvs, wallTriangles);
        }
        if (square.RightEdgeActive)
        {
            AddWallHeight(square.BottomRight.VertexIndex, square.TopRight.VertexIndex, wallVertices, uvs, wallTriangles);
        }
        if (square.TopEdgeActive)
        {
            AddWallHeight(square.TopRight.VertexIndex, square.TopLeft.VertexIndex, wallVertices, uvs, wallTriangles);
        }
    }

    private void AddWallHeight(int vertexIndexA, int vertexIndexB, List<Vector3> wallVertices, List<Vector2> uvs, List<int> wallTriangles)
    {
        for (int offset = 0; offset < _wallHeight; offset++)
        {
            int startIndex = wallVertices.Count;

            wallVertices.Add(_vertices[vertexIndexA] - Vector3.up * offset * _squareSize); //left
            wallVertices.Add(_vertices[vertexIndexB] - Vector3.up * offset * _squareSize); //right
            wallVertices.Add(_vertices[vertexIndexA] - Vector3.up * (offset + 1) * _squareSize); //bottom left
            wallVertices.Add(_vertices[vertexIndexB] - Vector3.up * (offset + 1) * _squareSize); //bottom right

            wallTriangles.Add(startIndex + 0);
            wallTriangles.Add(startIndex + 3);
            wallTriangles.Add(startIndex + 2);

            wallTriangles.Add(startIndex + 0);
            wallTriangles.Add(startIndex + 1);
            wallTriangles.Add(startIndex + 3);

            AddUVs(uvs);
        }
    }

    private void AddUVs(List<Vector2> uvs)
    {
        uvs.Add(new Vector2(0, 1));
        uvs.Add(new Vector2(1, 1));
        uvs.Add(new Vector2(0, 0));
        uvs.Add(new Vector2(1, 0));
    }
}