using UnityEngine;

public class LevelExit : MonoBehaviour
{
	public GameManager GameManager;

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			GameManager.StateMachine.ChangeGlobalState(GameRestartState.Instance);
		}
	}
}