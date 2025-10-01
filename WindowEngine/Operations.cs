using OpenTK.Mathematics;

namespace WindowEngine
{
    public static class Operations
    {
        public static Vector3 CalculateAddition(Vector3 a, Vector3 b)
        {
            return a + b;
        }

        public static Vector3 CalculateSubtraction(Vector3 a, Vector3 b)
        {
            return a - b;
        }

        public static float CalculateDot(Vector3 a, Vector3 b)
        {
            return Vector3.Dot(a, b);
        }

        public static Vector3 CalculateCross(Vector3 a, Vector3 b)
        {
            return Vector3.Cross(a, b);
        }

        public static Matrix4 Identity()
        {
            return Matrix4.Identity;
        }

        public static Matrix4 Scale(float f)
        {
            return Matrix4.CreateScale(f);
        }

        public static Matrix4 RotationY(float degrees)
        {
            return Matrix4.CreateRotationY(degrees);
        }

        public static Matrix4 Multiply(Matrix4 a, Matrix4 b)
        {
            return a * b;
        }
    }
}