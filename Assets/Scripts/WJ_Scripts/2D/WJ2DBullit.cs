using UnityEngine;

public class WJ2DBullit : MonoBehaviour
{
    private int _power = 10;
    private float _moveSpeed = 4f;
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
        transform.position += transform.right * _moveSpeed * Time.deltaTime;
    }

    private void TouchWall()
    {
        Debug.Log("벽과 충돌");


        DestroyBullit();
    }

    private void TouchEnemy(Collider2D collision)
    {
        Debug.Log("적과 충돌");

        DamageEnemy(collision);
        DestroyBullit();
    }

    private void DamageEnemy(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<WJ2DEnemy>(out WJ2DEnemy _enemy))
        {
            if (_enemy == null) return;
            _enemy.DecreaseCurrentHp(_power);
        }
    }

    private void DestroyBullit()
    {
        this.gameObject.SetActive(false);
    }
}
