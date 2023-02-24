using UnityEngine;

public class Attackable : Interactable
{
    public int Health;
    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        base.Update();
    }

    public override void FixedUpdate()
    {
        if (Health <= 0)
            Killed();
        base.FixedUpdate();
    }

    public override void AssignRadmin(Radmin radmin)
    {
        base.AssignRadmin(radmin);
    }

    public override void ReleaseRadmin()
    {
        base.ReleaseRadmin();
    }

    public override void ReleaseRadmin(Radmin radmin)
    {
        base.ReleaseRadmin(radmin);
    }

    public override bool IsInteractable()
    {
        return IsAttackable();
    }

    public override void Initialize()
    {
        base.Initialize();
    }

    public virtual void Killed()
    {
        Health = 0;
    }

    public virtual bool IsAttackable()
    {
        return true;
    }
}