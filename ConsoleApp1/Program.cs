using System;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            using (Renderer renderer = new Renderer(1280, 720, "TimeToLearnOpenGL"))
            {
                renderer.Run(60.0);
            }

        }
    }
}
