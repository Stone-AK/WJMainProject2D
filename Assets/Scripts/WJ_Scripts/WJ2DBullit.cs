using UnityEngine;

public class WJ2DBullit : MonoBehaviour
{
    [SerializeField] private Rigidbody2D BullitRb;

    private float speed = 1f;
    public float CollTime { get; private set; } = 3.0f;


    private void FixedUpdate()
    {
        MoveBullit();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if(collision.gameObject.layer == LayerMask.NameToLayer("Map_Wall"))
        {
            TouchWall();
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            TouchEnemy(collision);
        }
    }

    private void MoveBullit()
    {
        BullitRb.linearVelocity = transform.right * speed;
    }

    private void TouchWall()
    {
        Debug.Log("벽과 충돌");


        DestroyBullit();
    }

    private void TouchEnemy(Collider2D collision)
    {
        Debug.Log("적과 충돌");


        DestroyBullit();
    }

    private void DestroyBullit()
    {
        this.gameObject.SetActive(false);
    }
}
