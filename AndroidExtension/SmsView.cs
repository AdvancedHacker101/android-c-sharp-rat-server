using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AndroidExtension
{
    /// <summary>
    /// ReConstruct And Display SMS Conversations
    /// </summary>
    public partial class SmsView : Form
    {
        #region Variables

        /// <summary>
        /// The SMS messages to work with
        /// </summary>
        List<Class1.SmsData> smsMessages = new List<Class1.SmsData>();

        #endregion

        #region UI Functions

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="messages">The SMS Messages we want to display</param>
        public SmsView(List<Class1.SmsData> messages)
        {
            InitializeComponent(); //Load Controls
            smsMessages = messages; //Set the sms Messages
        }

        /// <summary>
        /// Load SMS Threads into the listView
        /// </summary>
        private void LoadThreads()
        {
            //Create threads variable
            Dictionary<int, string> threads = new Dictionary<int, string>();

            //Loop through messages
            foreach (Class1.SmsData sms in smsMessages)
            {
                //If thread not already added then add the thread and the remote phone number associated with it
                if (!threads.ContainsKey(sms.threadID)) threads.Add(sms.threadID, sms.phoneNumber);
            }

            //Loop thorugh the threads
            foreach (KeyValuePair<int, string> kvp in threads)
            {
                //Create new list view item
                ListViewItem lvi = new ListViewItem()
                {
                    Text = kvp.Key.ToString() //Set Thread ID
                };
                lvi.SubItems.Add(kvp.Value); //Set remote phone number
                listView1.Items.Add(lvi); //Add new item to listView
            }
        }

        /// <summary>
        /// Construc and display SMS conversation
        /// </summary>
        /// <param name="threadID">The threadID of the conversation</param>
        private void DisplayThread(int threadID)
        {
            //Store messages taht are in the selected thread
            List<Class1.SmsData> threadMessages = new List<Class1.SmsData>();

            //Loop through all messages
            foreach (Class1.SmsData sms in smsMessages)
            {
                if (sms.threadID == threadID) threadMessages.Add(sms); //If threadIDs match add it to threadMessages
            }

            //Sort messages in thread by date
            List<Class1.SmsData> sortedMessages = threadMessages.OrderBy(sms => long.Parse(sms.date)).ToList();
            richTextBox1.Clear(); //Clear the display

            //Loop through the sorted messages
            foreach (Class1.SmsData sms in sortedMessages)
            {
                //Construct sms messages
                StringBuilder line = new StringBuilder();
                if (sms.sent) line.Append("Device to " + sms.phoneNumber);
                else line.Append(sms.phoneNumber + " to Device");
                DateTime startDate = new DateTime(1970, 1, 1, 0, 0, 0);
                DateTime toDisplay = startDate.AddMilliseconds(long.Parse(sms.date));
                line.Append(" at " + toDisplay.ToString());
                line.Append(": " + sms.message + Environment.NewLine + Environment.NewLine);

                //Append message to the display
                richTextBox1.AppendText(line.ToString());
            }
        }

        #pragma warning disable IDE1006

        /// <summary>
        /// Selected Thread Changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1) //If we have a selected item
            {
                int selectedThread = int.Parse(listView1.SelectedItems[0].SubItems[0].Text); //Get the threadID of the selected item
                DisplayThread(selectedThread); //Load the selected thread
            }
        }

        /// <summary>
        /// Form Shown
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SmsView_Shown(object sender, EventArgs e)
        {
            LoadThreads(); //Load Messages by threadID
        }

        #endregion
    }
}
