using System.Collections;
using UnityEngine;

public class PumpkinMovement : MonoBehaviour 
{
    [Header("Ground Detection")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.5f;
    public LayerMask groundLayer;

    [Header("Movement Settings")]
    public float walkSpeed = 4f;
    public float runSpeed = 7f;
    public float slideSpeed = 8f;
    public float jumpForce = 10f;
    public float acceleration = 10f;
    public float deceleration = 15f;
    public float airControl = 0.7f;

    [Header("Spawn Settings")]
    public Vector3 spawnPosition = new Vector3(-8, 2, 0); // MULAI DARI KIRI

    [Header("Visual Effects")] // TAMBAH EFEK VISUAL
    public Color hurtColor = Color.red;
    public float hurtFlashDuration = 0.3f;

    // Private variables
    private Rigidbody2D rb;
    private Animator anim;
    private int healthPoints = 3; // 3 NYAWA
    private bool isDead;
    private bool facingRight = true;

    // TAMBAH: Method untuk get health points
    public int GetHealthPoints()
    {
        return healthPoints;
    }

    public int GetMaxHealthPoints()
    {
        return 3; // Karena default 3 nyawa
    }
    private Vector3 localScale;
    private bool isGrounded;
    private bool isSliding;
    private bool wasGrounded; // For landing detection
    private float footstepTimer;
    private float footstepInterval = 0.3f; // Time between footsteps
    private float moveInput;
    private float currentSpeed;
    private float targetSpeed;
    private SpriteRenderer spriteRenderer; // TAMBAH
    private Color originalColor; // TAMBAH

    void Start() 
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        localScale = transform.localScale;

        // TAMBAH: Get SpriteRenderer untuk visual effects
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

        // === RESET ALL STATE FOR LEVEL START/RESTART ===
        ResetPlayerState();

        // Rigidbody setup untuk movement yang smooth
        if (rb != null)
        {
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            rb.gravityScale = 3f;
        }

        // Validation
        if (groundCheck == null)
            Debug.LogError("GroundCheck tidak di-assign!");
    }

    /// <summary>
    /// Reset semua state player ke kondisi awal level
    /// </summary>
    void ResetPlayerState()
    {
        // Reset position ke spawn point
        transform.position = spawnPosition;
        
        // Reset facing direction
        facingRight = true;
        localScale = new Vector3(Mathf.Abs(localScale.x), localScale.y, localScale.z);
        transform.localScale = localScale;
        
        // Reset health dan state
        healthPoints = 3; // RESET KE 3 NYAWA
        isDead = false;

        // TAMBAH: Update health display saat reset
        if (GameManager.Instance != null)
        {
            GameManager.Instance.UpdateHealthDisplay();
        }
        
        // Reset movement state
        currentSpeed = 0;
        targetSpeed = 0;
        moveInput = 0;
        isSliding = false;
        
        // Reset visual effects
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
        transform.localScale = localScale;
        
        // Reset velocity dan physics constraints
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = 3f; // Reset gravity ke normal
            rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Reset constraints
        }
        
        // Reset animations - dengan safety check
        if (anim != null)
        {
            // Reset all animator parameters
            if (HasParameter("isDead"))
                anim.SetBool("isDead", false);
            if (HasParameter("isWalking"))
                anim.SetBool("isWalking", false);
            if (HasParameter("isRunning"))
                anim.SetBool("isRunning", false);
            if (HasParameter("isSliding"))
                anim.SetBool("isSliding", false);
            if (HasParameter("isJumping"))
                anim.SetBool("isJumping", false);
                
            // Coba play default state, tapi dengan safety
            TryPlayAnimation("Idle");
        }
        
