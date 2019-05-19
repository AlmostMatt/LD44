﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonManager : MonoBehaviour {

	public class SpawnPersonRecord
	{
		public Person.Attribute attr = Person.Attribute.NONE;
		public int level = -1;
	}

	public static int MAX_POPULATION = 10;

	public int numStartingPeople = 8;
	public static float birthInterval = 20;

	private List<Person> mPeople;

	public List<Person> People {
		get { return(mPeople); }
	}

	private float mRepopulateTimer;

	private List<GameObject> mPeopleChangedListeners;

	// Use this for initialization
	void Start () {

		mPeople = new List<Person>();
		List<SpawnPersonRecord> startingPeople = GameState.Scenario.GetStartingPeople();
		foreach(SpawnPersonRecord record in startingPeople)
		{
			SpawnPerson(record);
		}

		mPeopleChangedListeners = new List<GameObject>();

		mRepopulateTimer = birthInterval;
	}
	
	// Update is called once per frame
	void Update () {

		List<Person> civilians = FindPeople(Person.AttributeType.PROFESSION, Person.Attribute.CIVILIAN);
		float birthRate = 0f;
		foreach (Person person in civilians) {
			birthRate += person.Efficiency;
		}
		if(GameState.HasBoon(BoonType.SURPLUS_FOOD_TO_BIRTHRATE))
		{
			float birthrateIncrease = GameState.GetBoonValue(BoonType.SURPLUS_FOOD_TO_BIRTHRATE) / 100f;
			birthRate += birthrateIncrease * GameState.FoodSurplus;
		}
		birthRate += (GameState.GetBoonValue(BoonType.BONUS_FERTILITY) / 100f);

		if(mRepopulateTimer > 0)
		{
			mRepopulateTimer -= GameState.GameDeltaTime * birthRate;
		}
		else
		{
			if (mPeople.Count < MAX_POPULATION) {
				Person p = SpawnPerson();
				Debug.Log(p.Name + " was born!");
				Utilities.LogEvent(p.Name + " was born!", 1f);
			}
			mRepopulateTimer = birthInterval;
		}
		GameState.TimeBetweenBirths = (birthInterval / birthRate);
		GameState.TimeUntilBirth = (mRepopulateTimer / birthRate);

		// Update ArmySize
		float armyStrength = 0;
		List<Person> warriors = FindPeople(Person.AttributeType.PROFESSION, Person.Attribute.WARRIOR);
		foreach(Person p in warriors) {
			armyStrength += p.Efficiency;
		}

		if(GameState.HasBoon(BoonType.WARRIOR_CHILD_PROTECT))
		{
			int childAge = GameState.GetBoonValue(BoonType.WARRIOR_CHILD_PROTECT_AGE); 
			float effectiveness = GameState.GetBoonValue(BoonType.WARRIOR_CHILD_PROTECT) / 100f;
			List<Person> children = mPeople.FindAll(x => x.Age <= childAge);
			if(children != null)
			{
				armyStrength += warriors.Count * children.Count * effectiveness;
			}
		}

		GameState.ArmyStrength = (int)Mathf.Floor(armyStrength);
	}

	private Person SpawnPerson(SpawnPersonRecord record = null)
	{
		GameObject go = new GameObject("Person");
		Person p = go.AddComponent<Person>();
		if(record != null)
		p.OverrideRandomValues(record.attr, record.level);
		mPeople.Add(p);
		return(p);
	}

	public void AddPeopleChangedListener(GameObject go)
	{
		mPeopleChangedListeners.Add(go);
	}

	public void RemovePerson(Person p)
	{
		mPeople.Remove(p);
		GameObject.Destroy(p.gameObject);
		PeopleChanged();
	}

	public void RemovePeople(List<Person> people)
	{
		foreach(Person p in people)
		{
			GameObject.Destroy(p.gameObject);
			mPeople.Remove(p);
		}

		PeopleChanged();
	}

	public List<Person> FindPeople(Person.AttributeType attrType, Person.Attribute attrValue)
	{
		List<Person> results = new List<Person>();
		foreach(Person p in mPeople) {
			if(p.GetAttribute(attrType) == attrValue) {
				results.Add(p);
			}
		}
		return(results);
	}

	private void PeopleChanged()
	{
		// not sure if these might dangle... careful about adding listeners that could be destroyed!
		foreach(GameObject go in mPeopleChangedListeners)
		{
			if(go != null)
			{
				go.BroadcastMessage("OnPeopleChanged");
			}
		}
	}
}
