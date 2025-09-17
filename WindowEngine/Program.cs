using System;
using OpenTK.Mathematics;

namespace WindowEngine
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var a = new Vector3(1, 2, 3);
            var b = new Vector3(4, 5, 6);

            var add  = Operations.CalculateAddition(a, b);
            var sub  = Operations.CalculateSubtraction(a, b);
            var dot  = Operations.CalculateDot(a, b);
            var cross= Operations.CalculateCross(a, b);

            Console.WriteLine("Vector operations:");
            Console.WriteLine($"a + b = {add}");
            Console.WriteLine($"a - b = {sub}");
            Console.WriteLine($"dot product = {dot}");
            Console.WriteLine($"cross product = {cross}");

            Matrix4 scale = Operations.Scale(2.0f);
            Matrix4 rotation  = Operations.RotationX(45f);
            Matrix4 translation = Matrix4.CreateTranslation(1, 2, 0);

            Matrix4 transform = scale * rotation * translation;
            
            Vector4 v = new Vector4(1, 0, 0, 1);
            var result = transform * v;

            Console.WriteLine("\nMatrix operations:");
            Console.WriteLine($"identity = {Operations.Identity()}");
            Console.WriteLine($"result of matrix operations: {v} -> {result}");
            
            Quaternion q = Quaternion.FromAxisAngle(Vector3.UnitY, MathHelper.DegreesToRadians(90));
            Vector3 point = new Vector3(2, 3, 1);
            Vector3 rotated = Vector3.Transform(point, q);

            Console.WriteLine("\nQuaternion operations:");
            Console.WriteLine($"original point = {point}");
            Console.WriteLine($"rotated point  = {rotated}");
            //using var game = new Game();
            //game.Run();
        }
    }
}