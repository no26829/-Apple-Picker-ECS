using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;


public partial struct CollectAppleSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerScore>();
        state.RequireForUpdate<SimulationSingleton>();
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        using (var appleCounts = new NativeArray<int>(2, Allocator.TempJob)) // Two counts: normal and bad apples
        {
            // Schedule the job
            state.Dependency = new CollisionJob
            {
                AppleLookup = SystemAPI.GetComponentLookup<AppleTag>(true),
                BasketLookup = SystemAPI.GetComponentLookup<BasketTag>(true),
                BadAppleLookup = SystemAPI.GetComponentLookup<BadAppleTag>(true), // Updated to new tag
                ECB = ecb,
                AppleCounts = appleCounts // Pass the array to the job
            }.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);

            state.Dependency.Complete(); // Wait for job to complete

            // Update player score based on the type of apple collected
            if (appleCounts[0] != 0) // Check if a normal apple was collected
            {
                var playerScore = SystemAPI.GetSingleton<PlayerScore>();
                playerScore.Value += 100 * appleCounts[0]; // Increase score by 100 per normal apple
                SystemAPI.SetSingleton(playerScore); // Update score
            }

        } // 'using' block automatically disposes appleCounts
    }
    [BurstCompile]
    private struct CollisionJob : ICollisionEventsJob
    {
        [ReadOnly] public ComponentLookup<AppleTag> AppleLookup;
        [ReadOnly] public ComponentLookup<BasketTag> BasketLookup;
        [ReadOnly] public ComponentLookup<BadAppleTag> BadAppleLookup; // Updated to new tag

        public EntityCommandBuffer ECB;
        public NativeArray<int> AppleCounts; // 0 for normal apples, 1 for bad apples

        public void Execute(CollisionEvent collisionEvent)
        {
            var entityA = collisionEvent.EntityA;
            var entityB = collisionEvent.EntityB;

            // Check for normal apples
            if (AppleLookup.HasComponent(entityA) && BasketLookup.HasComponent(entityB) && !BadAppleLookup.HasComponent(entityA))
            {
                ECB.DestroyEntity(entityA); // Destroy the normal apple
                AppleCounts[0] = 1; // Increment normal apple count
                UnityEngine.Debug.Log("Normal apple collected.");
            }
            else if (AppleLookup.HasComponent(entityB) && BasketLookup.HasComponent(entityA) && !BadAppleLookup.HasComponent(entityB))
            {
                ECB.DestroyEntity(entityB); // Destroy the normal apple
                AppleCounts[0] = 1; // Increment normal apple count
                UnityEngine.Debug.Log("Normal apple collected.");
            }
            // Check for bad apples
            else if (BadAppleLookup.HasComponent(entityB) && BasketLookup.HasComponent(entityA))
            {
                ECB.DestroyEntity(entityB); // Destroy the bad apple
                AppleCounts[0] = -5; // Increment bad apple count
                UnityEngine.Debug.Log($"Bad apple collected! Count: {AppleCounts[1]}");
            }
            else if (BadAppleLookup.HasComponent(entityA) && BasketLookup.HasComponent(entityB))
            {
                ECB.DestroyEntity(entityA); // Destroy the bad apple
                AppleCounts[0] = -5; // Increment bad apple count
                UnityEngine.Debug.Log($"Bad apple collected! Count: {AppleCounts[1]}");
            }
        }
    }
}
