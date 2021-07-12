using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Questionnaire_App
{
    class Answer
    {
        public string answer;
        public Answer(string answer)
        {
            this.answer = answer;
        }
        public string GetAnswer()
        {
            return this.answer;
        }
    }
}
