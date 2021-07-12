using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Questionnaire_App
{
    class Question
    {
        public int questionID;
        public string title;
        public List<string> answers;

        public Question(int questionID, string title, List<string> answers)
        {
            this.questionID = questionID;
            this.title = title;
            this.answers = answers;
        }

        public void AddNewAnswer(string answer)
        {
            if(this.answers.Count <= 10)
            {
                this.answers.Add(answer);
            } else
            {
                throw new Exception("maximum amount of answers reached.");
            }
        }
        public string GetTitle()
        {
            return this.title;
        }
        public List<string> GetAnswers()
        {
            return this.answers;
        }
    }
}
