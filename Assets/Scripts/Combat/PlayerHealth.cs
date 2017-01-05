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

    public void ResetHealth()
    {
        CurrentHealth = StartingHealth;
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
    }
}