﻿using System.Diagnostics;
using UnityEngine;

namespace Assets.Scripts.MapGeneration.Types
{
    [DebuggerDisplay("VertexIndex: {VertexIndex}, Position: {Position}")]
    public class Node : IHeapItem<Node>
    {
        public Vector3 Position;

        public int VertexIndex = -1;

        public Node(Vector3 position)
        {
            Position = position;
        }
        public bool walkable;
        public Vector3 worldPosition;

        /// <summary>
        /// How far away the node is from the parent node
        /// </summary>
        public int gCost;
        /// <summary>
        /// How far node is away from target node
        /// </summary>
        public int hCost;
        public int gridX;
        public int gridY;
        int heapIndex;

        public Node Parent;
        public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY)
        {
            walkable = _walkable;
            worldPosition = _worldPos;
            gridX = _gridX;
            gridY = _gridY;
        }

        /// <summary>
        /// gCost + hCost
        /// </summary>
        public int fCost { get { return gCost + hCost; } }

        public int Heapindex
        {
            get
            {
                return heapIndex;
            }

            set
            {
                heapIndex = value;
            }
        }

        public int CompareTo(Node nodeToCompare)
        {
            int compare = fCost.CompareTo(nodeToCompare.fCost);
            if (compare == 0)
            {
                compare = hCost.CompareTo(nodeToCompare.hCost);
            }

            return -compare;
        }

    }
}