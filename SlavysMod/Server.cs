using System.Text;
using System.Web;
using System.Net;
using System.Collections.Generic;
using System;
using System.IO;

namespace SlavysMod
{
    public class Server
    {

        public readonly string HostName = "localhost";
        public readonly int Port = 6969;

        private readonly HttpListener _listener = new HttpListener();
        private Queue<Commands> commandQueue = new Queue<Commands>();
        public void Start()
        {
            Console.WriteLine("Starting server at " + HostName + ":" + Port.ToString());
            _listener.Prefixes.Add("http://" + HostName + ":" + Port.ToString() + "/");
            _listener.Start();
            Receive();
        }

        public void Stop()
        {
            _listener.Stop();
            _listener.Close();
        }

        private void Receive()
        {
            _listener.BeginGetContext(new AsyncCallback(CallBackMethod), _listener);
        }

        private void CallBackMethod(IAsyncResult result)
        {
            if (_listener.IsListening)
            {
                HttpListenerContext context = _listener.EndGetContext(result);
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;
                string username = "";
                string responseString;

                Console.WriteLine("Request received: " + request.HttpMethod + " " + request.Url);
                Console.WriteLine("RawURL: " + request.RawUrl);

                // Handle command from POST
                if (request.HttpMethod == "POST" && request.HasEntityBody)
                {
                    // Read the body of the request and process the username passed as value1
                    var reader = new StreamReader(request.InputStream, request.ContentEncoding);
                    string requestBody = reader.ReadToEnd();
                    var formData = HttpUtility.ParseQueryString(requestBody);

                    if (formData["value1"] != null && formData["value1"] != "")
                    { 
                        username = formData["value1"];
                    }
                }

                // Ignore OPTIONS requests to avoid double requests
                if (request.HttpMethod == "OPTIONS")
                {
                    responseString = "Ignored Message";
                }
                else
                {
                    // Create and queue command
                    Commands currCmd = ProcessDataToCommand(request.RawUrl, username);
                    commandQueue.Enqueue(currCmd);

                    // Response when there is a valid command 
                    responseString = $"Command: {currCmd.command} received from: {username}";
                }
                
                // Build and send the response 
                byte[] buffer = Encoding.UTF8.GetBytes(responseString);
                response.ContentLength64 = buffer.Length;
                using (var output = response.OutputStream)
                {
                    output.Write(buffer, 0, buffer.Length);
                }

                // Continue listening for other requests
                Receive();
            }
        }

        // Creates the command object from the endpoint and username
        private Commands ProcessDataToCommand(string endpoint, string username)
        {
            Commands command = new Commands();
            command.username = username;
            command.command = endpoint.Split('/')[1];
            return command;
        }

        // Returns the next command in the queue
        public Commands GetCommand()
        {
            if (commandQueue.Count > 0) 
                return commandQueue.Dequeue();
            else 
                return null;
        }
    }
}
