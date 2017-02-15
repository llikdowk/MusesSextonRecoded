using Cubiquity;
using Game.Entities;
using UnityEngine;

namespace Game.PlayerComponents.Movement.Behaviours.Interactions {
	public class CarveTerrainInteraction : Interaction {

		private static GameObject _digMarker;

		private readonly CarveTerrainVolumeComponent _terrainCarver;
		private readonly Player _player;

		public CarveTerrainInteraction() {
			if (_digMarker == null) {
				_digMarker = GameObject.CreatePrimitive(PrimitiveType.Cube);
				Object.Destroy(_digMarker.GetComponent<Collider>());
				_digMarker.SetActive(false);
				_digMarker.name = "_digMarker";
				_digMarker.transform.localScale = new Vector3(4, 0.1f, 4);
				_digMarker.layer = LayerMaskManager.Get(Layer.Outline);
				_digMarker.GetComponent<MeshRenderer>().material = new Material(Shader.Find("UI/Default"));
				GameObject marker = new GameObject("_marker");
				marker.transform.parent = _digMarker.transform;
				marker.transform.LocalReset();
				SpriteRenderer sr = marker.AddComponent<SpriteRenderer>();
				sr.sprite = Resources.Load<Sprite>("Sprites/dig");
				sr.material = new Material(Shader.Find("Custom/UniformSpriteFaceCamera"));
				sr.material.color = new Color(0, 81.0f / 255.0f, 240.0f / 255.0f);
			}

			_player = Player.GetInstance();

			string terrainName = "Terrain Volume";
			GameObject terrain = GameObject.Find(terrainName);
			if (!terrain) {
				DebugMsg.GameObjectNotFound(Debug.LogError, terrainName);
			}
			else {
				var terrainVolume = terrain.GetComponent<TerrainVolume>();
				if (!terrainVolume) DebugMsg.ComponentNotFound(Debug.LogError, typeof(TerrainVolume));

				_terrainCarver = terrain.GetComponent<CarveTerrainVolumeComponent>();
				if (!_terrainCarver) DebugMsg.ComponentNotFound(Debug.LogError, typeof(CarveTerrainVolumeComponent));
			}
		}

		public override void DoInteraction() {
			HideFeedback();

			Vector3[] v = _terrainCarver.DoCarveAction(new Ray(_player.transform.position, _player.Camera.transform.forward));
			v[0] = v[0] + new Vector3(-1, 0, 0); // hardcoded offset
			v[1] = v[1] + new Vector3(0, 0, -1);
			Vector3 position = (v[1] - v[0]) / 2.0f + v[0];

			GameObject tomb = new GameObject("_tomb");
			CreateTombComponent tombComponent = tomb.AddComponent<CreateTombComponent>();

			RaycastHit hit;
			_player.GetEyeSight(out hit);

			tomb.transform.position = position; 
			tomb.transform.up = hit.normal;

			GameObject _debugSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			_debugSphere.transform.position = tomb.transform.position;
			_debugSphere.transform.localScale = 0.2f * Vector3.one;
			_debugSphere = GameObject.CreatePrimitive(PrimitiveType.Cube);
			_debugSphere.transform.position = v[0];
			_debugSphere.name = "v0";
			_debugSphere.transform.localScale = 0.2f * Vector3.one;
			_debugSphere = GameObject.CreatePrimitive(PrimitiveType.Cube);
			_debugSphere.transform.position = v[1];
			_debugSphere.name = "v1";
			_debugSphere.transform.localScale = 0.2f * Vector3.one;

			Debug.DrawRay(v[0], Vector3.up*10, Color.magenta);
			Debug.DrawRay(v[1], Vector3.up*10, Color.magenta);
			Vector3 upperLeft = v[0];
			Vector3 lowerRight = v[1];
			Vector3 middleLow = new Vector3(upperLeft.x - 4.0f, hit.point.y, lowerRight.z - (lowerRight.z - upperLeft.z)/2.0f );
			Player.GetInstance().MoveImmediatlyTo(middleLow);
			Player.GetInstance().transform.rotation =  Quaternion.AngleAxis(80, Vector3.up);
			Player.GetInstance().CurrentState = new DigDownState(tombComponent.GetGround());

		}

		public override void ShowFeedback() {
			RaycastHit hit;
			_player.GetEyeSight(out hit);

			_digMarker.SetActive(true);
			_digMarker.transform.position = hit.point;
			_digMarker.transform.up = Vector3.Lerp(_digMarker.transform.up, hit.normal, 0.05f);
		}

		public override void HideFeedback() {
			_digMarker.SetActive(false);
		}

		public override Interaction CheckForPromotion() {
			RaycastHit hit;
			bool hasHit = _player.GetEyeSight(out hit);

			if (!hasHit) return null;
			GameObject g = hit.collider.gameObject;
			if (g.tag == TagManager.Get(Tag.Terrain) && hit.distance > 2.0f) {
				return this;
			}
			return null;
		}
	}
}