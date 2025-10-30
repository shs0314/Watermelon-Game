using System.Collections;
using UnityEngine;

public class Fruit : MonoBehaviour
{

    private int level;
    private bool isDrag;
    private bool isMerge;
    private float deadTime;

    private GameManager gameManager;
    private ParticleSystem particle;

    private Animator animator;
    private Rigidbody2D rigidBody;
    private CircleCollider2D circleCollider;
    private SpriteRenderer spriteRenderer;

    public int Level
    {
        get { return level; }
        set { level = value; }
    }

    public float DeadTime
    {
        get { return deadTime; }
        set { deadTime = value; }
    }

    public Rigidbody2D RigidBody { get { return rigidBody; } }
    public SpriteRenderer SpriteRenderer { get { return spriteRenderer; } }
    public GameManager GameManager { set { gameManager = value; } }
    public ParticleSystem Particle { set { particle = value; } }

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
        bool isFruit = collision.gameObject.tag == "Fruit";
        if (!isFruit) return;

        Fruit otherFruit = collision.gameObject.GetComponent<Fruit>();
        if (!IsMergeable(otherFruit)) return;

        Merge(otherFruit);
    }

    public void Move()
    {
        if (!isDrag) return;

        float mouseX = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
        mouseX = ClampToBorder(mouseX);

        Vector3 targetPosition = new Vector3(mouseX, 8f, 0f);

        transform.position = Vector3.Lerp(transform.position, targetPosition, 0.2f);
    }

    public void Drag()
    {
        isDrag = true;
    }

    public void Drop()
    {
        isDrag = false;
        rigidBody.simulated = true;
    }    

    public void Merge(Fruit fruit)
    {
        float x = transform.position.x;
        float y = transform.position.y;
        float otherX = fruit.transform.position.x;
        float otherY = fruit.transform.position.y;

        bool isTarget = y < otherY || (y == otherY && x > otherX);

        if (!isTarget) return;

        fruit.isMerge = true;
        fruit.Hide();

        isMerge = true;
        LevelUp();
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
        isMerge = false;
        gameObject.SetActive(false);
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
        PlayParticle();

        yield return new WaitForSeconds(0.01f);
        level++;

        gameManager.maxLevel = Mathf.Max(level, gameManager.maxLevel);
        isMerge = false;
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
        bool isMergeable = isSameLevel && isBelowMaxLevel && !isMerge && !fruit.isMerge;
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

}
