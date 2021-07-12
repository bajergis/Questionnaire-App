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
        public List<Question> questions;
        public bool isPublic;
        public string uuid;
        public Questionnaire(string title, string author, List<Question> questions, bool isPublic, string uuid)
        {
            this.title = title;
            this.author = author;
            this.questions = questions;
            this.isPublic = isPublic;
            this.uuid = Guid.NewGuid().ToString();
        }

        public void AddQuestions()
        {
            for (int k = 0; ; k++)
            {
                string userInput;
                Console.WriteLine("enter title for question");
                userInput = Console.ReadLine();
                Question newQuestion = new(questions.Count, userInput, new List<string>());
                for (int i = 0; i < 10; i++)
                {
                    Console.WriteLine("add new answer");
                    userInput = Console.ReadLine();
                    newQuestion.AddNewAnswer(userInput);
                    Console.WriteLine("add more answers? (y/n)");
                    userInput = Console.ReadLine();
                    if (userInput != "y")
                    {
                        if (Enumerable.Range(2,10).Contains(newQuestion.answers.Count))
                        {
                            break;
                        }
                        else
                        {
                            Console.WriteLine("need at least 2 answers to finish adding answers");
                        }
                    }
                }
                questions.Add(newQuestion);
                Console.WriteLine("add more questions? (y/n)");
                userInput = Console.ReadLine();
                if (userInput != "y")
                {
                    if (this.questions.Count > 4)
                    {
                        break;
                    }
                    else
                    {
                        Console.WriteLine("need at least 5 questions to finish adding answers");
                    }
                }
            }
        }
        public void FinalizeQuestionnaire()
        {
            string currentDir = Directory.GetCurrentDirectory();
            string jsonFile = currentDir + "/quizdata.json";
            Questionnaire finalQuiz = new(this.title, this.author, this.questions, this.isPublic, this.uuid);
            JObject r = HelperFunctions.JObjectFromQuestionnaire(finalQuiz);
            bool success = HelperFunctions.AddObjectToFile(r, jsonFile, this.title);
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
    }
}
