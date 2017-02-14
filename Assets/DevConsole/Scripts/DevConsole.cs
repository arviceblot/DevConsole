using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DevConsole : MonoBehaviour
{
	public delegate string CommandHandler(string[] arguments);

	[SerializeField]
	private Text output;
	[SerializeField]
	private InputField inputField;

	private static Dictionary<string, CommandHandler> commands;

	private void Start ()
	{
		commands = new Dictionary<string, CommandHandler>();

		// setup input handler
		inputField.onEndEdit.AddListener(val => HandleInput(val));

		// register some default commands
		//Register(Help);

		Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
		foreach (var assembly in assemblies)
		{
			var methods = (from type in assembly.GetTypes()
					from method in type.GetMethods()
					where Attribute.IsDefined(method, typeof(ConsoleCommand))
					select (CommandHandler)Delegate.CreateDelegate(typeof(CommandHandler), method)).ToList();
			methods.ForEach(m => Register(m));
		}

		// register local handlers
		Register(HandleHelp);
	}
	
	public void Register(CommandHandler handler)
	{
		var attr = (ConsoleCommand)handler.Method.GetCustomAttributes(typeof(ConsoleCommand), true).First();
		try
		{
			commands.Add(attr.Command, handler);
		}
		catch (ArgumentException)
		{
			Debug.Log("Command " + attr.Command + " already exists");
		}
	}

	private void HandleInput(string val)
	{
		if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
		{
			output.text += "> " + val + "\n";
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
					output.text += "No command found with name '" + input.First() + "'\n";
				}
			}

			inputField.Select();
			inputField.ActivateInputField();
		}
	}

	[ConsoleCommand("help", "how to use this")]
	private static string HandleHelp(string[] args)
	{
		if (args != null && args.Length > 0)
		{
			CommandHandler command;
			// find the matching command
			if (commands.TryGetValue(args.First(), out command))
			{
				var attr = (ConsoleCommand)command.Method.GetCustomAttributes(typeof(ConsoleCommand), true).First();
				//var attr = Attribute.GetCustomAttribute(command, typeof(ConsoleCommand));
				return attr.Command + ":\n" + attr.Description;
			}
			else
			{
				return "No command found with name '" + args.First() + "'";
			}
		}
		else
		{
			return HandleHelp(new string[] { "help" });
		}
	}
}