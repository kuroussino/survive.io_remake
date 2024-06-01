using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace CI.PowerConsole.Examples
{
    public class ExampleSceneManagerController : MonoBehaviour
    {
        public TMP_InputField LogMessageInputField;

        public void Start()
        {
            // Drag the PowerConsole prefab into your root scene

            // Initialise the console - make sure this is called once before trying to interact with it
            PowerConsole.Initialise();

            // Listen for any user entered command
            PowerConsole.CommandEntered += (s, e) =>
            {
                var enteredCommand = e.Command;
            };

            // Register a command with a descripton and two arguments
            PowerConsole.RegisterCommand(new CustomCommand()
            {
                Command = "start server",
                Description = "Starts a web server",
                Args = new List<CommandArgument>()
                {
                    new CommandArgument() { Name = "-p", Description = "Port number to host on" },
                    new CommandArgument() { Name = "-t", Description = "Title of the window" }
                },
                Callback = Command1Callback
            });
        }

        public void WriteLogMessage()
        {
            PowerConsole.Log(LogLevel.Trace, LogMessageInputField.text);
        }

        private void Command1Callback(CommandCallback callback)
        {
            // Raised when "start server" is entered into the terminal
            // Arguments are parsed and available in callback.Args
        }
    }
}