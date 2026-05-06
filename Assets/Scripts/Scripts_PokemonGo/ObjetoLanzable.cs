using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;
using System;

public class ObjetoLanzable : MonoBehaviour
{
    [Header("Configuracion de lanzamiento")]
    public float fuerzaLanzamientoMultiplicador = 10f;
    public float distanciaSwipMin = 50f;
    public float velocidadSeguimiento = 15f;
    public float distanciaSeparacion = 0.5f;

    private Rigidbody rb;
    private Camera arCamara;
    private ARRaycastManager raycastManager;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    //Control de touch
    private bool isDragging = false;
    private Vector2 touchEmpiezaPos;
    private Vector2 touchTerminaPos;
    private float touchEmpiezaTiempo;

    //Calcular swipe
    private Vector2 prevPosicionToque;
    private Vector3 prevPosMundo;
    private Vector3 velocidadSwipe;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        arCamara = Camera.main;
        raycastManager = FindFirstObjectByType<ARRaycastManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount == 0) return;

        Touch touch = Input.GetTouch(0);
        
        if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            return;
        
        switch (touch.phase)
        {
            case TouchPhase.Began:
                isDragging = true;
                touchEmpiezaPos = touch.position;
                break;

            case TouchPhase.Ended:
                if (isDragging)
                {
                    touchTerminaPos = touch.position;
                    isDragging = false;
                    Vector2 swipeDelta = touchTerminaPos - touchEmpiezaPos;
                    if(swipeDelta.magnitude >= distanciaSwipMin)
                        LaunchObject(swipeDelta);
                }
                break;
        }
    }
    
    void LaunchObject(Vector2 swipeDelta)
    {
        transform.SetParent(null);
        rb.isKinematic = false;
        rb.useGravity = true;

        float lateralNormalizado = swipeDelta.x / Screen.width; // Posicion horizontal del swipe normalizada entre -1 y 1, donde -1 es un swipe completamente a la izquierda, 0 es un swipe vertical y 1 es un swipe completamente a la derecha.

        float fuerzaMagnitud = Mathf.Clamp(swipeDelta.magnitude, distanciaSwipMin, 600f);//Clamp restringe la magnitud del swipe entre un valor mínimo (distanciaSwipMin) y un valor máximo (600f) para evitar que el lanzamiento sea demasiado débil o demasiado fuerte. //Cuidado con learp que puede suavizar demasiado la fuerza y hacer que el lanzamiento no responda bien a swipes rápidos.
        float fuerzaNormalizada = fuerzaMagnitud / 600f;

        Vector3 direccion = arCamara.transform.forward //Foward= eje z
                            + arCamara.transform.up * 0.6f //Up eje y
                            + arCamara.transform.right * lateralNormalizado; //Right eje x
        
        Vector3 fuerzaFinal = direccion.normalized
                              * fuerzaNormalizada
                              * fuerzaLanzamientoMultiplicador;
        
        rb.AddForce(fuerzaFinal, ForceMode.Impulse);// F/m = impulso instanteneo
        //hace giro random para efecto de fuerza
        // insideUnitSphere = genera un vector3 aleatorio con un rango entre 0 y 1
        rb.AddTorque(UnityEngine.Random.insideUnitSphere*3f, ForceMode.Impulse);
        this.enabled = false;
    }

    Vector3 GetWorldPositionFromTouch(Vector2 posPantalla)
    {
        if(raycastManager != null && raycastManager.Raycast(posPantalla, hits, TrackableType.PlaneWithinPolygon))
        {
            Vector3 planePos = hits[0].pose.position;

            return planePos + Vector3.up * 0.1f;
        }

        Ray ray = arCamara.ScreenPointToRay(posPantalla);
        return ray.GetPoint(distanciaSeparacion);
    }
}
