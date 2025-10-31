using System;           // Import basic system functionalities like Console, Math, etc.
using WindowEngine;     // Import the WindowEngine namespace, which contains Game class and other related classes

namespace WindowEngine
{
    class Program
    {
        static void Main(string[] args)
        {
            using (Game game = new Game())
            {
                game.Run();
            }
        }
    }
}