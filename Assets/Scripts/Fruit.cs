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

    //FixedUpdate로 수정할 예정
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
        if (!isFruit) return;

        Fruit otherFruit = collision.gameObject.GetComponent<Fruit>();
        if (!IsMergeable(otherFruit)) return;

        Merge(otherFruit);
    }

    public void Move()
    {
        if (!isDragging) return;

        float mouseX = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
        mouseX = ClampToBorder(mouseX);

        Vector3 targetPosition = new Vector3(mouseX, 8f, 0f);

        transform.position = Vector3.Lerp(transform.position, targetPosition, 0.2f);
    }

    public void Drag()
    {
        isDragging = true;
    }

    public void Drop()
    {
        isDragging = false;
        rigidBody.simulated = true;
        SoundManager.instance.PlaySfx(SoundManager.Sfx.Drop);
    }    

    public void Merge(Fruit fruit)
    {
        float x = transform.position.x;
        float y = transform.position.y;
        float otherX = fruit.transform.position.x;
        float otherY = fruit.transform.position.y;

        bool isTarget = y < otherY || (y == otherY && x > otherX);

        if (!isTarget) return;

        fruit.isMerging = true;
        fruit.Hide();

        isMerging = true;
        LevelUp();
        transform.position = new Vector3((x+otherX)/2, y, 0);
    }

    public void Hide()
    {
        rigidBody.simulated = false;
        circleCollider.enabled = false;
        StartCoroutine(HideCoroutine());
    }

    IEnumerator HideCoroutine()
    {
        yield return new WaitForSeconds(0.01f);
        isMerging = false;
        FruitPool.instance.Release(this);
        EffectPool.instance.Release(particle);
    }

    private void LevelUp()
    {
        rigidBody.velocity = Vector2.zero;
        rigidBody.angularVelocity = 0;

        gameManager.addScoreByFruit(this);

        StartCoroutine(LevelUpCoroutine());
    }

    IEnumerator LevelUpCoroutine()
    {
        yield return new WaitForSeconds(0.01f);

        animator.SetInteger("Level", level + 1);
        SoundManager.instance.PlaySfx(SoundManager.Sfx.LevelUp);
        PlayParticle();

        yield return new WaitForSeconds(0.01f);
        level++;

        gameManager.maxLevel = Mathf.Max(level, gameManager.maxLevel);
        isMerging = false;
    }

    public void PlayParticle()
    {
        particle.transform.position = transform.position;
        particle.transform.localScale = transform.localScale / 2;
        particle.Play();
    }

    public bool IsMergeable(Fruit fruit)
    {
        bool isSameLevel = level == fruit.level;
        bool isBelowMaxLevel = level < gameManager.FruitMaxLevel;
        bool isMergeable = isSameLevel && isBelowMaxLevel && !isMerging && !fruit.isMerging;
        return isMergeable;
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
        float leftLimit = -4.2f + radius;
        float rightLimit = 4.2f - radius;

        return Mathf.Clamp(x, leftLimit, rightLimit);
    }

    public void Initialize()
    {
        level = 0;
        isDragging = false;
        isMerging = false;

        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.zero;

        rigidBody.simulated = false;
        rigidBody.velocity = Vector2.zero;
        rigidBody.angularVelocity = 0;
        circleCollider.enabled = true;
    }

}
