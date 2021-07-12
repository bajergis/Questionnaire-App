using System;
using System.IO;
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
            User currentUser = new(false, "nulluser", "", uuid, 0);
            Console.WriteLine("Entering Menu...");
            HelperFunctions.ShowOptionMenuUnregistered(args);
            Console.WriteLine("currently the menu selection is " + HelperFunctions.menuSelection);
            HelperFunctions.HandleSelection(currentUser);
        }
    }
}
