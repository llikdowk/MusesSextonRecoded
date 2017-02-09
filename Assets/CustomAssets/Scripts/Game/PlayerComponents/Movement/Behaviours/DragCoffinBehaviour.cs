﻿using MiscComponents;
using UnityEngine;

namespace Game.PlayerComponents.Movement.Behaviours {

	public class DragCoffinBehaviour : WalkRunMovementBehaviour { // TODO: change movement (slower)

		private readonly GameObject _coffin;
		private readonly Transform _slot;

		public DragCoffinBehaviour(Transform transform, GameObject coffin, SuperConfig config) : base(transform, config) {
			_coffin = coffin;
			_coffin.GetComponent<Rigidbody>().isKinematic = true;
			_coffin.GetComponent<Collider>().enabled = false;

			Action<PlayerAction> useAction = Player.GetInstance().Actions.GetAction(PlayerAction.Use);
			useAction.StartBehaviour = () => {
				_coffin.transform.parent = null;
				_coffin.GetComponent<Collider>().enabled = true;
				Rigidbody rb = _coffin.GetComponent<Rigidbody>();
				rb.isKinematic = false;
				_coffin.layer = LayerMaskManager.Get(Layer.Default);

				MarkableComponent m = _coffin.GetComponent<MarkableComponent>();
				if (m == null) {
					Debug.LogWarning("Object <b>" + _coffin.gameObject.name + "</b> could not be marked. (Missing MarkableComponent!)"); // TODO extract to Error class
				}
				else {
					//TODO: ADD GUARD -> enable only if not throwed into a hole
					m.EnableMark();
				}

				Player.GetInstance().CurrentState = new WalkRunState();
			};

			_slot = Player.GetInstance().transform.Find("Camera/_slotCoffin");
			if (_slot == null) Debug.LogError("Camera/_slotCoffin not found in PlayerGameobject"); // TODO extract to Error class
			coffin.transform.parent = null;
			//coffin.transform.parent = _slot; // UNITY BUG?! GameObject disappears and behaves undefinedly
		}

		protected override void CheckForInteraction() {
			_coffin.transform.position = _slot.position; // bug workaround
			_coffin.transform.rotation = _slot.rotation;
			//_coffin.transform.rotation = Quaternion.RotateTowards(_coffin.transform.rotation, _slot.transform.rotation,
			//	Time.deltaTime * 100.0f);
		}

	}
}
