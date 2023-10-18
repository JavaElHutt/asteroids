using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Describimos toda la lógica asociada a la nave, que representa nuestro jugador.
 * En este script se definen los controles de "vuelo", las vulnerabilidades de la nave 
 * (qué acciones o sucesos pueden dañarla, como los choques con los asteroides o el
 * disparo del ovni). También se define el funcionamiento de la barrera protectora (superbarrera).
 */ 
public class NaveController : MonoBehaviour {

	public float velocidadGiro;
	public float velocidadAvance;
	public GameObject explosion;
	public GameObject shot;
    public Transform[] shotSpawns;
	public float fireRate;
	public AudioClip weaponSound;
	public AudioClip thrustSound;
	public AudioClip barreraSound;
    

    private AudioSource audioSource;
	private Rigidbody rb;
	private GameObject visualThrust; // Textura que aparece al ir hacia adelante
    private GameObject VFXThrustLight; // Luces para remarcar el efecto del propulsor
    private GameObject VFXlaserLights; // Se encienden al diparar los laser
    private GameObject superBarrera;
  

    private GameController gameController;
	private OvniController ovniController;

    // Tiempo que tiene que transcurrir entre disparo y disparo
	private float nextFire;
    // Indica si ha transcurrido el tiempo suficiente para poder disparar
	private float fireTime;

	// Contador para la barrera. Cada vez que se pulsa se pone a cero
    // Nos indica si ya se puede utilizar de nuevo la superbarrera
	private float shieldTime;

    // duración de la barrera = duración del audioClip = 2.209 segundos
    private float shieldLifeTime;

	// Tiempo que tiene que esperar el jugador hasta poder usar la superbarrera de nuevo
	private float waitForNextShield;

	// Indica si ha llegado el tiempo de poder usar la barrera
	private float nextShield;

    /**
     * Inicialización de variables. Este método es invocado de forma automática por Unity
     * cuando se instancia un GameObject que lleva asociado este script.
     * La invocación de Awake se produce antes de la llamada a los métodos responsables
     * de las actualizaciones de cada frame (Update y FixedUpdate)
     */ 
	void Awake() {
        // Inicializaciones para los disparos
		nextFire = 0.5F;
		fireTime = 0.0F;

        // Inicializaciones para la barrera protectora
		shieldTime = 0.0f; 
		shieldLifeTime = 2.209f;
		waitForNextShield = 2.5f;
		nextShield = 0.0f;

        // Inicializaciones de los componentes
		rb = GetComponent<Rigidbody>();
		audioSource = GetComponent<AudioSource>();

        // Desactivación de componentes durante la aparición de la nave
        // Tenemos que desactivar las luces de los efectos visuales (laser y propulsión)
        // También tenemos que desactivar la superbarrera y el chorro de propulsión de los motores
        VFXThrustLight = GameObject.FindGameObjectWithTag("ThrustLight");
        VFXThrustLight.SetActive(false);

        VFXlaserLights = GameObject.FindGameObjectWithTag("LaserLights");
        VFXlaserLights.SetActive(false);

        visualThrust = GameObject.FindGameObjectWithTag("Thrust");
		visualThrust.SetActive(false);

        superBarrera = GameObject.FindGameObjectWithTag("SuperBarrera");
		superBarrera.SetActive(false);

        GameObject gameControllerObject = GameObject.FindWithTag("GameController");
		if (gameControllerObject != null) {
			gameController = gameControllerObject.GetComponent<GameController>();
		}

        // Excepto en la primera aparición la superbarrera se activará al aparecer la nave en escena
		if (gameController.getExtraLives() < 3) {
			StartCoroutine(ActivarSuperBarrera());
		}

	}

