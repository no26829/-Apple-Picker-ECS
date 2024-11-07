using Unity.Entities;
using Unity.Mathematics;

public struct ZigzagMovement : IComponentData
{
    public float Speed; // Speed of the movement
    public float Amplitude; // Amplitude of the zigzag motion
    public float Frequency; // Frequency of the zigzag motion
    public float3 StartPosition; // Starting position for reference
}

