using System;
using System.Collections.Generic;

namespace Questionnaire_App
{
    class Program
    {
        static void Main(string[] args)
        {
            string uuid;
            Console.WriteLine("This is Jay's Questionaire App!");
            uuid = System.Guid.NewGuid().ToString();
            IUser currentUser = new User(true, "nulluser", "password", uuid, new List<string> {});
            Console.WriteLine("Press any key...");
            Console.ReadKey();
            currentUser.GenerateMenu();
        }
    }
}
