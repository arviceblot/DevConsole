using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AttributeUsage(AttributeTargets.Method)]
public class ConsoleCommand : Attribute
{
	private string command;
	private string description;

	public string Command
	{
		get { return command; }
	}

	public string Description
	{
		get { return description; }
	}

	public ConsoleCommand(string command, string description)
	{
		this.command = command;
		this.description = description;
	}
}