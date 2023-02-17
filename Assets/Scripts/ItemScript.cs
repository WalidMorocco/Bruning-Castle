using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemScript : MonoBehaviour
{
    public GameObject player;
    public Animator anim;
    public AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        player = GameObject.Find("Player");
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            audioSource.Play();
            player.GetComponent<PlayerScript>().itemsCollected++;
            anim.SetTrigger("gotChest");
        }
    }

    public void DestroyChest()
    {
        Destroy(gameObject);
    }
}
