using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleCommands : MonoBehaviour
{
	[SerializeField]
	private bool godMode;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	[ConsoleCommand("tgm", "toggle god mode")]
	public string HandleTgm(string[] args)
	{
		godMode = !godMode;
		return "";
	}
}
