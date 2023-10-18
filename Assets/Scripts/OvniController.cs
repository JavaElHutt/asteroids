using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OvniController : MonoBehaviour {
	public float speed;
	public GameObject explosion;

	public GameObject shot;
	public Transform[] shotSpawns;
	public float fireRate;
	public float delay;
    public int score;
    public AudioClip weaponSound;
    public AudioClip ovniSound;

    private AudioSource audioSource;

    private Rigidbody rb;
	private GameObject player;
	private GameObject torreta;
    private GameController gameController;
    private bool isOvniSoundPlayed;


    /**
     * En el método Awake ponemos todo el código que queremos que se ejecute
     * cuando Unity crea un nuevo GameObject, por eso es el lugar apropiado para
     * inicializar variables.
     */
    void Awake () {
		rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        isOvniSoundPlayed = false;

        Vector3 movimiento = new Vector3(-rb.position.x, 0, Random.Range(-9.0f, 9.0f));
		rb.velocity = movimiento * speed;

		// La torreta es el hijo número 2, tal que: 
		//		0 -> Ovni
		//		1 -> OVNI_Luz
		//		2 -> OVNI_Torreta
		torreta = gameObject.transform.GetChild(2).gameObject;

        // Le decimos a Unity que invoque este método cada fireRate segundos, esperando
        // la primera invocación delay segundos.
        InvokeRepeating("Fire", delay, fireRate);

        // El Ovni debe poder acceder al objeto GameController, para comunicarle cuando le han matado
        // y que sea el GameController el que aumente la puntuación del jugador 
        GameObject gameControllerObject = GameObject.FindWithTag("GameController");

        if (gameControllerObject != null)
        {
            gameController = gameControllerObject.GetComponent<GameController>();
        }
    }

	/**
     * Update se invoca una vez por cada frame. El encargado de realizar la llamada es el
     * propio motor de juego Unity. Dentro del método Update pondremos todas aquellas acciones 
     * que queramos comprobar para tomar "decisiones de juego". En este caso en particular
     * lo que hacemos es comprobar si el Ovni está dentro de la zona de juego y también
     * movemos la torreta para apuntar al jugador.
     */
	void Update () {
		player = GameObject.FindGameObjectWithTag("Player");

        if (isInsidePlayground() && !isOvniSoundPlayed) {
            isOvniSoundPlayed = true;
            audioSource.Play();
        } else if (!isInsidePlayground()) {
            isOvniSoundPlayed = false;
        }

		if (torreta != null && player != null) {

			torreta.transform.LookAt(player.transform);

		}
	}

    /**
     * El ovni dispara con una frecuencia de disparo, teniendo que esperar entre disparo y disparo
     * la cantidad de segundos especificada por la variable fireRate (su valor se define en el editor).
     * Además sólo se le permite disparar cuando se encuentra dentro de la "zona de juego" o campo de 
     * visión del jugador.
     */
    void Fire() {
        // El Ovni sólo puede disparar si se encuentra dentro del área de juego
        bool canShoot = isInsidePlayground();

        if (canShoot) {
 
            foreach (var shotSpawn in shotSpawns)
            {
                Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
                audioSource.PlayOneShot(weaponSound);
            }
        }
	}

    /**
     * Es el propio Ovni el que detecta sus colisiones y determina qué pasa en cada caso.
     * Vemos que sólo le afectan los disparos del jugador, mientras que los choques con 
     * los asteroides (a los que también consideramos "enemigos") no le afectan.
     */
	void OnCollisionEnter(Collision collisionInfo) {
		GameObject other = collisionInfo.collider.gameObject;

		// Detectamos el disparo del jugador
		if (other.tag == "Bolt") {
			Dead();
			Destroy(other);
		}
	}

    /**
     * Al definir un método Dead dentro de un GameObject estamos posibilitando que sea el propio
     * objeto el que defina los sucesos que ocurriran al morir. En este caso se instancia una explosión 
     * y se aumenta la puntuación del jugador.
     */
	public void Dead() {
		Instantiate(explosion, transform.position, transform.rotation);
		Destroy(gameObject);

        if (score > 0)
        {
            gameController.AddScore(score);
        }
    }

    /**
     * Comprobamos si el Ovni se encuentra dentro de la "zona de juego", es decir, del campo de visión del jugador.
     * Esto es útil para evitar que pueda disparar en cuanto es instanciado, lo cual probablemente pillaría al
     * jugador desprevenido.
     * 
     * Nota: ¿Por qué para comprobar si el ovni está dentro de la zona de juego se chequean sus coordenadas de posición
     * en forma absoluta? Esto se entiende mejor con un ejemplo: Supongamos que el Ovni se encuentra a la derecha del
     * objeto Right (hijo de BoundaryScreenWrapper), y su coordenada de posición x = 24 por lo tanto mayor que la de 
     * Right (x = 22). ¿Está fuera dentro de la zona de juego? No, está fuera del espacio delimitado por los hijos de
     * BoundaryScreenWrapper. ¿Cuando consideramos que está dentro? De forma extricta cuando su coordenada x sea
     * inferior a la de Right, en la práctica damos un margen de 3 unidades.
     * ¿Qué pasaría si se encontrara fuera pero por el lado izquierdo de BoundaryScreenWrapper (es decir, más allá del objeto Left)?
     * El valor de la coordenada x de Left es de -22, luego para que el ovni esté fuera de BoundaryScreenWrapper su x debe de ser mayor 
     * que -22, supongamos una x = -24. Al convertir la posición en valor absoluto, pasamos a tener x = 24, y al COMPARAR de
     * nuevo con el componente RIGHT de BoundaryScreenWrapper, que tiene un valor x = 22, vemos que el Ovni está fuera.
     * En resumen, es una forma de simplificar los cálculos, evitando tener que comparar con las coordenadas de los 4 hijos 
     * (Up, Down, Right y Left) de BoundaryScreenWrapper.
     * 
     */
    private bool isInsidePlayground() {
        bool result = false;

        float posX = Mathf.Abs(transform.position.x);
        float posZ = Mathf.Abs(transform.position.z);

		GameObject boundaryScreenWrapper = GameObject.FindGameObjectWithTag("BoundaryScreenWrapper");
        if (boundaryScreenWrapper != null)
        {
            float playgroundX = boundaryScreenWrapper.transform.GetChild(3).gameObject.transform.position.x; // Right
            float playgroundZ = boundaryScreenWrapper.transform.GetChild(0).gameObject.transform.position.z; // Up

			if (posX < (playgroundX - 3) && posZ < (playgroundZ - 3)) {
                result = true;
            }
        }

        return result;
    }
}
