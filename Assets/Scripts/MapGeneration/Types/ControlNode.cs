using Assets.Scripts.MapGeneration.Types;
using System.Diagnostics;
using UnityEngine;

[DebuggerDisplay("Active: {Active}, Position: {Position}")]
public class ControlNode : Node
{
    public bool Active;

    public ControlNode(bool active, Vector3 position, float squareSize) : base(position)
    {
        Active = active;
    }
}