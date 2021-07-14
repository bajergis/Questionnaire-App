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
        public List<Answer> answers;

        public Question(int questionID, string title, List<Answer> answers)
        {
            this.questionID = questionID;
            this.title = title;
            this.answers = answers;
        }

        public void AddNewAnswer(string answer)
        {
            if(this.answers.Count <= 10)
            {
                Answer a = new(answer, 0);
                this.answers.Add(a);
            } else
            {
                throw new Exception("maximum amount of answers reached.");
            }
        }
        public string GetTitle()
        {
            return this.title;
        }
        public List<Answer> GetAnswers()
        {
            return this.answers;
        }
    }
}
