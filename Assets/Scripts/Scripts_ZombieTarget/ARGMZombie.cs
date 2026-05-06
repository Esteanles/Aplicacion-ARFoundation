
using System;
using TMPro;
using UnityEditor.ShortcutManagement;
using UnityEngine;
using UnityEngine.XR.ARCore;

public class ARGMZombie : MonoBehaviour
{
    public GameObject throwablePrefab;
    public TextMeshProUGUI counterText;
    public float bulletforce = 10f;
    private int counter = 0;
    private Camera arCamera;

    private GameObject currentThrowable;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        arCamera = Camera.main;
        if(counterText != null)
        counterText.text = "Daño al zombie: 0";
    }

    public void CountCollision(int hitvalue)
    {
       counter = counter + hitvalue;
       Debug.Log($"Counter: {counter}");
       UpdateCounterText();
    }

    public void UpdateCounterText()
    {
        if (counterText != null)
        
            counterText.text = $"Daño al zombie:  {counter.ToString()}";
        
    }

    public void ResetCounter()
    {
        counter = 0;
        UpdateCounterText();
    }

    public void SpawnBullet()
    {
    if (currentThrowable != null){
DestroyBullet();
    }
    Ray ray = arCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

    Vector3 spawnPosition = ray.origin;
    Debug.Log($"Spawn position: {spawnPosition}");
    currentThrowable = Instantiate(throwablePrefab, spawnPosition, Quaternion.identity);

    Rigidbody rb = currentThrowable.GetComponent<Rigidbody>();
    rb.linearVelocity = ray.direction * bulletforce;
    }
    public void DestroyBullet()
    {
        
            Destroy(currentThrowable);
            currentThrowable = null;
        }
}
