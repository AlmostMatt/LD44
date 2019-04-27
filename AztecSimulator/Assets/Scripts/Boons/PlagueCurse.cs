﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlagueCurse : SacrificeResult {

	public PlagueCurse() : base("Plague", "SICKNESS")
	{

	}

	public override void DoEffect()
	{
		Debug.Log("A terrible plague befalls your people");
	}
}
