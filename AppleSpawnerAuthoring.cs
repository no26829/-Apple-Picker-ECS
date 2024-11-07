using Unity.Entities;
using UnityEngine;

public struct AppleSpawner : IComponentData
{
    public Entity NormalPrefab;
    public Entity PoisonPrefab;
    public Entity ZigZagPrefab;
    public float Interval;
}

[DisallowMultipleComponent]
public class AppleSpawnerAuthoring : MonoBehaviour
{
    [SerializeField] private GameObject applePrefab;
    [SerializeField] private GameObject poisonApplePrefab;
    [SerializeField] private GameObject zigzagApplePrefab;
    [SerializeField] private float appleSpawnInterval = 1f;

    private class AppleSpawnerAuthoringBaker : Baker<AppleSpawnerAuthoring>
    {
        public override void Bake(AppleSpawnerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new AppleSpawner
            {
                NormalPrefab = GetEntity(authoring.applePrefab, TransformUsageFlags.Dynamic),
                PoisonPrefab = GetEntity(authoring.poisonApplePrefab, TransformUsageFlags.Dynamic),
                ZigZagPrefab = GetEntity(authoring.zigzagApplePrefab, TransformUsageFlags.Dynamic),
                Interval = authoring.appleSpawnInterval
            });

            AddComponent(entity, new Timer { Value = 1f }); // Set initial timer value
        }
    }
}
