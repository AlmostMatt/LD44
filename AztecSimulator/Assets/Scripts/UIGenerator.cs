﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Generates UIPerson objects. Can be merged into PersonManager later
public class UIGenerator : MonoBehaviour {

	public GameObject uiPersonObject;

	private PersonManager mPersonManager;
	private List<GameObject> mUiPeoplePool;

	// Use this for initialization
	void Start () {
		mPersonManager = Utilities.GetPersonManager();
		mUiPeoplePool = new List<GameObject>();
	}

	// TODO: use people-changed-listener instead of update
	void Update () {
		List<Person> people = mPersonManager.People;
		for(int i = 0; i < Mathf.Max(people.Count, mUiPeoplePool.Count); i++)
		{
			GameObject uiPerson;
			// Spawn new UI person
			if (i >= mUiPeoplePool.Count) {
				uiPerson = Instantiate(uiPersonObject);
				mUiPeoplePool.Add(uiPerson);
				uiPerson.transform.SetParent(transform);
			} else {
				uiPerson = mUiPeoplePool[i];
			}
			// Update position
			RectTransform rt = uiPerson.GetComponent<RectTransform>();
			rt.anchoredPosition = new Vector2(0f,0f+30f*(i-people.Count/2));
			// Update visibility
			uiPerson.transform.gameObject.SetActive(i < people.Count);
			if (i < people.Count) {
				Text uiText = uiPerson.GetComponentInChildren<Text>();
				uiText.text = people[i].GetUIDescription();
			}
		}
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
}
