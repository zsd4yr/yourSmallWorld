﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FollowPath))]
public class SmolMan : MonoBehaviour {

	bool resourceActive = false;

	Community comm;

	SphereTerrain st;

	// Use this for initialization
	void Awake () {
		st = GameObject.FindObjectOfType(typeof(SphereTerrain)) as SphereTerrain;
		comm = GameObject.FindObjectOfType(typeof(Community)) as Community;
		GetComponent<FollowPath>().start = st.getVertex(st.findIndexOfNearest(comm.gameObject.transform.position));
		GetComponent<FollowPath>().targetGoal = st.getVertex(st.findIndexOfNearest(comm.gameObject.transform.position));
		if (comm == null) {
			Debug.LogError("Community does not exist!");
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void setResource(Vertex res) {
		GetComponent<FollowPath>().backAndForth = true;
		GetComponent<FollowPath>().targetGoal = res;
		resourceActive = true;
	}

	public void findNewBuilding() {
		Debug.Log("Finding new Building!");
		if (comm == null) {
			Debug.LogError("Comm is null");
			comm = GameObject.FindObjectOfType(typeof(Community)) as Community;
		}
		List<Vertex> buildings = comm.getBuildingLocations();
		buildings.Remove(comm.getCampfireVertex());
		int randIndex = Random.Range(0, buildings.Count);
		GetComponent<FollowPath>().targetGoal = buildings[randIndex];
	}


}
