using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.Rendering.DebugUI;

public abstract class CarryAble : Interactable
{
    public int RadminNeeded;
    public int MaxRadminMultiplier;
    private NavMeshAgent agent;

    private NavMeshAgent Agent
    {
        get
        {
            if (agent == null)
                agent = GetComponent<NavMeshAgent>();
            return agent;
        }
    }

    public Renderer objectRenderer;

    public Renderer ObjectRenderer
    {
        get
        {
            if (objectRenderer == null)
                objectRenderer = GetComponentInChildren<Renderer>();
            return objectRenderer;
        }
    }

    private Collider collider;

    private Collider Collider
    {
        get
        {
            if (collider == null)
                collider = GetComponent<Collider>();
            return collider;
        }
    }

    private float _originalAgentSpeed;
    private DestinationScript _destination;
    private Coroutine _carryRoutine;
    private Color _captureColor;
    private GameObject _fractionObject;
    public List<Radmin> RadminAssigned = new List<Radmin>();
    private int _radminCarryingCount = 0;
    private bool _carrying = false;
    public float radius;
    public Vector3 uiOffset;
    private Vector3 _lastPosition;

    private Transform canvas;

    private Transform Canvas
    {
        get
        {
            if (canvas == null)
                canvas = PrefabRetrieverScript.Instance.CanvasToDrawOn;
            return canvas;
        }
    }

    public int RadminCarryingCount
    {
        get
        {
            return _radminCarryingCount;
        }
        set
        {
            _radminCarryingCount = value;
            var dest = GetUpdatedDestination();
            bool ChangeFractionObject = _destination == null || _fractionObject == null || dest == null || dest.DestinationType != _destination.DestinationType;
            _destination = dest;
            if (ChangeFractionObject)
            {
                if (_fractionObject != null)
                {
                    DestroyImmediate(_fractionObject);
                    _fractionObject = null;
                }
                if (_destination != null)
                {
                    _fractionObject = PrefabRetrieverScript.GetPrefabFromDestinationType(_destination.DestinationType);
                    _fractionObject = Instantiate(_fractionObject, Canvas);
                    _fractionObject.SetActive(true);
                }
            }
            if (_fractionObject != null)
            {
                _fractionObject.transform.GetChild(0).DOComplete();
                _fractionObject.transform.GetChild(0).DOPunchScale(Vector3.one, .3f, 10, 1);
                _fractionObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = RadminCarryingCount.ToString();
                _fractionObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = RadminNeeded.ToString();
            }
            if (!_carrying && _radminCarryingCount >= RadminNeeded)
                _carryRoutine = StartCoroutine(StartCarrying());
            UpdateSpeed();
            if (_carrying && _radminCarryingCount < RadminNeeded)
                StopCarrying();
        }
    }

    public virtual DestinationScript GetUpdatedDestination()
    {
        throw new NotImplementedException();
    }

    private bool StopInteractions = false;

    public override bool IsInteractable()
    {
        if (RadminAssigned.Count >= RadminNeeded * MaxRadminMultiplier && StopInteractions)
            return false;
        return base.IsInteractable();
    }

    public virtual IEnumerator StartCarrying()
    {
        _carrying = true;
        Agent.avoidancePriority = 50;
        Agent.isStopped = false;
        Agent.SetDestination(_destination.Point());
        yield return new WaitUntil(() => Agent.IsDone());
        Agent.enabled = false;
        Collider.enabled = false;
        StopInteractions = true;
        _destination.ItemCollected(new CollectedEventArgs() { ObjectCollected = this, RadminCarrying = RadminAssigned });
    }

    public virtual void StopCarrying()
    {
        _carrying = false;
        Agent.avoidancePriority = 30;
        Agent.isStopped = true;
        if (_carryRoutine != null)
            StopCoroutine(_carryRoutine);
    }

    public override void Start()
    {
        base.Start();
        Physics.IgnoreLayerCollision(6, 9);
        _originalAgentSpeed = Agent.speed;
    }

    public override void Update()
    {
        base.Update();
        if (_fractionObject != null)
            _fractionObject.transform.position = Camera.main.WorldToScreenPoint(transform.position + uiOffset);
    }

    public override void AssignRadmin(Radmin radmin)
    {
        base.Update();
        if (RadminAssigned.Count < MaxRadminMultiplier * RadminNeeded)
        {
            RadminAssigned.Add(radmin);
            if (_carrying)
                StopCarrying();
            StartCoroutine(PutRadminInNextAvailableSpot(radmin));
        }
        else
        {
        }
    }

    public Vector3 GetNextAvailableSpot()
    {
        int index = (int)(RadminAssigned.Count * 2 - RadminAssigned.Count / (RadminNeeded + 1));
        float angle = (float)(index * Mathf.PI * 2f / (RadminNeeded * MaxRadminMultiplier));
        return transform.position + new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
    }

    private IEnumerator PutRadminInNextAvailableSpot(Radmin radmin)
    {
        radmin.State = RadminState.MovingIntoPosition;
        radmin.agent.SetDestination(GetNextAvailableSpot());
        yield return new WaitUntil(() => radmin.agent.IsDone());
        if (radmin.State == RadminState.MovingIntoPosition)
        {
            radmin.agent.enabled = false;
            radmin.transform.parent = transform;
            radmin.rigidBody.isKinematic = true;
            radmin.rigidBody.useGravity = false;
            radmin.transform.DOLookAt(new Vector3(transform.position.x, radmin.transform.position.y, transform.position.z), .2f);
            radmin.State = RadminState.Carrying;
            if (RadminAssigned.Count <= RadminCarryingCount + 1)
            {
                RadminCarryingCount++;
            }
        }
    }

    public virtual void UpdateSpeed()
    {
        Agent.speed = (float)(((double)RadminCarryingCount) / RadminNeeded * _originalAgentSpeed);
    }

    public override void ReleaseRadmin()
    {
        while (RadminAssigned.Count > 0)
        {
            Radmin radmin = RadminAssigned[0];
            RadminAssigned.Remove(radmin);
            if (radmin.State == RadminState.Carrying)
                RadminCarryingCount--;
            radmin.SetIdle();
        }
        base.ReleaseRadmin();
    }

    public override void ReleaseRadmin(Radmin radmin)
    {
        if (RadminAssigned.Contains(radmin))
        {
            RadminAssigned.Remove(radmin);
            if (radmin.State == RadminState.Carrying)
                RadminCarryingCount--;
            radmin.SetIdle();
        }
        base.ReleaseRadmin(radmin);
    }

    public override void Initialize()
    {
        base.Initialize();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }
}