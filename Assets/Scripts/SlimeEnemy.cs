using System.Collections;
using UnityEngine;

public class SlimeEnemy : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public float patrolDistance = 3f;
    public float detectionRange = 5f;
    public float attackRange = 1.5f;
    public float attackCooldown = 2f;
    
    [Header("Combat Settings")]
    public int damage = 1;
    public float knockbackForce = 3f;
    
    [Header("Visual Effects")]
    public float squashAmount = 0.8f;
    public float stretchAmount = 1.2f;
    public float effectDuration = 0.2f;
    public Color hurtColor = Color.red;
    public Color deadColor = Color.gray;
    public float idleBobSpeed = 2f;
    public float idleBobAmount = 0.05f;
    
    [Header("Boundary Settings")]
    public float minYBoundary = -10f;
    public float maxYBoundary = 20f;
    public float minXBoundary = -50f;
    public float maxXBoundary = 50f;
    public float boundaryGracePeriod = 1f;
    
    [Header("Jump Attack Effects")]
    public Color jumpAttackColor = Color.red;
    public float jumpAttackFlashDuration = 0.3f;

    [Header("Bounce Settings")]
    public float bounceForce = 5f;
    public Color bounceColor = Color.yellow;
    
    [Header("Animation")]
    public Animator animator;
    
    // Private variables
    private Rigidbody2D rb;
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private bool movingRight = true;
    private bool isChasingPlayer = false;
    private bool canAttack = true;
    private Transform player;
    private Vector3 originalScale;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private float outOfBoundsTimer = 0f;
    private bool isOutOfBounds = false;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;
        targetPosition = startPosition + Vector3.right * patrolDistance;
        
        // Get visual components
        originalScale = transform.localScale;
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
        
        // Setup Rigidbody
        if (rb != null)
        {
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
        
        // Find player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        
        // Get animator if not assigned
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        
        Debug.Log("🐸 Slime spawned!");
        
        // Start idle bobbing effect
        StartCoroutine(IdleBob());
    }
    
    void Update()
    {
        if (player == null) return;
        
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        // Check if player is in detection range
        if (distanceToPlayer <= detectionRange)
        {
            isChasingPlayer = true;
            
            // SIDE ATTACK: Hanya attack jika player tidak sedang di atas slime
            if (distanceToPlayer <= attackRange && canAttack)
            {
                // Double check: pastikan player tidak di atas slime
                bool isPlayerAbove = player.position.y > transform.position.y + 0.5f;
                
                if (!isPlayerAbove) // Hanya attack jika player TIDAK di atas
                {
                    Debug.Log("💥 SIDE ATTACK! Player in range - attacking!");
                    AttackPlayer();
                }
                else
                {
                    Debug.Log("🎯 Player above slime - waiting for jump attack collision");
                }
            }
        }
        else
        {
            isChasingPlayer = false;
        }
        
        // Check boundaries
        CheckBoundaries();
        
        UpdateAnimations();
    }
    
    void FixedUpdate()
    {
        if (isChasingPlayer && player != null)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }
        
        FlipSprite();
    }
    
    IEnumerator IdleBob()
    {
        while (true)
        {
            float bobAmount = Mathf.Sin(Time.time * idleBobSpeed) * idleBobAmount;
            transform.localScale = new Vector3(originalScale.x, originalScale.y + bobAmount, originalScale.z);
            yield return null;
        }
    }
    
    void Patrol()
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);
        
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            movingRight = !movingRight;
            
            if (movingRight)
            {
                targetPosition = startPosition + Vector3.right * patrolDistance;
            }
            else
            {
                targetPosition = startPosition + Vector3.left * patrolDistance;
            }
        }
    }
    
    void ChasePlayer()
    {
        if (player == null) return;
        
        Vector3 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);
    }
    
    // SIDE ATTACK - Slime menyerang player dari samping = PLAYER KEHILANGAN NYAWA
    void AttackPlayer()
    {
        canAttack = false;
        
        Debug.Log("🐸 SIDE ATTACK - Slime attacking player!");
        
        // Deal damage to player - PLAYER KEHILANGAN NYAWA
        PumpkinMovement playerScript = player.GetComponent<PumpkinMovement>();
        if (playerScript != null)
        {
            playerScript.TakeDamage(); // Player nyawa berkurang
            
            Vector2 knockbackDir = (player.position - transform.position).normalized;
            Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                playerRb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);
            }
        }
        
        StartCoroutine(ResetAttack());
    }
    
    IEnumerator ResetAttack()
    {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
    
    // JUMP ATTACK DETECTION - Player injak dari atas = SLIME MATI LANGSUNG
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                // Cek kondisi untuk jump attack
                bool isPlayerFalling = playerRb.linearVelocity.y < -1f; // Player sedang jatuh
                bool isPlayerAbove = collision.transform.position.y > transform.position.y + 0.5f; // Player di atas slime
                
                Debug.Log($"🎯 Collision: Falling={isPlayerFalling}, Above={isPlayerAbove}");
                Debug.Log($"Player Y: {collision.transform.position.y}, Slime Y: {transform.position.y}");
                
                if (isPlayerFalling && isPlayerAbove)
                {
                    // JUMP ATTACK: Player injak dari atas = SLIME MATI LANGSUNG
                    Debug.Log("🦘 JUMP ATTACK SUCCESS! Slime dies instantly!");
                    
                    // TAMBAH: Visual effects sebelum mati
                    StartCoroutine(JumpAttackEffects());
                    
                    // HAPUS: Bounce player - langsung mati tanpa bounce
                    // playerRb.AddForce(Vector2.up * 8f, ForceMode2D.Impulse);
                }
                else
                {
                    // NORMAL COLLISION: Saling terpental (bounce effect)
                    Debug.Log("💥 NORMAL COLLISION! Bounce effect!");
                    BounceOffPlayer(collision);
                }
            }
        }
    }

    // TAMBAH: DeadZone Detection - Slime mati saat terkena deadzone
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("DeadZone") || col.gameObject.name.Contains("DeadZone"))
        {
            Debug.Log("💀 Slime fell into deadzone - dying!");
            Die();
        }
    }

    // TAMBAH: Bounce effect method - TETAP ADA untuk collision normal
    void BounceOffPlayer(Collision2D collision)
    {
        Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
        
        if (playerRb != null && rb != null)
        {
            // Hitung arah knockback (dari slime ke player)
            Vector2 bounceDirection = (collision.transform.position - transform.position).normalized;
            
            // Bounce slime ke arah berlawanan
            rb.linearVelocity = Vector2.zero; // Reset velocity
            rb.AddForce(-bounceDirection * 5f, ForceMode2D.Impulse); // Slime terpental ke belakang
            
            // Bounce player ke arah berlawanan
            playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x * 0.5f, playerRb.linearVelocity.y); // Kurangi horizontal velocity
            playerRb.AddForce(bounceDirection * 5f, ForceMode2D.Impulse); // Player terpental ke depan
            
            // TAMBAH: Visual feedback
            StartCoroutine(BounceVisualEffect());
            
            Debug.Log($"💥 Bounce! Slime direction: {-bounceDirection}, Player direction: {bounceDirection}");
        }
    }

    // TAMBAH: Bounce visual effect
    IEnumerator BounceVisualEffect()
    {
        // Squash effect saat bounce
        Vector3 squashedScale = new Vector3(originalScale.x * stretchAmount, originalScale.y * squashAmount, originalScale.z);
        transform.localScale = squashedScale;
        
        // Color flash
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.yellow; // Warna kuning untuk bounce
        }
        
        yield return new WaitForSeconds(0.2f);
        
        // Reset visual
        transform.localScale = originalScale;
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
    }
    
    // TAMBAH: Jump attack visual effects
    IEnumerator JumpAttackEffects()
    {
        Debug.Log("🔴 Slime taking jump attack damage - visual effects!");
        
        // Stop idle bobbing
        StopCoroutine(IdleBob());
        
        // 1. Color flash to red
        if (spriteRenderer != null)
        {
            spriteRenderer.color = jumpAttackColor;
            yield return new WaitForSeconds(jumpAttackFlashDuration / 2);
        }
        
        // 2. Squash effect (penyok)
        Vector3 squashedScale = new Vector3(originalScale.x * stretchAmount, originalScale.y * squashAmount, originalScale.z);
        transform.localScale = squashedScale;
        
        // HAPUS: Upward movement - langsung mati tanpa bounce
        // if (rb != null)
        // {
        //     rb.AddForce(Vector2.up * 2f, ForceMode2D.Impulse);
        // }
        
        yield return new WaitForSeconds(jumpAttackFlashDuration / 2);
        
        // 4. Transition to death
        Die();
    }
    
    void CheckBoundaries()
    {
        Vector3 currentPos = transform.position;
        bool currentlyOutOfBounds = false;
        
        if (currentPos.y < minYBoundary || currentPos.y > maxYBoundary ||
            currentPos.x < minXBoundary || currentPos.x > maxXBoundary)
        {
            currentlyOutOfBounds = true;
        }
        
        if (currentlyOutOfBounds)
        {
            if (!isOutOfBounds)
            {
                isOutOfBounds = true;
                outOfBoundsTimer = 0f;
                Debug.Log("⚠️ Slime entered boundary zone!");
            }
            
            outOfBoundsTimer += Time.deltaTime;
            
            // Flash warning
            if (spriteRenderer != null)
            {
                float alpha = Mathf.PingPong(Time.time * 5f, 1f);
                spriteRenderer.color = Color.Lerp(originalColor, Color.red, alpha);
            }
            
            if (outOfBoundsTimer >= boundaryGracePeriod)
            {
                HandleOutOfBounds();
            }
        }
        else
        {
            if (isOutOfBounds)
            {
                isOutOfBounds = false;
                if (spriteRenderer != null)
                {
                    spriteRenderer.color = originalColor;
                }
                Debug.Log("✅ Slime back in bounds!");
            }
            outOfBoundsTimer = 0f;
        }
    }
    
    void HandleOutOfBounds()
    {
        Debug.Log("💥 Slime out of bounds - destroying!");
        StopAllCoroutines();
        Destroy(gameObject);
    }
    
    // DIE - Slime mati (dipanggil saat jump attack)
    void Die()
    {
        Debug.Log("💀 Slime defeated!");
        
        StopAllCoroutines();
        
        enabled = false;
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.isKinematic = true;
        }
        
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (Collider2D col in colliders)
        {
            col.enabled = false;
        }
        
        if (spriteRenderer != null)
        {
            spriteRenderer.color = deadColor;
        }
        
        StartCoroutine(DeathSquash());
        
        Destroy(gameObject, 1.5f);
    }
    
    IEnumerator DeathSquash()
    {
        float duration = 0.8f;
        float elapsed = 0;
        Vector3 startScale = transform.localScale;
        Vector3 endScale = new Vector3(startScale.x * 1.5f, startScale.y * 0.3f, startScale.z);
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            t = 1 - Mathf.Pow(1 - t, 3);
            transform.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }
        
        transform.localScale = endScale;
    }
    
    void UpdateAnimations()
    {
        // Empty for now
    }
    
    bool HasParameter(string paramName)
    {
        if (animator == null) return false;
        
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == paramName)
                return true;
        }
        return false;
    }
    
    void FlipSprite()
    {
        if (rb.linearVelocity.x > 0.1f)
        {
            transform.localScale = new Vector3(Mathf.Abs(originalScale.x), transform.localScale.y, originalScale.z);
        }
        else if (rb.linearVelocity.x < -0.1f)
        {
            transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), transform.localScale.y, originalScale.z);
        }
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(startPosition - Vector3.right * patrolDistance, startPosition + Vector3.right * patrolDistance);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        Gizmos.color = Color.black;
        Gizmos.DrawLine(new Vector3(-100, minYBoundary, 0), new Vector3(100, minYBoundary, 0));
        Gizmos.DrawLine(new Vector3(-100, maxYBoundary, 0), new Vector3(100, maxYBoundary, 0));
        Gizmos.DrawLine(new Vector3(minXBoundary, -100, 0), new Vector3(minXBoundary, 100, 0));
        Gizmos.DrawLine(new Vector3(maxXBoundary, -100, 0), new Vector3(maxXBoundary, 100, 0));
    }
}