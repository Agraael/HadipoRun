﻿using System.IO;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	[Tooltip("Vitesse du Joueur")]
	[Range(0, 15)]
	public float speed;
    public string[] bigErrors;

	[Tooltip("Temps d'invulnerabilité après un contact avec un virus")]
	public float InvulnerabilityTime;
	private float timeCount = 0;

	private Rigidbody2D rb;
	private SpriteRenderer sprite;
	private Color color;
    private PopUpManager popUp;
    
	void Start () 
	{
		rb = GetComponent<Rigidbody2D>();
		popUp = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<PopUpManager>();
        bigErrors = File.ReadAllLines("NameDatabase/errorsExplanations.dat");
		sprite = GetComponent<SpriteRenderer> ();
	}

	void FixedUpdate ()
	{
		float moveHorizontal = Input.GetAxis ("Horizontal");

		Vector2 movement = new Vector2 (moveHorizontal, 0.0f);
		rb.velocity = new Vector2(Mathf.Lerp(0, moveHorizontal * speed, 0.8f), 0.0f);
		rb.AddForce (movement * speed);
		if (timeCount > 0) {
			sprite.color = new Color (1f, 1f, 1f, 0.3f);
			timeCount -= Time.deltaTime;
		} else
			sprite.color = new Color (1f, 1f, 1f, 1f);
	}
	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.CompareTag("Item")) {
			FileDescription File = other.GetComponent<FileDescription> ();
			popUp.GenericAdd (PopUpType.DOWNLOAD, File.title, File.sizeFile);
            Destroy (other.gameObject);
        }
		if (other.gameObject.CompareTag ("Virus") && timeCount <= 0)
        {
            int virusType = Random.Range(0, 4);
            if (virusType == 0)
            {
                int max = Random.Range(3, 9);
                for (int i = 0; i < max; i++)
                    popUp.GenericAdd(PopUpType.ALERT, "Error", "ふたなり-" + i + ".dll was not found.");
            }
            else if (virusType == 1)
            {
                string errors = "";
                for (int i = 0; i < 4; i++)
                    errors += bigErrors[Random.Range(0, bigErrors.Length)] + System.Environment.NewLine;
                popUp.GenericAdd(PopUpType.BIGERROR, "Fatal Error", "An error occured while executing the application", errors);
            }
            else if (virusType == 2)
            {
                popUp.GenericAdd(PopUpType.BROWSER, "Internet Explorer 4.0ad");
            }
            else
            {
                popUp.GenericAdd(PopUpType.SYS32, "Confirm Folder Delete", "Are you sure you want to delete 'C:\\System32\\' and all of its content?");
            }
			timeCount = InvulnerabilityTime;
            Destroy (other.gameObject);
		}
    }

}