using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public struct MoveWithMouse : IComponentData
{
}

public partial struct MoveWithMouseSystem : ISystem
{
    private EntityQuery difficultyQuery;

    public void OnCreate(ref SystemState state)
    {
        // Create an EntityQuery to find DifficultyComponent
        difficultyQuery = state.GetEntityQuery(ComponentType.ReadOnly<DifficultyComponent>());
    }

    public void OnUpdate(ref SystemState state)
    {
        // Default to false if no DifficultyComponent is found
        bool isHardMode = false;

        // Check if the DifficultyComponent is present and retrieve its value
        if (!difficultyQuery.IsEmpty)
        {
            var difficulty = difficultyQuery.GetSingleton<DifficultyComponent>();
            isHardMode = difficulty.IsHardMode;
        }

        // Get the current mouse position in world space
        var mousePosition2D = Input.mousePosition;
        mousePosition2D.z = -Camera.main.transform.position.z;
        var mousePosition3D = Camera.main.ScreenToWorldPoint(mousePosition2D);

        // Update the position based on difficulty level
        foreach (var transform in SystemAPI.Query<RefRW<LocalTransform>>().WithAll<MoveWithMouse>())
        {
            var position = transform.ValueRO.Position;
            // Invert the x position only if in "Hard" mode
            position.x = isHardMode ? -mousePosition3D.x : mousePosition3D.x;
            transform.ValueRW.Position = position;
        }
    }
}
