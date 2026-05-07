using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

public class ARPOZombie : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("Componentes AR")]
    

    public ARRaycastManager raycastManager;
    public ARPlaneManager planeManager;

    [Header ("Indicador visual")]
    public GameObject indicadorPrefab; 
    [Header("Prefab para colocar")] public GameObject zombiePrefab;

    [Header("GameManager")]
    public ARGMZombie gameManagerZombie;

    [Header("UI")]
    public GameObject btnPlaceObject;

    public GameObject btnRefresh;
    public GameObject btnShot;
    public GameObject targetImage;
    public GameObject textCounter;
    public GameObject detectionPlanesText;



    private GameObject indicator;
    private GameObject model;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private bool detectedPlanes = false;
    private bool setedModel = false;

    
    void Start()
    {
        btnRefresh.SetActive(false);
        btnShot.SetActive(false);
        targetImage.SetActive(false);
        textCounter.SetActive(false);
        btnPlaceObject.SetActive(false);
        detectionPlanesText.SetActive(true);

        indicator = Instantiate(indicadorPrefab);
        indicator.SetActive(false);
        planeManager.planesChanged += OnPlanesChanged;
    }

void    OnPlanesChanged(ARPlanesChangedEventArgs eventArgs)
    {
    
        if(eventArgs.added.Count > 0)
        detectedPlanes = true;
        }
    // Update is called once per frame
    void Update()
    {
        if (setedModel) return;
        if(!detectedPlanes) return;

            Vector2 screenCenter = new Vector2(Screen.width/2, Screen.height/2);
        // Lanza un raycast desde el centro de la pantalla para detectar planos en el entorno y actualiza la posición del indicador.
        if(raycastManager.Raycast(screenCenter, hits, TrackableType.PlaneWithinPolygon ))//Detecta si el raycast golpea un plano detectado
        {
            Pose hitPose = hits[0].pose;
            indicator.SetActive(true);
            //Posiciona el indicador en la posición del plano detectado y lo rota para que esté alineado con el plano, pero solo en el eje Y para que siempre esté orientado hacia la cámara.
            Debug.Log("Plano detectado en la posición: " + hitPose.position);
            indicator.transform.position = hitPose.position;
            indicator.transform.rotation = Quaternion.Euler(0f, hitPose.rotation.eulerAngles.y, 0f);
            btnPlaceObject.SetActive(true);
        }
        else
        // Si el raycast no golpea ningún plano, se oculta el indicador.
        {
            indicator.SetActive(false);
            btnPlaceObject.SetActive(false);
        }

    }
    public void PlaceModel()
    {
        
        
        if (indicator.activeSelf == false) return; // Asegura que el modelo solo se coloque si el indicador está activo.
        Vector3 spawnPos = indicator.transform.position;//   Utiliza la posición del indicador para colocar el modelo 3D
        Quaternion spawnRot = Quaternion.Euler(0f, 180f, 0f);//Rotación del modelo 3D para que esté orientado hacia la cámara 

        model = Instantiate(zombiePrefab, spawnPos, spawnRot); 

        indicator.SetActive(false);
        /// <summary> Desactiva todos los planos detectados para ahorrar recursos.
        /// </summary>
        DesactivarPlanos(); 

        setedModel = true;

        if(btnPlaceObject != null)
        {
            btnPlaceObject.SetActive(false);
        }
        if(btnShot != null)
        {
            btnShot.SetActive(true);
        }
        if(targetImage != null)
        {
            targetImage.SetActive(true);
        }
        if(textCounter != null)
        {
            textCounter.SetActive(true);
        }

        if (btnRefresh != null)
        {
            btnRefresh.SetActive(true); 
        }
    
    }

    private void DesactivarPlanos()
    {
        foreach (var plano in planeManager.trackables)
        {
            plano.gameObject.SetActive(false);
        }
    planeManager.enabled = false;
}
    private void OnDestroy()
    {
        if (planeManager != null)
        {
            planeManager.planesChanged -= OnPlanesChanged;
        }
    }

    public void ResetScene()
    {
        if(model != null)
        {
            Destroy(model);
            model = null;
        }
    if(gameManagerZombie !=null)

        {
         gameManagerZombie.ResetCounter();
         gameManagerZombie.DestroyBullet();
         gameManagerZombie.UpdateCounterText();   
        }

        planeManager.enabled = true;
        foreach(var plano in planeManager.trackables)
        {
            plano.gameObject.SetActive(true);
        }

        setedModel = false;
        if(indicator != null) indicator.SetActive(false);

        if(btnPlaceObject != null)
        {
            btnPlaceObject.SetActive(false);
        }
        if(btnShot != null)
        {
            btnShot.SetActive(false);
        }
        if(btnRefresh != null)
        {
            btnRefresh.SetActive(false); 
        }
        if(targetImage != null)
        {
            targetImage.SetActive(false);
        }
        if(textCounter != null)
        {
            textCounter.SetActive(false);
        }
        
        Debug.Log("Escena actualizada");
    }
    
    
    }
