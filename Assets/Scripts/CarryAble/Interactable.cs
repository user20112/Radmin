using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public virtual void Start()
    {
    }

    public virtual void Update()
    {
    }

    public virtual void FixedUpdate()
    {
    }

    public virtual void AssignPikmin(Pikmin pikmin)
    {
    }

    public virtual void ReleasePikmin()
    {
    }

    public virtual void ReleasePikmin(Pikmin pikmin)
    {
    }

    public virtual bool IsInteractable()
    {
        return true;
    }

    public virtual void Initialize()
    {
    }
}