using UnityEngine;
using System.Collections;

public class PlayerDeathState : State
{
	private static PlayerDeathState _instance;

	public static State Instance
	{
		get
		{
			if (_instance == null) _instance = new PlayerDeathState();
			return _instance;
		}
	}

	public override void Enter(GameObject entity)
	{

	}

	public override void Execute(GameObject entity)
	{

	}

	public override void Exit(GameObject entity)
	{
		
	}
}