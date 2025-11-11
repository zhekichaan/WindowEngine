using OpenTK.Mathematics;
using System;

namespace WindowEngine
{
    public struct AABB
    {
        public Vector2 Center;
        public Vector2 HalfSize;

        public AABB(Vector2 center, Vector2 halfSize)
        {
            Center = center;
            HalfSize = halfSize;
        }

        public Vector2 Min => Center - HalfSize;
        public Vector2 Max => Center + HalfSize;

        public bool Intersects(AABB other, out Vector2 mtv)
        {
            mtv = Vector2.Zero;
            float dx = other.Center.X - Center.X;
            float px = (other.HalfSize.X + HalfSize.X) - MathF.Abs(dx);
            if (px <= 0f) return false;

            float dy = other.Center.Y - Center.Y;
            float py = (other.HalfSize.Y + HalfSize.Y) - MathF.Abs(dy);
            if (py <= 0f) return false;

            if (px < py)
            {
                mtv.X = -MathF.Sign(dx) * px;
                mtv.Y = 0f;
            }
            else
            {
                mtv.Y = -MathF.Sign(dy) * py;
                mtv.X = 0f;
            }

            return true;
        }
    }
}