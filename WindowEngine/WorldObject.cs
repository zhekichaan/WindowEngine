using OpenTK.Mathematics;

namespace WindowEngine;

public class WorldObject
{
    protected Mesh Mesh;
    public Vector3 Position;
    protected Vector3 Scale;
    protected float Rotation;
    protected bool HasCollision = true;

    public List<BoundingBox> CollisionBoxes { get; private set; } = new List<BoundingBox>();
    
    private List<CollisionBoxData>? _customCollisionBoxes;

    public WorldObject(Mesh mesh, Vector3 position, Vector3 scale, float rotation, Vector3? customCollisionSize = null)
        : this(mesh, position, scale, rotation,
            customCollisionSize.HasValue
                ? new List<CollisionBoxData> { new CollisionBoxData(customCollisionSize.Value, Vector3.Zero) }
                : null)
    {
    }

    // Constructor for multiple custom collision boxes
    public WorldObject(Mesh mesh, Vector3 position, Vector3 scale, float rotation,
        List<CollisionBoxData>? customCollisionBoxes)
    {
        Mesh = mesh;
        Position = position;
        Scale = scale;
        Rotation = rotation;
        _customCollisionBoxes = customCollisionBoxes;

        UpdateBoundingBox();
    }

    public virtual void UpdateTransform()
    {
        Matrix4 model = Matrix4.Identity;
        model *= Matrix4.CreateScale(Scale);
        model *= Matrix4.CreateRotationY(Rotation);
        model *= Matrix4.CreateTranslation(Position);
        Mesh.Transform = model;
    }

    public void UpdateBoundingBox()
    {
        CollisionBoxes.Clear();
    
        if (_customCollisionBoxes != null && _customCollisionBoxes.Count > 0)
        {
            // Use custom collision boxes with offsets
            foreach (var boxData in _customCollisionBoxes)
            {
                // Apply the offset to the position
                Vector3 boxPosition = Position + boxData.Offset;
                BoundingBox box = BoundingBox.FromCenterAndSize(boxPosition, boxData.Size, Vector3.One);
                CollisionBoxes.Add(box);
            }
        }
        else
        {
            // Use mesh bounds (single box)
            BoundingBox box = BoundingBox.Transform(
                Mesh.OriginalMin,
                Mesh.OriginalMax,
                Position,
                Scale,
                Rotation
            );
            CollisionBoxes.Add(box);
        }
    }

    public void Draw(Vector3 diffuseColor)
    {
        UpdateTransform();

        Mesh.Draw(diffuseColor);
    }
    
    public bool CheckCollision(BoundingBox other)
    {
        if (!HasCollision) return false;
        
        // Check against all collision boxes
        foreach (var box in CollisionBoxes)
        {
            if (box.Intersects(other))
            {
                return true;
            }
        }
    
        return false;
    }
}

