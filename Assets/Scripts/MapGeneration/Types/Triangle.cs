using UnityEngine;

namespace Assets.Scripts.MapGeneration.Types
{
    internal struct Triangle
    {
        public int VertexIndexA;
        public int VertexIndexB;
        public int VertexIndexC;

        private int[] _vertices;

        public Triangle(int a, int b, int c)
        {
            VertexIndexA = a;
            VertexIndexB = b;
            VertexIndexC = c;

            _vertices = new int[3];
            _vertices[0] = a;
            _vertices[1] = b;
            _vertices[2] = c;
        }

        public int this[int i]
        {
            get { return _vertices[i]; }
        }

        public bool Contains(int vertexIndex)
        {
            return vertexIndex == VertexIndexA ||
                    vertexIndex == VertexIndexB ||
                    vertexIndex == VertexIndexC;
        }

        public void Print()
        {
            Debug.Log(string.Format("VertexIndexA: {0}, VertexIndexB: {1}, VertexIndexC: {2}", VertexIndexA, VertexIndexB, VertexIndexC));
        }
    }
}