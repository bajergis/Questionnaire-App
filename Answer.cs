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
        public int selected;
        public Answer(string answer, int selected)
        {
            this.answer = answer;
            this.selected = selected;
        }
        public string GetAnswer()
        {
            return this.answer;
        }
        public int GetSelected()
        {
            return this.selected;
        }
    }
}
