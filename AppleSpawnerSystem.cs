using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using Unity.Collections;

[UpdateAfter(typeof(TimerSystem))]
public partial struct AppleSpawnerSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        state.RequireForUpdate<DifficultyComponent>(); 
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        float deltaTime = SystemAPI.Time.DeltaTime;
        var random = new Unity.Mathematics.Random((uint)UnityEngine.Random.Range(1, 10000));

    
        var difficultySettings = SystemAPI.GetSingleton<DifficultyComponent>();
        bool isMediumMode = difficultySettings.IsMediumMode;
        bool isHardMode = difficultySettings.IsHardMode;

        Debug.Log($"AppleSpawnerSystem: isMediumMode = {isMediumMode}, isHardMode = {isHardMode}");

        new SpawnJob
        {
            ECB = ecb,
            DeltaTime = deltaTime,
            Random = random,
            IsMediumMode = isMediumMode,
            IsHardMode = isHardMode
        }.Schedule();
    }

    [BurstCompile]
    private partial struct SpawnJob : IJobEntity
    {
        public EntityCommandBuffer ECB;
        public float DeltaTime;
        public Unity.Mathematics.Random Random;
        public bool IsMediumMode;
        public bool IsHardMode;

        private void Execute(in LocalTransform transform, in AppleSpawner spawner, ref Timer timer)
        {
            if (timer.Value > 0)
            {
                timer.Value -= DeltaTime;
                return;
            }

            timer.Value = spawner.Interval;
            float randomValue = Random.NextFloat();

            if (randomValue < 0.5f)
            {
                // Spawn normal apple
                var normalAppleEntity = ECB.Instantiate(spawner.NormalPrefab);
                ECB.SetComponent(normalAppleEntity, LocalTransform.FromPosition(transform.Position));
                Debug.Log("SpawnJob: Spawned Normal Apple");
            }
            else if (IsMediumMode && randomValue >= 0.5f && randomValue < 0.8f)
            {
                // Spawn poison apple in medium mode
                var poisonAppleEntity = ECB.Instantiate(spawner.PoisonPrefab);
                ECB.SetComponent(poisonAppleEntity, LocalTransform.FromPosition(transform.Position));
                Debug.Log("SpawnJob: Spawned Poison Apple (Medium Mode)");
            }
            else if (IsHardMode && randomValue >= 0.8f)
            {
                // Spawn zigzag apple in hard mode
                var zigzagAppleEntity = ECB.Instantiate(spawner.ZigZagPrefab);
                ECB.SetComponent(zigzagAppleEntity, LocalTransform.FromPosition(transform.Position));

                // Add ZigzagMovement component for zigzag motion in hard mode
                ECB.AddComponent(zigzagAppleEntity, new ZigzagMovement
                {
                    Speed = 3f, // Faster speed for hard mode
                    Amplitude = 1.5f,
                    Frequency = 3f,
                    StartPosition = transform.Position
                });

                Debug.Log("SpawnJob: Spawned Zigzag Apple with Zigzag Movement (Hard Mode)");
            }
        }
    }
}
