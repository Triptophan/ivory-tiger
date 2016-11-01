using UnityEngine;

namespace Assets.Scripts
{
    public class ControlNode : Node
    {
        public Node Above,
                    Right;

        public bool Active;

        public ControlNode(bool active, Vector3 position, float squareSize) : base(position)
        {
            Active = active;
            Above = new Node(position + Vector3.forward * squareSize);
            Right = new Node(position + Vector3.right * squareSize);
        }
    }
}