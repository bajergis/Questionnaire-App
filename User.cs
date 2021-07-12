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
        int AnsweredQuestionnaires
        {
            get;
            set;
        }

        // For unregistered
        void Register(string username, string password);

        // For registered
        void CreateQuestionnaire(string title, bool isPublic);

        // For Both
        void SelectQuiz();
        void ViewStatistics();
        void View10Questionnaires();
        void SearchQuestionnaire();
        void TakeQuestionnaire();

    }
    class User : IUser
    {
        private bool _isRegistered;
        private string _username;
        private string _password;
        private string _uuid;
        private int _playedQuizzes;
        private int _answeredQuestionnaires;
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
        public int AnsweredQuestionnaires
        {
            get => _answeredQuestionnaires;
            set => _answeredQuestionnaires = value;
        }

        public User(bool isRegistered, string username, string password, string uuid, int answeredQuestionnaires)
        {
            this.IsRegistered = isRegistered;
            this.Username = username;
            this.Password = password;
            this.Uuid = uuid;
            this.AnsweredQuestionnaires = answeredQuestionnaires;
        }

        public void Register(string username, string password)
        {
            this.Username = username;
            this.Password = password;

            string currentDir = Directory.GetCurrentDirectory();
            string jsonFile = currentDir + "/userdata.json";
            RegisteredUser currentUser = new(true, this.Username, this.Password, this.Uuid, this.AnsweredQuestionnaires);
            JObject r = HelperFunctions.JObjectFromUser(currentUser);
            bool success = HelperFunctions.AddObjectToFile(r, jsonFile, this.Username);
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
            throw new NotImplementedException();
        }
        public void View10Questionnaires()
        {
            throw new NotImplementedException();
        }
        public void SearchQuestionnaire()
        {
            throw new NotImplementedException();
        }
        public void TakeQuestionnaire()
        {
            throw new NotImplementedException();
        }

        public void CreateQuestionnaire(string title, bool isPublic)
        {
            Console.WriteLine("Can't create questionnaire as unregistered user");
        }
    }

    class RegisteredUser : IUser
    {
        private bool _isRegistered;
        private string _username;
        private string _password;
        private string _uuid;
        private int _answeredQuestionnaires;
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
        public int AnsweredQuestionnaires
        {
            get => _answeredQuestionnaires;
            set => _answeredQuestionnaires = value;
        }

        private static RegisteredUser instance;

        public RegisteredUser(bool isRegistered, string username, string password, string uuid, int answeredQuestionnaires)
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
                if (instance == null)
                {
                    instance = new RegisteredUser(true, "", "", "", 0);
                }
                return instance;
            }
        }
        public void CreateQuestionnaire(string title, bool isPublic)
        {
            string quuid = System.Guid.NewGuid().ToString();
            Questionnaire newQuestionnaire = new(title, Instance.Username, new List<Question>(), isPublic, quuid);
            for (int i = 0; ; i++)
            {
                string userInput;
                newQuestionnaire.AddQuestions();
                Console.WriteLine("add more questions? (y/n)");
                userInput = Console.ReadLine();
                if (userInput != "y")
                {
                    newQuestionnaire.FinalizeQuestionnaire();
                    break;
                }
            }
        }
        public void ViewStatisticsOfQuestionnaire()
        {
            throw new NotImplementedException();
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
            jsonUsername = json[username]["username"].ToString();
            string jsonPassword = json[username]["password"].ToString();
            string jsonUUID = json[username]["uuid"].ToString();
            int jsonPlayedQuizzes = Int32.Parse(json[username]["playedQuizzes"].ToString());
            int jsonAnsweredQuestionnaires = Int32.Parse(json[username]["answeredQuestions"].ToString());
            if (jsonUsername == username && jsonPassword == password)
            {
                RegisteredUser registeredUser = new(true, username, password, jsonUUID, jsonAnsweredQuestionnaires);
                instance = registeredUser;
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
            throw new NotImplementedException();
        }
        public void View10Questionnaires()
        {
            throw new NotImplementedException();
        }
        public void SearchQuestionnaire()
        {
            throw new NotImplementedException();
        }
        public void TakeQuestionnaire()
        {
            throw new NotImplementedException();
        }
    }
}
