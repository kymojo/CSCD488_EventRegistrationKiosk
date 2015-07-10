using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace RegistrationKiosk {
    public class ChoiceEntry {
        public string choiceText { get; set; }
        public ChoiceEntry(string choiceText) {
            this.choiceText = choiceText;
        }
    }
    public class QuestionEntry {

        //===========================================================================
        #region Variables
        //===========================================================================

        public string questionText { get; set; }
        List<ChoiceEntry> choices = new List<ChoiceEntry>();

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
        
        public ChoiceEntry GetChoiceAt(int index) {
            if (index >= 0 && index < choices.Count)
                return choices[index];
            return null;
        }

        public int GetChoiceCount() {
            return choices.Count;
        }

        public void AddNewChoice(string newChoice) {
            choices.Add(new ChoiceEntry(newChoice));
        }

        public void RemoveChoiceAt(int index) {
            if (index >= 0 && index < choices.Count)
                choices.RemoveAt(index);
        }

        public void EditChoiceAt(int index, string newChoice) {
            if (index >= 0 && index < choices.Count)
                choices[index] = new ChoiceEntry(newChoice);
        }
        #endregion
        //===========================================================================
    }
}
