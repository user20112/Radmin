using UnityEngine;

public class Attackable : MonoBehaviour
{
    public int Health;

    public virtual void Start()
    {
    }

    public virtual void Update()
    {
    }

    public virtual void FixedUpdate()
    {
    }

    public virtual void Killed()
    {
    }

    public virtual bool IsAttackable()
    {
        return true;
    }

    public virtual void Initialize()
    {
    }
}