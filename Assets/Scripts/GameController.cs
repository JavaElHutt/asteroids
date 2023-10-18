using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * El objeto GameController es el centrro neurálgico del juego, el director de orquesta
 * que determina cuando deben aparecer en escena cada uno de los actores que conforman
 * el juego. Así, por ejemplo, se encarga en primer lugar de crear la nave que controlará
 * el jugador, posicionándola en el centro del espacio de juego. También se ocupa de
 * ir creando las distintas amenzas, como son los asteroides o los ovnis.
 * 
 * A parte de todo lo anterior, el GameController lleva la cuenta de los puntos que el 
 * jugador va obteniendo durante el transcurso de la partida y es el responsable de
 * quitarnos una vida cada vez que nos matan.
 */
public class GameController : MonoBehaviour {

    public GameObject[] asteroides;
    public GameObject ovni;
    public GameObject nave;
    public float maxX;
    public float maxZ;
    public float intervaloAsteroides;
    public float intervaloOvni;
    public float tiempoPreparacion;

    public TextMesh scoreText;
    public TextMesh gameOverText;
    public TextMesh scorePointsText;
    public TextMesh youWinText;    
    public bool gameOver;

    private int extraLives;
    private int score;
    private GameObject live;
	private GameObject boundaryScreenWrapper;
    private NaveController naveController;


    /**
     * En cuanto que Unity detecta este objeto en la escena y lo crea este script arranca,
     * y con ello la partida. El juego comienza.
     * Awake es el primer método que se ejecuta cuando se crea un GameObject.
     * 
     * Nota: ver la documentación de Unity donde se especifican la secuencia de invocaciones:
     * https://docs.unity3d.com/Manual/ExecutionOrder.html
     * 
     * 1º) Podemos ver que lo primero que se crea e inicializa es el elemento gráfico del Interface de usuario
     * que reflejará la puntuación (SCORE).
     * 2º) Después se inicializan las coordenadas máximas, con las cuales se posicionarán los objetos que se creen.
     * 3º) Se crean: El jugador, los asteroides y los ovnis.
     */
    void Awake()
    {
		extraLives = 3;
		gameOver = false;
        score = 0;
        scoreText.text = "SCORE";
        UpdateScore();
        gameOverText.text = "";
        youWinText.text = "";
        

		// Detectamos los límites del Juego, vienen marcados por las posiciones
		// Up y Right del BoundaryScreenWrapper. Si este objeto no existiera,
		// dejaremos los límites que se hubieran introducido en el editor de Unity.
		boundaryScreenWrapper = GameObject.FindGameObjectWithTag("BoundaryScreenWrapper");
 		if (boundaryScreenWrapper != null) {
			maxX = boundaryScreenWrapper.transform.GetChild(3).gameObject.transform.position.x; // Right
			maxZ = boundaryScreenWrapper.transform.GetChild(0).gameObject.transform.position.z; // Up
		}


		CrearPlayer();

        StartCoroutine(CrearAsteroides());
        StartCoroutine(CrearOvnis());
    }

    /**
     * Método encargado de la creación y puesta en escena de la nave del jugador.
     */ 
    private void CrearPlayer()
    {
        if (!gameOver)
        {
            Instantiate(nave);
        }

    }

    /**
     * En esta corrutina se crean los Asteroides. Podemos ver que hay una espera inicial llamada
     * tiempoPreparacion, que solo se produce una vez. Esta variable se encuentra definida en 
     * el editor con un valor de 2 segundos. Esto quiere decir que el primer asteroide aparecerá 
     * a los dos segundos de arrancado el juego.
     * Mientras no se alcance el fin del juego, la condición gameOver, la corrutina permanecerá
     * dentro de un bucle while. Este bucle queda suspendido por otra sentencia yield, esta vez
     * esperamos el tiempo intervaloAsteroides. Como resultado podríamos decir que se forma un
     * asteroide cada medio segundo aproximadamente.
     */
    IEnumerator CrearAsteroides()
    {
        yield return new WaitForSeconds(tiempoPreparacion);

        while (!gameOver)
        {

            GameObject asteroide = asteroides[Random.Range(0, asteroides.Length)];
            Vector3 posicion = generarPosicionInicial();                
            Quaternion rotacion = Quaternion.identity; // Sin rotación
            Instantiate(asteroide, posicion, rotacion);
            yield return new WaitForSeconds(intervaloAsteroides);
    
        }

    }

