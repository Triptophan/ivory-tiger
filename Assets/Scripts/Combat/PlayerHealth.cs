using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float StartingHealth = 100f;
    public float CurrentHealth = 100f;
    public float MaxHealth = 100f;

    public float HealthIndicatorFillAmount
    {
        get { return CurrentHealth / MaxHealth; }
    }

    public Action OnPlayerDeath { get; set; }

    public Action PlayerHealthChanged { get; set; }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyUp(KeyCode.Minus))
        {
            DoDamage(50f);
        }
#endif
    }

    public void ResetHealth()
    {
        UpdateHealth(-StartingHealth);
    }

    public void DoDamage(float amount)
    {
        UpdateHealth(amount);
    }

    public void Heal(float amount)
    {
        UpdateHealth(-amount);
    }

    private void UpdateHealth(float value)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth - value, 0f, 100f);

        if (OnPlayerDeath == null)
        {
            throw new NotImplementedException("OnPlayerDeath is not implemented in PlayerHealth, please assign a listener.");
        }

        OnPlayerHealthChanged();

        if (CurrentHealth == 0)
        {
            OnPlayerDeath();
        }
    }

    private void OnPlayerHealthChanged()
    {
        if(PlayerHealthChanged != null)
        {
            PlayerHealthChanged();
        }
    }
}