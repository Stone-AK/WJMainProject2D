using System;
using UnityEngine;

public class WJ2DUnit : MonoBehaviour
{
    public int _instId { get; protected set; }
    [SerializeField] protected int _hp;
    [SerializeField] protected int _curHP;
    [SerializeField] protected float _moveSpeed;

    protected event Action<int, int> _onHpChanged;


    public virtual void DecreaseCurrentHp(int dmg)
    {
        _curHP -= dmg;
        InvokeStatChangedEvent();
    }

    public void BindOnStatChangedEvent(Action<int, int> hpCjangeCallback)
    {
        _onHpChanged += hpCjangeCallback;
    }

    public void ResetStatChangedEvent()
    {
        _onHpChanged = null;
    }

    private void InvokeStatChangedEvent()
    {
        _onHpChanged?.Invoke(_curHP, _hp);
    }
}
