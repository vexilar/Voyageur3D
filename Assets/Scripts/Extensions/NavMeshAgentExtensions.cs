using UnityEngine.AI;

public static class NavMeshAgentExtensions
{
    public static bool CanPath(this NavMeshAgent agent)
    {
        return agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh;
    }

    public static bool TrySetDestination(this NavMeshAgent agent, UnityEngine.Vector3 destination)
    {
        if (!agent.CanPath())
        {
            return false;
        }

        return agent.SetDestination(destination);
    }

    public static void TryResetPath(this NavMeshAgent agent)
    {
        if (!agent.CanPath())
        {
            return;
        }

        agent.ResetPath();
    }
}
