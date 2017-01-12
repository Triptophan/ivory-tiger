using Assets.Scripts.MapGeneration.Enumerations;
using Assets.Scripts.MapGeneration.Generators;
using Assets.Scripts.MapGeneration.Types;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    public SquareGrid SquareGrid;
    public MeshFilter Walls;
    public MeshCollider WallCollider;
    public GameObject FloorObject;
    public GameObject CeilingObject;

    private Transform _floorTransform;
    private MeshFilter _ceilingMesh;
    private MeshCollider _ceilingCollider;
    private List<Vector3> _vertices;
    private List<int> _triangles;

    private int _wallHeight;
    private int _squareSize;

    private Dictionary<int, List<Triangle>> _triangleDictionary = new Dictionary<int, List<Triangle>>();
    private HashSet<int> _checkedVertices = new HashSet<int>();
    private List<List<int>> _outlines = new List<List<int>>();

    public void GenerateMesh(TileType[,] map, List<Room> rooms, int wallHeight, int squareSize)
    {
        _wallHeight = wallHeight;
        _squareSize = squareSize;
        _floorTransform = FloorObject.transform;
        _floorTransform.position = new Vector3(_floorTransform.position.x, wallHeight * -squareSize, _floorTransform.position.z);
        _ceilingMesh = CeilingObject.GetComponent<MeshFilter>();
        _ceilingCollider = CeilingObject.GetComponent<MeshCollider>();
        _triangleDictionary.Clear();
        _outlines.Clear();
        _checkedVertices.Clear();

        SquareGrid = new SquareGrid(map, rooms, squareSize);

        _vertices = new List<Vector3>();
        _triangles = new List<int>();

        for (int x = 0; x < SquareGrid.Squares.GetLength(0); x++)
            for (int y = 0; y < SquareGrid.Squares.GetLength(1); y++)
            {
                var square = SquareGrid.Squares[x, y];
                if (square == null || square.TileType != TileType.Wall) continue;

                MeshGeneratorHelper.MeshFromPoints(_vertices, _triangles, square.TopLeft, square.TopRight, square.BottomRight, square.BottomLeft);
            }

        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        mesh.vertices = _vertices.ToArray();
        mesh.triangles = _triangles.ToArray();
        mesh.RecalculateNormals();

        CreateWallMesh();
        CreateFloorMesh();
        CreateCeilingMesh();
    }

    private void CreateWallMesh()
    {
        List<Vector3> wallVertices = new List<Vector3>();
        List<int> wallTriangles = new List<int>();
        Mesh wallmesh = new Mesh();
        List<Vector2> uvs = new List<Vector2>();

        foreach (var square in SquareGrid.Squares)
        {
            if (square == null || square.TileType != TileType.Wall) continue;
            AddActiveWalls(wallVertices, wallTriangles, uvs, square);
        }

        wallmesh.vertices = wallVertices.ToArray();
        wallmesh.triangles = wallTriangles.ToArray();
        wallmesh.RecalculateNormals();

        wallmesh.uv = uvs.ToArray();
        Walls.mesh = wallmesh;

        WallCollider.sharedMesh = wallmesh;
    }

    private void CreateFloorMesh()
    {
        var _floorMesh = FloorObject.GetComponent<MeshFilter>();
        var _floorCollider = FloorObject.GetComponent<MeshCollider>();
        var floorMeshGenerator = new FloorMeshGenerator(SquareGrid, _floorMesh, _floorCollider);

        floorMeshGenerator.Generate();
    }

    private void CreateCeilingMesh()
    {
        List<Vector3> ceilingVertices = new List<Vector3>();
        List<int> ceilingTriangles = new List<int>();
        Mesh ceilingMesh = new Mesh();
        List<Vector2> uvs = new List<Vector2>();

        foreach (var square in SquareGrid.Squares)
        {
            if (square == null || square.TileType != TileType.Room) continue;
            MeshGeneratorHelper.ResetSquareNodes(square);
            MeshGeneratorHelper.MeshFromPoints(ceilingVertices, ceilingTriangles, square.TopRight, square.TopLeft, square.BottomLeft, square.BottomRight);
            MeshGeneratorHelper.AddUVs(uvs);
        }

        ceilingMesh.vertices = ceilingVertices.ToArray();
        ceilingMesh.triangles = ceilingTriangles.ToArray();
        ceilingMesh.RecalculateNormals();

        ceilingMesh.uv = uvs.ToArray();
        _ceilingMesh.mesh = ceilingMesh;

        _ceilingCollider.sharedMesh = ceilingMesh;
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