using UnityEngine;
using Assets.Scripts.StateMachine.States.GameFlowStates;

namespace Assets.Scripts.Environment
{
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
}