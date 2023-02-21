﻿using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Events;

[System.Serializable] public class PikminEvent : UnityEvent<int> { }

[System.Serializable] public class PlayerEvent : UnityEvent<Vector3> { }

public class PikminManager : MonoBehaviour
{
    [SerializeField] private Vector3 followOffset;

    public int MaxSelectionRadius = 5;
    public Transform MouseIcon = default;
    [SerializeField] private Vector3 MouseIconOffset;
    private Camera cam = default;
    private LineRenderer line = default;
    public AudioSource audio;
    private const int linePoints = 5;
    public float Radius;

    [Header("Visual")]
    public Transform visualCylinder;

    public static PikminManager Instance
    {
        get
        {
            if (_Instance == null)
                _Instance = new PikminManager();
            return _Instance;
        }
    }

    private static PikminManager _Instance;
    private MovementInput charMovement;

    [Header("Positioning")]
    public Transform pikminThrowPosition;

    [Header("Targeting")]
    [SerializeField] private Transform target = default;

    [SerializeField] private Transform Character = default;

    [SerializeField] private float selectionRadius = 1;

    [Header("Events")]
    public PikminEvent pikminFollow;

    //public PlayerEvent pikminHold;
    public PlayerEvent pikminThrow;

    public List<Pikmin> AllPikmin = new List<Pikmin>();
    private int controlledPikmin = 0;
    public Rig whistleRig;
    public ParticleSystem whistlePlayerParticle;

    public PikminManager()
    {
        _Instance = this;
    }

    private void Start()
    {
        cam = Camera.main;
        line = GetComponentInChildren<LineRenderer>();
        line.positionCount = linePoints;
        charMovement = FindObjectOfType<MovementInput>();
        PikminSpawner[] spawners = FindObjectsOfType(typeof(PikminSpawner)) as PikminSpawner[];
        foreach (PikminSpawner spawner in spawners)
        {
            spawner.SpawnStartPikmin(ref AllPikmin);
        }
    }

    public void SetWhistleRadius(float radius)
    {
        selectionRadius = radius;
    }

    public void SetWhistleRigWeight(float weight)
    {
        whistleRig.weight = weight;
    }

    private void UpdateMousePosition()
    {
        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            Vector3 hitPoint = hit.point;
            float x = hit.point.x - Character.position.x;
            float y = hit.point.y - Character.position.y;
            float z = hit.point.z - Character.position.z;
            float Distance = Mathf.Sqrt(x * x + z * z + y * y);
            if (Distance > Radius)
            {
                //we are outside the circle so just get the direction and max it out.
                float Divisor = Distance / Radius;
                x /= Divisor;
                z /= Divisor;
                y /= Divisor;
            }
            hit.point = new Vector3(x, y, z);
            MouseIcon.position = hit.point + MouseIconOffset + Character.position;
            MouseIcon.up = Vector3.Lerp(Character.up, hit.normal, .3f);
            for (int i = 0; i < linePoints; i++)
            {
                Vector3 linePos = Vector3.Lerp(MouseIcon.position + MouseIconOffset, Character.position, (float)i / 5f);
                line.SetPosition(i, linePos);
            }
        }
        MouseIcon.rotation = new Quaternion(0, 0, 0, 0);
        visualCylinder.rotation = new Quaternion(0, 0, 0, 0);
    }

    private void Update()
    {
        UpdateMousePosition();
        visualCylinder.transform.position = MouseIcon.position;
        if (Input.GetMouseButton(1))
        {
            foreach (Pikmin pikmin in AllPikmin)
            {
                if (Vector3.Distance(pikmin.transform.position, MouseIcon.position) < selectionRadius)
                {
                    if (pikmin.State != PikminState.Follow && pikmin.State != PikminState.InAir && pikmin.State != PikminState.MoveStickFollow)
                    {
                        pikmin.SetTarget(target, 0.25f);
                        controlledPikmin++;
                        pikminFollow.Invoke(controlledPikmin);
                    }
                }
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            foreach (Pikmin pikmin in AllPikmin)
            {
                if (pikmin.State == PikminState.Follow && Vector3.Distance(pikmin.transform.position, charMovement.transform.position) < 2)
                {
                    pikmin.agent.enabled = false;
                    float delay = .05f;
                    pikmin.transform.DOMove(pikminThrowPosition.position, delay);
                    pikmin.Throw(MouseIcon.position, .5f, delay);
                    controlledPikmin--;
                    pikminThrow.Invoke(MouseIcon.position);
                    pikminFollow.Invoke(controlledPikmin);
                    break;
                }
            }
        }
        if (Input.GetMouseButtonDown(1))
            SetWhistleCylinder(true);
        if (Input.GetMouseButtonUp(1))
            SetWhistleCylinder(false);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(MouseIcon.position, selectionRadius);
    }

    //Polish
    public void SetWhistleCylinder(bool on)
    {
        if (on)
        {
            whistlePlayerParticle.Play();
            audio.Play();
            DOVirtual.Float(0, (MaxSelectionRadius / 2) + .5f, .5f, SetWhistleRadius).SetId(2);
            DOVirtual.Float(0, 1, .2f, SetWhistleRigWeight).SetId(1);
            charMovement.transform.GetChild(0).DOScaleY(27f, .05f).SetLoops(-1, LoopType.Yoyo).SetId(3);
            visualCylinder.localScale = Vector3.zero;
            visualCylinder.DOScaleX(MaxSelectionRadius, .5f);
            visualCylinder.DOScaleZ(MaxSelectionRadius, .5f);
            visualCylinder.DOScaleY(2, .4f).SetDelay(.4f);
        }
        else
        {
            whistlePlayerParticle.Stop();
            audio.Stop();
            DOTween.Kill(2);
            DOTween.Kill(1);
            DOTween.Kill(3);
            charMovement.transform.GetChild(0).DOScaleY(28, .1f);
            DOVirtual.Float(whistleRig.weight, 0, .2f, SetWhistleRigWeight);
            selectionRadius = 0;
            visualCylinder.DOKill();
            visualCylinder.DOScaleX(0, .2f);
            visualCylinder.DOScaleZ(0, .2f);
            visualCylinder.DOScaleY(0f, .05f);
        }
    }
}