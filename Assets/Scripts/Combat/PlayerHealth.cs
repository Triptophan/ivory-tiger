using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public float StartingHealth = 100f;
    public float CurrentHealth = 100f;
    public float MaxHealth = 100f;

    public Image HealthIndicatorImage;

    public void ResetHealth()
    {
        CurrentHealth = StartingHealth;
        UpdateHealthUI();
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
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        if (HealthIndicatorImage)
        { 
            HealthIndicatorImage.fillAmount = CurrentHealth / MaxHealth;
        } 
    }
}