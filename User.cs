using System;
using System.IO;
using Newtonsoft.Json.Linq;
using ConsoleTools;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Questionnaire_App
{
    interface IUser
    {
        bool IsRegistered
        {
            get;
            set;
        }
        string Username
        {
            get;
            set;
        }
        string Password
        {
            get;
            set;
        }
        string Uuid
        {
            get;
            set;
        }
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

        // For unregistered
        void Register(string username, string password);

        // For registered
        void CreateQuestionnaire(string title, bool isPublic, (DateTime, DateTime) timeframe);

        // For Both
        void Login(string username, string password);
        void SelectQuiz();
        void ViewStatistics();
        void View10Questionnaires();
        (Questionnaire q, Exception e) SearchQuestionnaire(string title);
        void TakeQuestionnaire(Questionnaire q);
        void ViewStatisticsOfQuestionnaire(string title);

    }
    class User : IUser
    {
        private bool _isRegistered;
        private string _username;
        private string _password;
        private string _uuid;
        private List<string> _answeredQuestionnaires;
        private static IUser _instance;
        public bool IsRegistered
        {
            get => _isRegistered;
            set => _isRegistered = value;
        }
        public string Username
        {
            get => _username;
            set => _username = value;
        }
        public string Password
        {
            get => _password;
            set => _password = value;
        }
        public string Uuid
        {
            get => _uuid;
            set => _uuid = value;
        }
        public List<string> AnsweredQuestionnaires
        {
            get => _answeredQuestionnaires;
            set => _answeredQuestionnaires = value;
        }
        public User(bool isRegistered, string username, string password, string uuid, List<string> answeredQuestionnaires)
        {
            this.IsRegistered = isRegistered;
            this.Username = username;
            this.Password = password;
            this.Uuid = uuid;
            this.AnsweredQuestionnaires = answeredQuestionnaires;
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
        public void SelectQuiz()
        {
            throw new NotImplementedException();
        }
        public void ViewStatistics()
        {
            Console.WriteLine("Username: " + this._username);
            Console.WriteLine("Number of answered questionnaires: " + this._answeredQuestionnaires.Count);
            foreach (string x in this._answeredQuestionnaires)
            {
                Console.WriteLine(x);
            }
        }
        public void View10Questionnaires()
        {
            string currentDir = Directory.GetCurrentDirectory();
            string jsonFile = currentDir + "/quizdata.json";
            string fileContent = File.ReadAllText(jsonFile);
            JObject json = JObject.Parse(fileContent);
            int counter = 1;
            foreach (JProperty p in json.Children())
            {
                if (counter > 10) break;
                JToken value = p.Value;
                if (value.Type == JTokenType.Object)
                {
                    string t = ((JObject)value).GetValue("title").ToString();
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
                            Console.WriteLine(t + " [" + s + "]" + " (blocked, start date: {0}", time1);
                            break;
                        case "late":
                            Console.WriteLine(t + " [" + s + "]" + " (blocked, end date: {0}", time2);
                            break;
                        default:
                            Console.WriteLine(t + " [" + s + "]");
                            break;
                    }
                }
                counter++;
            }
        }
        public (Questionnaire, Exception) SearchQuestionnaire(string title)
        {
            Questionnaire q = new("", "", new List<Question>(), false, "", (default(DateTime), default(DateTime)), 0);
            Exception ex = null;
            string currentDir = Directory.GetCurrentDirectory();
            string jsonFile = currentDir + "/quizdata.json";
            string fileContent = File.ReadAllText(jsonFile);
            JObject json = JObject.Parse(fileContent);
            JObject o = new();
            bool found = false;
            foreach (JProperty p in json.Children())
            {
                JToken value = p.Value;
                if (value.Type == JTokenType.Object)
                {
                    if (title == ((JObject)value).GetValue("title").ToString())
                    {
                        o[title] = value;
                        q = HelperFunctions.QuestionnaireFromJObject(o);
                        found = true;
                    }
                }
            }
            if (!found)
            {
                ex = new Exception("Questionnaire not found!");
            }
            return (q, ex);
        }
        public void TakeQuestionnaire(Questionnaire q)
        {
            throw new NotImplementedException();
        }

        public void CreateQuestionnaire(string title, bool isPublic, (DateTime, DateTime) timeframe)
        {
            Console.WriteLine("Can't create questionnaire as unregistered user");
        }
        public void Login(string username, string password)
        {
            this.Username = username;
            this.Password = password;
            string currentDir = Directory.GetCurrentDirectory();
            string jsonFile = currentDir + "/userdata.json";
            string fileContent = File.ReadAllText(jsonFile);
            JObject json = JObject.Parse(fileContent);
            string jsonUsername;
            jsonUsername = json[username]["Username"].ToString();
            string jsonPassword = json[username]["Password"].ToString();
            string jsonUUID = json[username]["Uuid"].ToString();
            List<string> jsonAnsweredQuestionnaires = json[username]["answeredQuestionnaires"].ToObject<List<string>>();
            if (jsonUsername == username && jsonPassword == password)
            {
                RegisteredUser registeredUser = new(true, username, password, jsonUUID, jsonAnsweredQuestionnaires);
                _instance = registeredUser;
                Console.WriteLine("Login Successful!");
            }
            else
            {
                Console.WriteLine("Username or password wrong");
            }
        }
        public void ViewStatisticsOfQuestionnaire(string title)
        {
            Console.WriteLine("only registered Users can view the stats of a questionnaire");
        }
    }

    class RegisteredUser : IUser
    {
        private bool _isRegistered;
        private string _username;
        private string _password;
        private string _uuid;
        private List<string> _answeredQuestionnaires;
        private static RegisteredUser _instance;
        public bool IsRegistered
        {
            get => _isRegistered;
            set => _isRegistered = value;
        }
        public string Username
        {
            get => _username;
            set => _username = value;
        }
        public string Password
        {
            get => _password;
            set => _password = value;
        }
        public string Uuid
        {
            get => _uuid;
            set => _uuid = value;
        }
        public List<string> AnsweredQuestionnaires
        {
            get => _answeredQuestionnaires;
            set => _answeredQuestionnaires = value;
        }
        public RegisteredUser(bool isRegistered, string username, string password, string uuid, List<string> answeredQuestionnaires)
        {
            this.IsRegistered = isRegistered;
            this.Username = username;
            this.Password = password;
            this.Uuid = uuid;
            this.AnsweredQuestionnaires = answeredQuestionnaires;
        }

        public static RegisteredUser Instance
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
        public void CreateQuestionnaire(string title, bool isPublic, (DateTime, DateTime) timeframe)
        {
            string quuid = System.Guid.NewGuid().ToString();
            Questionnaire newQuestionnaire = new(title, this.Username, new List<Question>(), isPublic, quuid, timeframe, 0);
            newQuestionnaire.AddQuestions();
            newQuestionnaire.FinalizeQuestionnaire();
        }
        public void ViewStatisticsOfQuestionnaire(string title)
        {
            (Questionnaire q, Exception e) = SearchQuestionnaire(title);
            int timesTaken = q.timesTaken;
            if (e != null)
            {
                Console.WriteLine(e);
            }
            if (timesTaken>0)
            {
                Console.WriteLine("times taken: {0} \n", timesTaken);
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
            } else
            {
                Console.WriteLine("No statistics available yet :/");
            }
        }

        public void Login(string username, string password)
        {
            this.Username = username;
            this.Password = password;
            string currentDir = Directory.GetCurrentDirectory();
            string jsonFile = currentDir + "/userdata.json";
            string fileContent = File.ReadAllText(jsonFile);
            JObject json = JObject.Parse(fileContent);
            string jsonUsername;
            jsonUsername = json[username]["Username"].ToString();
            string jsonPassword = json[username]["Password"].ToString();
            string jsonUUID = json[username]["Uuid"].ToString();
            List<string> jsonAnsweredQuestionnaires = json[username]["AnsweredQuestionnaires"].ToObject<List<string>>();
            if (jsonUsername == username && jsonPassword == password)
            {
                RegisteredUser registeredUser = new(true, username, password, jsonUUID, jsonAnsweredQuestionnaires);
                _instance = registeredUser;
                Console.WriteLine("Login Successful!");
            }
            else
            {
                Console.WriteLine("Username or password wrong");
            }
        }
        public void Register(string username, string password)
        {
            Console.WriteLine("registered users can't register again.");
        }
        public void SelectQuiz()
        {
            throw new NotImplementedException();
        }

        public void ViewStatistics()
        {
            Console.WriteLine("Username: " + this._username);
            Console.WriteLine("Number of answered questionnaires: " + this._answeredQuestionnaires.Count);
            foreach (string x in this._answeredQuestionnaires)
            {
                Console.WriteLine(x);
            }
        }
        public void View10Questionnaires()
        {
            string currentDir = Directory.GetCurrentDirectory();
            string jsonFile = currentDir + "/quizdata.json";
            string fileContent = File.ReadAllText(jsonFile);
            JObject json = JObject.Parse(fileContent);
            int counter = 1;
            foreach (JProperty p in json.Children())
            {
                if (counter > 10) break;
                JToken value = p.Value;
                if (value.Type == JTokenType.Object)
                {
                    string t = ((JObject)value).GetValue("title").ToString();
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
                            Console.WriteLine(t + " [" + s + "]" + " (blocked, start date: {0}", time1);
                            break;
                        case "late":
                            Console.WriteLine(t + " [" + s + "]" + " (blocked, end date: {0}", time2);
                            break;
                        default:
                            Console.WriteLine(t + " [" + s + "]");
                            break;
                    }
                }
                counter++;
            }
        }
        public (Questionnaire, Exception) SearchQuestionnaire(string title)
        {
            Questionnaire q = new("", "", new List<Question>(), false, "", (default(DateTime), default(DateTime)), 0);
            Exception ex = null;
            string currentDir = Directory.GetCurrentDirectory();
            string jsonFile = currentDir + "/quizdata.json";
            string fileContent = File.ReadAllText(jsonFile);
            JObject json = JObject.Parse(fileContent);
            JObject o = new();
            bool found = false;
            foreach (JProperty p in json.Children())
            {
                JToken value = p.Value;
                if (value.Type == JTokenType.Object)
                {
                    if (title == ((JObject)value).GetValue("title").ToString())
                    {
                        o[title] = value;
                        q = HelperFunctions.QuestionnaireFromJObject(o);
                        found = true;
                    }
                }
            }
            if (!found)
            {
                ex = new Exception("Questionnaire not found!");
            }
            return (q, ex);
        }
        public void TakeQuestionnaire(Questionnaire q)
        {
            throw new NotImplementedException();
        }
    }
}
