// Char1_0Controller.cs
// Unity 6 (6.03+) uyumlu 2D karakter kontrolcüsü
// Kontroller: W (zıplama/çift zıplama), A (sol), D (sağ), Shift (dash)

using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(AudioSource))]
public class Char1_0Controller : MonoBehaviour
{
    [Header("Hareket Ayarları")]
    [Tooltip("Normal yatay hareket hızı")]
    public float moveSpeed = 7f;

    [Tooltip("Zıplama kuvveti")]
    public float jumpForce = 12f;

    [Tooltip("Maksimum zıplama sayısı (çift zıplama için 2)")]
    public int maxJumps = 2;

    [Header("Dash Ayarları")]
    [Tooltip("Dash sırasında hız çarpanı (2x)")]
    public float dashSpeedMultiplier = 2f;

    [Tooltip("Dash süresi (saniye)")]
    public float dashDuration = 0.2f;

    [Tooltip("Dash bekleme süresi (cooldown) (saniye)")]
    public float dashCooldown = 5f;


    [Header("Zemin Kontrolü")]
    [Tooltip("Zemin kontrol noktası (Transform)")]
    public Transform groundCheck;

    [Tooltip("Zemin kontrol yarıçapı")]
    public float groundCheckRadius = 0.2f;

    [Tooltip("Zemin layer mask'i")]
    public LayerMask groundLayer;

    [Header("Ses Efektleri")]
    [Tooltip("Zıplama Sesi")]
    public AudioClip jumpSound;
    [Tooltip("Hareket Sesleri (Adım)")]
    public AudioClip[] moveSounds;
    [Tooltip("Adım sesi aralığı (saniye)")]
    public float stepInterval = 0.35f;

    // Public properties for Animation Script
    public bool IsGrounded => isGrounded;
    public bool IsDashing => isDashing;
    public float MoveDirection => currentMoveDirection;
    public Vector2 Velocity => rb.linearVelocity;
    
    // Event for Jump trigger
    public event System.Action OnJump;
    
    // Event for Collect trigger
    public event System.Action OnCollect;

    // Private değişkenler
    private Rigidbody2D rb;
    private int currentJumpCount = 0;
    private bool isGrounded = false;
    private bool isDashing = false;
    private float dashTimeRemaining = 0f;
    private float currentMoveDirection = 0f;
    private float originalGravityScale;
    private float nextGroundCheckTime = 0f; // Zıplama sonrası hemen zemin kontrolü yapmamak için
    private float nextDashTime = 0f; // Bir sonraki dash zamanı
    private float nextStepTime = 0f; // Bir sonraki adım sesi zamanı
    private AudioSource audioSource;


    // Sprite yönlendirmesi için
    private SpriteRenderer spriteRenderer;
    private Vector3 originalScale;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>(); // AudioSource bileşenini al
        
        // --- AUDIO FIX: Sesi 2D Olarak Zorla ---
        if (audioSource != null)
        {
            audioSource.spatialBlend = 0f; // 0 = 2D, 1 = 3D
        }
        else
        {
            Debug.LogError("Char1_0Controller: AudioSource bileşeni bulunamadı!");
        }

        // --- AUDIO DEBUG: Atamaları Kontrol Et ---
        if (jumpSound == null) Debug.LogWarning("Char1_0Controller: Jump Sound atanmamış!");
        if (moveSounds == null || moveSounds.Length == 0) Debug.LogWarning("Char1_0Controller: Move Sounds atanmamış!");
        // Daha iyi çarpışma algılama için
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        originalGravityScale = rb.gravityScale;

        // --- FIX: Duvarlara yapışmayı önlemek için sürtünmesiz materyal oluştur ve ata ---
        PhysicsMaterial2D slipperyMaterial = new PhysicsMaterial2D("Slippery");
        slipperyMaterial.friction = 0f;
        slipperyMaterial.bounciness = 0f;
        rb.sharedMaterial = slipperyMaterial;
        // ---------------------------------------------------------------------------------

        spriteRenderer = GetComponent<SpriteRenderer>();
        originalScale = transform.localScale;

        // Eğer groundCheck atanmamışsa, karakterin altında otomatik oluştur
        if (groundCheck == null)
        {
            GameObject groundCheckObj = new GameObject("GroundCheck");
            groundCheckObj.transform.parent = transform;
            groundCheckObj.transform.localPosition = new Vector3(0, -0.5f, 0);
            groundCheck = groundCheckObj.transform;
        }

