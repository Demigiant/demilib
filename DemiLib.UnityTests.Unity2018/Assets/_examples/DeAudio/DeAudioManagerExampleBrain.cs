using UnityEngine;
using System.Collections;
using DG.DeAudio;

public enum InitMode
{
    Simple,
    InstantiateDeAudioManagerPrefab,
    CreateRuntimeAudioGroups
}

public class DeAudioManagerExampleBrain : MonoBehaviour
{
	[Header("Settings")]
	public InitMode initMode = InitMode.Simple;
	public string prefabName = "- DeAudioManager";
	[Header("FX")]
    public DeAudioClipData fxA;
    public DeAudioClipData fxB;
    [Header("MUSIC")]
    public DeAudioClipData musicA;
    public DeAudioClipData musicB;

    IEnumerator Start()
    {
        switch (initMode) {
        case InitMode.Simple:
    		// Create a new DeAudioManager without any group (which means only the implicit GLOBAL group)
            DeAudioManager.Init();
            break;
        case InitMode.InstantiateDeAudioManagerPrefab:
    		// Instantiate existing prefab from Resources folder
    		DeAudioManager.Init(prefabName);
            break;
        case InitMode.CreateRuntimeAudioGroups:
            // Create DeAudioManager with AudioGroups at runtime
            DeAudioManager.Init(new[] {
                new DeAudioGroup(DeAudioGroupId.Music, 1, 4, 2),
                new DeAudioGroup(DeAudioGroupId.FX, 1),
            });
            break;
        }

        DeAudioSource s = DeAudioManager.Play(musicA);
        Debug.Log(s.time +  "/" + s.duration);
        yield return new WaitForSeconds(2);
        Debug.Log(s.time);
        s.SeekPercentage(40);
        Debug.Log(">>>> " + s.time);
    }

    public void PlayFXA()
    {
        DeAudioManager.Play(fxA);
    }

    public void PlayFXB()
    {
        DeAudioManager.Play(fxB);
    }

    public void PlayMusicA()
    {
        DeAudioManager.Play(musicA);
    }

    public void PlayMusicB()
    {
        DeAudioManager.Play(musicB);
    }

    public void StopAll()
    {
        DeAudioManager.Stop();
    }

    public void StopAllMusic()
    {
        DeAudioManager.Stop(DeAudioGroupId.Music);
    }

    public void StopMusicA()
    {
        DeAudioManager.Stop(musicA.clip);
    }

    public void FadeOutGlobal()
    {
    	DeAudioManager.FadeOut();
    }

    public void FadeOutMusic()
    {
    	DeAudioManager.FadeOut(DeAudioGroupId.Music);
    }

    public void FadeOutMusicA()
    {
    	DeAudioManager.FadeOut(musicA.clip);
    }

    public void FadeInGlobal()
    {
    	DeAudioManager.FadeIn();
    }

    public void FadeInMusic()
    {
    	DeAudioManager.FadeIn(DeAudioGroupId.Music);
    }

    public void FadeInMusicA()
    {
    	DeAudioManager.FadeIn(musicA);
    }

    public void CrossfadeMusicToB()
    {
        DeAudioManager.Crossfade(musicB);
    }
}