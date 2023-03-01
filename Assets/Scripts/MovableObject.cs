using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MovableObject : MonoBehaviour
{
    public NavMeshAgent agent = default;
    public Rigidbody rigidBody = default;
    private float onMeshThreshold = .5f;
    private bool TryWarpToNearestPoint()
    {
        Vector3 agentPosition = agent.transform.position;
        NavMeshHit hit;

        // Check for nearest point on navmesh to agent, within onMeshThreshold
        if (NavMesh.SamplePosition(agentPosition, out hit, onMeshThreshold, NavMesh.AllAreas))
        {
            agent.Warp(hit.position);
            return true;
        }
        return false;
    }
    public virtual void Start()
    {

    }
    public virtual void Update()
    {
    }
    private bool usePhysics
    {
        get
        {
            return !rigidBody.isKinematic;
        }
        set
        {
            Debug.Log("UsePhysicsSet");
            if (value && value != usePhysics)
            {
                useAgent = false;
                agent.enabled = false;
                rigidBody.isKinematic = false;
                rigidBody.useGravity = true;
                rigidBody.transform.parent = null;
                rigidBody.WakeUp();
            }
        }
    }
    public bool useAgent
    {
        get { return agent.enabled; }
        set
        {
            Debug.Log("UseAgentSet");
            if (value && value != useAgent)
            {
                usePhysics = false;
                agent.enabled = true;
                rigidBody.isKinematic = true;
                rigidBody.useGravity = false;
                rigidBody.transform.parent = null;
            }
        }
    }
    private bool useTransform
    {
        get { return rigidBody.transform.parent != null; }
    }
    public void SetToTransform(Transform transform)
    {
        useAgent = false;
        usePhysics = false;
        Debug.Log("SettingToTransform");
        if (!useTransform)
        {
            rigidBody.transform.parent = transform;
            rigidBody.isKinematic = true;
            rigidBody.useGravity = false;
            agent.enabled = false;
        }
    }
    public virtual void FixedUpdate()
    {
        if (useTransform)
        {
        }
        if (usePhysics)
        {
            if (TryWarpToNearestPoint())
            {
                useAgent = true;
            }
        }
        if (useAgent && !(agent.isOnNavMesh || agent.isOnOffMeshLink))
        {
            usePhysics = true;
        }
    }
    public virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        rigidBody = GetComponent<Rigidbody>();
        usePhysics = false;
        agent.enabled = true;
        rigidBody.isKinematic = true;
        rigidBody.useGravity = false;
        rigidBody.transform.parent = null;
    }
}
