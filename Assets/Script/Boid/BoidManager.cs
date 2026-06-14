using System.Collections.Generic;
using UnityEngine;

public class BoidManager : MonoBehaviour
{
    [Header("Spawn")]
    [SerializeField] private GameObject boidPrefab;
    [SerializeField] private int boidCount = 30;
    [SerializeField] private float spawnRadius = 10f;

    [Header("Voisinage")]
    [SerializeField] private float neighborRadius = 5f;
    [SerializeField] private float separationRadius = 1.5f;

    [Header("Poids troupeau")]
    [SerializeField, Range(0f, 5f)] private float weightCohesion = 1f;
    [SerializeField, Range(0f, 5f)] private float weightAlignment = 1f;
    [SerializeField, Range(0f, 5f)] private float weightSeparation = 2f;

    [Header("Déplacement")]
    [SerializeField] private float speed = 3.5f;
    [SerializeField] private float wanderRadius = 20f;

    [Header("Nourriture")]
    [SerializeField] private List<string> foodTags = new() { "Food" };
    [SerializeField] private float foodAttractionRadius = 10f;
    [SerializeField] private float foodConsumeRadius = 0.8f;
    [SerializeField, Range(0f, 5f)] private float weightFood = 3f;

    [Header("Prédateurs")]
    [SerializeField] private List<string> predatorTags = new() { "Predator" };
    [SerializeField] private float predatorFleeRadius = 8f;
    [SerializeField, Range(0f, 5f)] private float weightFlee = 5f;

    [Header("Cache")]
    [SerializeField] private int refreshRate = 10;   // Rafraîchit toutes les N frames

    // ── Accesseurs publics (lecture seule pour les Boids) ──────────────────
    public float NeighborRadius => neighborRadius;
    public float SeparationRadius => separationRadius;
    public float WeightCohesion => weightCohesion;
    public float WeightAlignment => weightAlignment;
    public float WeightSeparation => weightSeparation;
    public float Speed => speed;
    public float WanderRadius => wanderRadius;
    public float FoodAttractionRadius => foodAttractionRadius;
    public float FoodConsumeRadius => foodConsumeRadius;
    public float WeightFood => weightFood;
    public float PredatorFleeRadius => predatorFleeRadius;
    public float WeightFlee => weightFlee;

    // ── Listes internes ────────────────────────────────────────────────────
    [HideInInspector] public List<Boid> allBoids = new();
    [HideInInspector] public List<Transform> cachedPredators = new();
    [HideInInspector] public List<Transform> cachedFood = new();

    int _frame;

    void Start()
    {
        RefreshCache();

        for (int i = 0; i < boidCount; i++)
        {
            Vector3 pos = transform.position + Random.insideUnitSphere * spawnRadius;
            pos.y = transform.position.y;
            var go = Instantiate(boidPrefab, pos, Quaternion.identity, transform);
            var boid = go.GetComponent<Boid>();
            boid.manager = this;
            allBoids.Add(boid);
        }
    }

    void Update()
    {
        if (_frame % refreshRate == 0)
            RefreshCache();
        _frame++;
    }

    void RefreshCache()
    {
        RefreshList(predatorTags, cachedPredators);
        RefreshList(foodTags, cachedFood);
    }

    void RefreshList(List<string> tags, List<Transform> cache)
    {
        cache.Clear();
        foreach (var tag in tags)
        {
            if (string.IsNullOrEmpty(tag)) continue;
            foreach (var go in GameObject.FindGameObjectsWithTag(tag))
                cache.Add(go.transform);
        }
    }
}