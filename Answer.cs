namespace Questionnaire_App
{
    class Answer
    {
        private string answer;
        private int selected;
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
        public void SetAnswer(string answer)
        {
            this.answer = answer;
        }
        public void SetSelected(int selected)
        {
            this.selected = selected;
        }
    }
}
