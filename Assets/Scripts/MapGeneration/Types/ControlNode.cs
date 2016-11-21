using UnityEngine;
using System.Diagnostics;

namespace Assets.Scripts.MapGeneration.Types
{
    [DebuggerDisplay("Active: {Active}, Position: {Position}")]
    public class ControlNode : Node
    {
        public bool Active;

        public ControlNode(bool active, Vector3 position, float squareSize) : base(position)
        {
            Active = active;
        }
    }
}