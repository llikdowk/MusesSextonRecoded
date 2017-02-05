﻿using UnityEngine;
using Cubiquity;
using UnityEngine.UI;

public class LoadComponents : MonoBehaviour {
	public TerrainVolume Terrain;
	public Image TitleImage;
	public Image FallbackImage;
	public Camera UICamera;

	private float _originalTimeScale;
	private bool _hasClicked = false;

	public void Awake() {
		Terrain.gameObject.SetActive(true);
		Terrain.enabled = true;
		_originalTimeScale = Time.timeScale;
		Time.timeScale = 0.0f;
	}

	public void Start() {
		TitleImage.gameObject.SetActive(true);
		TitleImage.enabled = true;
		FallbackImage.gameObject.SetActive(true);
		FallbackImage.enabled = true;
	}

	public void Update() {
		if (Terrain.isMeshSyncronized && _hasClicked) {
			Time.timeScale = _originalTimeScale;
			enabled = false;
			TitleImage.gameObject.SetActive(false);
			FallbackImage.gameObject.SetActive(false);
		}
		else {
			if (Input.GetKeyDown(KeyCode.Mouse0)) {
				_hasClicked = true;
				StartCoroutine(Utils.FadeOut(TitleImage, 1.0f));
				FallbackImage.gameObject.SetActive(true);
			}
		}
	}
}