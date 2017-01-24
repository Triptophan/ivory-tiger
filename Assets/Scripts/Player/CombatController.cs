using System;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class CombatController : MonoBehaviour
{
    private GameObject _playerObject;
    private Transform _viewTransform;

    private PlayerHealth _playerHealth;

    public GameObject Projectile;

    public Camera PlayerView;

    public float PlayerHealthIndicatorFillAmount
    {
        get { return _playerHealth.HealthIndicatorFillAmount; }
    }

    public Action PlayerHealthChanged { get; set; }
    public Action OnDeath { get; set; }

    [HideInInspector]
    public bool CanFire;

    [HideInInspector]
    public bool IsDead;

    private void Awake()
    {
        _playerObject = gameObject;
        if (PlayerView != null) _viewTransform = PlayerView.transform;

        _playerHealth = _playerObject.GetComponent<PlayerHealth>();
        _playerHealth.OnPlayerDeath = PlayerDied;
        _playerHealth.PlayerHealthChanged = OnPlayerHeathChanged;
    }

    private void Update()
    {
        GetInput();
    }

    public void ResetPlayer()
    {
        _playerHealth.ResetHealth();
    }

    private void GetInput()
    {
        if (CanFire && Input.GetButtonUp("Fire1"))
        {
            var projectileObject = (GameObject)Instantiate(Projectile, _viewTransform.TransformPoint(Vector3.forward - Vector3.up / 2f), _viewTransform.rotation);

            var projectileBehavior = projectileObject.GetComponent<Rock>();
            projectileBehavior.OwnerTransform = _viewTransform;
        }
    }

    private Vector3 GetCameraCenter()
    {
        return new Vector3(Screen.width / 2, Screen.height / 2, PlayerView.nearClipPlane);
    }

    private void OnCollisionEnter(Collision collision)
    {
        CheckEnemyCollision(collision.gameObject);
    }

    private void OnCollisionStay(Collision collision)
    {
        CheckEnemyCollision(collision.gameObject);
    }

    private void CheckEnemyCollision(GameObject collidedObject)
    {
        if (collidedObject.tag == "Enemy")
        {
            _playerHealth.DoDamage(10f);
        }
    }

    private void OnPlayerHeathChanged()
    {
        if (PlayerHealthChanged != null)
        {
            PlayerHealthChanged();
        }
    }

    private void PlayerDied()
    {
        if (OnDeath == null)
        {
            throw new NotImplementedException("CombatController:OnDeath cannot be null.  Please assign a listener.");
        }

        CanFire = false;

        //We do not have access to the GameManager state machine here, bubble event up.
        OnDeath();
    }
}