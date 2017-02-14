using System;

/// <summary>
/// See how one man adds command metadata to static methods with this one easy trick. Programmers hate him!
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class ConsoleCommand : Attribute
{
    /// <summary>
    /// The command the user will type to issue this command.
    /// </summary>
    private string command;

    /// <summary>
    /// A nice descriptoin of what this command actually does so you users can figure out how to use it.
    /// </summary>
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