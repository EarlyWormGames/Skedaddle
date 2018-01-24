using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

// State object for receiving data from remote device.
public class StateObject
{
    // Client socket.
    public Socket workSocket = null;
    // Size of receive buffer.
    public const int BufferSize = 256;
    // Receive buffer.
    public byte[] buffer = new byte[BufferSize];
    // Received data string.
    public StringBuilder sb = new StringBuilder();
}

public class HeatmapClient
{
    public static bool Quitting = false;

    // The port number for the remote device.
    private const int port = 39976;

    // The response from the remote device.
    private static String response = String.Empty;
    private static Socket m_Client;


    //The current file being sent
    private static string m_CurrentFilePath = null;
    private static string m_MainFolder = "";

    public static void Init()
    {
        m_MainFolder = Application.persistentDataPath + "/TempHeatmaps/";
    }

    public static void StartClient(string a_address = "54.153.183.52")
    {
        if (m_Client != null)
        {
            if (m_Client.Connected)
            {
                // Release the socket.
                m_Client.Shutdown(SocketShutdown.Both);
                m_Client.Close();
            }
        }

        // Connect to a remote device.
        try
        {
            // Establish the remote endpoint for the socket.
            IPHostEntry ipHostInfo = Dns.GetHostEntry(a_address);
            IPAddress ipAddress = null;
            for (int i = 0; i < ipHostInfo.AddressList.Length; ++i)
            {
                if (ipHostInfo.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                    ipAddress = ipHostInfo.AddressList[i];
            }
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

            // Create a TCP/IP socket.
            m_Client = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

            // Connect to the remote endpoint.
            m_Client.BeginConnect(remoteEP,
                new AsyncCallback(ConnectCallback), m_Client);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    private static void ConnectCallback(IAsyncResult ar)
    {
        try
        {
            // Retrieve the socket from the state object.
            Socket client = (Socket)ar.AsyncState;

            // Complete the connection.
            client.EndConnect(ar);

            Console.WriteLine("Socket connected to {0}",
                client.RemoteEndPoint.ToString());

            //Start waiting for a response
            Receive();

            //Check for any files to send to the server
            CheckFiles();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    private static void Receive()
    {
        if (m_Client == null)
            return;
        if (!m_Client.Connected)
            return;

        try
        {
            // Create the state object.
            StateObject state = new StateObject();
            state.workSocket = m_Client;

            // Begin receiving the data from the remote device.
            m_Client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReceiveCallback), state);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    private static void ReceiveCallback(IAsyncResult ar)
    {
        try
        {
            // Retrieve the state object and the client socket 
            // from the asynchronous state object.
            StateObject state = (StateObject)ar.AsyncState;
            Socket client = state.workSocket;

            // Read data from the remote device.
            int bytesRead = client.EndReceive(ar);

            if (bytesRead > 0)
            {
                // There might be more data, so store the data received so far.
                state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                // Get the rest of the data.
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReceiveCallback), state);
            }
            else
            {
                // All the data has arrived; put it in response.
                if (state.sb.Length > 1)
                {
                    response = state.sb.ToString();
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    private static void Send(String data)
    {
        if (m_Client == null)
            return;
        if (!m_Client.Connected)
            return;

        data += "<EOF>";

        // Convert the string data to byte data using ASCII encoding.
        byte[] byteData = Encoding.ASCII.GetBytes(data);

        // Begin sending the data to the remote device.
        m_Client.BeginSend(byteData, 0, byteData.Length, 0,
            new AsyncCallback(SendCallback), m_Client);
    }

    private static void SendCallback(IAsyncResult ar)
    {
        try
        {
            // Retrieve the socket from the state object.
            Socket client = (Socket)ar.AsyncState;

            // Complete sending the data to the remote device.
            int bytesSent = client.EndSend(ar);
            Console.WriteLine("Sent {0} bytes to server.", bytesSent);

            if (m_CurrentFilePath != null)
            {
                File.SetAttributes(m_CurrentFilePath, FileAttributes.Normal);
                File.Delete(m_CurrentFilePath);
                m_CurrentFilePath = null;
                CheckFiles();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    public static void CheckFiles()
    {
        if (Quitting)
            return;

        if (m_Client == null)
        {
            StartClient();
            return;
        }
        else if (!m_Client.Connected)
        {
            StartClient();
            return;
        }

        if (m_CurrentFilePath != null)
            return;

        ProcessDirectory(m_MainFolder);
    }

    private static bool ProcessDirectory(string targetDirectory)
    {
        // Process the list of files found in the directory.
        string[] fileEntries = Directory.GetFiles(targetDirectory);
        foreach (string fileName in fileEntries)
        {
            m_CurrentFilePath = fileName;

            //Add the header and remove the data path and main directory from the path
            string data = "ADD\n" + targetDirectory.Replace(m_MainFolder, "") + "\n";

            //Append the contents of the file and send it to the server
            Send(data + File.ReadAllText(m_CurrentFilePath));
            return true;
        }

        // Recurse into subdirectories of this directory.
        string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
        foreach (string subdirectory in subdirectoryEntries)
        {
            if (ProcessDirectory(subdirectory))
                return true;
        }
        return false;
    }
}
