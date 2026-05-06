using UnityEngine;
/// <summary>
/// Se encarga de gestionar el objeto lanzable en el juego, permitiendo su aparición, desaparición y respawn.
/// </summary>
/// <remarks>
/// 
public class ARGameManager : MonoBehaviour
{
    [Header("Objeto Lanzable")]
    /// <summary>Prefab del objeto lanzable que se lanzará en el juego.
    /// </summary>
    public GameObject prefabLanzable;
/// <summary>
/// Desplazamiento del spawn del objeto lanzable respecto a la posición de la cámara para que aparezca frente al jugador.
/// </summary>
    public Vector3 spawnOffset = new Vector3(0f, 0f, -0.4f); 
    private Camera arCamera;
    private GameObject actualLanzable;// Posición de spawn a 2 metros frente a la cámara
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    /// <summary>
    /// Localiza la cámara principal del dispositivo para usarla como referencia para el spawn del objeto lanzable.
    /// </summary>
    void Start()
    {
       arCamera = Camera.main; 
    }

    // Update is called once per frame
    /// <summary>
    /// Crea una inistancia del objeto lanzable en la posición de spawn cada vez que se llama a este método.
    /// </summary>
   public void SpawnThrowable()
    {
        // Si ya existe un objeto lanzable en la escena, lo destruye antes de crear uno nuevo para evitar múltiples objetos lanzables al mismo tiempo.
        if(actualLanzable != null)
        {
            Destroy(actualLanzable);


        }
        /// Calcula la posición de spawn sumando la posición de la cámara con el offset definido para que el objeto aparezca frente al jugador.
        Vector3 spawnPosition=arCamera.transform.position //Sumar la poscion de la cámara con el offset para obtener la posición de spawn
        + arCamera.transform.TransformDirection(spawnOffset);

        actualLanzable = Instantiate(prefabLanzable, spawnPosition, Quaternion.identity, arCamera.transform); // Instanciar el objeto lanzable como hijo de la cámara para que se mueva con ella.

    }
    /// <summary>
    /// Reinicia el objeto lanzable a su posición de spawn original, destruyendo el objeto actual y creando uno nuevo en la posición de spawn.
    /// </summary>
    /// <remarks> Llama a este método desde el botón de UI para respawnear el objeto lanzable
     /// </remarks>
    public void RespawnLanzable() 
    {
        SpawnThrowable();
    }
/// <summary>
/// Destruye el objeto lanzable actual.
/// </summary> 
/// <remarks> Llama a este método desde el botón de UI para eliminar el objeto lanzable de la escena
/// </remarks>
    public void DestroyThrowable()
    {
        if(actualLanzable != null)
        {
            Destroy(actualLanzable);
            actualLanzable = null;
        }
    }
}
