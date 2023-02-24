using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[SelectionBase]
public class Radmin : MonoBehaviour
{
    [HideInInspector]
    public NavMeshAgent agent = default;

    public Rigidbody rigidBody = default;

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

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        rigidBody = GetComponent<Rigidbody>();
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

    public virtual void Start()
    {
        Physics.IgnoreLayerCollision(7, 9);
        Trail = GetComponentInChildren<TrailRenderer>();
        Trail.emitting = false;
    }

    public virtual void Update()
    {
    }

    private float onMeshThreshold = .1f;

    public bool IsRadminOnNavMesh()
    {
        Vector3 agentPosition = agent.transform.position;
        NavMeshHit hit;

        // Check for nearest point on navmesh to agent, within onMeshThreshold
        if (NavMesh.SamplePosition(agentPosition, out hit, onMeshThreshold, NavMesh.AllAreas))
        {
            // Check if the positions are vertically aligned
            if (Mathf.Approximately(agentPosition.x, hit.position.x) && Mathf.Approximately(agentPosition.z, hit.position.z))
            {
                // Lastly, check if object is below navmesh
                return agentPosition.y >= hit.position.y;
            }
        }

        return false;
    }
    public void SetCarrying(Transform transform)
    {
        SetToTransform(transform);
        State = RadminState.Carrying;
    }
    private bool _isSetToPhysics = false;
    public void SetToPhysics()
    {
        if (!_isSetToPhysics)
        {
            _isSetToPhysics = true;
            _isSetToAgent = false;
            _isSetToTransform = false;
            agent.enabled = false;
            rigidBody.isKinematic = false;
            rigidBody.useGravity = true;
            rigidBody.transform.parent = null;
            rigidBody.WakeUp();
        }
    }
    private bool _isSetToAgent = false;
    public void SetToAgent()
    {
        if (!_isSetToAgent)
        {
            _isSetToPhysics = false;
            _isSetToAgent = true;
            _isSetToTransform = false;
            agent.enabled = true;
            rigidBody.isKinematic = true;
            rigidBody.useGravity = true;
            rigidBody.transform.parent = null;
        }
    }
    private bool _isSetToTransform = false;
    public void SetToTransform(Transform transform)
    {
        if (!_isSetToAgent)
        {
            _isSetToPhysics = false;
            _isSetToAgent = false;
            _isSetToTransform = true;
            transform.parent = transform;
            rigidBody.isKinematic = true;
            rigidBody.useGravity = false;
            agent.enabled = false;
        }
    }

    public virtual void FixedUpdate()
    {
        if (State == RadminState.Idle)
        {
            CheckInteraction();
        }
        if (agent.isOnNavMesh || agent.isOnOffMeshLink || IsRadminOnNavMesh())
        {
            if (State == RadminState.InAir)
            {
                State = RadminState.Idle;
                OnEndThrow.Invoke(0);
            }
            if (State != RadminState.Carrying)
            {
                SetToAgent();
            }
        }
        else
        {
            if (State != RadminState.Carrying)
            {
                SetToPhysics();
            }
        }
    }

    public void SetTarget(Transform target, float updateTime = 1f)
    {
        if (Objective != null)
        {
            Objective.ReleaseRadmin(this);
            Objective = null;
            SetIdle();
        }
        State = RadminState.Follow;
        OnStartFollow.Invoke(0);
        if (this.UpdateTarget != null)
            StopCoroutine(this.UpdateTarget);
        WaitForSeconds wait = new WaitForSeconds(updateTime);
        this.UpdateTarget = StartCoroutine(UpdateTarget());
        IEnumerator UpdateTarget()
        {
            while (true)
            {
                if (agent.enabled)
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
        SetToPhysics();
        Vector3 gravity = Physics.gravity;
        Vector3 PosDif = target - transform.position;
        //float TimeInAir = .5f * JumpMultiplier;
        //Vector3 Velocity = new Vector3(PosDif.x * TimeInAir, -1 * TimeInAir * gravity.y, PosDif.z * TimeInAir);
        //rigidBody.velocity += Velocity;
        transform.DOJump(target, 2, 1, time).SetDelay(delay).SetEase(Ease.Linear).OnComplete(() =>
        {
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
        if (agent.isOnNavMesh || agent.isOnOffMeshLink || IsRadminOnNavMesh())
        {
            SetToAgent();
        }
        else
        {
            SetToPhysics();
        }
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