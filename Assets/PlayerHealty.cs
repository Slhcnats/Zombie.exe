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

    private bool oldu;
    private float hasarKorumaBitisZamani;

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

    public void MaximumCaniArtir(int miktar)
    {
        maxHealth += miktar;
        currentHealth = maxHealth;

        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
    }

    public void TakeDamage(int damageAmount)
    {
        if (oldu || Time.time < hasarKorumaBitisZamani) return;

        currentHealth -= damageAmount;
        HasarEkranEfekti.Goster();

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
        if (oldu) return;
        oldu = true;

        // --- OPTİMİZASYON: Arama (Find) yapmak yerine doğrudan Singleton'ı çağırdık ---
        if (GameManager.instance != null)
        {
            GameManager.instance.OyuncuOldu(this);
            return;
        }

        Destroy(gameObject);
    }

    public void OlumAnindaDevreDisiBirak()
    {
        PlayerMovement hareket = GetComponent<PlayerMovement>();
        if (hareket != null) hareket.enabled = false;

        PlayerShooting ates = GetComponent<PlayerShooting>();
        if (ates != null) ates.enabled = false;

        Rigidbody2D fizik = GetComponent<Rigidbody2D>();
        if (fizik != null) fizik.linearVelocity = Vector2.zero;
    }

    public void HayataDondur()
    {
        oldu = false;
        currentHealth = maxHealth;
        hasarKorumaBitisZamani = Time.time + 2f;

        if (healthBar != null) healthBar.value = currentHealth;

        PlayerMovement hareket = GetComponent<PlayerMovement>();
        if (hareket != null) hareket.enabled = true;

        PlayerShooting ates = GetComponent<PlayerShooting>();
        if (ates != null) ates.enabled = true;
    }

    public void OlumuKesinlestir()
    {
        Destroy(gameObject);
    }
}
