using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public static class RadminUtility
{
    public static bool IsDone(this NavMeshAgent agent)
    {
        if (!agent.enabled || !agent.isOnNavMesh)
            return true;
        return (!agent.pathPending &&
        agent.remainingDistance <= agent.stoppingDistance &&
        (!agent.hasPath || agent.velocity.sqrMagnitude == 0f));
    }
}
