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
            Matrix4 rotation  = Operations.RotationY(45f);
            Matrix4 translation = Matrix4.CreateTranslation(1, 2, 0);

            Matrix4 transform = scale * rotation * translation;
            
            Vector4 v = new Vector4(1, 0, 0, 1);
            var result = transform * v;

            Console.WriteLine("\nMatrix operations:");
            Console.WriteLine($"identity = {Operations.Identity()}");
            Console.WriteLine($"result of matrix operations: {v} -> {result}");
            
            using var game = new Game();
            game.Run();
        }
    }
}