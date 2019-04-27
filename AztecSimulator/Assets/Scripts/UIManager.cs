﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Creates and manages UI objects.
public class UIManager : MonoBehaviour {

	public GameObject uiPersonObject;

	private PersonManager mPersonManager;
	private List<GameObject> mUiPeoplePool;
	private int mMaxEventMessages = 10;
	private List<string> mEventMessages = new List<string>();

	// Use this for initialization
	void Start () {
		mPersonManager = Utilities.GetPersonManager();
		mUiPeoplePool = new List<GameObject>();
	}

	// TODO: use people-changed-listener instead of update
	void Update () {
		List<Person> people = mPersonManager.People;
		Transform peoplePanel = transform.Find("Left/People");
		for(int i = 0; i < Mathf.Max(people.Count, mUiPeoplePool.Count); i++)
		{
			GameObject uiPerson;
			// Spawn new UI person
			if (i >= mUiPeoplePool.Count) {
				uiPerson = Instantiate(uiPersonObject);
				mUiPeoplePool.Add(uiPerson);
				uiPerson.transform.SetParent(peoplePanel);
			} else {
				uiPerson = mUiPeoplePool[i];
			}
			// Update position
			RectTransform rt = uiPerson.GetComponent<RectTransform>();
			float verticalOffset = 0.05f * peoplePanel.GetComponent<RectTransform>().rect.height;
			rt.anchoredPosition = new Vector2(0f,verticalOffset-30f*(i-(people.Count-1)/2f));
			// Update visibility
			uiPerson.transform.gameObject.SetActive(i < people.Count);
			if (i < people.Count) {
				Text uiText = uiPerson.GetComponentInChildren<Text>();
				uiText.text = people[i].GetUIDescription(); // todo: get separate text elements so we can layout the info in a nicer way
			}
		}
		transform.Find("Top/PopulationText").GetComponent<Text>().text = "Population: " + people.Count;
		transform.Find("Top/ResourceText").GetComponent<Text>().text = "Food: " + GameState.FoodSupply;
	}

	private List<Person> getSelectedPeople() {
		List<Person> people = mPersonManager.People;
		List<Person> result = new List<Person>();
		for(int i = 0; i < Mathf.Min(people.Count, mUiPeoplePool.Count); i++)
		{
			GameObject uiPerson = mUiPeoplePool[i];
			Toggle selectedToggle = uiPerson.transform.GetComponentInChildren<Toggle>();
			if (selectedToggle.isOn) {
				result.Add(people[i]);
			}
		}
		return result;
	}

	public void OnSacrifice() {
		List<Person> selectedPeople = getSelectedPeople();
		Debug.Log("Sacrificing " + selectedPeople.Count + " people.");
		if (selectedPeople.Count == 0) { return; }
		string sacrificedNames = Utilities.ConcatStrings(selectedPeople.ConvertAll(person => person.Name));
		LogEvent("You sacrifice " + sacrificedNames + " to the god.");
        
		God god = Utilities.GetGod();
		if(god != null) {
			god.MakeSacrifice(0, selectedPeople);
        }

		for(int i = 0; i < mUiPeoplePool.Count; i++)
		{
			GameObject uiPerson = mUiPeoplePool[i];
			Toggle selectedToggle = uiPerson.transform.GetComponentInChildren<Toggle>();
			selectedToggle.isOn = false;
		}
	}

	public void LogEvent(string message) {
		mEventMessages.Add(message);
		string newLogText = "";
		// Concatenate the last K messages
		for (int i = Mathf.Max(0, mEventMessages.Count-mMaxEventMessages); i < mEventMessages.Count; i++) {
			newLogText += mEventMessages[i] + "\n";
		}
		transform.Find("Right/Log/LogText").GetComponent<Text>().text = newLogText;
	}
}