    /**
     * En Update realizamos las acciones relaccionadas con el manejo de la nave.
     * Update se ejecuta una vez por frame, pero no siempre a la misma velocidad,
     * es decir, el número de llamadas o invocaciones dependerá de la velocidad 
     * a la que el motor de renderizado es capaz de realizar cada frame.
     */ 
	void Update() {

		fireTime = fireTime + Time.deltaTime;
		shieldTime = shieldTime + Time.deltaTime;

		if (Input.GetButton("Fire1") && fireTime > nextFire) {

			audioSource.PlayOneShot(weaponSound);
			nextFire = fireTime + fireRate;
			foreach (var shotSpawn in shotSpawns) {
				Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
				VFXlaserLights.SetActive(true);
			}

			nextFire = nextFire - fireTime;
			fireTime = 0.0F;
		}

        // Dejamos las luces del laser encendidas un breve espacio de tiempo encendidas,
        // por eso no las apagamos en el mismo frame, sino que dejamos pasar un tiempo,
        // un tercio de nextFire
		if (fireTime > (nextFire/3)) {
			VFXlaserLights.SetActive(false);
		}

        // Activación de la superbarrera cuando el jugador pulsa el botón correspondiente
		if (Input.GetButton("Jump") && shieldTime > nextShield) {
			StartCoroutine(ActivarSuperBarrera());
		}

	}


    /**
     * Utilizamos FixedUpdate cuando vamos a realizar cálculos que tienen que ver con el sistema de físicas,
     * sobre todo cuando hacemos uso del RigidBody. Se ejecuta en sincronía con el motor de físicas y su frecuencia
     * se especifica en Edit > Project Settings > Time > Fixed Timestep
     */ 
	void FixedUpdate() {
		float adelante = Input.GetAxis("Vertical");

		rb.AddRelativeForce(Vector3.forward * adelante * velocidadAvance);

		if (adelante != 0) {
			visualThrust.SetActive(true);
			VFXThrustLight.SetActive(true);
			audioSource.PlayOneShot(thrustSound);
		} else if (adelante == 0) {
			visualThrust.SetActive(false);
			VFXThrustLight.SetActive(false);
		}

		float alLado = Input.GetAxis("Horizontal");

		rb.AddTorque(0.0f, alLado * velocidadGiro, 0.0f);

    }

    /**
     * Definimos los sucesos que pueden dañar al jugador:
     *  - Choques con asteroides o con el ovni
     *  - Disparos enemigos
     */ 
    void OnCollisionEnter(Collision collisionInfo) {

        GameObject other = collisionInfo.collider.gameObject;

        if (other.tag.Contains("Ovni")) {

			if (other.tag == "BoltOvni") {
				Destroy(other); // Eliminamos el disparo del ovni
			}
			
			if (other.tag == "Ovni") {
				GameObject ovni = GameObject.FindGameObjectWithTag("Ovni");
				if (ovni != null) {
					ovniController = ovni.GetComponent<OvniController>();
				}

				ovniController.Dead(); // Si chocamos con el ovni lo destruimos
			}

			Dead(); // muerte del jugador (por el impacto)
		}
	}

    /**
     * Al definir un método Dead dentro de un GameObject estamos posibilitando que sea el propio
     * objeto el que defina los sucesos que ocurriran al morir. En este caso se instancia una explosión 
     * y se elimina una vida extra, informando al GameController.
     */
    public void Dead() {
		if (!superBarrera.activeSelf) {
			Instantiate(explosion, transform.position, transform.rotation);
			Destroy(gameObject);
			gameController.RemoveLive();
		}
	}

    /**
     * Se define la activación de la superbarrera como una corrutina.
     * Se activa en dos momentos diferentes del juego:
     *  - Cuando el jugador pulsa la tecla correspondiente, siempre que haya pasado 
     *    un tiempo determinado desde la última pulsación
     *  - Cuando la nave reaparece tras una muerte (es decir cuando aparece en escena, 
     *    excepto en la primera aparición de la partida)
     */ 
	IEnumerator ActivarSuperBarrera() {

		if (!superBarrera.activeSelf) {
			superBarrera.SetActive(true);
			audioSource.PlayOneShot(barreraSound);

			yield return new WaitForSeconds(shieldLifeTime);

			superBarrera.SetActive(false);
			nextShield = waitForNextShield;
			shieldTime = 0.0f;
		}

	}

}
