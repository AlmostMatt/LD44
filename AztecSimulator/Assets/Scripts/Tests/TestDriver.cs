﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDriver : MonoBehaviour {

	public GameObject godGameObject;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButtonUp("Submit"))
		{
			RunTests();
		}
	}

	private void RunTests()
	{
		God g = godGameObject.GetComponent<God>();
		PersonManager personMgr = Utilities.GetPersonManager();
		List<Person> people = new List<Person>();
		people.Add(personMgr.People[Random.Range(0, personMgr.People.Count)]);
		List<SacrificeResult> results = g.MakeSacrifice(g.Demands[0], people);
		foreach(SacrificeResult r in results)
		{
			Debug.Log("YOUR SACRIFICE YIELDED:");
			r.DebugPrint();
		}
	}
}
