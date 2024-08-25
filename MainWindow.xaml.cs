
using FellowOakDicom;
using FellowOakDicom.Network;
using FellowOakDicom.Network.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.Data.SqlClient;
using System.Collections;
using System.Configuration;

namespace Files2Dicom_Test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

/*
        private string serverIp = "192.168.1.114";   // Replace with the DICOM server IP address
        private int serverPort = 4242;            // Replace with the DICOM server port
        private string callingAeTitle = "MYAET"; // Replace with your AE Title
        private string calledAeTitle = "SERVER"; // Replace with the server's AE Title
*/
        private string serverIp ;   // Replace with the DICOM server IP address
        private int serverPort;            // Replace with the DICOM server port
        private string callingAeTitle; // Replace with your AE Title
        private string calledAeTitle; // Replace with the server's AE Title

        private int filecount = 0;
        private int nonDicomFileCount= 0;
        private int dicomFileCount= 0;
        private int dicomStoreOkCount= 0;
        private int dicomStorErrorCount = 0;
        private DateTime startTime;
        private DateTime endTime;

        //private string connectionString = "Server=mssql;Database=dicomImport;Integrated Security=true;TrustServerCertificate=True;";
        private string connectionString;
        private SqlConnection connection;
        


        public MainWindow()
        {
            InitializeComponent();

            connectionString = ConfigurationManager.AppSettings["DBconnectionString"];
            serverIp = ConfigurationManager.AppSettings["serverIp"];
            serverPort = int.Parse(ConfigurationManager.AppSettings["serverPort"]);
            callingAeTitle = ConfigurationManager.AppSettings["callingAeTitle"];
            calledAeTitle = ConfigurationManager.AppSettings["calledAeTitle"];

            connection = new SqlConnection(connectionString);

            try
            {
                connection.Open();
                UpdateTextBox("Connection to database established successfully.");

            }
            catch (Exception e)
            {
                UpdateTextBox("Connection to database error: " + e.Message );
                throw;
            }
            
        }



        private async void btnStartFileScan_Click(object sender, RoutedEventArgs e)
        {
            
            tbFileCount.Text = filecount.ToString();
            tbNonDicomFiles.Text = nonDicomFileCount.ToString();



            UpdateTextBox("running DICOM Echo...");
            string successEcho = await SendDicomEcho(serverIp, serverPort, callingAeTitle, calledAeTitle);
            //UpdateTextBox($"DICOM Echo {(successEcho ? "succeeded" : "failed")}");
            UpdateTextBox("DICOM Echo result: " + successEcho );

            // await Task.Delay(5000);

            if (successEcho != "Success")
            {
                UpdateTextBox("Failed DICOM Echo - stopping task ");
            } else
            {
                // Call the asynchronous method and wait for it to complete
                // await PerformAsyncFileScan();
                startTime = DateTime.Now;
                UpdateTextBox("starting: " + startTime.ToString());
                await StartFileScanAsync();
                endTime = DateTime.Now;
                UpdateTextBox("finished: " + endTime.ToString());
                TimeSpan timeDifference = endTime - startTime;
                UpdateTextBox($"Hours: {timeDifference.Hours}, Minutes: {timeDifference.Minutes}, Seconds: {timeDifference.Seconds}, Milliseconds: {timeDifference.Milliseconds}");
                UpdateTextBox("files processed: " + filecount.ToString());
            }

            
        }


        private CancellationTokenSource _cancellationTokenSource;
        private PauseTokenSource _pauseTokenSource;

        public async Task StartFileScanAsync()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _pauseTokenSource = new PauseTokenSource();

            try
            {
                await PerformAsyncFileScan(_cancellationTokenSource.Token, _pauseTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Operation was canceled.");
            }
        }

        public void StopFileScan()
        {
            UpdateTextBox("stopping opperation...");
            _cancellationTokenSource?.Cancel();
            UpdateTextBox("stopping stopped");
        }

        public void PauseFileScan()
        {
            UpdateTextBox("pausing opperation...");
            _pauseTokenSource?.Pause();
            UpdateTextBox("paused...");
        }

        public void ResumeFileScan()
        {
            UpdateTextBox("resuming opperation...");
            _pauseTokenSource?.Resume();
        }
        private async Task PerformAsyncFileScan(CancellationToken cancellationToken, PauseToken pauseToken)
        {

        

            btnStartScan.IsEnabled = false;
            UpdateTextBox("PerformAsyncTask started.");

            // Simulate a task that takes time to complete (e.g., an I/O operation)
            //await Task.Delay(1000); // Delays for 1 seconds

            string directoryPath = @"Z:\JoJo\DicomTestDaten"; // Specify your directory path here

            try
            {
                // Get all files recursively in the directory
                string[] filePaths = Directory.GetFiles(directoryPath, "*.*", SearchOption.AllDirectories);

                // Loop through and print each file path
                foreach (string filePath in filePaths)
                {

                    filecount++;
                    // Check if cancellation is requested
                    if (cancellationToken.IsCancellationRequested)
                    {
                        UpdateTextBox("File scan canceled.");
                        break;
                    }
                    // Wait if paused
                    await pauseToken.WaitWhilePausedAsync();


                    // test if file was already processed
                    string queryFileExists = "select filepath from files where filepath = @filepath;";

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        SqlCommand commandFileExists = new SqlCommand(queryFileExists, connection);
                        commandFileExists.Parameters.AddWithValue("@filePath", filePath.ToString());

                        connection.Open();
                         SqlDataReader reader = commandFileExists.ExecuteReader();

                        bool fileAlreadyExists = false;
                        // toDo : Non-DICOM Files kommen noch nicht in DB
                        // if the file aleready exists in db
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                // Process the result (e.g., read the columns)
                               // UpdateTextBox($"File Path: {reader["FilePath"]}");
                                fileAlreadyExists = true;
                                // You can access other columns in the result using reader["ColumnName"]
                            }
                        }
                        else
                        {
                            fileAlreadyExists = false;
                        }

                        reader.Close();


                        // ToDo: mache bool check und  führe dann aus wenn datei nicht gefunden wurde
                        if (fileAlreadyExists == true)
                        {
                            //nothing
                            UpdateTextBox("File already found: filePath");
                        } else
                        {

                        

                           // UpdateTextBox("File path does not exist in the table.");
                            // Get the file size
                            FileInfo fileInfo = new FileInfo(filePath);
                            long fileSizeInBytes = fileInfo.Length;

                            //await Task.Delay(500); // Delays 
                            bool isValidDicom = IsDicomFile(filePath);
                            
                            if (isValidDicom)
                            {

                                dicomFileCount++;
                                UpdateTextBox("valid DICOM File: " + filePath);
                              //  UpdateStats();
                                bool successStore = await SendDicomFile(serverIp, serverPort, callingAeTitle, calledAeTitle, filePath, fileSizeInBytes.ToString());
                                //UpdateTextBox($"C-STORE {(successStore ? "succeeded" : "failed")}");



                            }
                            else
                            {
                                nonDicomFileCount++;
                              //  UpdateStats();
                                // SQL query to insert data into the Employees table
                                string query = "INSERT INTO files (filepath,fileSizeInBytes,error) VALUES (@filepath,@fileSizeInBytes,'no-dicom-file')";

                                UpdateTextBox("C-STORE response: Success");
                                try
                                {


                                    // Create a SqlCommand object
                                    using (SqlCommand command = new SqlCommand(query, connection))
                                    {
                                        // Add parameters to the command
                                        command.Parameters.AddWithValue("@filepath", filePath);
                                        command.Parameters.AddWithValue("@fileSizeInBytes", fileSizeInBytes.ToString());

                                        // Execute the INSERT command
                                        int rowsAffected = command.ExecuteNonQuery();
                                        //UpdateTextBox($"Rows inserted: {rowsAffected}");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    UpdateTextBox($"An error occurred on SQL insert: {query} {ex.Message}");
                                }
                            }

                        }

                    }

                 
                    
                }
            }
            catch (Exception ex)
            {
                
                UpdateTextBox("An error occurred: " + ex.Message);
            }
            UpdateTextBox("PerformAsyncTask finished.");
            btnStartScan.IsEnabled = true;
        }

        private  void UpdateStats()
        {
            if (Dispatcher.CheckAccess())
            {
                // If we are on the UI thread, directly update the TextBox
                //tbStatus.Text += Environment.NewLine + "UI-Thread: " + text;                
                 tbFileCount.Text = filecount.ToString();
                tbNonDicomFiles.Text = nonDicomFileCount.ToString();
                
            }
        }


        private void UpdateTextBox(string message)
        {
            if (!tbStatus.Dispatcher.CheckAccess()) // Check if the call needs to be marshaled to the UI thread
            {
                tbStatus.Dispatcher.Invoke(() => UpdateTextBox(message));
            }
            else
            {
                // reset to avoid high memory
                if (tbStatus.LineCount >= 50)
                {
                    tbStatus.Text = "";
                }
                tbStatus.AppendText(message + Environment.NewLine); // Update the UI
                tbStatus.ScrollToEnd();
            }
        }

       
        private  bool IsDicomFile(string filePath)
        {
            try
            {
                // Attempt to open the file as a DICOM file
                var dicomFile = DicomFile.Open(filePath);

                // If the file is successfully opened and parsed, it's a valid DICOM file
                return dicomFile != null;
            }
            catch (DicomFileException)
            {
                // If there's an exception, it means the file is not a valid DICOM file
                return false;
            }
            catch (Exception ex)
            {
                UpdateTextBox($"An error occurred: {ex.Message}");
                return false;
            }
        }



        private  async Task<string> SendDicomEcho(string serverIp, int serverPort, string callingAeTitle, string calledAeTitle)
        {
            try
            {
                // Create a DicomClient instance with default settings
                var client = DicomClientFactory.Create(serverIp, serverPort, false, callingAeTitle, calledAeTitle);

                // Create a C-ECHO request
                var echoRequest = new DicomCEchoRequest();
                string resultText = "";
                // Handle the response from the server
                echoRequest.OnResponseReceived += (req, res) =>
                {
                    if (res.Status == DicomStatus.Success)
                    {
                        UpdateTextBox("C-ECHO response: Success");
                        resultText = "C-ECHO response: Success";
                    }
                    else
                    {
                        resultText = $"C-ECHO response: Failure, Status: {res.Status}";
                        UpdateTextBox($"C-ECHO response: Failure, Status: {res.Status}");
                        
                    }
                };

                // Add the request to the client
                await client.AddRequestAsync(echoRequest);

                // Send the request to the DICOM server
                await client.SendAsync();

                return resultText;
            }
            catch (Exception ex)
            {
                UpdateTextBox($"Error performing DICOM Echo: {ex.Message}");
                return "Error performing DICOM Echo: " + ex.Message;
            }
        }

     /*   private async void sendDicomFile(string filePath)
        {
            string serverIp = "192.168.1.114";   // Replace with the DICOM server IP address
            int serverPort = 4242;            // Replace with the DICOM server port
            string callingAeTitle = "MYAET"; // Replace with your AE Title
            string calledAeTitle = "SERVER"; // Replace with the server's AE Title
            //string filePath = @"C:\Path\To\Your\File.dcm"; // Replace with the path to your DICOM file

            bool success = await SendDicomFile(serverIp, serverPort, callingAeTitle, calledAeTitle, filePath);

            UpdateTextBox($"C-STORE {(success ? "succeeded" : "failed")}");

        }*/

        private  async Task<bool> SendDicomFile(string serverIp, int serverPort, string callingAeTitle, string calledAeTitle, string filePath, string fileSizeInBytes)
        {
            try
            {
                // Load the DICOM file
                var dicomFile = await DicomFile.OpenAsync(filePath);

                // Example: Accessing Patient's Name (Tag: (0010,0010))                
                string patName = dicomFile.Dataset.GetString(DicomTag.PatientName);
                string patBirthd = dicomFile.Dataset.GetString(DicomTag.PatientBirthDate);
                string institutionName  = dicomFile.Dataset.GetString(DicomTag.InstitutionName);


                // Create a DicomClient instance with default settings
                var client = DicomClientFactory.Create(serverIp, serverPort, false, callingAeTitle, calledAeTitle);

                // Create a C-STORE request for the loaded DICOM file
                var storeRequest = new DicomCStoreRequest(dicomFile);

                // Handle the response from the server
                storeRequest.OnResponseReceived += (req, res) =>
                {
                    if (res.Status == DicomStatus.Success)
                    {
                        // SQL query to insert data into the Employees table
                        string query = "INSERT INTO files (filepath,fileSizeInBytes, patname,patbirthd,institutionName,error) VALUES (@filepath,@fileSizeInBytes, @patName,@patBirthd,@institutionName,'0')";

                        UpdateTextBox("C-STORE response: Success");
                        try
                        {
                            

                            // Create a SqlCommand object
                            using (SqlCommand command = new SqlCommand(query, connection))
                            {
                                // Add parameters to the command
                                command.Parameters.AddWithValue("@filepath", filePath);
                                command.Parameters.AddWithValue("@fileSizeInBytes", fileSizeInBytes);                                
                                command.Parameters.AddWithValue("@patName", patName);
                                command.Parameters.AddWithValue("@patBirthd", patBirthd);
                                command.Parameters.AddWithValue("@institutionName", institutionName);

                                // Execute the INSERT command
                                int rowsAffected = command.ExecuteNonQuery();
                                UpdateTextBox($"Rows inserted: {rowsAffected}");
                            }
                        }
                        catch (Exception ex)
                        {
                            UpdateTextBox($"An error occurred on SQL insert: {query} {ex.Message}");
                        }
                    }
                    else
                    {
                        UpdateTextBox($"C-STORE response: Failure, Status: {res.Status}");
                        // SQL query to insert data into the Employees table
                        string query = "INSERT INTO files (filepath,fileSizeInBytes,error) VALUES (@filepath,@fileSizeInBytes,@error)";

                        // Create a SqlCommand object
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            // Add parameters to the command
                            command.Parameters.AddWithValue("@filepath", filePath);
                            command.Parameters.AddWithValue("@fileSizeInBytes", fileSizeInBytes);
                            command.Parameters.AddWithValue("@error", res.Status);
                            

                            // Execute the INSERT command
                            int rowsAffected = command.ExecuteNonQuery();
                            UpdateTextBox($"Rows inserted: {rowsAffected}");
                        }

                    }
                };

                // Add the request to the client
                await client.AddRequestAsync(storeRequest);

                // Send the request to the DICOM server
                await client.SendAsync();

                return true;
            }
            catch (Exception ex)
            {
                UpdateTextBox($"Error performing C-STORE: {ex.Message}");
                // SQL query to insert data into the Employees table
                string query = "INSERT INTO files (filepath,fileSizeInBytes,error) VALUES (@filepath,@fileSizeInBytes,@error)";

                // Create a SqlCommand object
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Add parameters to the command
                    command.Parameters.AddWithValue("@filepath", filePath);
                    command.Parameters.AddWithValue("@fileSizeInBytes", fileSizeInBytes);
                    command.Parameters.AddWithValue("@error", ex.Message);


                    // Execute the INSERT command
                    int rowsAffected = command.ExecuteNonQuery();
                    //UpdateTextBox($"Rows inserted: {rowsAffected}");
                }

                return false;
            }
        }

        private bool pauseActivated = false;
        private void btnPauseScan_Click(object sender, RoutedEventArgs e)
        {
            if (!pauseActivated) {
                    pauseActivated = true;
                    btnPauseScan.Content = "resume";
                PauseFileScan();

            }
            else {
                pauseActivated = false;
                btnPauseScan.Content = "pause";
                ResumeFileScan();

            }
        }

        private void btnStopScan_Click(object sender, RoutedEventArgs e)
        {
            StopFileScan();
            pauseActivated = false;
            btnPauseScan.Content = "pause";
        }
    }
}



public class PauseTokenSource
{
    private volatile TaskCompletionSource<bool> _paused;
    private readonly object _lockObject = new object();

    public PauseTokenSource()
    {
        _paused = new TaskCompletionSource<bool>();
        _paused.SetResult(true); // Start unpaused
    }

    public PauseToken Token => new PauseToken(this);

    public bool IsPaused => !_paused.Task.IsCompleted;

    public void Pause()
    {
        lock (_lockObject)
        {
            if (!_paused.Task.IsCompleted) return;
            _paused = new TaskCompletionSource<bool>();
        }
    }

    public void Resume()
    {
        lock (_lockObject)
        {
            if (_paused.Task.IsCompleted) return;
            _paused.SetResult(true);
        }
    }

    public Task WaitWhilePausedAsync()
    {
        return _paused.Task;
    }
}

public struct PauseToken
{
    private readonly PauseTokenSource _source;

    public PauseToken(PauseTokenSource source)
    {
        _source = source;
    }

    public bool IsPaused => _source?.IsPaused ?? false;

    public Task WaitWhilePausedAsync()
    {
        return IsPaused ? _source.WaitWhilePausedAsync() : Task.CompletedTask;
    }
}