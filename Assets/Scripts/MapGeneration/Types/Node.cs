﻿using UnityEngine;

namespace Assets.Scripts.MapGeneration.Types
{
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