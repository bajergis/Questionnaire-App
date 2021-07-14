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
            RegisteredUser currentUser = new(false, "nulluser", "123", uuid, new List<string> { "default questionnaire 1" });
            Console.WriteLine("Entering Menu...");
            HelperFunctions.ShowOptionMenuRegistered(args);
            Console.WriteLine("currently the menu selection is " + HelperFunctions.menuSelection);
            HelperFunctions.HandleSelection(currentUser);
        }
    }
}
