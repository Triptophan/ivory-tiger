using UnityEngine;

public abstract class State
{
    public abstract void Enter(GameObject entity);

    public abstract void Execute(GameObject entity);

    public abstract void Exit(GameObject entity);
}