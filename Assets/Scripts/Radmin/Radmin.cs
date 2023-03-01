using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[SelectionBase]
public class Radmin : MovableObject
{
    [HideInInspector]
    public TrailRenderer Trail;
    public ParticleSystem ParticleTrail;
    public ParticleSystem LeafParticle;
    public ParticleSystem ActivationParticle;
    public Transform Model;
    private Coroutine UpdateTarget = default;
    private RadminState state = RadminState.Idle;
    public Interactable Objective;
    public RadminEvent OnStartFollow;
    public RadminEvent OnStartThrow;
    public RadminEvent OnEndThrow;
    public RadminEvent OnStartCarry;
    public RadminEvent OnEndCarry;
    public float JumpMultiplier = 1;
    public int Damage = 1;
    public int AttackSpeed = 300;
    public int Ticker = 0;

    public RadminState State
    {
        get
        {
            return state;
        }
        set
        {
            state = value;
            switch (value)
            {
                case RadminState.Idle:
                    LeafParticle.Play();
                    break;

                case RadminState.Follow:
                    ActivationParticle.Play();
                    LeafParticle.Clear();
                    LeafParticle.Stop();
                    break;

                case RadminState.Carrying:
                case RadminState.Attacking:
                case RadminState.MovingIntoPosition:
                case RadminState.MoveStickFollow:
                case RadminState.InAir:
                    LeafParticle.Clear();
                    LeafParticle.Stop();
                    break;
            }
        }
    }

    public override void Awake()
    {
        base.Awake();
        OnStartFollow.AddListener((x) => StartFollow(x));
        OnStartThrow.AddListener((x) => StartThrow(x));
        OnEndThrow.AddListener((x) => EndThrow(x));
    }

    public void StartFollow(int num)
    {
        transform.DOJump(transform.position, .4f, 1, .3f);
        transform.DOPunchScale(-Vector3.up / 2, .3f, 10, 1).SetDelay(UnityEngine.Random.Range(0, .1f));
    }

    public void StartThrow(int num)
    {
        Trail.Clear();
        Trail.emitting = true;
    }

    public void EndThrow(int num)
    {
        ParticleTrail.Stop();
        Trail.emitting = false;
    }

    public override void Start()
    {
        base.Start();
        Physics.IgnoreLayerCollision(7, 9);
        Trail = GetComponentInChildren<TrailRenderer>();
        Trail.emitting = false;
    }

    public override void Update()
    {
        base.Update();
    }
    public void SetCarrying(Transform transform)
    {
        SetToTransform(transform);
        State = RadminState.Carrying;
    }
    public virtual void FixedUpdate()
    {
        base.FixedUpdate();
        Ticker++;
        if (Ticker > 10000)
            Ticker = 0;
        if (Ticker % 10 == 0)
            if (State == RadminState.Idle && useAgent)
            {
                CheckInteraction();
            }
    }

    public void SetTarget(Transform target, float updateTime = 1f)
    {
        if (Objective != null)
        {
            Objective.ReleaseRadmin(this);
            Objective = null;
        }
        State = RadminState.Follow;
        OnStartFollow.Invoke(0);
        if (this.UpdateTarget != null)
            StopCoroutine(this.UpdateTarget);
        useAgent = true;
        WaitForSeconds wait = new WaitForSeconds(updateTime);
        this.UpdateTarget = StartCoroutine(UpdateTarget());
        IEnumerator UpdateTarget()
        {
            while (true)
            {
                agent.SetDestination(target.position);
                yield return wait;
            }
        }
    }
    public void Throw(Vector3 target, float time, float delay)
    {
        OnStartThrow.Invoke(0);
        State = RadminState.InAir;
        if (UpdateTarget != null)
            StopCoroutine(UpdateTarget);
        Vector3 gravity = Physics.gravity;
        Vector3 PosDif = target - transform.position;
        //float TimeInAir = .5f * JumpMultiplier;
        //Vector3 Velocity = new Vector3(PosDif.x * TimeInAir, -1 * TimeInAir * gravity.y, PosDif.z * TimeInAir);
        //rigidBody.velocity += Velocity;
        transform.DOJump(target, 2, 1, time).SetDelay(delay).SetEase(Ease.Linear).OnComplete(() =>
        {
            if (state == RadminState.InAir)
                SetIdle();
            OnEndThrow.Invoke(0);
        });
        transform.LookAt(new Vector3(target.x, transform.position.y, target.z));
        Model.DOLocalRotate(new Vector3(360 * 3, 0, 0), time, RotateMode.LocalAxisAdd).SetDelay(delay);
    }

    public void SetCarrying(bool on)
    {
        if (on)
            OnStartCarry.Invoke(0);
        else
            OnEndCarry.Invoke(0);
    }

    public void SetIdle()
    {
        rigidBody.transform.parent = null;
        useAgent = true;
        State = RadminState.Idle;
    }

    private void CheckInteraction()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 2.5f);
        if (colliders.Length == 0)
            return;
        List<Collider> Colliders = new List<Collider>();
        foreach (Collider collider in colliders)
            Colliders.Add(collider);
        Colliders.Sort(comparer);
        foreach (Collider collider in Colliders)
        {
            Interactable InteractableObject = collider.GetComponent<Interactable>();
            if (InteractableObject != null && InteractableObject.IsInteractable())
            {
                Objective = InteractableObject;
                Objective.AssignRadmin(this);
                break;
            }
        }
        OnEndThrow.Invoke(0);
    }

    private int comparer(Collider x, Collider y)
    {
        float DistanceToX = (x.transform.position.x - transform.position.x) * (x.transform.position.x - transform.position.x) + (x.transform.position.y - transform.position.y) * (x.transform.position.y - transform.position.y) + (x.transform.position.z - transform.position.z) * (x.transform.position.z - transform.position.z);
        float DistanceToY = (y.transform.position.x - transform.position.x) * (y.transform.position.x - transform.position.x) + (y.transform.position.y - transform.position.y) * (y.transform.position.y - transform.position.y) + (y.transform.position.z - transform.position.z) * (y.transform.position.z - transform.position.z);
        if (DistanceToX < DistanceToY)
            return -1;
        else if (DistanceToX > DistanceToY)
            return 1;
        return 0;
    }
}