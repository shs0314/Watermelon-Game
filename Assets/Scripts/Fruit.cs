using System.Collections;
using UnityEngine;

public class Fruit : MonoBehaviour
{
    public int level;
    public float deadTime;
    public bool isDragging;
    public bool isMerging;

    public GameManager gameManager;
    public ParticleSystem particle;

    public Animator animator;
    public Rigidbody2D rigidBody;
    public CircleCollider2D circleCollider;
    public SpriteRenderer spriteRenderer;

    public void Awake()
    {
        InitializeComponents();
    }

    public void Update()
    {
        Move();
    }

    public void OnEnable()
    {
        animator.SetInteger("Level", level);
    }

    public void OnCollisionStay2D(Collision2D collision)
    {
        bool isFruit = collision.gameObject.CompareTag("Fruit");
        if(!isFruit) return;
        Fruit fruit = collision.gameObject.GetComponent<Fruit>();
        if(!IsMergeable(fruit)) return;
        Merge(fruit);
    }

    public void Move()
    {
        if(!isDragging) return;

        float mouseX = ClampToBorder(
            Camera.main.ScreenToWorldPoint(Input.mousePosition).x
        );

        Vector3 targetPosition = new(mouseX, 5.1f, 0f);
        transform.position = Vector3.Lerp(transform.position, targetPosition, 0.2f);
        gameManager.MoveIndicator(transform.position.x);
    }

    public void Drag()
    {
        isDragging = true;
    }

    public void Drop()
    {
        isDragging = false;
        rigidBody.simulated = true;
        SoundManager.instance.PlaySfx(Sfx.Drop);
    }

    public void Merge(Fruit fruit)
    {
        if(!isMergingTarget(fruit)) return;
        float averageX = (transform.position.x + fruit.transform.position.x) / 2;

        fruit.isMerging = true;
        fruit.Hide();
        EffectPool.instance.Release(fruit.particle);

        isMerging = true;
        LevelUp();
        transform.position = new Vector3(averageX, transform.position.y, 0);
    }

    public bool IsMergeable(Fruit fruit)
    {
        bool isSameLevel = level == fruit.level;
        bool isBelowMaxLevel = level < gameManager.FRUIT_MAX_LEVEL;
        bool isMergeable = isSameLevel && isBelowMaxLevel && !isMerging && !fruit.isMerging;
        return isMergeable;
    }

    public void Hide()
    {
        rigidBody.simulated = false;
        circleCollider.enabled = false;
        StartCoroutine(HideCoroutine());
    }

    private IEnumerator HideCoroutine()
    {
        yield return new WaitForSeconds(0.01f);
        isMerging = false;
        FruitPool.instance.Release(this);
    }

    public void LevelUp()
    {
        rigidBody.linearVelocity = Vector2.zero;
        rigidBody.angularVelocity = 0;
        gameManager.AddScoreByFruit(this);
        StartCoroutine(LevelUpCoroutine());
    }

    private IEnumerator LevelUpCoroutine()
    {
        yield return new WaitForSeconds(0.01f);

        animator.SetInteger("Level", level + 1);
        SoundManager.instance.PlaySfx(Sfx.LevelUp);
        PlayParticle();

        yield return new WaitForSeconds(0.01f);
        level++;

        gameManager.gameMaxLevel = Mathf.Max(level, gameManager.gameMaxLevel);
        isMerging = false;
    }

    public void PlayParticle()
    {
        particle.transform.position = transform.position;
        particle.transform.localScale = transform.localScale / 2;
        particle.Play();
    }

    public void Initialize()
    {
        ResetState();
        ResetTransform();
        ResetPhysics();
    }

    private void ResetState()
    {
        level = 0;
        isDragging = false;
        isMerging = false;
    }

    private void ResetTransform()
    {
        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        transform.localScale = Vector3.zero;  
    }

    private void ResetPhysics()
    {
        rigidBody.simulated = false;
        rigidBody.linearVelocity = Vector2.zero;
        rigidBody.angularVelocity = 0;
        circleCollider.enabled = true;
    }

    private bool isMergingTarget(Fruit fruit)
    {
        float x = transform.position.x;
        float y = transform.position.y;
        float otherX = fruit.transform.position.x;
        float otherY = fruit.transform.position.y;
        bool isTarget = y < otherY || (y == otherY && x > otherX);
        return isTarget;
    }

    private void InitializeComponents()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        circleCollider = GetComponent<CircleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private float ClampToBorder(float x)
    {
        float radius = transform.localScale.x / 2f;
        float leftLimit = -5.0f + radius;
        float rightLimit = 5.0f - radius;
        return Mathf.Clamp(x, leftLimit, rightLimit);
    }

}
