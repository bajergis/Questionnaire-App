using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Questionnaire_App
{
    class Questionnaire
    {
        public string title;
        public string author;
        public bool isPublic;
        public string uuid;
        public (DateTime, DateTime) timeframe;
        public List<Question> questions;
        public int timesTaken;
        public Questionnaire(string title, string author, List<Question> questions, bool isPublic, string uuid, (DateTime, DateTime) timeframe, int timesTaken)
        {
            this.title = title;
            this.author = author;
            this.questions = questions;
            this.isPublic = isPublic;
            this.timeframe = timeframe;
            this.uuid = uuid;
            this.timesTaken = timesTaken;
        }

        public void AddQuestions()
        {
            for (int k = 0; ; k++)
            {
                string userInput;
                Console.WriteLine("enter question number " + (this.questions.Count+1));
                userInput = Console.ReadLine();
                Question newQuestion = new(this.questions.Count, userInput, new List<Answer>());
                for (int i = 0; i < 10; i++)
                {
                    Console.WriteLine("add answer number " + (newQuestion.answers.Count + 1));
                    userInput = Console.ReadLine();
                    newQuestion.AddNewAnswer(userInput);
                    if (Enumerable.Range(2, 10).Contains(newQuestion.answers.Count))
                    {
                        Console.WriteLine("add more answers? (y/n)");
                        userInput = Console.ReadLine();
                        if (userInput != "y")
                        {
                            break;
                        }
                    }
                }
                questions.Add(newQuestion);
                if (this.questions.Count > 4)
                {
                    Console.WriteLine("add more questions? (y/n)");
                    userInput = Console.ReadLine();
                    if (userInput != "y")
                    {
                        break;
                    }
                }
            }
        }
        public void FinalizeQuestionnaire()
        {
            string currentDir = Directory.GetCurrentDirectory();
            string jsonFile = currentDir + "/quizdata.json";
            Questionnaire finalQuiz = new(this.title, this.author, this.questions, this.isPublic, this.uuid, this.timeframe, this.timesTaken);
            JObject r = HelperFunctions.JObjectFromQuestionnaire(finalQuiz);
            bool success = HelperFunctions.AddQuestionnaireToFile(r, jsonFile, this.title);
            if (success)
            {
                Console.WriteLine("Quiz creation successful!");
            }
            else
            {
                Console.WriteLine("Couldn't create quiz.");
            }
        }
        public string GetTitle()
        {
            return this.title;
        }
        public string GetAuthor()
        {
            return this.author;
        }
        public bool GetIsPublic()
        {
            return this.isPublic;
        }
        public string GetUUID()
        {
            return this.uuid;
        }
        public List<Question> GetQuestions()
        {
            return this.questions;
        }
    }
}
