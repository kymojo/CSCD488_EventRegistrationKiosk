using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace RegistrationKiosk {
    public class QuestionEntry {

        //===========================================================================
        #region Variables
        //===========================================================================
        
        string questionText;
        List<string> choices = new List<string>();

        #endregion
        //===========================================================================
        #region Constructor
        //===========================================================================
        public QuestionEntry(string questionText) {
            this.questionText = questionText;
        }

        public QuestionEntry(string questionText, string[] choices) {
            this.questionText = questionText;
            int i = 0;
            while (i < choices.Length) {
                AddNewChoice(choices[i]);
                i++;
            }
        }
        #endregion
        //===========================================================================
        #region Get/Set
        //===========================================================================
        public string GetQuestionText() {
            return questionText;
        }
        
        public string GetChoiceAt(int index) {
            return choices[index];
        }

        public int GetChoiceCount() {
            return choices.Count;
        }

        public void ChangeText(string newText) {
            questionText = newText;
        }

        public void AddNewChoice(string newChoice) {
            choices.Add(newChoice);
        }

        public void RemoveChoice(int index) {
            choices.RemoveAt(index);
        }

        public void EditChoice(int index, string newChoice) {
            choices[index] = newChoice;
        }
        #endregion
        //===========================================================================
    }
}
