using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ConsoleTools;

namespace Questionnaire_App
{
    class Program
    {
        static void Main(string[] args)
        {
            string uuid;
            Console.WriteLine("This is Jay's Questionaire App!");
            uuid = System.Guid.NewGuid().ToString();
            IUser currentUser = new User(true, "jsn", "swag", uuid, new List<string> { "default questionnaire 1" });
            Console.WriteLine("Press any key...");
            Console.ReadKey();
            currentUser.GenerateMenu();
        }
    }
}
