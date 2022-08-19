using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using rnd = UnityEngine.Random;
using KModkit;

public class BookOfMario : MonoBehaviour
{
	public KMAudio Audio;
	public KMBombInfo bomb;

	public KMSelectable[] buttons;
	public TextMesh[] btnTxts;
	public TextMesh DisplayText;
	public SpriteRenderer stageShroom;
	public SpriteRenderer[] buttonSprites;
	public Sprite[] shrooms;
	public Sprite[] genBtnSprites;
	private int stage = 1;
	int x, y, z, t;
	private List<int> btnIndex = new List<int>();
	private List<string> check = new List<string>();
	private static int _moduleCount;

	// Log stuff
	static int moduleIdCounter = 1;
	int moduleId;
	private bool moduleSolved = false;

	private static string[][] quotes = new string[13][]
	{
		new string[6] { "Belda", "Do carrots. I do not\nhave it. Censorship!", "Roasted bread,\ncharacter! Do not \ndeal with this unusual\noutlook!", "Viviana! You are poop!\nConcern!\nShadow shadow!", "Where am I now?\nYou! Viviana!\nDon’t tell me that you\nlost our baby!", "So why don’t you go\nsee me and Lyn doing\na good trap?" },
		new string[4] { "Bob", "HUP! HUP!\nWOT WOT?!?\nYobie boobs...PIRATES!", "Let's get cancer, Mario...", "Now who do you think\nI'm gonna do?" },
		new string[4] { "God\nBrowser", "Bol.", "Could it be? Oh, yes!\nChristians! Benefits,\nBrowser!", "Stop talking with\npuzzles, chicken sperm" },
		new string[7] { "Carbon", "People said I cried...", "Man, that's a salad...", "I came here to eat\nHoko Saba and then\ndestroy our city.", "We have chestnuts.", "But the truth is,\nI’m gonna end my\nfather. Him. All.", "You will regret\neverything!" },
		new string[4] { "Flavio", "Everything went well...\nI'm still full of terrorism...", "Altough we are\nreturning to Rogueport,\nwe are here to kill...", "My womb is brown" },
		new string[5] { "Goombell", "Dark Koopatrol.\nThese people just blow\nhard,\ndon’t you think?", "I came, Mario! You finna", "Absolutely, I came!\nGot it!", "Well, I’m so desperate,\nso you better save\nme..." },
		new string[7] { "Prosecutor\nGrubba", "That’s a ghost!\nYou talk a little. I came.", "He drowned in the\nwomb until it was\ntoo late for a walk!\nHoo!", "Now take me and\nshow me the pills!", "Listen, son:\nAt this point, I want\nyou to pull it out.\nDo...not...forget...", "Now I’m afraid you\nknow my big deal...\nI have a bed.", "He... found my secret...\nMy secret... pumpkin..." },
		new string[4] { "Make", "Give me your face,\nPETA!", "This is the Flirting\nCouch! Yes! Let's put\nit back!", "90 breasts was a\npleasure! Now it is 101!" },
		new string[5] { "Mario", "I came, you can go!", "Piss off, nerd!", "No, he suffers\nsuffocation", "Damn circus!" },
		new string[6] { "Mr. Krump", "Everyone...\nSyphillis, sign up!", "Dude, you're a crap!\nSmall details, right\nAnd you came, did you?", "Do not think it would\nbe better, like my\ndaughter, that little\npremature death...", "'Oh, do you say the\nstar of the glass,\nMr Krump?' BOOM!\nDirectly online.", "I call it the\nLIFE DETECTOR with\nLOVE PUMP." },
		new string[6] { "Prince\nPeach", "That thing is sick", "In other words, I came!", "If you like me, I will do\nmy best to create\nproblems for you.", "For all of you, the world\nwill be facing a\nterrible darkness.", "Love... How can I\nexplain it? Love tells\nyou when you’re not\ndone yet." },
		new string[4] { "Quiz\nThwomb", "HEY, women and\ninsects! Welcome, \neveryone,\nlet's go Super Quiz\nSuper 65 Quiz!", "Excellent headline\nopportunity!\nMMM HELLO HA HA!", "Bad is not right!\nDumbass!" },
		new string[3] { "Yoshi Kid", "Whoa! Free cake in\nthe country!\nLet's be calm.", "Pigs can not be\non the road!" }
	};

