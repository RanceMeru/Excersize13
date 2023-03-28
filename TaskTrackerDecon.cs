using System;
using System.Collections.Generic;

class TaskTrackerDecon
{
    static void Main()
    {//This application keeps prompting the user for tasks to add to the task list until the user says they are done. 
//Then it displays the task list with each task marked as either "completed" or "not completed". 
//The user can then enter the numbers of the tasks they completed that day, and the application will update the task list accordingly.
        List<string> tasks = new List<string>();

        while (true)
        {
            Console.WriteLine("Enter a task to add to your task list: ");
            string task = Console.ReadLine();

            tasks.Add(task);

            Console.WriteLine("Task added!");

            Console.WriteLine("Would you like to add another task? (y/n)");
            string answer = Console.ReadLine();

            if (answer.ToLower() == "n")
            {
                break;
            }
        }

        Console.Clear();

        Console.WriteLine("Your task list:");

        for (int i = 0; i < tasks.Count; i++)
        {
            Console.Write((i + 1) + ". " + tasks[i]);

            if (IsTaskComplete(tasks[i]))
            {
                Console.WriteLine(" (completed)");
            }
            else
            {
                Console.WriteLine(" (not completed)");
            }
        }

        Console.WriteLine("Enter the numbers of the tasks you completed today (comma-separated):");
        string[] completedTasksInput = Console.ReadLine().Split(',');
        List<int> completedTasks = new List<int>();
        
        foreach (string taskIdx in completedTasksInput)
        {
            if (int.TryParse(taskIdx.Trim(), out int taskIndex))
            {
                completedTasks.Add(taskIndex);
            }
        }

        foreach (int idx in completedTasks)
        {
            tasks[idx - 1] += " (completed)";
        }

        Console.Clear();

        Console.WriteLine("Your task list:");

        for (int i = 0; i < tasks.Count; i++)
        {
            Console.WriteLine((i + 1) + ". " + tasks[i]);
        }
    }

    private static bool IsTaskComplete(string task)
    {
        return task.EndsWith("(completed)");
    }
}

//This application keeps prompting the user for tasks to add to the task list until the user says they are done. 
//Then it displays the task list with each task marked as either "completed" or "not completed". 
//The user can then enter the numbers of the tasks they completed that day, and the application will update the task list accordingly.