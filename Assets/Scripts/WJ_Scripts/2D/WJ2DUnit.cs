using UnityEngine;

public class WJ2DUnit : MonoBehaviour
{
    [SerializeField] protected int _hp;
    [SerializeField] protected int _curHP;
    [SerializeField] protected float _moveSpeed;

    public virtual void DecreaseCurrentHp(int dmg)
    {
        _curHP -= dmg;
    }
}
