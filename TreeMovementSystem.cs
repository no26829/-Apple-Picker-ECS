using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct TreeMovementSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var deltaTime = SystemAPI.Time.DeltaTime;

        new MoveTreeJob { DeltaTime = deltaTime }.Schedule();
    }

    [BurstCompile]
    private partial struct MoveTreeJob : IJobEntity
    {
        public float DeltaTime;

        [BurstCompile]
        private void Execute(ref LocalTransform transform, ref AppleTreeSpeed speed, in AppleTreeBounds bounds)
        {
            transform.Position.x += speed.Value * DeltaTime;

            if (transform.Position.x > bounds.Right)
            {
                speed.Value = -math.abs(speed.Value);
            }
            else if (transform.Position.x < bounds.Left)
            {
                speed.Value = math.abs(speed.Value);
            }
        }
    }
}
