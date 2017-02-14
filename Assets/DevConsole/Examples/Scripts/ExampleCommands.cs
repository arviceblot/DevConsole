using UnityEngine;

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
}
