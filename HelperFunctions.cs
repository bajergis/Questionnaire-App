using System;
using ConsoleTools;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Globalization;
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
                    currentUser.ViewStatistics();
                    return;
                case MSelect.View10Questionnaires:
                    currentUser.View10Questionnaires();
                    return;
                case MSelect.CreateQuestionnaire:
                    string t;
                    bool p = true;
                    string d;
                    Console.WriteLine("Enter a title below!");
                    t = Console.ReadLine();
                    Console.WriteLine("Make public?(y/n)");
                    string inp = Console.ReadLine();
                    if (inp != "y")
                    {
                        p = false;
                    }
                    Console.WriteLine("Enter the start date.(dd/mm/yy)");
                    d = Console.ReadLine();
                    DateTime start;
                    DateTime end;
                    Exception ex;
                    (start, ex) = ValidateDateInput(d);
                    if (ex != null)
                    {
                        throw ex;
                    }
                    Console.WriteLine("Enter the end date.(dd/mm/yy)");
                    d = Console.ReadLine();
                    (end, ex) = ValidateDateInput(d);
                    if (ex != null)
                    {
                        throw ex;
                    }
                    currentUser.CreateQuestionnaire(t, p, (start, end));
                    return;
                case MSelect.SearchQuestionnaires:
                    string s;
                    Console.WriteLine("enter title:");
                    s = Console.ReadLine();
                    (Questionnaire q, Exception e) = currentUser.SearchQuestionnaire(s);
                    if (e == null)
                    {
                        Console.WriteLine("start taking questionnaire?(y/n)");
                        s = Console.ReadLine();
                        if (s == "y")
                        {
                            currentUser.TakeQuestionnaire(q);
                        }
                    }
                    return;
                case MSelect.ViewQuestionnaireStats:
                    string title;
                    Console.WriteLine("Enter questionnaire title:");
                    title = Console.ReadLine();
                    currentUser.ViewStatisticsOfQuestionnaire(title);
                    return;
                case MSelect.Register:
                    string registerName;
                    string registerPassword;
                    Console.WriteLine("Enter a username below!");
                    registerName = Console.ReadLine();
                    var regex = new Regex("^[a-zA-Z0-9]*$");
                    if (regex.Match(registerName).Success)
                    {
                        Console.WriteLine("Enter your password below!");
                        registerPassword = ReadPassword();
                        currentUser.Register(registerName, registerPassword);
                    }
                    else
                    {
                        Console.WriteLine("Invalid username.");
                    }
                    return;
                case MSelect.Login:
                    string loginName;
                    string loginPassword;
                    Console.WriteLine("Enter a username below!");
                    loginName = Console.ReadLine();
                    Console.WriteLine("Enter your password below!");
                    loginPassword = ReadPassword();
                    RegisteredUser currentRegisteredUser = new(true, loginName, loginPassword, "", new List<string>());
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
            return password;
        }
        public static JObject JObjectFromUser(IUser user)
        {
            JObject o = new();
            var json = JsonConvert.SerializeObject(user, Formatting.Indented);
            o[user.Username] = JObject.Parse(json);
            return o;
        }
        public static User UserFromJObject(JObject o)
        {
            string username = "";
            foreach (JProperty p in o.Children())
            {
                JToken value = p.Value;
                if (value.Type == JTokenType.Object)
                {
                    username = ((JObject)value).GetValue("Username").ToString();
                }
            }
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
        public static Questionnaire QuestionnaireFromJObject(JObject o)
        {
            string title = "";
            foreach (JProperty p in o.Children())
            {
                JToken value = p.Value;
                if (value.Type == JTokenType.Object)
                {
                    title = ((JObject)value).GetValue("title").ToString();
                    Console.WriteLine(title);
                }
            }
            string json = o[title].ToString();
            Questionnaire q = JsonConvert.DeserializeObject<Questionnaire>(json);
            return q;
        }
        public static bool AddUserToFile(JObject userObj, string jFile, string key)
        {
            bool success = false;
            string writeJson;
            if (File.Exists(jFile))
            {
                string fileContent = File.ReadAllText(jFile);
                JObject o = JObject.Parse(fileContent);
                User u = UserFromJObject(userObj);
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
        public static bool AddQuestionnaireToFile(JObject userObj, string jFile, string key)
        {
            bool success = false;
            string writeJson;
            if (File.Exists(jFile))
            {
                string fileContent = File.ReadAllText(jFile);
                JObject o = JObject.Parse(fileContent);
                Questionnaire u = QuestionnaireFromJObject(userObj);
                var json = JsonConvert.SerializeObject(u, Formatting.Indented);
                if (!o.ContainsKey(key))
                {
                    o.Add(u.title, JObject.Parse(json));
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
        public static (DateTime, Exception) ValidateDateInput(string s)
        {
            DateTime dt;
            Exception ex;
            string[] formats = { "dd/MM/yy" , "MM/dd/yyyy hh:mm:ss"};
            if (DateTime.TryParseExact(s, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
            {
                ex = null;
            }
            else
            {
                ex = new Exception("invalid date.");
            }
            return (dt, ex);
        }
        public static string CheckValidTimeframe((DateTime, DateTime) timeframe)
        {
            string result;

            // startdate is later than today
            if (timeframe.Item1 > DateTime.Now)
            {
                result = "early";
            }
            //enddate is earlier than today
            else if (timeframe.Item2 < DateTime.Now)
            {
                result = "late";
            }
            else result = "valid";
            
            return result;
        }
    }
}
