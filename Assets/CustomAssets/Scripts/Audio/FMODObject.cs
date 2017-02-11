﻿using FMOD.Studio;

namespace Audio {
	public class FMODObject {

		private readonly FMOD.Studio.EventInstance _audio;
		private readonly FMOD.Studio.EventDescription _info;

		public FMODObject(string eventName) {
			_audio = FMODUnity.RuntimeManager.CreateInstance(eventName);
			_audio.getDescription(out _info);
			_info.loadSampleData();
		}

		/// <summary>
		/// Returns current timeline position in seconds
		/// </summary>
		public float GetTimelinePosition() {
			int current_ms;
			_audio.getTimelinePosition(out current_ms);
			return current_ms / 1000.0f;
		}

		/// <summary>
		/// Returns normalized current timeline position (from 0 to 1)
		/// </summary>
		/// <returns></returns>
		public float GetNormalizedTimelinePosition() {
			int current_ms;
			_audio.getTimelinePosition(out current_ms);

			int length_ms;
			_info.getLength(out length_ms);

			return current_ms / (float) length_ms;
		}

		public void Play() {
			_audio.start();
		}

		public void StopFading() {
			_audio.stop(STOP_MODE.ALLOWFADEOUT);
		}

		public void StopImmediate() {
			_audio.stop(STOP_MODE.IMMEDIATE);
		}

		public void SetParameter(string paramName, float value) {
			_audio.setParameterValue(paramName, value);
		}

		public float GetParameter(string paramName) {
			ParameterInstance p;
			_audio.getParameter(paramName, out p);
			float value;
			p.getValue(out value);
			return value;
		}

		public bool IsPlaying() {
			PLAYBACK_STATE state;
			_audio.getPlaybackState(out state);
			return state == PLAYBACK_STATE.PLAYING;
		}
	}
}