using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public partial struct ZigzagMovementSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;

        foreach (var (transform, zigzag) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<ZigzagMovement>>())
        {
            float time = (float)SystemAPI.Time.ElapsedTime;

            float xOffset = math.sin(time * zigzag.ValueRO.Frequency) * zigzag.ValueRO.Amplitude;
            float3 newPosition = zigzag.ValueRO.StartPosition + new float3(xOffset, -zigzag.ValueRO.Speed * deltaTime, 0);

            transform.ValueRW.Position = newPosition;
        }
    }
}
