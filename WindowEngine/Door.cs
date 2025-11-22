using OpenTK.Mathematics;

namespace WindowEngine
{
    public class Door : WorldObject
    {
        public bool IsOpen { get; private set; }
        private float _closedRotation;
        private float _openRotation;
        private Vector3 _pivotOffset = new Vector3(0.5f, 0f, 0f);

        public Door(Mesh mesh, Vector3 position, Vector3 scale, float rotation, Vector3? customCollisionSize = null)
            : base(mesh, position, scale, rotation, customCollisionSize)
        {
            _closedRotation = rotation;
            _openRotation = rotation + MathHelper.DegreesToRadians(90f); // 90 degrees open
            IsOpen = false;
        }

        public void Toggle()
        {
            IsOpen = !IsOpen;
            Rotation = IsOpen ? _openRotation : _closedRotation;
            HasCollision = !IsOpen; // Disable collision when open
    
            UpdateBoundingBox();
        }
        
        public override void UpdateTransform()
        {
            Matrix4 model = Matrix4.Identity;
            model *= Matrix4.CreateScale(Scale);
            model *= Matrix4.CreateTranslation(_pivotOffset * Scale);
            model *= Matrix4.CreateRotationY(Rotation);
            model *= Matrix4.CreateTranslation(-_pivotOffset * Scale);
            model *= Matrix4.CreateTranslation(Position);
            Mesh.Transform = model;
        }
    }
}