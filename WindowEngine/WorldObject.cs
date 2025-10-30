using OpenTK.Mathematics;

namespace WindowEngine;

public class WorldObject
{
    public Mesh Mesh;
    public Vector3 Position;
    public Vector3 Scale;
    public float Rotation;

    public WorldObject(Mesh mesh, Vector3 position, Vector3 scale, float rotation)
    {
        Mesh = mesh;
        Position = position;
        Scale = scale;
        Rotation = rotation;
    }

    public void Draw(Vector3 diffuseColor, Shader shader)
    {
        Mesh.Transform = Matrix4.CreateScale(Scale)* Matrix4.CreateRotationY(Rotation) * Matrix4.CreateTranslation(Position);
        Mesh.Draw(diffuseColor, shader);
    }
}

