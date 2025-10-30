using UnityEngine;

public class DeadLine : MonoBehaviour
{

    public GameManager gameManager;

    void OnTriggerStay2D(Collider2D collision)
    {
        bool isFruit = collision.CompareTag("Fruit");
        if (!isFruit) return;

        Fruit fruit = collision.gameObject.GetComponent<Fruit>();
        fruit.DeadTime += Time.deltaTime;

        if (fruit.DeadTime > 2) fruit.SpriteRenderer.color = Color.red;
        if (fruit.DeadTime > 5) gameManager.FinishGame();
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        bool isFruit = collision.CompareTag("Fruit");
        if (!isFruit) return;

        Fruit fruit = collision.gameObject.GetComponent<Fruit>();

        fruit.DeadTime = 0;
        fruit.SpriteRenderer.color = Color.white;
    }

}
