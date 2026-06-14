using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Boid : MonoBehaviour
{
    [HideInInspector] public BoidManager manager;

    NavMeshAgent _agent;
    Transform _targetFood;

    void Awake() => _agent = GetComponent<NavMeshAgent>();

    void Start()
    {
        _agent.speed = manager.Speed;
        SetRandomDestination();
    }

    void Update()
    {
        Vector3 flee = ComputeFlee();

        if (flee != Vector3.zero)
        {
            _targetFood = null;
            ApplyForce(flee * manager.WeightFlee);
        }
        else
        {
            UpdateFoodTarget();

            Vector3 force = ComputeFlockForce();

            if (_targetFood != null)
                force += (_targetFood.position - transform.position).normalized * manager.WeightFood;

            ApplyForce(force);
        }

        if (_agent.remainingDistance < 1f && !_agent.pathPending && _targetFood == null)
            SetRandomDestination();
    }

    // ── Nourriture ─────────────────────────────────────────────────────────

    void UpdateFoodTarget()
    {
        if (_targetFood != null)
        {
            if (!manager.cachedFood.Contains(_targetFood))
            {
                // Mangée par un autre boid ou disparue
                _targetFood = null;
            }
            else if (Vector3.Distance(transform.position, _targetFood.position) < manager.FoodConsumeRadius)
            {
                ConsumeFood(_targetFood);
                return;
            }
            else return;   // Cible toujours valide
        }

        // Cherche la plus proche dans le rayon
        Transform closest = null;
        float minDist = manager.FoodAttractionRadius;

        foreach (var food in manager.cachedFood)
        {
            if (food == null) continue;
            float d = Vector3.Distance(transform.position, food.position);
            if (d < minDist) { minDist = d; closest = food; }
        }

        _targetFood = closest;
        if (_targetFood != null)
            _agent.SetDestination(_targetFood.position);
    }

    void ConsumeFood(Transform food)
    {
        manager.cachedFood.Remove(food);
        Destroy(food.gameObject);
        _targetFood = null;
        SetRandomDestination();
    }

    // ── Fuite ──────────────────────────────────────────────────────────────

    Vector3 ComputeFlee()
    {
        Vector3 force = Vector3.zero;
        foreach (var pred in manager.cachedPredators)
        {
            if (pred == null) continue;
            float dist = Vector3.Distance(transform.position, pred.position);
            if (dist < manager.PredatorFleeRadius)
                force += (transform.position - pred.position) / (dist + 0.01f);
            Debug.Log(force);
        }
        return force.normalized;
    }

    // ── Troupeau ───────────────────────────────────────────────────────────

    Vector3 ComputeFlockForce()
    {
        var neighbors = GetNeighbors();
        if (neighbors.Count == 0) return Vector3.zero;

        return ComputeCohesion(neighbors) * manager.WeightCohesion
             + ComputeAlignment(neighbors) * manager.WeightAlignment
             + ComputeSeparation(neighbors) * manager.WeightSeparation;
    }

    Vector3 ComputeCohesion(List<Boid> neighbors)
    {
        Vector3 center = Vector3.zero;
        foreach (var n in neighbors) center += n.transform.position;
        center /= neighbors.Count;
        return (center - transform.position).normalized;
    }

    Vector3 ComputeAlignment(List<Boid> neighbors)
    {
        Vector3 avg = Vector3.zero;
        foreach (var n in neighbors) avg += n._agent.velocity;
        return (avg / neighbors.Count).normalized;
    }

    Vector3 ComputeSeparation(List<Boid> neighbors)
    {
        Vector3 force = Vector3.zero;
        foreach (var n in neighbors)
        {
            float dist = Vector3.Distance(transform.position, n.transform.position);
            if (dist < manager.SeparationRadius && dist > 0.001f)
                force += (transform.position - n.transform.position) / dist;
        }
        return force.normalized;
    }

    // ── NavMesh ────────────────────────────────────────────────────────────

    void ApplyForce(Vector3 force)
    {
        if (force == Vector3.zero) return;
        Vector3 newDest = _agent.destination + force * Time.deltaTime * manager.Speed;
        if (NavMesh.SamplePosition(newDest, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            _agent.SetDestination(hit.position);
    }

    void SetRandomDestination()
    {
        Vector3 rand = transform.position + Random.insideUnitSphere * manager.WanderRadius;
        rand.y = transform.position.y;
        if (NavMesh.SamplePosition(rand, out NavMeshHit hit, 5f, NavMesh.AllAreas))
            _agent.SetDestination(hit.position);
    }

    List<Boid> GetNeighbors()
    {
        var result = new List<Boid>();
        foreach (var b in manager.allBoids)
        {
            if (b == this) continue;
            if (Vector3.Distance(transform.position, b.transform.position) < manager.NeighborRadius)
                result.Add(b);
        }
        return result;
    }
}