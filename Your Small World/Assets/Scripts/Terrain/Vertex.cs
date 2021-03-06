﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vertex {

	int index;
		
	Vector3 vert;

	float height;

	bool isEditable;

	string biome;

	GameObject resource;

	Color color;

	Vertex[] neighbors;

	SphereTerrain parent;

	SmolMan attachedMan;

	public Vertex(int index, Vector3 loc, SphereTerrain parent) {
		this.parent = parent;
		this.index = index;
		this.vert = loc;
		height = 0;
		isEditable = true;
		this.biome = SphereTerrain.DESERT_BIOME;
		this.color = SphereTerrain.yellows [Random.Range (0, SphereTerrain.yellows.Count)];
		resource = null;
		neighbors = null;
	}

	public float getHeight() {
		return height;
	}

	public void setHeight(float h) {
		if (isEditable) {
			height = Mathf.Max(Mathf.Min(h, parent.maxHeight), parent.minHeight);
			if (height > 0) {
				setTerrain (SphereTerrain.HIGH_BIOME);
			} else if (height < 0.0f) {
				setTerrain(SphereTerrain.LOW_BIOME);
				calculateNeighbors ();
				if (nextTo(SphereTerrain.WATER_BIOME)) {
					parent.spreadWaterBiome (this);
				} else if (nextTo(SphereTerrain.OIL_BIOME)) {
					parent.SpreadOilBiome (this);
				}
			} else {
				if (biome != SphereTerrain.MED_BIOME) {
					setTerrain(SphereTerrain.DESERT_BIOME);
				}
			}
		}
	}

	public string getBiome() {
		return getTerrain ();
	}

	public string getTerrain() {
		return biome;
	}

	public void setBiome(string b) {
		setTerrain (b);
	}

	public void setTerrain(string b) {
		if (this.biome == b || this.biome == SphereTerrain.CITY_BIOME) {
			return;
		}
		Debug.Log("Setting terrain from " + biome + " to " + b);
		if (biome != b){
			if (b == SphereTerrain.WATER_BIOME || b == SphereTerrain.OIL_BIOME || b == SphereTerrain.STONE_BIOME || b == SphereTerrain.DEITON_BIOME || b == SphereTerrain.DESERT_BIOME){
				Debug.Log("Detaching Man");
				detachMan();
			}
			biome = b;
		}
		switch (b) {
		case SphereTerrain.WATER_BIOME:
			color = SphereTerrain.blues[Random.Range(0, SphereTerrain.blues.Count)];
			break;
		case SphereTerrain.DESERT_BIOME:
			color = SphereTerrain.yellows[Random.Range(0, SphereTerrain.yellows.Count)];
			break;
		case SphereTerrain.HIGH_BIOME:
			color = SphereTerrain.greays[Random.Range(0, SphereTerrain.greays.Count)];
			break;
		case SphereTerrain.MED_BIOME:
			color = SphereTerrain.greens[Random.Range(0, SphereTerrain.greens.Count)];
			break;
		case SphereTerrain.LOW_BIOME:
			color = SphereTerrain.browns[Random.Range(0, SphereTerrain.browns.Count)];
			break;
		case SphereTerrain.STONE_BIOME:
			color = SphereTerrain.stones [Random.Range (0, SphereTerrain.stones.Count)];
			break;
		case SphereTerrain.OIL_BIOME:
			color = SphereTerrain.oils [Random.Range (0, SphereTerrain.oils.Count)];
			break;
		case SphereTerrain.DEITON_BIOME:
			color = new Color(1.0f, 0.0f, 1.0f, 1.0f);
			break;
		}
		parent.updateColors ();
	}

	public void attachMan(SmolMan man) {
		attachedMan = man;
	}

	private void detachMan() {
		if (attachedMan != null) {
			Community comm = GameObject.FindObjectOfType<Community>();
			if (comm != null) {
				comm.FreeOneBoi(attachedMan);
				attachedMan = null;
			}
		}
	}

	public bool getIsEditable() {
		return isEditable;
	}

	public void setIsEditable(bool e) {
		isEditable = e;
	}

	public Vector3 getSphereVector() {
		return getSphereVector (parent.radius, parent.gameObject.transform.position);
	}

	public Vector3 getSphereVector(float radius, Vector3 center) {
		return (this.vert - center).normalized * (radius + this.height);
	}

	public Vector3 getTransformedPoint() {
		return parent.transform.TransformPoint(getSphereVector (parent.radius, parent.gameObject.transform.position));
	}

	public GameObject getResource() {
		return this.resource;
	}

	public void removeResource() {
		Debug.Log("Removing Resource");
		if (resource != null) detachMan();
		GameObject temp = this.resource;
		this.resource = null;
		GameObject.Destroy (temp);
	}

	public void setResource(GameObject g) {
		this.resource = g;
	}

	public Color getColor() {
		return this.color;
	}

	public void setColor(Color c) {
		this.color = c;
		parent.updateColors ();
	}

	public bool getTransversable() {
		return (biome == SphereTerrain.DESERT_BIOME || biome == SphereTerrain.MED_BIOME || biome == SphereTerrain.CITY_BIOME);
	}

	public Vertex[] getNeighbors() {
		if (this.neighbors == null || this.neighbors.Length == 0) {
			calculateNeighbors ();
		}
		return this.neighbors;
	}

	public void calculateNeighbors() {
		if (this.neighbors != null && this.neighbors.Length > 0) {
			return;
		}
		int[] curTriangles = parent.filter.mesh.triangles;
		List<int> neighborIndices = new List<int> (); 
		for (int i = 0; i < curTriangles.Length; i++) {
			if (curTriangles [i] == index) {
				int relativePosition = i % 3;
				switch (relativePosition) {
				case 0:
					if (i + 1 < curTriangles.Length && !neighborIndices.Contains (curTriangles[i + 1])) {
						neighborIndices.Add (curTriangles[i + 1]);
					}
					if (i + 2 < curTriangles.Length && !neighborIndices.Contains (curTriangles[i + 2])) {
						neighborIndices.Add (curTriangles[i + 2]);
					}
					break;
				case 1:
					if (!neighborIndices.Contains (curTriangles[i - 1])) {
						neighborIndices.Add (curTriangles[i - 1]);
					}
					if (i + 1 < curTriangles.Length && !neighborIndices.Contains (curTriangles[i + 1])) {
						neighborIndices.Add (curTriangles[i + 1]);
					}
					break;
				case 2:
					if (!neighborIndices.Contains (curTriangles[i - 1])) {
						neighborIndices.Add (curTriangles[i - 1]);
					}
					if (!neighborIndices.Contains (curTriangles[i - 2])) {
						neighborIndices.Add (curTriangles[i - 2]);
					}
					break;
				}
			}
		}
		neighbors = new Vertex[neighborIndices.Count];
		for (int j = 0; j < neighbors.Length; j++) {
			neighbors [j] = parent.getVertex (neighborIndices [j]);
		}
	}

	public int getIndex() {
		return this.index;
	}

	public bool nextTo(string biom) {
		calculateNeighbors ();
		for (int i = 0; i < neighbors.Length; i++) {
			if (neighbors [i].getBiome () == biom) {
				return true;
			}
		}
		return false;
	}
}