        Debug.Log("🔄 Player state reset! Health: 3/3");
    }
    
    void TryPlayAnimation(string stateName)
    {
        if (anim == null) return;
        
        // Cek apakah state ada sebelum play
        AnimatorStateInfo[] states = new AnimatorStateInfo[anim.layerCount];
        for (int i = 0; i < anim.layerCount; i++)
        {
            states[i] = anim.GetCurrentAnimatorStateInfo(i);
        }
        
        // Coba play, tapi jangan force jika state tidak ada
        try
        {
            anim.Play(stateName, 0, 0f);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"⚠️ Could not play animation '{stateName}': {e.Message}");
            // Fallback: biarkan animator di state default
        }
    }

    void Update()
    {
        if (isDead)
        {
            rb.linearVelocity = Vector2.zero; // Stop all movement when dead
            return;
        }

        CheckGround();
        GetInput();
        HandleJump();
        HandleFootsteps();
        UpdateAnimations();
    }

    void FixedUpdate()
    {
        if (isDead) return;
        
        ApplyMovement();
        FlipSprite();
    }

    void CheckGround()
    {
        bool wasGroundedBefore = isGrounded;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Detect landing (was in air, now on ground)
        if (!wasGroundedBefore && isGrounded && !isDead)
        {
            // Play land sound
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayLandSound();
            }
        }

        // Reset slide saat landing
        if (isGrounded && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.DownArrow))
        {
            isSliding = false;
        }
    }

    void GetInput()
    {
        // Input horizontal (A/D atau Arrow keys)
        moveInput = Input.GetAxisRaw("Horizontal");
        
        // Tentukan target speed berdasarkan input
        if (Mathf.Abs(moveInput) > 0.01f)
        {
            // Cek mode: Slide > Run > Walk
            bool slideInput = (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) && isGrounded;
            bool runInput = Input.GetKey(KeyCode.LeftShift);
            
            if (slideInput)
            {
                targetSpeed = slideSpeed;

                // Play slide sound only when starting to slide
                if (!isSliding && AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlaySlideSound();
                }

                isSliding = true;
            }
            else if (runInput)
            {
                targetSpeed = runSpeed;
                isSliding = false;
            }
            else
            {
                targetSpeed = walkSpeed;
                isSliding = false;
            }
        }
        else
        {
            targetSpeed = 0;
            isSliding = false;
        }
    }

    void ApplyMovement()
    {
        // Hitung kecepatan target
        float targetVelocity = moveInput * targetSpeed;
        
        // Smooth acceleration/deceleration
        float accelRate;
        if (Mathf.Abs(targetVelocity) > 0.01f)
        {
            accelRate = acceleration;
        }
        else
        {
            accelRate = deceleration;
        }
        
        // Kurangi kontrol di udara
        if (!isGrounded)
        {
            accelRate *= airControl;
        }
        
        // Smooth lerp ke target velocity
        currentSpeed = Mathf.Lerp(currentSpeed, targetVelocity, accelRate * Time.fixedDeltaTime);
        
        // Apply velocity (pertahankan velocity Y untuk jump)
        rb.linearVelocity = new Vector2(currentSpeed, rb.linearVelocity.y);
    }

    void HandleJump()
    {
        // Jump hanya bisa saat grounded dan TIDAK sliding
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isSliding)
        {
            // Play jump sound
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayJumpSound();
            }

            // Reset velocity Y untuk jump konsisten
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);

            // Apply jump force
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    void HandleFootsteps()
    {
        // Play footstep sound when walking/running on ground
        if (isGrounded && !isSliding && !isDead)
        {
            float speed = Mathf.Abs(rb.linearVelocity.x);
            bool isMoving = speed > 0.5f;

            if (isMoving)
            {
                footstepTimer -= Time.deltaTime;
                if (footstepTimer <= 0)
                {
                    // Play footstep sound
                    if (AudioManager.Instance != null)
                    {
                        AudioManager.Instance.PlayFootstepSound();
                    }

                    // Reset timer - faster footsteps when running
                    bool isRunning = speed > (walkSpeed + 1f);
                    footstepTimer = isRunning ? footstepInterval * 0.7f : footstepInterval;
                }
            }
            else
            {
                // Reset timer when not moving
                footstepTimer = 0;
            }
        }
    }

    void UpdateAnimations()
    {
        if (anim == null) return;

        // Hitung state berdasarkan velocity aktual (bukan input)
        float speed = Mathf.Abs(rb.linearVelocity.x);
        bool moving = speed > 0.5f;
        bool running = speed > (walkSpeed + 1f);
        
        // Set animator parameters
        if (HasParameter("isWalking"))
            anim.SetBool("isWalking", moving && !running && !isSliding && isGrounded);
        
        if (HasParameter("isRunning"))
            anim.SetBool("isRunning", running && !isSliding && isGrounded);
        
        if (HasParameter("isSliding"))
            anim.SetBool("isSliding", isSliding && isGrounded);
        
        if (HasParameter("isJumping"))
            anim.SetBool("isJumping", !isGrounded);
        
        if (HasParameter("isDead"))
            anim.SetBool("isDead", isDead);
    }

    bool HasParameter(string paramName)
    {
        if (anim == null) return false;
        
        foreach (AnimatorControllerParameter param in anim.parameters)
        {
            if (param.name == paramName)
                return true;
        }
        return false;
    }

    void FlipSprite()
    {
        // Flip berdasarkan input, bukan velocity (lebih responsive)
        if (moveInput > 0.1f)
        {
            facingRight = true;
        }
        else if (moveInput < -0.1f)
        {
            facingRight = false;
        }

        // Apply flip
        if ((facingRight && localScale.x < 0) || (!facingRight && localScale.x > 0))
        {
            localScale.x *= -1;
        }

        transform.localScale = localScale;
    }

    // ===================================================================================
    // COLLISION & DAMAGE
    // ===================================================================================

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("DeadZone") || col.gameObject.name.Contains("DeadZone"))
        {
            // INSTANT DEATH - langsung mati saat terkena deadzone
            Debug.Log("💀 Player fell into deadzone - instant death!");
            Die(); // Langsung mati tanpa efek damage
        }
    }

    // UPDATED: TakeDamage - setiap kali pumpkin jadi merah, nyawa berkurang 1
    public void TakeDamage()
    {
        if (isDead)
        {
            Debug.Log("💀 Player already dead, ignoring damage");
            return;
        }

        Debug.Log("🎃 Player terkena damage - memulai efek merah");

        // TAMBAH: Visual feedback - screen flash effect (sprite jadi merah)
        // EFEK MERAH yang akan mengurangi nyawa saat selesai
        StartCoroutine(DamageFlash());
    }

    // TAMBAH: Screen flash effect saat take damage
    // SETIAP KALI EFEK MERAH SELESAI, NYAWA BERKURANG 1
    IEnumerator DamageFlash()
    {
        Debug.Log("⚡ Starting damage flash effect (pumpkin jadi merah)");

        // Play player hurt sound when damage flash starts
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayPlayerHurtSound();
        }

        // Flash merah pada sprite renderer
        if (spriteRenderer != null)
        {
            Color originalColor = spriteRenderer.color;
            spriteRenderer.color = Color.red;

            yield return new WaitForSeconds(0.1f);

            spriteRenderer.color = originalColor;
        }

        // SETIAP KALI EFEK MERAH SELESAI → NYAWA BERKURANG 1
        Debug.Log("🔴 Efek merah selesai - nyawa berkurang 1");
        LoseOneHealth();

        // Optional: Camera shake effect (jika ada camera controller)
        // Camera.main.transform.DOShakePosition(0.2f, 0.1f);

        Debug.Log("⚡ Damage flash effect completed");
    }

    // TAMBAH: Method untuk kurangi nyawa 1 (dipanggil setelah efek merah)
    void LoseOneHealth()
    {
        int oldHealth = healthPoints;
        healthPoints--; // Kurangi 1 nyawa

        Debug.Log($"❤️ NYAWA BERKURANG! Health: {oldHealth} → {healthPoints}/3");

        // TAMBAH: Update UI health display
        if (GameManager.Instance != null)
        {
            GameManager.Instance.UpdateHealthDisplay();
        }

        if (healthPoints > 0)
        {
            Debug.Log("🤕 Player masih hidup - hurt animation");
            StartCoroutine(Hurt());
        }
        else
        {
            Debug.Log("💀 Nyawa habis! Player mati");
            Die();
        }
    }

    // UPDATED: Hurt dengan visual effects
    IEnumerator Hurt()
    {
        Debug.Log("💥 Player hurt! Showing visual effects...");
        
        // Visual feedback - color flash
        if (spriteRenderer != null)
        {
            spriteRenderer.color = hurtColor;
        }
        
        // Squash effect saat hurt
        Vector3 originalScale = transform.localScale;
        transform.localScale = new Vector3(originalScale.x * 0.8f, originalScale.y * 1.2f, originalScale.z);
        
        // Knockback
        rb.linearVelocity = Vector2.zero;
        
        Vector2 knockbackDir = facingRight ? new Vector2(-5f, 8f) : new Vector2(5f, 8f);
        rb.AddForce(knockbackDir, ForceMode2D.Impulse);

        yield return new WaitForSeconds(0.2f); // Lebih cepat untuk hurt effect
        
        // Reset visual effects
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
        transform.localScale = originalScale;
        
        yield return new WaitForSeconds(0.3f); // Sisa waktu untuk cooldown
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;

        // Play death sound
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayDeathSound();
        }

        // STOP ALL MOVEMENT IMMEDIATELY
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = 0f; // Matikan gravitasi agar tidak jatuh lagi
            rb.constraints = RigidbodyConstraints2D.FreezeAll; // Freeze semua movement
        }

        if (HasParameter("isDead"))
            anim.SetBool("isDead", true);

        Debug.Log("💀 Pumpkin died! Movement stopped, resetting level...");

        // Panggil GameManager untuk reset level
        if (GameManager.Instance != null)
        {
            GameManager.Instance.PlayerDied();
        }
        else
        {
            // Fallback: reload scene langsung
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
    }

    // ===================================================================================
    // DEBUG GIZMOS
    // ===================================================================================

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}