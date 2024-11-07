using Unity.Entities;
using UnityEngine;

// Empty components can be used to tag entities
public struct AppleTag : IComponentData
{
}
public struct BadAppleTag : IComponentData
{
}

public struct AppleBottomY : IComponentData
{
    // If you have only one field in a component, name it "Value"

    public float Value;
}

[DisallowMultipleComponent]
public class AppleAuthoring : MonoBehaviour
{
    [SerializeField] private float bottomY = -14f;
    [SerializeField] private bool isPoison = false;

    private class AppleAuthoringBaker : Baker<AppleAuthoring>
    {
        public override void Bake(AppleAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent<AppleTag>(entity);
            if (authoring.isPoison)
            {
                AddComponent<BadAppleTag>(entity);
            }

            AddComponent(entity, new AppleBottomY { Value = authoring.bottomY });
        }
    }
}
