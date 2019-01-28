using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatController : MonoBehaviour {
	static ChatController sf;
	new Text name;
	static Text curText;
	ScrollRect scrollVert;
	ChatY curChat;
	static GameObject content, npc;
	static GameObject nextButton;
	public GameObject textPrefab;
	private const int MAX_CHARS = 36;
	private static NPCType type; private static int seqId;
	private static int maxLines = 0;
	private int pointer = 0, linePointer = 0;
	public static int line = 0;
	private char[] curLine;

	private bool selfV = false;
	private bool self {
		get { return selfV; }
		set {
			ChatCamera.Change ((value)?npc.transform:GManager.playerLocation.GetChild (0));
			selfV = value;
		}
	}
		
	public static bool start = false;

	// Use this for initialization
	void Awake () {
		sf = this;
		name = transform.GetChild (1).GetComponent<Text> ();
		scrollVert = transform.GetChild (2).GetComponent<ScrollRect> ();
		content = transform.GetChild (2).GetChild (0).GetChild (0).gameObject;
		nextButton = transform.GetChild (3).gameObject;
	}
		

	void OnEnable(){
		nextButton.SetActive (false);
		StartCoroutine (typer());
	}

	public static void setSeq(GameObject npcc, NPCType types, int seqIds){
		npc = npcc; type = types; seqId = seqIds;
		maxLines = GManager.npcChats [type.ToString ()].seqs [seqId].Length;
		start = true;
		sf.getChat ();
	}

	public void getChat(){
		curChat = GManager.npcChats [type.ToString ()].seqs [seqId] [line];
		self = curChat.me;
		nextButton.SetActive (false);
	}

	public void getLine(){
		string check = curChat.msg.Substring (linePointer);
		if (check.Length >= MAX_CHARS){
			check = check.Substring (0, MAX_CHARS);
		}
		curLine = check.ToCharArray ();
		linePointer += MAX_CHARS;
	}

	public static void Next(){
		Next (false);
	}

	public static void Next(bool forceClean){
		line++;
		sf.linePointer = 0;
		sf.pointer = 0;
		if (line >= maxLines || forceClean){
			line = 0;
			npc = null;
			maxLines = 0;
			start = false;
			GManager.self.UIChat.SendMessage ("show", false);
			ChatCamera.Change (sf.transform, true);
		} else {
			sf.getChat ();
		}
		Clean ();
	}

	public static void Clean(){
		if (content.transform.childCount > 0){
			for (int i = 0; i < content.transform.childCount; i++){
				Destroy(content.transform.GetChild (i).gameObject);
			}
		}
	}
	
	// Update is called once per frame
	IEnumerator typer(){
		if (start){
			if (curText == null){
				GameObject newText = Instantiate (textPrefab, Vector3.zero, Quaternion.identity, content.transform);
				Vector3 pos = newText.transform.localPosition;
				pos.z = 0;
				newText.transform.localPosition = pos;
				newText.transform.localRotation = Quaternion.identity;
				curText = newText.GetComponent<Text> ();
				getLine ();
			} else {
				bool doNextBu = false;
				if (curLine != null) {
					if (pointer < curLine.Length) {
						doNextBu = false;
						curText.text += curLine [pointer];
						scrollVert.verticalNormalizedPosition = 0f;
						pointer++;
						if (pointer >= MAX_CHARS) {
							curText.text += "-";
							pointer = 0;
							curText = null;
						}
					} else {
						doNextBu = true;
					}
				}
				if (doNextBu && curText !=null) {
					if (!nextButton.activeSelf) {
						linePointer = 0; pointer = 0; curLine = null;
						nextButton.SetActive (true);
					}
				}
			}
		}
		yield return new WaitForSeconds (.001f);
		if (start) {
			StartCoroutine (typer ());
		}
	}
}
