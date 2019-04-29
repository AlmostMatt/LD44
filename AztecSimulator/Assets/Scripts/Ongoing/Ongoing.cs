﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ongoing
{
	public string mIconName;
	public string mTitle;
	public string mDescription;
	public float mDuration;
	public bool mIsGood; // TODO: generalize to good/bad/neutral

	public Ongoing (string iconName, string title, string description, float duration, bool isGood)
	{
		mIconName = iconName;
		mTitle = title;
		mDescription = description;
		mDuration = duration;
		mIsGood = isGood;
	}

	public void RenderTo(GameObject uiOngoing) {
		// TODO: update text, icon, timer, background color, etc
		//uiOngoing.transform.Find("Icon").GetComponent<Image>().sprite;
		uiOngoing.transform.Find("VGroup/Text1").GetComponent<Text>().text = mTitle;
		uiOngoing.transform.Find("VGroup/Text2").GetComponent<Text>().text = mDuration.ToString();
	}
}
