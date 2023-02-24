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

    public virtual void AssignRadmin(Radmin radmin)
    {
    }

    public virtual void ReleaseRadmin()
    {
    }

    public virtual void ReleaseRadmin(Radmin radmin)
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