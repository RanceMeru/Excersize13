using System;
using System.Collections.Generic;
using System.IO;

namespace TaskList
{
    class Program
    {
        static void Main(string[] args)
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            Console.WriteLine("Welcome to the Task List program!");
            Console.WriteLine("Enter 'exit' to quit the program.");
            Console.WriteLine();

            List<string> taskList = new List<string>();

            while (true)
            {
                Console.Write("Enter a task: ");
                string input = Console.ReadLine().Trim();

                if (input.ToLower() == "exit")
                {
                    break;
                }

                taskList.Add(input);
                Console.WriteLine("Task added to the list!");
                Console.WriteLine();
            }

           string filePath = Path.Combine(desktopPath, "TaskList.txt");

           using (StreamWriter writer = new StreamWriter(filePath, true))//the true boolean in the parentheses makes the file update and not overite
        {
            foreach (string task in taskList)
            {
             writer.WriteLine(task);
            }
        }

            Console.WriteLine("Task List saved to file!");
            Console.WriteLine($"Location: {filePath}");

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }
}
