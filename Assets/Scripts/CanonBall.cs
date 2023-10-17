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
        this.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            this.gameObject.SetActive(true);
            this.transform.GetChild(0).gameObject.SetActive(true);

            if (playCannonBallSound)
            {
                audioSource.Play();
                playCannonBallSound = false;
            }
            StartCoroutine(Wait8Seconds());
        }
    }
    private IEnumerator Wait8Seconds()
    {
        yield return new WaitForSeconds(8);//8 since sound of cannon shot is of similar length
        playCannonBallSound = true;
    }
}
