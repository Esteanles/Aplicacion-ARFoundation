using JetBrains.Annotations;
using TMPro;
using UnityEngine;

public class Objetivo : MonoBehaviour
{
/// <summary>
/// Referencia al ARGameManager para gestionar el respawn del objeto lanzable una vez que se golpea el objetivo.
/// </summary>
    public ARGameManager gameManager;
    /// <summary>
    ///  Regula las animaciones del efecto visual de impacto al golpear el objetivo.
    /// </summary>
    public Animator hitEffect;
/// <summary>
/// Texto de UI para mostrar el contador de golpes al jugador cada vez que se golpea el objetivo.
/// </summary>
    public TextMeshPro texto;

    private int counter;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    /// <summary>
    /// Localiza el ARGameManager en la escena para poder llamar a sus métodos desde este script. También localiza el componente Animator del efecto de impacto.
    /// </summary>
    void Start()

    {
        /// Busca automáticamente el ARGameManager en la escena para obtener una referencia a él 
        /// y poder llamar a sus métodos desde este script sin necesidad de asignarlo manualmente en el inspector.
        gameManager = GameObject.FindFirstObjectByType<ARGameManager>();//
        if(hitEffect != null)
        {
            /// Busca el componente Animator en los hijos de este objeto para controlar
            ///  las animaciones del efecto de impacto al golpear el objetivo.
            hitEffect = this.gameObject.GetComponentInChildren<Animator>();
        }

    }

    // Update is called once per frame
    /// <summary>
    /// Detecta cuando el objeto lanzable colisiona con el objetivo y activa el efecto de impacto.
    /// </summary>
    /// <param name="collision"> Información sobre la colisión </param>

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.GetComponent<ObjetoLanzable>() != null &&
        collision.gameObject.GetComponent<ObjetoLanzable>().enabled == true)// Verificar que el objeto colisionado tenga el componente ObjetoLanzable y que esté habilitado
        {
            OnHit(collision.contacts[0].point);// Llamar al método OnHit y pasar la posición del punto de contacto de la colisión
        }
    }
    /// <summary>
    /// Procesa los resultados de un golpe al objetivo.
    /// </summary>
    /// <param name="hitPoint">Posición del punto de impacto</param>
    private void OnHit(Vector3 hitPoint)
    {
        /// Se activa la animación del efecto de impacto al golpear el objetivo.
        if(hitEffect != null)
        
            
            hitEffect.SetTrigger("Hit");
        
       if(gameManager != null)
        
            Invoke(nameof(Respawn), 1.5f);//Una vez que lancemos nuestra pokebola despues 1.5 segundos se llamará al método Respawn para respawnear la pokebola
            counter++;
            texto.text = $"Golpes: {counter}";//Actualizar el texto del contador de golpes cada vez que se golpea el objetivo
        }
/// <summary>
/// Solicita al ARGameManager que respawnee el objeto lanzable después de un golpe exitoso al objetivo.
/// </summary>
        public void Respawn()
    {
        gameManager.RespawnLanzable();
        
    }
    /// <summary>
    /// Reinicia el contador de golpes a cero. 
    /// </summary>
    public  void ResetValue()
    {
      counter= 0;  
    }
 
    }

