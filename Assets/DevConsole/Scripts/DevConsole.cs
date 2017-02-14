using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Main controller for the console.
/// </summary>
public class DevConsole : MonoBehaviour
{
    public delegate string CommandHandler(string[] arguments);

    [SerializeField]
    private Text output;
    [SerializeField]
    private InputField inputField;
    [SerializeField]
    private Text consoleLabel;
    [SerializeField]
    private RectTransform cursorPanel;
    [SerializeField]
    private Text cursorText;

    /// <summary>
    /// The selected gameObject.
    /// </summary>
    private static GameObject selected;

    /// <summary>
    /// The gameObject the cursor is currently over.
    /// </summary>
    private static GameObject cursorOver;

    /// <summary>
    /// How we store all the commands.
    /// </summary>
    private static Dictionary<string, CommandHandler> commands;

    public static GameObject SelectedObject
    {
        get { return selected; }
    }

    #region MonoBehaviour

    private void Start()
    {
        commands = new Dictionary<string, CommandHandler>();

        // setup input handler
        inputField.onEndEdit.AddListener(val => HandleInput(val));

        // register commands
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            var methods = (from type in assembly.GetTypes()
                           from method in type.GetMethods()
                           where method.IsDefined(typeof(ConsoleCommand), true)
                           select (CommandHandler)Delegate.CreateDelegate(typeof(CommandHandler), method)).ToList();
            methods.ForEach(m => Register(m));
        }
    }

    private void Update()
    {
        // check for toggle key

        // update cursor panel
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            var hitObject = hit.transform.root.name;
            cursorText.text = hitObject;
            cursorOver = hit.transform.root.gameObject;
            cursorPanel.position = Input.mousePosition + new Vector3(40, 0);
            cursorPanel.gameObject.SetActive(true);
        }
        else
        {
            cursorPanel.gameObject.SetActive(false);
            cursorOver = null;
        }

        // checkout for click
        if (Input.GetMouseButtonDown(0))
        {
            if (cursorOver != null)
            {
                selected = cursorOver;
                consoleLabel.text = "DevConsole - " + selected.name;
            }
            else
            {
                consoleLabel.text = "DevConsole";
            }
        }

        // keep focus active
        inputField.Select();
        inputField.ActivateInputField();
    }

    #endregion

    /// <summary>
    /// Registers a command handler for a specific command.
    /// </summary>
    /// <param name="handler">
    /// The command handler.
    /// </param>
    public void Register(CommandHandler handler)
    {
        var attr = GetConsoleCommand(handler);
        try
        {
            commands.Add(attr.Command, handler);
        }
        catch (ArgumentException)
        {
            Debug.Log("Command " + attr.Command + " already exists");
        }
    }

    /// <summary>
    /// Handler user input to the console.
    /// </summary>
    /// <param name="val">
    /// The input string.
    /// </param>
    private void HandleInput(string val)
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            // mirror input
            output.text += "> " + val + "\n";

            // clear input
            inputField.text = "";

            // find the command
            var input = val.Split(null);
            if (input != null)
            {
                CommandHandler command;

                // try to get the command
                if (commands.TryGetValue(input.First(), out command))
                {
                    // call the command
                    output.text += command(input.Skip(1).ToArray()) + '\n';
                }
                else
                {
                    output.text += CommandNotFound(input.First()) + "\n";
                }
            }
        }
    }

    /// <summary>
    /// Handles the 'help' command.
    /// </summary>
    /// <param name="args">
    /// Other arguments.
    /// </param>
    /// <returns>
    /// The output string.
    /// </returns>
    [ConsoleCommand("help", "how to use this")]
    public static string HandleHelp(string[] args)
    {
        if (args != null && args.Length > 0)
        {
            CommandHandler command;
            // find the matching command
            if (commands.TryGetValue(args.First(), out command))
            {
                var attr = GetConsoleCommand(command);
                return attr.Command + ":\n" + attr.Description;
            }
            else
            {
                return CommandNotFound(args.First());
            }
        }
        else
        {
            return HandleHelp(new string[] { "help" });
        }
    }

    /// <summary>
    /// Build an error message for when we can't find the specified command.
    /// </summary>
    /// <param name="command">
    /// The spooky unfound command.
    /// </param>
    /// <returns>
    /// The error message.
    /// </returns>
    private static string CommandNotFound(string command)
    {
        return "No command found with name '" + command + "'";
    }

    /// <summary>
    /// Get the ConsoleComand attribute from a CommandHandler.
    /// </summary>
    /// <param name="handler">
    /// The command handler.
    /// </param>
    /// <returns>
    /// The console command attribute.
    /// </returns>
    private static ConsoleCommand GetConsoleCommand(CommandHandler handler)
    {
        return (ConsoleCommand)handler.Method.GetCustomAttributes(typeof(ConsoleCommand), true).First();
    }
}