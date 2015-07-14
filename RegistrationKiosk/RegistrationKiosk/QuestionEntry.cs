using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace RegistrationKiosk {

    // Choice Entry Class, used in QuestionEntry (Used for easier Datagrid syncing)
    public class ChoiceEntry {
        // Variable
        public string choiceText { get; set; }
        // Constructor
        public ChoiceEntry(string choiceText) {
            this.choiceText = choiceText;
        }
    }

    // Question Entry Class
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
        #region Methods
        //===========================================================================
        
        /// <summary>
        /// Gets the choice at a given index.
        /// </summary>
        /// <param name="index">Index of requested choice (0 based)</param>
        /// <returns>Returns requested ChoiceEntry, otherwise returns NULL</returns>
        public ChoiceEntry GetChoiceAt(int index) {
            if (index >= 0 && index < choices.Count)
                return choices[index];
            return null;
        }

        /// <summary>
        /// Returns the number of choices the question has
        /// </summary>
        /// <returns></returns>
        public int GetChoiceCount() {
            return choices.Count;
        }

        /// <summary>
        /// Adds a new choice to the question
        /// </summary>
        /// <param name="newChoice">Text for the choice</param>
        public void AddNewChoice(string newChoice) {
            choices.Add(new ChoiceEntry(newChoice));
        }

        /// <summary>
        /// Removes a choice at a given index
        /// </summary>
        /// <param name="index">Index to remove choice from (0 based)</param>
        public void RemoveChoiceAt(int index) {
            if (index >= 0 && index < choices.Count)
                choices.RemoveAt(index);
        }
        #endregion
        //===========================================================================
    }
}
