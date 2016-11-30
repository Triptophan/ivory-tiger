using System.Diagnostics;
using UnityEngine;

namespace Assets.Scripts.MapGeneration.Types
{
    [DebuggerDisplay("VertexIndex: {VertexIndex}, Position: {Position}")]
    public class Node
    {
        public Vector3 Position;

        public int VertexIndex = -1;

        public Node(Vector3 position)
        {
            Position = position;
        }
    }
}