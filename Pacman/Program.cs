using System;

namespace Pacman
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            using (var game = new PacmanGame())
            {
                game.Run();
            }
        }
    }
}