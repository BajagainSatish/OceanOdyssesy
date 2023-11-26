using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSoundController : MonoBehaviour
{
    //This incomplete script is not used for this phase of project
    private AudioSource audioSource;
    private bool playProjectileSound;
    private float soundPlayDuration;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        playProjectileSound = true;
        this.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            this.gameObject.SetActive(true);
            this.transform.GetChild(0).gameObject.SetActive(true);

            if (playProjectileSound)
            {
                audioSource.Play();
                playProjectileSound = false;
            }
            StartCoroutine(Wait8Seconds());
        }
    }
    private IEnumerator Wait8Seconds()
    {
        yield return new WaitForSeconds(soundPlayDuration);//8 since sound of cannon shot is of similar length
        playProjectileSound = true;
    }
}
