using System;
using System.Text;
using System.Windows.Forms;

namespace AndroidExtension
{
    public partial class AddEvent : Form
    {
        #region Variables

        /// <summary>
        /// Class 1 reference
        /// </summary>
        Class1 ctx;
        /// <summary>
        /// Dialog Mode for adding and event
        /// </summary>
        public static int MODE_ADD = 0;
        /// <summary>
        /// Dialog mode for editing an event
        /// </summary>
        public static int MODE_EDIT = 1;
        /// <summary>
        /// The current dialog mode
        /// </summary>
        private int _dialogMode = -1;
        /// <summary>
        /// The event ID in case of editing mode
        /// </summary>
        private int eventID = 0;

        #endregion

        #region UI Functions

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">The Main Class1 object</param>
        /// <param name="dialogMode">The dialog mode defined by the static vars</param>

        public AddEvent(Class1 context, int dialogMode)
        {
            InitializeComponent(); //Load controls
            ctx = context; //Set context
            _dialogMode = dialogMode; //Set dialog mode
        }

        #pragma warning disable IDE1006

        /// <summary>
        /// Cancel button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            Close(); //Close the form
        }

        /// <summary>
        /// Load event data if mode is edit
        /// </summary>
        /// <param name="data">The event data to load</param>
        public void LoadEvent(Class1.EventData data)
        {
            textBox1.Text = data.name;
            textBox2.Text = data.description;
            textBox3.Text = data.location;
            dateTimePicker1.Value = ToDateTime(data.startTime);
            dateTimePicker2.Value = ToDateTime(data.endTime);
            eventID = int.Parse(data.id);
        }

        /// <summary>
        /// Convert epoch to DateTime
        /// </summary>
        /// <param name="input">String epoch time input</param>
        /// <returns>The converted DateTime object</returns>
        private DateTime ToDateTime(string input)
        {
            DateTime startDate = new DateTime(1970, 1, 1, 0, 0, 0); //Get epoch starting date
            DateTime toDisplay = startDate.AddMilliseconds(long.Parse(input)); //Add millisecods from event data
            return toDisplay; //Return the resulting DateTime object
        }

        /// <summary>
        /// Action button edit or add event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            StringBuilder command = new StringBuilder();
            StringBuilder startTime = new StringBuilder();
            StringBuilder endTime = new StringBuilder();

            if (_dialogMode == MODE_ADD) command.Append("add-calendar|"); //Command
            if (_dialogMode == MODE_EDIT) command.Append("update-calendar|").Append(eventID).Append("|"); //Comamnd & event id to change
            command.Append(textBox1.Text).Append("|"); //Name
            command.Append(textBox2.Text).Append("|"); //Description
            command.Append(textBox3.Text).Append("|"); //Location

            startTime.Append(dateTimePicker1.Value.Year).Append(";") //Build start time
                .Append(dateTimePicker1.Value.Month).Append(";")
                .Append(dateTimePicker1.Value.Day).Append(";")
                .Append(dateTimePicker1.Value.Hour).Append(";")
                .Append(dateTimePicker1.Value.Minute).Append(";");

            endTime.Append(dateTimePicker2.Value.Year).Append(";") //Build end time
                .Append(dateTimePicker2.Value.Month).Append(";")
                .Append(dateTimePicker2.Value.Day).Append(";")
                .Append(dateTimePicker2.Value.Hour).Append(";")
                .Append(dateTimePicker2.Value.Minute).Append(";");

            command.Append(startTime.ToString()).Append("|"); //Start time
            command.Append(endTime.ToString()); //End time

            ctx.SendCommand(command.ToString()); //Send command
            Close(); //Close dialog
        }

        /// <summary>
        /// Form shown event, interact with graphical elements
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddEvent_Shown(object sender, EventArgs e)
        {
            if (_dialogMode == MODE_EDIT) //if in editing mode
            {
                button1.Text = "Edit Event"; //Change button text
            }
        }

        #endregion
    }
}