        // Eğer groundLayer atanmamışsa, "Default" layer kullan
        if (groundLayer == 0)
        {
            groundLayer = LayerMask.GetMask("Default");
        }
    }

    void Update()
    {
        if (!isInputActive) return;

        // Zemin kontrolü (zıplama cooldown süresince yapma)
        if (Time.time >= nextGroundCheckTime)
        {
            CheckGrounded();
        }

        // Hareket girişi
        HandleMovementInput();

        // Zıplama girişi
        HandleJumpInput();

        // Dash girişi
        HandleDashInput();

        // Hareket sesi
        HandleMovementAudio();
    }

    void FixedUpdate()
    {
        // Fiziği FixedUpdate'de uygula
        ApplyMovement();
    }

    /// <summary>
    /// Zemin kontrolü
    /// </summary>
    void CheckGrounded()
    {
        bool wasGrounded = isGrounded;
        isGrounded = false; // Varsayılan olarak havada varsay

        // Tüm çakışmaları al
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, groundCheckRadius, groundLayer);
        
        foreach (Collider2D collider in colliders)
        {
            // Eğer çarptığımız şey kendimiz değilse, yerdedir
            if (collider.gameObject != gameObject)
            {
                isGrounded = true;
                break;
            }
        }

        // Yerdeysek zıplama hakkını sıfırla
        if (isGrounded)
        {
            currentJumpCount = 0;
            
            // Eğer dash bitmişse ama hala gravity bozuksa düzelt (Güvenlik önlemi)
            if (!isDashing && rb.gravityScale == 0)
            {
                 rb.gravityScale = originalGravityScale;
            }
        }
    }

    /// <summary>
    /// A ve D tuşlarıyla yatay hareket girişi
    /// </summary>
    void HandleMovementInput()
    {
        currentMoveDirection = 0f;

        if (Input.GetKey(KeyCode.A))
        {
            currentMoveDirection = -1f;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            currentMoveDirection = 1f;
        }

        // Sprite yönlendirmesi
        if (currentMoveDirection != 0)
        {
            FlipSprite(currentMoveDirection > 0);
        }
    }

    /// <summary>
    /// W tuşuyla zıplama ve çift zıplama
    /// </summary>
    void HandleJumpInput()
    {
        // Dash sırasında zıplamaya izin ver veya verme (tercihe bağlı, şu an veriyoruz)
        if (Input.GetKeyDown(KeyCode.W))
        {
            // Sadece zıplama hakkımız varsa zıpla
            // isGrounded kontrolünü buraya eklemeye gerek yok çünkü isGrounded true ise zaten count 0'dır.
            if (currentJumpCount < maxJumps)
            {
                Jump();
            }
        }
    }

    /// <summary>
    /// Shift tuşuyla dash
    /// </summary>
    void HandleDashInput()
    {
        // Dash başlat
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            // Hareket ediyor olmalı, dash yapıyor olmamalı VE cooldown bitmiş olmalı
            if (!isDashing && currentMoveDirection != 0 && Time.time >= nextDashTime)
            {
                StartDash();
            }
        }

        // Dash süresini güncelle
        if (isDashing)
        {
            dashTimeRemaining -= Time.deltaTime;
            if (dashTimeRemaining <= 0)
            {
                EndDash();
            }
        }
    }


    /// <summary>
    /// Hareketi uygula
    /// </summary>
    void ApplyMovement()
    {
        float speed = moveSpeed;

        // Dash aktifse hızı 2 katına çıkar
        if (isDashing)
        {
            speed *= dashSpeedMultiplier;
        }

        // Yatay hızı ayarla, dikey hızı koru (Dash sırasında dikey hız 0 olabilir eğer istenirse)
        if (isDashing)
        {
             // Dash sırasında yerçekimi yok, dümdüz git
             rb.linearVelocity = new Vector2(currentMoveDirection * speed, 0f);
        }
        else
        {
             rb.linearVelocity = new Vector2(currentMoveDirection * speed, rb.linearVelocity.y);
        }
    }

    /// <summary>
    /// Zıplama işlemi
    /// </summary>
    void Jump()
    {
        // Dikey hızı sıfırla ve yukarı kuvvet uygula
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        
        currentJumpCount++;

        // Trigger event
        OnJump?.Invoke();
        
        // Zıpladık, kısa bir süre ground check bizi resetlemesin
        isGrounded = false;
        nextGroundCheckTime = Time.time + 0.1f; // 0.1 saniye bekle

        // Zıplama Sesi
        if (audioSource != null && jumpSound != null)
        {
        // Zıplama Sesi
        if (audioSource != null && jumpSound != null)
        {
            audioSource.PlayOneShot(jumpSound);
            // Debug.Log("Zıplama sesi çalındı.");
        }
        else if (jumpSound == null) 
        {
             // Debug.LogWarning("Zıplama sesi çalınamadı (jumpSound eksik).");
        }
        }
    }

    /// <summary>
    /// Dash başlat
    /// </summary>
    void StartDash()
    {
        isDashing = true;
        dashTimeRemaining = dashDuration;
        
        // Cooldown'ı başlat
        nextDashTime = Time.time + dashCooldown;
        
        // Dash sırasında yerçekimini kapat
        rb.gravityScale = 0f;

        // Dikey hızı sıfırla
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
    }

    /// <summary>
    /// Dash bitir
    /// </summary>
    void EndDash()
    {
        isDashing = false;
        dashTimeRemaining = 0f;
        
        // Yerçekimini geri aç
        rb.gravityScale = originalGravityScale;
    }

    /// <summary>
    /// Sprite yönlendirmesi
    /// </summary>
    void FlipSprite(bool faceRight)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = !faceRight;
        }
        else
        {
            // SpriteRenderer yoksa scale ile çevir
            Vector3 scale = originalScale;
            scale.x = faceRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
    }

    /// <summary>
    /// Editor'da zemin kontrol alanını görselleştir
    /// </summary>
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }

    /// <summary>
    /// Çarpışma başladığında (Garanti zemin algılama)
    /// </summary>
    void OnCollisionEnter2D(Collision2D collision)
    {
        EvaluateCollision(collision);
    }

    /// <summary>
    /// Çarpışma devam ederken (Garanti zemin algılama)
    /// </summary>
    void OnCollisionStay2D(Collision2D collision)
    {
        EvaluateCollision(collision);
    }

    /// <summary>
    /// Hareket seslerini yönetir
    /// </summary>
    void HandleMovementAudio()
    {
        // Yerdeysek ve hareket ediyorsak ve zamanı geldiyse
        if (isGrounded && Mathf.Abs(rb.linearVelocity.x) > 0.1f && Time.time >= nextStepTime)
        {
            PlayStepSound();
            // Eğer dash yapıyorsak adımlar daha sık çıksın
            float interval = isDashing ? stepInterval / 2f : stepInterval;
            nextStepTime = Time.time + interval;
        }
    }

    void PlayStepSound()
    {
        if (audioSource != null && moveSounds != null && moveSounds.Length > 0)
        {
            // Rastgele bir adım sesi seç
            int index = Random.Range(0, moveSounds.Length);
            // Sesi çal (pitch'i biraz değiştirerek doğal duyulmasını sağla)
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(moveSounds[index]);
            audioSource.pitch = 1f; // Pitch'i resetle
        }
    }

    /// <summary>
    /// Çarpışma analizi: Eğer zeminse zıplama hakkını yenile
    /// </summary>
    void EvaluateCollision(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            // Eğer çarpışma normali yukarı bakıyorsa (zemin)
            if (contact.normal.y > 0.5f)
            {
                isGrounded = true;
                currentJumpCount = 0;
                
                // Gravity fix
                if (!isDashing && rb.gravityScale == 0)
                {
                     rb.gravityScale = originalGravityScale;
                }
                
                break;
            }
        }
    }
    
    /// <summary>
    /// Collect olayını tetikler
    /// </summary>
    public void TriggerCollectAction()
    {
        OnCollect?.Invoke();
    }

    /// <summary>
    /// Oyuncu girdilerini açar veya kapatır (Oyun sonu, zafer vb. durumlar için)
    /// </summary>
    public void SetInputActive(bool active)
    {
        isInputActive = active;
        if (!active)
        {
            // Hareket durdur
            rb.linearVelocity = Vector2.zero;
            currentMoveDirection = 0;
            // Animasyonları durdurmak için gerekirse buraya ekleme yapılabilir
        }
    }
    
    private bool isInputActive = true;
}
