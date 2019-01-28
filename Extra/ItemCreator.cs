using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

#if (UNITY_EDITOR)
public class ItemCreator : EditorWindow {

	public ItemList items = new ItemList();

	[MenuItem("Window/Items")]
	static void Init(){
		EditorWindow.GetWindow (typeof(ItemCreator)).Show ();
	}

	void OnGUI(){
		if (items != null){
			SerializedObject serializedObject = new SerializedObject (this);
			SerializedProperty serializedProperty = serializedObject.FindProperty ("items");
			EditorGUILayout.PropertyField (serializedProperty, true);
			serializedObject.ApplyModifiedProperties ();

			if (GUILayout.Button("Save Items")){
				SaveItems ();
			}
		}
		if (GUILayout.Button("Load items")){
			LoadItems ();
		}
	}

	void OnSelectionChange(){
		Debug.Log(Selection.activeObject);
	}

	private void LoadItems(){
		string path = Application.streamingAssetsPath + "/items.json";
		if (File.Exists (path)){
			string dataAsJson = File.ReadAllText (path);
			items = JsonUtility.FromJson<ItemList> (dataAsJson);
		} else {
			items = new ItemList ();
		}
	}

	private void SaveItems(){
		string path = Application.streamingAssetsPath + "/items.json";
		string jsonAsData = string.Empty;
		//foreach (Item item in items.items) {
			jsonAsData += JsonUtility.ToJson (items);
		//}
		File.WriteAllText (path, jsonAsData);
	}
}
#endif