using System;
using System.IO;
using Newtonsoft.Json.Linq;
using ConsoleTools;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Questionnaire_App
{
    interface IUser
    {
        List<string> AnsweredQuestionnaires
        {
            get;
            set;
        }
        static IUser Instance
        {
            get;
            set;
        }
        bool IsRegistered
        {
            get;
            set;
        }
        string Password
        {
            get;
            set;
        }
        string Username
        {
            get;
            set;
        }
        string Uuid
        {
            get;
            set;
        }
        void CreateQuestionnaire();

        // Menu Changes Depending on User Type
        public void GenerateMenu()
        {
            string[] args = Environment.GetCommandLineArgs();
            var menu = new ConsoleMenu(args, level: 0)
              .Add("View 10 Questionnaires", (thisMenu) => { View10Questionnaires(); })
              .Add("Search Questionnaires", (thisMenu) =>
              {
                  string title;
                  Console.WriteLine("enter title:");
                  title = Console.ReadLine();
                  (Questionnaire q, bool found) = SearchQuestionnaire(title);
                  if (found)
                  {
                      if (!this.AnsweredQuestionnaires.Contains(title))
                      {
                          Console.WriteLine("start taking questionnaire?(y/n)");
                          string input = Console.ReadLine();
                          if (input == "y")
                          {
                              TakeQuestionnaire(q);
                          }
                      }
                      else
                      {
                          Console.WriteLine("Already completed questionnaire.");
                          Console.WriteLine("Press any key...");
                          Console.ReadKey();
                      }
                  }
                  else
                  {
                      Console.WriteLine("Questionnaire not found!");
                      Console.WriteLine("Press any key...");
                      Console.ReadKey();
                  }
              })
              .Add("View Stats", (thisMenu) => { ViewStatistics(); });
            if (this is User)
            {
                menu.Add("Register", (thisMenu) =>
                {
                    string registerName;
                    string registerPassword;
                    Console.WriteLine("Enter a username below!");
                    registerName = Console.ReadLine();
                    var regex = new Regex("^[a-zA-Z0-9]*$");
                    if (regex.Match(registerName).Success)
                    {
                        Console.WriteLine("Enter your password below!");
                        registerPassword = HelperFunctions.ReadPassword();
                        Register(registerName, registerPassword);
                    }
                    else
                    {
                        Console.WriteLine("Invalid username.");
                    }
                    Console.WriteLine("Press any key...");
                    Console.ReadKey();
                })
                .Add("Log In", (thisMenu) =>
                {
                    string loginName;
                    string loginPassword;
                    Console.WriteLine("Enter a username below!");
                    loginName = Console.ReadLine();
                    Console.WriteLine("Enter your password below!");
                    loginPassword = HelperFunctions.ReadPassword();
                    IUser currentRegisteredUser = new RegisteredUser(true, loginName, loginPassword, "", new List<string>());
                    currentRegisteredUser.Login(loginName, loginPassword);
                    thisMenu.CloseMenu();
                });
            }
            if (this is RegisteredUser)
            {
                menu.Add("Create Questionnaire", (thisMenu) => { CreateQuestionnaire(); thisMenu.CloseMenu(); })
                    .Add("View Stats of Questionnaire", (thisMenu) =>
                    {
                        Console.WriteLine("You selected viewing the stats of a certain Questionnaire!");
                        string title;
                        Console.WriteLine("Enter questionnaire title:");
                        title = Console.ReadLine();
                        ViewStatisticsOfQuestionnaire(title);
                    });
            }
            menu.Add("Exit", () => Environment.Exit(0))
              .Configure(config =>
              {
                  config.Selector = "--> ";
                  config.EnableFilter = false;
                  config.Title = "User Menu";
                  config.EnableWriteTitle = false;
                  config.EnableBreadcrumb = true;
              });
            menu.Show();
        }
        // Login for User and RegisteredUser
        public void Login(string username, string password)
        {
            string currentDir = Directory.GetCurrentDirectory();
            string jsonFile = currentDir + "/userdata.json";
            string fileContent = File.ReadAllText(jsonFile);
            JObject json = JObject.Parse(fileContent);
            foreach (JProperty p in json.Children())
            {
                JToken value = p.Value;
                if (value.Type == JTokenType.Object && ((JObject)value).GetValue("Username").ToString() == username)
                {
                    string uuid = ((JObject)value).GetValue("Uuid").ToString();
                    string jsonUsername;
                    jsonUsername = json[uuid]["Username"].ToString();
                    string jsonPassword = json[uuid]["Password"].ToString();
                    string jsonUUID = json[uuid]["Uuid"].ToString();
                    List<string> jsonAnsweredQuestionnaires = json[uuid]["AnsweredQuestionnaires"].ToObject<List<string>>();
                    if (jsonUsername == username && jsonPassword == password)
                    {
                        this.Username = username;
                        this.Password = password;
                        RegisteredUser registeredUser = new(true, username, password, jsonUUID, jsonAnsweredQuestionnaires);
                        Instance = registeredUser;
                        Console.WriteLine("Login Successful!");
                    }
                    else
                    {
                        Console.WriteLine("Username or password wrong");
                    }
                }
            }
            this.GenerateMenu();
        }
        void Register(string username, string password);

        // Returns the Questionnaire if Found, Boolean is False if Not Found
        public (Questionnaire, bool) SearchQuestionnaire(string title)
        {
            Questionnaire q = new("", "", new List<Question>(), false, "", (default(DateTime), default(DateTime)), 0);
            string currentDir = Directory.GetCurrentDirectory();
            string jsonFile = currentDir + "/quizdata.json";
            string fileContent = File.ReadAllText(jsonFile);
            JObject json = JObject.Parse(fileContent);
            JObject o = new();
            bool found = false;
            foreach (JProperty p in json.Children())
            {
                JToken value = p.Value;
                if (value.Type == JTokenType.Object && ((JObject)value).GetValue("title").ToString() == title)
                {
                    if (title == ((JObject)value).GetValue("title").ToString())
                    {
                        string uuid = ((JObject)value).GetValue("uuid").ToString();
                        o[uuid] = value;
                        q = HelperFunctions.QuestionnaireFromJObject(o);
                        found = true;
                    }
                }
            }
            return (q, found);
        }

        // Generates Menu Out of Questions From Questionnaire in File
        public void TakeQuestionnaire(Questionnaire q)
        {
            q.timesTaken++;
            string[] args = Environment.GetCommandLineArgs();
            List<Question> questions = q.questions;
            string currentDir = Directory.GetCurrentDirectory();
            string quizFile = currentDir + "/quizdata.json";
            string userFile = currentDir + "/userdata.json";
            foreach (Question x in questions)
            {
                var menu = new ConsoleMenu(args, level: 0);
                List<Answer> answers = x.answers;
                Console.WriteLine(x.title);
                foreach (Answer y in answers)
                {
                    menu.Add(y.answer, (thisMenu) => { y.selected++; thisMenu.CloseMenu(); });
                }
                menu.Configure(config =>
                {
                    config.Selector = "--> ";
                    config.EnableFilter = false;
                    config.Title = q.title;
                    config.EnableWriteTitle = false;
                    config.EnableBreadcrumb = true;
                });
                menu.Show();
                menu.CloseMenu();
            }
            this.AnsweredQuestionnaires.Add(q.title);
            if (IsRegistered)
            {
                var userVar = JsonConvert.SerializeObject(this, Formatting.Indented);
                JObject jUser = JObject.Parse(userVar);
                HelperFunctions.UpdateJson(userFile, jUser, this.Username);
            }
            var qVar = JsonConvert.SerializeObject(q, Formatting.Indented);
            JObject jQuest = JObject.Parse(qVar);
            HelperFunctions.UpdateJson(quizFile, jQuest, q.uuid);
        }

        // Views 10 Questionnaires from File, Skips Users Own Questionnaires and Already Taken Ones
        public void View10Questionnaires()
        {
            string[] args = Environment.GetCommandLineArgs();
            string currentDir = Directory.GetCurrentDirectory();
            string jsonFile = currentDir + "/quizdata.json";
            string fileContent = File.ReadAllText(jsonFile);
            JObject json = JObject.Parse(fileContent);
            int counter = 1;
            var menu = new ConsoleMenu(args, level: 0);
            foreach (JProperty p in json.Children())
            {
                if (counter > 10) break;
                JToken value = p.Value;
                if (value.Type == JTokenType.Object)
                {
                    string a = ((JObject)value).GetValue("author").ToString();
                    string t = ((JObject)value).GetValue("title").ToString();

                    if (this.Username != a && !this.AnsweredQuestionnaires.Contains(t))
                    {
                        string s = ((JObject)value).GetValue("timesTaken").ToString();
                        JToken tf = ((JObject)value).GetValue("timeframe");
                        string tf1 = tf.Value<string>("Item1");
                        string tf2 = tf.Value<string>("Item2");
                        (DateTime time1, _) = HelperFunctions.ValidateDateInput(tf1);
                        (DateTime time2, _) = HelperFunctions.ValidateDateInput(tf2);
                        string validCheck = HelperFunctions.CheckValidTimeframe((time1, time2));
                        switch (validCheck)
                        {
                            case "early":
                                menu.Add((t + " [" + s + "]" + " (blocked, start date: " + time1 + ")"), (thisMenu) => {; });
                                break;
                            case "late":
                                menu.Add(t + " [" + s + "]" + " (blocked, end date: " + time1 + ")", (thisMenu) => {; });
                                break;
                            default:
                                menu.Add(t + " [" + s + "]", (thisMenu) => { (Questionnaire q, _) = SearchQuestionnaire(t); TakeQuestionnaire(q); });
                                break;
                        }
                        counter++;
                    }
                }
            }
            menu.Add("Back", (thisMenu) => { this.GenerateMenu(); });
            menu.Configure(config =>
            {
                config.Selector = "--> ";
                config.EnableFilter = false;
                config.Title = "Top Questionnaires";
                config.EnableWriteTitle = false;
                config.EnableBreadcrumb = true;
            });
            menu.Show();
        }

        // Writes Username, Amount of Answered Questionnaires and All Taken Questionnaires
        public void ViewStatistics()
        {
            Console.WriteLine("Username: " + this.Username);
            Console.WriteLine("Number of answered questionnaires: " + this.AnsweredQuestionnaires.Count);
            foreach (string x in this.AnsweredQuestionnaires)
            {
                Console.WriteLine(x);
            }
            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }       
        void ViewStatisticsOfQuestionnaire(string title);
    }

    // Subclass Unregistered User
    class User : IUser
    {
        private List<string> _answeredQuestionnaires;
        private static IUser _instance;
        private bool _isRegistered;
        private string _password;
        private string _username;
        private string _uuid;
        public List<string> AnsweredQuestionnaires
        {
            get => _answeredQuestionnaires;
            set => _answeredQuestionnaires = value;
        }
        public static IUser Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new User(false, "", "", "", new List<string> { "default questionnaire 1" });
                }
                return _instance;
            }
        }
        public bool IsRegistered
        {
            get => _isRegistered;
            set => _isRegistered = value;
        }
        public string Password
        {
            get => _password;
            set => _password = value;
        }
        public string Username
        {
            get => _username;
            set => _username = value;
        }
        public string Uuid
        {
            get => _uuid;
            set => _uuid = value;
        }
        public User(bool isRegistered, string username, string password, string uuid, List<string> answeredQuestionnaires)
        {
            this.IsRegistered = isRegistered;
            this.Username = username;
            this.Password = password;
            this.Uuid = uuid;
            this.AnsweredQuestionnaires = answeredQuestionnaires;
        }

        // No Function for Unregistered
        public void CreateQuestionnaire()
        {
            Console.WriteLine("only registered users can create questionnaires.");
        }

        // Only Unregistered User Can Register
        public void Register(string username, string password)
        {
            this.Username = username;
            this.Password = password;

            string currentDir = Directory.GetCurrentDirectory();
            string jsonFile = currentDir + "/userdata.json";
            RegisteredUser currentUser = new(true, this.Username, this.Password, this.Uuid, this.AnsweredQuestionnaires);
            JObject r = HelperFunctions.JObjectFromUser(currentUser);
            bool success = HelperFunctions.AddUserToFile(r, jsonFile, this.Username);
            if (success)
            {
                Console.WriteLine("Registration successful!");
            }
            else
            {
                Console.WriteLine("user already exists.");
            }
        }

        // No Function for Unregistered
        public void ViewStatisticsOfQuestionnaire(string title)
        {
            Console.WriteLine("only registered users can view their own questionnaire stats.");
        }
    }

    // Subclass Registered User
    class RegisteredUser : IUser
    {
        private List<string> _answeredQuestionnaires;
        private static IUser _instance;
        private string _username;
        private bool _isRegistered;
        private string _password;
        private string _uuid;
        public List<string> AnsweredQuestionnaires
        {
            get => _answeredQuestionnaires;
            set => _answeredQuestionnaires = value;
        }
        public static IUser Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new RegisteredUser(true, "", "", "", new List<string> { "default questionnaire 1" });
                }
                return _instance;
            }
        }
        public bool IsRegistered
        {
            get => _isRegistered;
            set => _isRegistered = value;
        }
        public string Password
        {
            get => _password;
            set => _password = value;
        }
        public string Username
        {
            get => _username;
            set => _username = value;
        }
        public string Uuid
        {
            get => _uuid;
            set => _uuid = value;
        }
        public RegisteredUser(bool isRegistered, string username, string password, string uuid, List<string> answeredQuestionnaires)
        {
            this.IsRegistered = isRegistered;
            this.Username = username;
            this.Password = password;
            this.Uuid = uuid;
            this.AnsweredQuestionnaires = answeredQuestionnaires;
        }

        // Create a Questionnaire From User Input and Write to File
        public void CreateQuestionnaire()
        {
            string title;
            bool isPublic = true;
            string date;
            Console.WriteLine("Enter a title below!");
            title = Console.ReadLine();
            Console.WriteLine("Make public?(y/n)");
            string pInput = Console.ReadLine();
            if (pInput != "y")
            {
                isPublic = false;
            }
            Console.WriteLine("Enter the start date.(dd/mm/yy)");
            date = Console.ReadLine();
            DateTime start;
            DateTime end;
            Exception ex;
            (start, ex) = HelperFunctions.ValidateDateInput(date);
            if (ex != null)
            {
                throw ex;
            }
            Console.WriteLine("Enter the end date.(dd/mm/yy)");
            date = Console.ReadLine();
            (end, ex) = HelperFunctions.ValidateDateInput(date);
            if (ex != null)
            {
                throw ex;
            }
            string quuid = System.Guid.NewGuid().ToString();
            Questionnaire newQuestionnaire = new(title, this.Username, new List<Question>(), isPublic, quuid, (start, end), 0);
            newQuestionnaire.AddQuestions();
            newQuestionnaire.FinalizeQuestionnaire();
        }

        // No Function For Registered User
        public void Register(string username, string password)
        {
            Console.WriteLine("registered users can't register again.");
        }

        // Views How Often Questionnaire Has Been Taken and Writes the Count for All Answers in the Questionnaire
        public void ViewStatisticsOfQuestionnaire(string title)
        {
            IUser u = this;
            (Questionnaire q, bool found) = u.SearchQuestionnaire(title);
            int times = q.timesTaken;
            string auth = q.author;
            if (found)
            {
                if (times > 0 && auth == this.Username)
                {
                    Console.WriteLine("times taken: {0} \n", times);
                    List<Question> questions = q.questions;
                    foreach (Question x in questions)
                    {
                        Console.WriteLine("\n" + x.title + "\n");
                        List<Answer> answers = x.answers;
                        foreach (Answer y in answers)
                        {
                            Console.WriteLine(y.answer + " [" + y.selected + "]");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Can't view questionnaire stats from user {0}; times taken: {1}", auth, times);
                }
            }
            else
            {
                Console.WriteLine("Questionnaire not found!");
            }
            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }
    }
}
