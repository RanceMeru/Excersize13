using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading.Tasks.Dataflow;
using System.Linq;

namespace Udemy_Project
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello My Dude!");
            

            //Excersie 13 

            //write a program that asks the user to enter a total amount of time in minutes only.
            //Print to the console the same amount of time in hours and minutes only
//computer

            Console.WriteLine("Enter your toal  amount in minutes please. ");
            int userMinutes = int.Parse(Console.ReadLine());
            int hours = userMinutes / 60;
            int minutes = userMinutes % 60;
            Console.WriteLine("Your total time in Hours is {0} hours and {1} minutes", hours, minutes);
        }
    }
}