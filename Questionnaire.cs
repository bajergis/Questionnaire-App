using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Linq;

namespace Questionnaire_App
{
    class Questionnaire
    {
        private string author;
        private bool isPublic;
        private List<Question> questions;
        private (DateTime, DateTime) timeframe;
        private int timesTaken;
        private string title;
        private string uuid;
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

        // Let User Add Questions to Object Lost of Questionnaire
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

        // Write the Questionnaire Into a File
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
        public string GetAuthor()
        {
            return this.author;
        }
        public bool GetIsPublic()
        {
            return this.isPublic;
        }
        public List<Question> GetQuestions()
        {
            return this.questions;
        }
        public string GetTitle()
        {
            return this.title;
        }
        public (DateTime, DateTime) GetTimeFrame()
        {
            return this.timeframe;
        }
        public int GetTimesTaken()
        {
            return this.timesTaken;
        }
        public string GetUUID()
        {
            return this.uuid;
        }
        public void SetAuthor(string author)
        {
            this.author = author;
        }
        public void SetIsPublic(bool isPublic)
        {
            this.isPublic = isPublic;
        }
        public void GetQuestions(List<Question> questions)
        {
            this.questions = questions;
        }
        public void SetTitle(string title)
        {
            this.title = title;
        }
        public void SetTimeFrame((DateTime, DateTime) timeframe)
        {
            this.timeframe = timeframe;
        }
        public void SetTimesTaken(int timesTaken)
        {
            this.timesTaken = timesTaken;
        }
        public void SetUUID(string uuid)
        {
            this.uuid = uuid;
        }
    }
}
