using Assets.Scripts;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    public SquareGrid SquareGrid;
    public MeshFilter Walls;

    public int WallHeight = 5;

    private List<Vector3> _vertices;
    private List<int> _triangles;

    private Dictionary<int, List<Triangle>> _triangleDictionary = new Dictionary<int, List<Triangle>>();
    private HashSet<int> _checkedVertices = new HashSet<int>();
    private List<List<int>> _outlines = new List<List<int>>();

    public void GenerateMesh(TileType[,] map, float squareSize)
    {
        _triangleDictionary.Clear();
        _outlines.Clear();
        _checkedVertices.Clear();

        SquareGrid = new SquareGrid(map, squareSize);

        _vertices = new List<Vector3>();
        _triangles = new List<int>();

        for (int x = 0; x < SquareGrid.Squares.GetLength(0); x++)
            for (int y = 0; y < SquareGrid.Squares.GetLength(1); y++)
            {
                TriangulateSquare(SquareGrid.Squares[x, y]);
            }

        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        mesh.vertices = _vertices.ToArray();
        mesh.triangles = _triangles.ToArray();
        mesh.RecalculateNormals();

        CreateWallMesh();
    }

    private void TriangulateSquare(Square square)
    {
        if (square.Configuration == 0) return;

        if ((square.Configuration & (1 << 0)) != 0)
        {
            MeshFromPoints(square.TopLeft, square.TopRight, square.BottomRight, square.BottomLeft);
        }
    }

    private void CreateWallMesh()
    {
        CalculateMeshOutlines();

        List<Vector3> wallVertices = new List<Vector3>();
        List<int> wallTriangles = new List<int>();
        Mesh wallmesh = new Mesh();

        foreach (List<int> outline in _outlines)
        {
            for (int i = 0; i < outline.Count - 1; i++)
            {                
                AddWallHeight(wallVertices, outline, wallTriangles, i);               
            }
        }

        wallmesh.vertices = wallVertices.ToArray();
        wallmesh.triangles = wallTriangles.ToArray();
        Walls.mesh = wallmesh;
    }

    private void AddWallHeight(List<Vector3> wallVertices, List<int> outline, List<int> wallTriangles, int outlineIndex)
    {
        for(int offset = 0; offset < WallHeight; offset++)
        {
            int startIndex = wallVertices.Count;

            wallVertices.Add(_vertices[outline[outlineIndex]] - Vector3.up * offset); //left
            wallVertices.Add(_vertices[outline[outlineIndex + 1]] - Vector3.up * offset); //right
            wallVertices.Add(_vertices[outline[outlineIndex]] - Vector3.up * (offset + 1)); //bottom left
            wallVertices.Add(_vertices[outline[outlineIndex + 1]] - Vector3.up * (offset + 1)); //bottom right
            
            wallTriangles.Add(startIndex + 0);
            wallTriangles.Add(startIndex + 3);
            wallTriangles.Add(startIndex + 2);

            wallTriangles.Add(startIndex + 0);
            wallTriangles.Add(startIndex + 1);
            wallTriangles.Add(startIndex + 3);
        }    
    }

    private void MeshFromPoints(params Node[] points)
    {
        AssignVertices(points);

        CreateTriangle(points[0], points[1], points[2]);
        CreateTriangle(points[1], points[3], points[2]);
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

        Triangle triangle = new Triangle(a.VertexIndex, b.VertexIndex, c.VertexIndex);
        AddTriangleToDictionary(triangle.VertexIndexA, triangle);
        AddTriangleToDictionary(triangle.VertexIndexB, triangle);
        AddTriangleToDictionary(triangle.VertexIndexC, triangle);
    }

    private void AddTriangleToDictionary(int vertexIndexKey, Triangle triangle)
    {
        if (_triangleDictionary.ContainsKey(vertexIndexKey))
        {
            _triangleDictionary[vertexIndexKey].Add(triangle);
        }
        else
        {
            List<Triangle> triangleList = new List<Triangle>();
            triangleList.Add(triangle);
            _triangleDictionary.Add(vertexIndexKey, triangleList);
        }
    }

    private void CalculateMeshOutlines()
    {
        for (int vertexIndex = 0; vertexIndex < _vertices.Count; vertexIndex++)
        {
            if (!_checkedVertices.Contains(vertexIndex))
            {
                int newOutlineVertex = GetConnectedOutlineVertex(vertexIndex);
                if (newOutlineVertex != -1)
                {
                    _checkedVertices.Add(vertexIndex);

                    List<int> newOutline = new List<int>();
                    newOutline.Add(vertexIndex);
                    _outlines.Add(newOutline);
                    FollowRoomOutline(newOutlineVertex, _outlines.Count - 1);
                    _outlines[_outlines.Count - 1].Add(vertexIndex);
                }
            }
        }
    }

    private void FollowRoomOutline(int vertexIndex, int outlineIndex)
    {
        _outlines[outlineIndex].Add(vertexIndex);
        _checkedVertices.Add(vertexIndex);
        int nextVertexIndex = GetConnectedOutlineVertex(vertexIndex);

        if (nextVertexIndex != -1) FollowRoomOutline(nextVertexIndex, outlineIndex);
    }

    private int GetConnectedOutlineVertex(int vertexIndex)
    {
        List<Triangle> trianglesContainingVertex = _triangleDictionary[vertexIndex];

        for (int i = 0; i < trianglesContainingVertex.Count; i++)
        {
            Triangle triangle = trianglesContainingVertex[i];

            for (int j = 0; j < 3; j++)
            {
                int vertexB = triangle[j];
                if (vertexB != vertexIndex && !_checkedVertices.Contains(vertexB))
                {
                    if (IsRoomOutlineEdge(vertexIndex, vertexB)) return vertexB;
                }
            }
        }

        return -1;
    }

    private bool IsRoomOutlineEdge(int vertexA, int vertexB)
    {
        List<Triangle> trianglesContainingVertexA = _triangleDictionary[vertexA];
        int sharedTriangleCount = 0;

        for (int i = 0; i < trianglesContainingVertexA.Count; i++)
        {
            if (trianglesContainingVertexA[i].Contains(vertexB))
            {
                sharedTriangleCount++;
                if (sharedTriangleCount > 1) break;
            }
        }

        return sharedTriangleCount == 1;
    }
}