using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;
/// <summary>
/// Se encarga de detectar los planos y coloca el modelo 3d en la posición del indicador.
/// Además de la interacción UI para colocar el modelo y resetear la escena.
/// También se comunica con el ARGameManager para mostrar el objeto lanzable una vez que el modelo 3D ha sido colocado.
/// </summary>

public class ARPlacementObject : MonoBehaviour
{
    [Header("Componentes AR")]
    /// <summary>/ Lanza el raycast para detectar planos en el entorno
    /// </summary
    public ARRaycastManager raycastManager; 
        /// <summary>Permite detectar planos en el entorno
        /// </summary>
    public ARPlaneManager planeManager;

    [Header("Indicador Visual")] 
    /// <summary>Prefab del indicador visual que muestra dónde se colocará el modelo 3D
    /// </summary>
    public GameObject indicadorPrefab;
    [Header("Modelos 3D")] 
    /// <summary>Prefab del modelo 3D que se colocará en la escena.
    /// </summary>
    public GameObject modelo3D;
    [Header("UI")]
    /// <summary>Botón de UI para colocar el modelo 3D en la posición del indicador.
    /// </summary>
    public GameObject boton;
    /// <summary>Botón de UI para resetear la escena y volver a detectar planos.
    /// </summary>
    public GameObject botonReset;
    [Header("Game Manager")]
/// <summary>Referencia al ARGameManager para mostrar el objeto lanzable una vez que el modelo 3D ha sido colocado.
    public ARGameManager gameManager;
    /*Variables Privadas*/
    private GameObject indicador;

    private GameObject modelo;

    private List <ARRaycastHit> hits = new List<ARRaycastHit>();

    private bool planosDetectados = false;

    private bool modeloColocado = false;
// <summary>
/// <summary>
/// Inicia el indicador visual y se suscribe al evento planesChanged del ARPlaneManager para detectar cambios en los planos del entorno.
/// </summary>
    void Start()
    {
        indicador = Instantiate(indicadorPrefab);
        indicador.SetActive(false);
        planeManager.planesChanged += OnPlanesChanged;
    }
/// <summary>
/// Se llama cada vez que el ARPlaneManager detecta cambios en los planos del entorno.
/// </summary>
/// <param name="args">Argumentos que contienen la información sobre los cambios en los planos</param>
    void OnPlanesChanged(ARPlanesChangedEventArgs args)
    {
        if(args.added.Count > 0)
            planosDetectados = true;
    }

    // Update is called once per frame
    /// <summary> Actualiza la posicion del indicador en el centro de la pantalla.
    void Update()
    {
        // Si el modelo ya ha sido colocado o no se han detectado planos, no se actualiza el indicador
        if (modeloColocado == true) return;
        if (planosDetectados == false) return;
        //  Calcula el centro de la pantalla para lanzar el raycast desde esa posición
        Vector2 screenCenter = new Vector2(Screen.width/2, Screen.height/2);
        // Lanza un raycast desde el centro de la pantalla para detectar planos en el entorno y actualiza la posición del indicador.
        if(raycastManager.Raycast(screenCenter, hits, TrackableType.PlaneWithinPolygon ))//Detecta si el raycast golpea un plano detectado
        {
            Pose hitPose = hits[0].pose;
            indicador.SetActive(true);
            //Posiciona el indicador en la posición del plano detectado y lo rota para que esté alineado con el plano, pero solo en el eje Y para que siempre esté orientado hacia la cámara.
            Debug.Log("Plano detectado en la posición: " + hitPose.position);
            indicador.transform.position = hitPose.position;
            indicador.transform.rotation = Quaternion.Euler(0f, hitPose.rotation.eulerAngles.y, 0f);
        }
        else
        // Si el raycast no golpea ningún plano, se oculta el indicador.
        {
            indicador.SetActive(false);
        }
    }
    /// <summary> Coloca el modelo 3D en la posición del indicador y desactiva los planos para que el modelo permanezca fijo en esa posición.
    /// </summary>
    /// <remarks>Llama a este método desde el botón de UI.
    ///</remarks>
    public void PlaceModel()
    {
        
    
        
        if (indicador.activeSelf == false) return; // Asegura que el modelo solo se coloque si el indicador está activo.
        Vector3 spawnPos = indicador.transform.position;//   Utiliza la posición del indicador para colocar el modelo 3D
        Quaternion spawnRot = Quaternion.Euler(0f, 180f, 0f);//Rotación del modelo 3D para que esté orientado hacia la cámara 

        modelo = Instantiate(modelo3D, spawnPos, spawnRot); 

        indicador.SetActive(false);
        /// <summary> Desactiva todos los planos detectados para ahorrar recursos.
        /// </summary>
        DesactivarPlanos(); 

        modeloColocado = true;

        if(boton != null)
        {
            boton.SetActive(false);
        }

        if(gameManager != null)
        {
            gameManager.SpawnThrowable();
        } 
        

        if (botonReset != null)
        {
            botonReset.SetActive(true); 
        }
    }
/// <summary>
/// Desactiva todos los planos detectados para ahorrar recursos.
/// </summary>

    private void DesactivarPlanos()
    {
        foreach (var plano in planeManager.trackables)
        {
            plano.gameObject.SetActive(false);
        }
        planeManager.enabled = false;
    }
/// <summary>
/// Limpia la suscripción al evento planesChanged del ARPlaneManager para evitar posibles errores o fugas de memoria.
/// </summary>
    private void OnDestroy()
    {
        if (planeManager != null)
        {
            planeManager.planesChanged -= OnPlanesChanged;
        }
    }
/// <summary> Actualiza la escena para volver a detectar planos y colocar el modelo 3D nuevamente.
/// </summary>
    public void ResetScene()
    {
        if (modelo != null)
        {
            Destroy(modelo);
            modelo = null; 
        }

        
        if(gameManager != null)
        {
            gameManager.DestroyThrowable(); 
        }
        

        planeManager.enabled = true;
        planeManager.planesChanged += OnPlanesChanged;

        foreach(var plano in planeManager.trackables)
        {
            plano.gameObject.SetActive(true);
        }

        planosDetectados = true;
        modeloColocado = false;

        indicador.SetActive(true);

        if(boton != null)
        {
            boton.SetActive(true);
        }

        if(botonReset != null)
        {
            botonReset.SetActive(true);
        }
    }
}

    
