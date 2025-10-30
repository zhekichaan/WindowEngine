using OpenTK.Mathematics;

namespace WindowEngine;

public class WorldObject
{
    public Mesh Mesh;
    public Vector3 Position;
    public Vector3 Scale;

    public WorldObject(Mesh mesh, Vector3 position, Vector3 scale)
    {
        Mesh = mesh;
        Position = position;
        Scale = scale;
    }

    public void Draw()
    {
        Mesh.Transform = Matrix4.CreateScale(Scale) * Matrix4.CreateTranslation(Position);
        Mesh.Draw();
    }
}

