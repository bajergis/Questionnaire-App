using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Questionnaire_App
{
    class Question
    {
        public List<Answer> answers;
        public int questionID;
        public string title;

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
        public List<Answer> GetAnswers()
        {
            return this.answers;
        }
        public string GetTitle()
        {
            return this.title;
        }
    }
}