    /**
     * Este es el método de creación de ovnis. Es muy similar al de creación de asteroides,
     * aunque cambian la frecuencia en la creación de ovnis con un valor, establecido desde
     * el editor, de 10 segundos. También hay que observar que se ha puesto la restrición de
     * que no puede haber más un ovni en la escena a la vez.
     */ 
    IEnumerator CrearOvnis()
    {
        yield return new WaitForSeconds(tiempoPreparacion);

        while (!gameOver)
        {

            GameObject unOvni = ovni;
            Vector3 posicion = generarPosicionInicial();

            Quaternion rotacion;

            if (posicion.x > 0)
            {
                rotacion = Quaternion.Euler(0, -90, 0);

            }else if(posicion.x < 0)
            {
                rotacion = Quaternion.Euler(0, 90, 0);
            }
            else
            {
                rotacion = Quaternion.identity;
            }

            // Evitamos haya dos ovnis a la vez en la escena
            GameObject[] ovnis = GameObject.FindGameObjectsWithTag("Ovni");
            if (ovnis.Length == 0)
            {
                Instantiate(unOvni, posicion, rotacion);
            }

            
            yield return new WaitForSeconds(intervaloOvni);

        }

    }

    /**
     *  Método auxiliar que se utiliza para generar las coordenadas de posición de los objetos (asteroides y ovnis)
     *  Se toman como referencia las coordenadas del objeto BoundaryScreenWrapper, más concretamente 
     *  las de sus hijos Up y Right.
     *  Los objetos del juego (excepto la nave que se crea siempre en el centro de coordenadas) se crearán en los límites
     *  de BoundaryScreenWrapper.
     */
    private Vector3 generarPosicionInicial()
    {
        Vector3 posicion = Vector3.zero;

        int flag = Random.Range(1, 4);
        float valorX = Random.Range(-maxX, maxX);
        float valorZ = Random.Range(-maxZ, maxZ);


		switch (flag) {
			case 1:
				posicion = new Vector3(valorX, 0, maxZ);
				break;
			case 2:
				posicion = new Vector3(valorX, 0, -maxZ);
				break;
			case 3:
				posicion = new Vector3(maxX, 0, valorZ);
				break;
			case 4:
				posicion = new Vector3(-maxX, 0, valorZ);
				break;
		}

        return posicion;
        
    }

    /**
     * Cuando el jugador destruye un asteroide, fragmentos de asteroide o al ovni, se le recompensa con una
     * puntuación determinada, definida en la variable score de cada uno de estos objetos "puntuables".
     * Este método se invoca desde los scripts AsteroidController y OvniController.
     */ 
    public void AddScore(int newScore)
    {
        score += newScore;
        UpdateScore();

    }

    /**
     * Actualizamos el texto que le mostramos al jugador en el interface de usuario.
     */ 
    private void UpdateScore()
    {

        scorePointsText.text = score.ToString();

    }

    /**
     * Cada vez que el jugador pierde una vida hay que actualizar la variable que lleva la cuenta.
     * Este método se invoca desde el método Dead() de NaveController.
     * Además de actualizar el valor de la variable extraLives también se ocupa de ocultar uno de los
     * iconos que representa una vida, situado en la esquina superior derecha.
     */
    public void RemoveLive()
    {

        if (extraLives == 3)
        {
            live = GameObject.FindWithTag("Live03");
            live.SetActive(false);
            extraLives = 2;
            Invoke("CrearPlayer", 2);

        }
        else if (extraLives == 2)
        {
            live = GameObject.FindWithTag("Live02");
            live.SetActive(false);
            extraLives = 1;
            Invoke("CrearPlayer", 2);

        }
        else if (extraLives == 1)
        {
            live = GameObject.FindWithTag("Live01");
            live.SetActive(false);
            GameOver();
        }

    }

    /**
     * Cuando el juagdor pierde la última vida el juego se termina.
     * Al cambiar el valor de la variable gameOver a verdadero se termina
     * la ejecución de las corrutinas encargadas de la creación de asteroides y ovnis.
     */
    public void GameOver()
    {
        gameOver = true;
		DestroyAllPlayers();
        gameOverText.text = "GAME OVER!!";
    }

    /**
     * Método auxiliar para forzar el final de la partida y que evita que queden jugadores aunque
     * se haya lanzado la señal de final de juego (gameOver = true)
     */
    private void DestroyAllPlayers() {
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

		if (players != null && players.Length > 0) {
			for(int i = 0; i < players.Length; i++) {
				Destroy(players[0]);
			}
		}
	}

    /**
     * Método que permite la activación de la barrera de protección (Superbarrera) de la nave
     * cuando esta reaparece tras una muerte. Es decir la superbarrera se activa automáticamente
     * cuando la nave aparece en las vidas 2 y tres, pero no aparece al comienzo del juego, cuando
     * aparece por primera vez (ya que no hay enemigos)
     */ 
	public int getExtraLives() {
		return this.extraLives;
	}
}
