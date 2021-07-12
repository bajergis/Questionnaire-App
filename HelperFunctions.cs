using System;
using ConsoleTools;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Questionnaire_App
{
    class HelperFunctions
    {
        public static MSelect menuSelection;
        public static void ShowOptionMenuUnregistered(string[] args)
        {
            var menu = new ConsoleMenu(args, level: 0)
              .Add("Take Questionnaire", (thisMenu) => { Console.WriteLine("You selected taking a questionnaire!"); menuSelection = MSelect.TakeQuestionnaire; thisMenu.CloseMenu(); })
              .Add("View 10 Questionnaires", (thisMenu) => { Console.WriteLine("You selected viewing 10 questionnaires!"); menuSelection = MSelect.View10Questionnaires; thisMenu.CloseMenu(); })
              .Add("Search Questionnaires", (thisMenu) => { Console.WriteLine("You selected searching for questionnaires!"); menuSelection = MSelect.SearchQuestionnaires; thisMenu.CloseMenu(); })
              .Add("View Stats", (thisMenu) => { Console.WriteLine("You selected viewing your statistics!"); menuSelection = MSelect.ViewUserStats; thisMenu.CloseMenu(); })
              .Add("Log In", (thisMenu) => { Console.WriteLine("You selected log in!"); menuSelection = MSelect.Login; thisMenu.CloseMenu(); })
              .Add("Register", (thisMenu) => { Console.WriteLine("You selected registering!"); menuSelection = MSelect.Register; thisMenu.CloseMenu(); })
              .Add("Exit", () => Environment.Exit(0))
              .Configure(config =>
              {
                  config.Selector = "--> ";
                  config.EnableFilter = false;
                  config.Title = "Unregistered User Menu";
                  config.EnableWriteTitle = false;
                  config.EnableBreadcrumb = true;
              });

            menu.Show();
        }
        public static void ShowOptionMenuRegistered(string[] args)
        {
            var menu = new ConsoleMenu(args, level: 0)
              .Add("Create Questionnaire", (thisMenu) => { Console.WriteLine("You selected creating a questionnaire!"); menuSelection = MSelect.CreateQuestionnaire; thisMenu.CloseMenu(); })
              .Add("Take Questionnaire", (thisMenu) => { Console.WriteLine("You selected taking a questionnaire!"); menuSelection = MSelect.TakeQuestionnaire; thisMenu.CloseMenu(); })
              .Add("View 10 Questionnaires", (thisMenu) => { Console.WriteLine("You selected viewing 10 questionnaires!"); menuSelection = MSelect.View10Questionnaires; thisMenu.CloseMenu(); })
              .Add("Search Questionnaires", (thisMenu) => { Console.WriteLine("You selected searching for questionnaires!"); menuSelection = MSelect.SearchQuestionnaires; thisMenu.CloseMenu(); })
              .Add("View Stats", (thisMenu) => { Console.WriteLine("You selected viewing your statistics!"); menuSelection = MSelect.ViewUserStats; thisMenu.CloseMenu(); })
              .Add("Register", (thisMenu) => { Console.WriteLine("You selected registering!"); menuSelection = MSelect.Register; thisMenu.CloseMenu(); })
              .Add("View Stats of Questionnaire", (thisMenu) => { Console.WriteLine("You selected viewing the stats of a certain Questionnaire!"); menuSelection = MSelect.ViewQuestionnaireStats; thisMenu.CloseMenu(); })
              .Add("Exit", () => Environment.Exit(0))
              .Configure(config =>
              {
                  config.Selector = "--> ";
                  config.EnableFilter = false;
                  config.Title = "Registered User Menu";
                  config.EnableWriteTitle = false;
                  config.EnableBreadcrumb = true;
              });

            menu.Show();
        }
        public static void HandleSelection(IUser currentUser)
        {
            switch (menuSelection)
            {
                case MSelect.TakeQuestionnaire:
                    return;
                case MSelect.ViewUserStats:
                    return;
                case MSelect.View10Questionnaires:
                    return;
                case MSelect.CreateQuestionnaire:
                    string t;
                    bool p;
                    Console.WriteLine("Enter a title below!");
                    t = Console.ReadLine();
                    Console.WriteLine("Make public?");
                    p = Convert.ToBoolean(Console.ReadLine());
                    currentUser.CreateQuestionnaire(t, p);
                    return;
                case MSelect.SearchQuestionnaires:
                    return;
                case MSelect.ViewQuestionnaireStats:
                    return;
                case MSelect.Register:
                    string registerName;
                    string registerPassword;
                    Console.WriteLine("Enter a username below!");
                    registerName = Console.ReadLine();
                    Console.WriteLine("Enter your password below!");
                    registerPassword = ReadPassword();
                    currentUser.Register(registerName, registerPassword);
                    return;
                case MSelect.Login:
                    string loginName;
                    string loginPassword;
                    Console.WriteLine("Enter a username below!");
                    loginName = Console.ReadLine();
                    Console.WriteLine("Enter your password below!");
                    loginPassword = ReadPassword();
                    RegisteredUser currentRegisteredUser = new(true, loginName, loginPassword, "", 0);
                    currentRegisteredUser.Login(loginName, loginPassword);
                    return;
                default:
                    throw new Exception("no enum selected");
            }
        }
        public static string ReadPassword()
        {
            string password = "";

            ConsoleKeyInfo info = Console.ReadKey(true);
            while (info.Key != ConsoleKey.Enter)
            {
                if (info.Key != ConsoleKey.Backspace)
                {
                    Console.Write("*");
                    password += info.KeyChar;
                }
                else if (info.Key == ConsoleKey.Backspace)
                {
                    if (!string.IsNullOrEmpty(password))
                    {
                        password = password[0..^1];
                        int pos = Console.CursorLeft;
                        Console.SetCursorPosition(pos - 1, Console.CursorTop);
                        Console.Write(" ");
                        Console.SetCursorPosition(pos - 1, Console.CursorTop);
                    }
                }
                info = Console.ReadKey(true);
            }
            Console.WriteLine();
            return password;
        }
        public static JObject JObjectFromUser(IUser user)
        {
            JObject o = new();
            var json = JsonConvert.SerializeObject(user, Formatting.Indented);
            o[user.Username] = JObject.Parse(json);
            return o;
        }
        public static User UserFromJObject(JObject o, string username)
        {
            string json = o[username].ToString();
            User u = JsonConvert.DeserializeObject<User>(json);
            return u;
        }
        public static JObject JObjectFromQuestionnaire(Questionnaire q)
        {
            JObject o = new();
            var json = JsonConvert.SerializeObject(q, Formatting.Indented);
            o[q.title] = JObject.Parse(json);
            return o;
        }
        public static Questionnaire QuestionnaireFromJObject(JObject o, string username)
        {
            string json = o[username].ToString();
            Questionnaire q = JsonConvert.DeserializeObject<Questionnaire>(json);
            return q;
        }
        public static bool AddObjectToFile(JObject userObj, string jFile, string key)
        {
            bool success = false;
            string writeJson;
            if (File.Exists(jFile))
            {
                string fileContent = File.ReadAllText(jFile);
                JObject o = JObject.Parse(fileContent);
                User u = UserFromJObject(userObj, key);
                var json = JsonConvert.SerializeObject(u, Formatting.Indented);
                if (!o.ContainsKey(key))
                {
                    o.Add(u.Username, JObject.Parse(json));
                    success = true;
                    writeJson = JsonConvert.SerializeObject(o, Formatting.Indented);
                }
                else return success;
            }
            else
            {
                writeJson = JsonConvert.SerializeObject(userObj, Formatting.Indented);
                success = true;
            }
            File.WriteAllText(jFile, writeJson);
            return success;
        }
    }
}
