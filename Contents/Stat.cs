using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stat : MonoBehaviour
{
    [SerializeField] protected int _level;
    [SerializeField] protected int _hp;
    [SerializeField] protected int _maxHp;
    [SerializeField] protected int _mp;
    [SerializeField] protected int _maxMp;
    [SerializeField] protected int _attack;
    [SerializeField] protected int _deadExp;
    [SerializeField] protected float _movespeed;
    
    public int Level { get { return _level; } set { _level = value; } }
    public int Hp { get { return _hp; } set { _hp = value; } }
    public int Mp { get { return _mp; } set { _mp = value; } }
    public int MaxHp { get { return _maxHp; } set { _maxHp = value; } }
    public int MaxMp { get { return _maxMp; } set { _maxMp = value; } }
    public int Attack { get { return _attack; } set { _attack = value; } }
    public int DeadExp { get { return _deadExp; } set { _deadExp = value; } }
    public float MoveSpeed { get { return _movespeed; } set { _movespeed = value; } }
    
    void Start()
    {
        _level = 1;
        _hp = 100;
        _maxHp = 100;
        _mp = 100;
        _maxMp = 100;
        _attack = 10;
        _deadExp = 5;
        _movespeed = 5.0f;
    }

    // 공격을 받았을 때
    public virtual void OnAttacked(int skillAttack=0)
    {
        GetComponent<MonsterController>().State = Define.State.Hit;

        int damage;
        if (skillAttack != 0)
            damage = Mathf.Max(0, skillAttack);
        else 
            damage = Mathf.Max(0, Managers.Game.Attack);
            
        Hp -= damage;
        Debug.Log("Hit Damage : " + damage + "\nSTR : " + Managers.Game.STR);

        if (Hp <= 0)
        {
            Hp = 0;
            OnDead();
        }
    }

    // 죽었을 때
    protected virtual void OnDead()
    {
        Managers.Game.Exp += _deadExp;
        GetComponent<MonsterController>().State = Define.State.Die;
    }
}
