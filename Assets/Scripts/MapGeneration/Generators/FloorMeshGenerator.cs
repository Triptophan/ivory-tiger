using Assets.Scripts.MapGeneration.Enumerations;
using Assets.Scripts.MapGeneration.Types;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.MapGeneration.Generators
{
    public class FloorMeshGenerator
    {
        private SquareGrid _squareGrid;
        private MeshFilter _meshFilter;
        private MeshCollider _meshCollider;

        private Dictionary<int, List<int>> _triangleDictionary;

        public int MaterialsRequired = 1;

        public FloorMeshGenerator(
            SquareGrid squareGrid,
            MeshFilter meshFilter,
            MeshCollider meshCollider)
        {
            _triangleDictionary = new Dictionary<int, List<int>>();
            _squareGrid = squareGrid;
            _meshFilter = meshFilter;
            _meshCollider = meshCollider;
        }

        public void Generate()
        {
            List<Vector3> vertices = new List<Vector3>();
            Mesh mesh = new Mesh();
            List<Vector2> uvs = new List<Vector2>();

            foreach (var square in _squareGrid.Squares)
            {
                if (square == null || square.TileType != TileType.Room) continue;
                MeshGeneratorHelper.ResetSquareNodes(square);
                GenerateSubMeshes(square, vertices);
                MeshGeneratorHelper.AddUVs(uvs);
            }

            mesh.vertices = vertices.ToArray();

            mesh.subMeshCount = _triangleDictionary.Keys.Count;
            MaterialsRequired = _triangleDictionary.Keys.Count;
            for(int i = 0; i < _triangleDictionary.Keys.Count; i++)
            {
                var triangleIndex = _triangleDictionary.ContainsKey(-1) ? i - 1 : i;
                mesh.SetTriangles(_triangleDictionary[triangleIndex], i);
            }

            mesh.RecalculateNormals();

            mesh.uv = uvs.ToArray();
            _meshFilter.mesh = mesh;

            _meshCollider.sharedMesh = mesh;
        }

        private void GenerateSubMeshes(Square square, List<Vector3> vertices)
        {
            Node[] points = { square.TopLeft, square.TopRight, square.BottomRight, square.BottomLeft };

            MeshGeneratorHelper.AssignVertices(vertices, points);

            CreateTriangles(square.RoomIndex, points[1], points[3], points[0]);
            CreateTriangles(square.RoomIndex, points[1], points[2], points[3]);
        }

        private void CreateTriangles(int roomIndex, Node a, Node b, Node c)
        {
            var triangles = new List<int>();
            triangles.Add(a.VertexIndex);
            triangles.Add(b.VertexIndex);
            triangles.Add(c.VertexIndex);

            AddTrianglesToDictionary(roomIndex, triangles);
        }

        private void AddTrianglesToDictionary(int roomIndex, List<int> triangles)
        {
            if(_triangleDictionary.ContainsKey(roomIndex))
            {
                _triangleDictionary[roomIndex].AddRange(triangles);
            }
            else
            {
                _triangleDictionary.Add(roomIndex, triangles);
            }
        }
    }
}