	void Awake()
	{
		_moduleCount = 0;
		moduleId = moduleIdCounter++;
		foreach(KMSelectable button in buttons)
		{
			KMSelectable pressedButton = button;
			button.OnInteract += delegate () { ButtonPress(pressedButton); return false; };
		}
		DisplayText.text = "Welcome to Book of Mario";
	}

	void Start()
	{
		_moduleCount++;
		if(_moduleCount == 1)
			Audio.PlaySoundAtTransform("BoM_Start", transform);
		ActivateModule();
	}

	void ActivateModule()
	{
		stageShroom.sprite = shrooms[stage - 1];
		x = rnd.Range(0, 13);
		y = rnd.Range(0, quotes[x].Length);
		DisplayText.text = quotes[x][y];
		Debug.LogFormat("[Book of Mario #{0}] The chosen quote for stage {1} is '{2}'", moduleId, stage, Regex.Replace(quotes[x][y], @"\n", " "));
		int ansBtn = rnd.Range(0, 4);
		btnTxts[ansBtn].text = quotes[x][0];
		Debug.LogFormat("[Book of Mario #{0}] The quote for stage {1} came from '{2}'", moduleId, stage, Regex.Replace(quotes[x][0], @"\n", " "));
		buttonSprites[ansBtn].sprite = genBtnSprites[x];
		btnIndex.Add(ansBtn);
		check.Add(quotes[x][0]);
		for(int i = 0; i < 3; i++)
		{
			do
			{
				z = rnd.Range(0, 13);
				t = rnd.Range(0, 4);
			} while (btnIndex.Contains(t) || check.Contains(quotes[z][0]));
			buttonSprites[t].sprite = genBtnSprites[z];
			btnIndex.Add(t);
			btnTxts[t].text = quotes[z][0];
			check.Add(quotes[z][0]);
		}
		btnIndex.Clear();
		check.Clear();
	}

	void ButtonPress(KMSelectable button)
	{
		button.AddInteractionPunch();
		Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, button.transform);
		if(moduleSolved)
		{
			return;
		}
		if(button.GetComponentInChildren<TextMesh>().text == quotes[x][0])
		{
			++stage;
			if(stage == 4)
				Solve();
			else
				ActivateModule();
		}
		else
		{
			GetComponent<KMBombModule>().HandleStrike();
			stage = 1;
			ActivateModule();
		}
	}

	void Solve()
	{
		moduleSolved = true;
		GetComponent<KMBombModule>().HandlePass();
		Audio.PlaySoundAtTransform("BoM_End", transform);
		DisplayText.text = "Many thanks to those\nwho play Mario:\nThousands of gates";
		for(int i = 0; i < 4; i++)
		{
			btnTxts[i].text = "";
			buttonSprites[i].sprite = genBtnSprites[13];
		}
	}

	#pragma warning disable 414
   		private readonly string TwitchHelpMessage = "Use “!{0} tr br tl bl” to press the corresponding button.";
	#pragma warning restore 414

	IEnumerator ProcessTwitchCommand(string command)
	{
        command = command.ToLower().Trim();

        switch (command) {
            case "tr":
            case "topright":
            case "topr":
            case "top right":
                yield return null;
                ButtonPress(buttons[1]);
                break;
            case "tl":
            case "topleft":
            case "topl":
            case "top left":
                yield return null;
                ButtonPress(buttons[0]);
                break;
            case "br":
            case "bottomright":
            case "bottomr":
            case "bottom right":
                yield return null;
                ButtonPress(buttons[3]);
                break;
            case "bl":
            case "bottomleft":
            case "bottoml":
            case "bottom left":
                yield return null;
                ButtonPress(buttons[2]);
                break;
            default:
                yield return "sendtochaterror Invalid command, you idiot. Try again.";
				break;
        }
    }
}
