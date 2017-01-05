using Assets.Scripts.MapGeneration.Types;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.MapGeneration.Generators
{
    public static class MeshGeneratorHelper
    {
        public static void MeshFromPoints(List<Vector3> vertices, List<int> triangles, params Node[] points)
        {
            AssignVertices(vertices, points);

            CreateTriangle(triangles, points[1], points[3], points[0]);
            CreateTriangle(triangles, points[1], points[2], points[3]);
        }

        public static void AssignVertices(List<Vector3> vertices, Node[] points)
        {
            for (int i = 0; i < points.Length; i++)
            {
                if (points[i].VertexIndex == -1)
                {
                    points[i].VertexIndex = vertices.Count;
                    vertices.Add(points[i].Position);
                }
            }
        }

        private static void CreateTriangle(List<int> triangles, Node a, Node b, Node c)
        {
            triangles.Add(a.VertexIndex);
            triangles.Add(b.VertexIndex);
            triangles.Add(c.VertexIndex);
        }

        public static void ResetSquareNodes(Square square)
        {
            ResetNodeIndex(square.TopLeft);
            ResetNodeIndex(square.TopRight);
            ResetNodeIndex(square.BottomRight);
            ResetNodeIndex(square.BottomLeft);
        }

        public static void ResetNodeIndex(Node node)
        {
            node.VertexIndex = -1;
        }

        public static void AddUVs(List<Vector2> uvs)
        {
            uvs.Add(new Vector2(0, 1));
            uvs.Add(new Vector2(1, 1));
            uvs.Add(new Vector2(1, 0));
            uvs.Add(new Vector2(0, 0));
        }
    }
}