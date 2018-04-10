using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManagerScript : MonoBehaviour {

	public static AudioClip JumpSound;
	static AudioSource AudioSource;

	// Use this for initialization
	void Start () {
		JumpSound = Resources.Load<AudioClip> ("JumpSound");
		AudioSource = GetComponent<AudioSource> ();
	}

	public static void PlayJumpSound () {
		AudioSource.PlayOneShot (JumpSound);	
	}
}
