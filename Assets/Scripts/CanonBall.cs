using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanonBall : MonoBehaviour
{
    private AudioSource audioSource;
    private bool playCannonBallSound;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        playCannonBallSound = true;
    }

    private void Update()
    {
        if (this.gameObject.activeInHierarchy)
        {
            if (playCannonBallSound)
            {
                audioSource.Play();
                playCannonBallSound = false;
            }
            StartCoroutine(Wait5Seconds());
        }
    }
    private IEnumerator Wait5Seconds()
    {
        yield return new WaitForSeconds(8);//8 since sound of cannon shot is of similar length
        gameObject.SetActive(false);
        playCannonBallSound = true;
    }
}
