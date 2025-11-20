using OpenTK.Mathematics;

namespace WindowEngine
{
    public class CollisionBoxData
    {
        public Vector3 Size { get; set; }
        public Vector3 Offset { get; set; }  // Position offset from object center

        public CollisionBoxData(Vector3 size, Vector3 offset)
        {
            Size = size;
            Offset = offset;
        }
    }
}