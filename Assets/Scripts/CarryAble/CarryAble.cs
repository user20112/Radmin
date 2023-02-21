using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public abstract class CarryAble : Interactable
{
    public int PikminNeeded;
    public int MaxPikminMultiplier;
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

    private float OriginalAgentSpeed;
    private DestinationScript Destination;
    private Coroutine CarryRoutine;
    private Color CaptureColor;
    private GameObject FractionObject;
    public List<Pikmin> PikminAssigned = new List<Pikmin>();
    private int pikminCarryingCount = 0;
    private bool Carrying = false;
    public float radius;
    public Vector3 uiOffset;

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

    public int PikminCarryingCount
    {
        get
        {
            return pikminCarryingCount;
        }
        set
        {
            pikminCarryingCount = value;
            var dest = GetUpdatedDestination();
            bool ChangeFractionObject = Destination == null || FractionObject == null || dest == null || dest.DestinationType != Destination.DestinationType;
            Destination = dest;
            if (ChangeFractionObject)
            {
                if (FractionObject != null)
                {
                    DestroyImmediate(FractionObject);
                    FractionObject = null;
                }
                if (Destination != null)
                {
                    FractionObject = PrefabRetrieverScript.GetPrefabFromDestinationType(Destination.DestinationType);
                    FractionObject = Instantiate(FractionObject, Canvas);
                    FractionObject.SetActive(true);
                }
            }
            if (FractionObject != null)
            {
                FractionObject.transform.GetChild(0).DOComplete();
                FractionObject.transform.GetChild(0).DOPunchScale(Vector3.one, .3f, 10, 1);
                FractionObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = PikminCarryingCount.ToString();
                FractionObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = PikminNeeded.ToString();
            }
            if (!Carrying && pikminCarryingCount >= PikminNeeded)
                CarryRoutine = StartCoroutine(StartCarrying());
            UpdateSpeed();
            if (Carrying && pikminCarryingCount < PikminNeeded)
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
        if (PikminAssigned.Count >= PikminNeeded * MaxPikminMultiplier && StopInteractions)
            return false;
        return base.IsInteractable();
    }

    public virtual IEnumerator StartCarrying()
    {
        Carrying = true;
        Agent.avoidancePriority = 50;
        Agent.isStopped = false;
        Agent.SetDestination(Destination.Point());
        yield return new WaitUntil(() => Agent.IsDone());
        Agent.enabled = false;
        Collider.enabled = false;
        StopInteractions = true;
        Destination.ItemCollected(new CollectedEventArgs() { ObjectCollected = this, PikminCarrying = PikminAssigned });
    }

    public virtual void StopCarrying()
    {
        Carrying = false;
        Agent.avoidancePriority = 30;
        Agent.isStopped = true;
        if (CarryRoutine != null)
            StopCoroutine(CarryRoutine);
    }

    public override void Start()
    {
        base.Start();
        Physics.IgnoreLayerCollision(6, 9);
        OriginalAgentSpeed = Agent.speed;
    }

    public override void Update()
    {
        base.Update();
        if (FractionObject != null)
            FractionObject.transform.position = Camera.main.WorldToScreenPoint(transform.position + uiOffset);
    }

    public override void AssignPikmin(Pikmin pikmin)
    {
        base.Update();
        if (PikminAssigned.Count < MaxPikminMultiplier * PikminNeeded)
        {
            PikminAssigned.Add(pikmin);
            if (Carrying)
                StopCarrying();
            StartCoroutine(PutPikminInNextAvailableSpot(pikmin));
        }
        else
        {
        }
    }

    public Vector3 GetNextAvailableSpot()
    {
        int index = (int)(PikminAssigned.Count * 2 - PikminAssigned.Count / (PikminNeeded + 1));
        float angle = (float)(index * Mathf.PI * 2f / (PikminNeeded * MaxPikminMultiplier));
        return transform.position + new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
    }

    private IEnumerator PutPikminInNextAvailableSpot(Pikmin pikmin)
    {
        pikmin.State = PikminState.MovingIntoPosition;
        pikmin.agent.SetDestination(GetNextAvailableSpot());
        yield return new WaitUntil(() => pikmin.agent.IsDone());
        if (pikmin.State == PikminState.MovingIntoPosition)
        {
            pikmin.agent.enabled = false;
            pikmin.transform.parent = transform;
            pikmin.transform.DOLookAt(new Vector3(transform.position.x, pikmin.transform.position.y, transform.position.z), .2f);
            pikmin.State = PikminState.Carrying;
            PikminCarryingCount++;
        }
    }

    public virtual void UpdateSpeed()
    {
        Agent.speed = (float)(((double)PikminCarryingCount) / PikminNeeded * OriginalAgentSpeed);
    }

    public override void ReleasePikmin()
    {
        while (PikminAssigned.Count > 0)
        {
            Pikmin pikmin = PikminAssigned[0];
            PikminAssigned.Remove(pikmin);
            if (pikmin.State == PikminState.Carrying)
                PikminCarryingCount--;
            pikmin.SetIdle();
        }
        base.ReleasePikmin();
    }

    public override void ReleasePikmin(Pikmin pikmin)
    {
        PikminAssigned.Remove(pikmin);
        if (pikmin.State == PikminState.Carrying)
            PikminCarryingCount--;
        pikmin.SetIdle();
        base.ReleasePikmin(pikmin);
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