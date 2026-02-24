using UnityEngine;
using UnityEngine.AI;

public class SpiderWander : MonoBehaviour
{
    public float wanderRadius = 20f;
    public float wanderDelay = 2f;

    NavMeshAgent agent;
    float timer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        timer = wanderDelay;
        SetNewDestination();
    }

    void Update()
    {
        timer += Time.deltaTime;

        // Si ha llegado casi al destino o ha pasado el tiempo, busca otro punto
        if (!agent.pathPending && agent.remainingDistance <= 0.5f && timer >= wanderDelay)
        {
            SetNewDestination();
        }
    }

    void SetNewDestination()
    {
        Vector3 randomDir = Random.insideUnitSphere * wanderRadius;
        randomDir += transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDir, out hit, wanderRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
            timer = 0f;
        }
    }
}