using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Ses Ayarları")]
    public AudioSource hoparlor;
    public AudioClip hasarSesi;

    public int maxHealth = 100;
    public int currentHealth;

    public Slider healthBar;

    void Start()
    {
        currentHealth = maxHealth;

        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
    }

    public void CaniFulle(int miktar)
    {
        currentHealth += miktar;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        if (healthBar != null)
        {
            healthBar.value = currentHealth;
        }

        Debug.Log("Harita değişti, oyuncu ödüllendirildi! Mevcut can: " + currentHealth);
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;

        if (healthBar != null)
        {
            healthBar.value = currentHealth;
        }

        if (hoparlor != null && hasarSesi != null)
        {
            hoparlor.PlayOneShot(hasarSesi);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // --- OPTİMİZASYON: Arama (Find) yapmak yerine doğrudan Singleton'ı çağırdık ---
        if (GameManager.instance != null)
        {
            GameManager.instance.OyunuBitir();
        }

        Destroy(gameObject);
    }
}