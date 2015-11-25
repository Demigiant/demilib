using UnityEngine;
using System.Collections;
using DG.DeAudio;

public class AudioCollection : MonoBehaviour
{
	public DeAudioClipData[] audios;

	public void PlayFx0()
	{
		DeAudioManager.Play(DeAudioGroupId.FX, audios[0].clip, audios[0].volume);
	}
}