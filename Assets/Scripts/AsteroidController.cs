using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidController : MonoBehaviour {
	public float speed;
	public GameObject[] trocitos;
	public GameObject explosion;
	public int score;

	private GameController gameController;
	private Rigidbody rb;

    /**
     * Cuando se crea un asteroide o fragmento, se calcula su movimiento linear, a este
     * movimiento se le añadirá después otro movimiento de rotación, el cual viene determinado 
     * por otro script: Random Rotator.
     */ 
	void Awake() {
		// Movimiento
		rb = GetComponent<Rigidbody>();
		Vector3 movimiento = new Vector3(Random.Range(-12.0f, 12.0f), 0, Random.Range(-9.0f, 9.0f));
		rb.velocity = movimiento * speed;

		GameObject gameControllerObject = GameObject.FindWithTag("GameController");

		if (gameControllerObject != null) {
			gameController = gameControllerObject.GetComponent<GameController>();
		}
	}

    /**
     * Los asteroides sólo se destruyen si:
     *   - Alguien les dispara: el jugador o el Ovni
     *   - Chocan con la nave del jugador
     *   
     * Cuando chocan entre sí no se destruyen y tampoco si es el ovni el que choca con ellos.
     * Se podría programar para se detectara el choque con el ovni y este también explotara,
     * pero esto le quitaría gracia al juego.
     * 
     * Al explotar un asteroide se divide en tantos fragmentos como indique el array
     * denominado trocitos. (Ver variable en el editor)
     * Los fragmentos no se subdividen, debido a que el array trocitos tiene un tamaño 0.
     * 
     */ 
	void OnCollisionEnter(Collision collisionInfo) {

		GameObject other = collisionInfo.collider.gameObject;

		if (other.tag == "Player" || other.tag.Contains("Bolt")) {
            // Creamos una explosión en el lugar que ocupaba el asteroide
			Instantiate(explosion, transform.position, transform.rotation);

            // Creamos los fragmentos
			if (trocitos.Length > 0) {
				Vector3 explosionPlace = new Vector3(transform.position.x, transform.position.y, transform.position.z);

				Instantiate(trocitos[0], explosionPlace, transform.rotation);

				Vector3 movement1 = new Vector3(0.0f, 0f, 5f);
				transform.Translate(movement1 * 2);

				Instantiate(trocitos[1], explosionPlace, transform.rotation);

				Vector3 movement2 = new Vector3(0.0f, 0f, -5f);
				transform.Translate(movement2 * 2);
			}

            // Aumentamos la puntuación del jugador. Cada asteroide o fragmento tiene un valor (Score)
			if (score > 0) {
				gameController.AddScore(score);
			}

			// Destruimos los objetos que intervienen en el choque...
			// Por un lado el objeto con el que ha colisionado el Asteroide:
			// a.1) Bien el jugador
			if (other.tag == "Player") {
				NaveController playerController = other.GetComponent<NaveController>();
				playerController.Dead();
			}

			// a.2) ...o el disparo
			if (other.tag.Contains("Bolt")) {
				Destroy(other);
			}

			// b) Destruimos el propio jugador
			Destroy(gameObject);
		}

	}
}
