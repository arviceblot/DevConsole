using UnityEngine;

/// <summary>
/// Some example commands that do more than return output.
/// </summary>
public class ExampleCommands : MonoBehaviour
{
    [SerializeField]
    private static bool godMode;

    [ConsoleCommand("tgm", "toggle god mode")]
    public static string HandleTgm(string[] args)
    {
        godMode = !godMode;
        return "god mode " + (godMode ? "on" : "off");
    }

    [ConsoleCommand("disable", "disables the active object")]
    public static string HandleDisable(string[] args)
    {
        DevConsole.SelectedObject.SetActive(false);
        return "";
    }

    [ConsoleCommand("enable", "disables the active object")]
    public static string HandleEnable(string[] args)
    {
        DevConsole.SelectedObject.SetActive(true);
        return "";
    }
}
