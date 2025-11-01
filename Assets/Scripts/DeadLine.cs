using UnityEngine;

public class DeadLine : MonoBehaviour
{

    public GameManager gameManager;

    void OnTriggerStay2D(Collider2D collision)
    {
        bool isFruit = collision.CompareTag("Fruit");
        if (!isFruit) return;

        Fruit fruit = collision.gameObject.GetComponent<Fruit>();
        fruit.deadTime += Time.deltaTime;

        if (fruit.deadTime > 2) fruit.spriteRenderer.color = Color.red;
        if (fruit.deadTime > 5) gameManager.FinishGame();
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        bool isFruit = collision.CompareTag("Fruit");
        if (!isFruit) return;

        Fruit fruit = collision.gameObject.GetComponent<Fruit>();

        fruit.deadTime = 0;
        fruit.spriteRenderer.color = Color.white;
    }

}
