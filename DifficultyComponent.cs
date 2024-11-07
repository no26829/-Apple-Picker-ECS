using Unity.Entities;

public struct DifficultyComponent : IComponentData
{
    public bool IsMediumMode;
    public bool IsHardMode;
}
