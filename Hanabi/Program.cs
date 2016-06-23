using System;

namespace Hanabi
{
    public class Program
    {
        public static void Main()
        {
            while (true)
            {
                string commandWithArgs = Console.ReadLine();
                if (string.IsNullOrEmpty(commandWithArgs))
                    break;
                string resultGame = Game.ProcessCommand(commandWithArgs);
                if (!string.IsNullOrEmpty(resultGame))
                    Console.WriteLine(resultGame);
            }
        }
    }
}
