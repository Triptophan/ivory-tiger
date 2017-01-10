using Assets.Scripts.Combat.Projectiles;
using UnityEngine;
using System;

namespace Assets.Scripts.Player
{
	public class CombatController : MonoBehaviour
	{
		GameObject _playerObject;
		Transform _viewTransform;

		PlayerHealth _playerHealth;

		public GameObject Projectile;

		public Camera PlayerView;

		public float PlayerHealthIndicatorFillAmount
		{
			get { return _playerHealth.HealthIndicatorFillAmount; }
		}

		public bool CanFire;

		public bool IsDead;
		public Action OnDeath { get; set; }

		public void Awake()
		{
			_playerObject = gameObject;
			if (PlayerView != null) _viewTransform = PlayerView.transform;

			_playerHealth = _playerObject.GetComponent<PlayerHealth>();
		}

		public void Update()
		{
			GetInput();
		}

		public void ResetPlayer()
		{
			_playerHealth.ResetHealth();
			IsDead = false;
		}

		void GetInput()
		{
			if (CanFire && Input.GetButtonUp("Fire1"))
			{
				var projectileObject = (GameObject)Instantiate(Projectile, _viewTransform.TransformPoint(Vector3.forward - Vector3.up / 2f), _viewTransform.rotation);

				var projectileBehavior = projectileObject.GetComponent<Rock>();
				projectileBehavior.OwnerTransform = _viewTransform;
			}
		}

		Vector3 GetCameraCenter()
		{
			return new Vector3(Screen.width / 2, Screen.height / 2, PlayerView.nearClipPlane);
		}

		void OnCollisionEnter(Collision collision)
		{
			CheckEnemyCollision(collision.gameObject);
		}

		void OnCollisionStay(Collision collision)
		{
			CheckEnemyCollision(collision.gameObject);
		}

		void CheckEnemyCollision(GameObject collidedObject)
		{
			if (collidedObject.tag == "Enemy")
			{
				_playerHealth.DoDamage(10f);

				IsDead = _playerHealth.CurrentHealth <= 0;
				if (IsDead) OnDeath();
			}
		}
	}
}