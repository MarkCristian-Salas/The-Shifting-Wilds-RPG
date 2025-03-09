using UnityEngine;

public class ShapeshiftController : MonoBehaviour
{
    [SerializeField] GameObject humanPrefab;
    [SerializeField] GameObject bearPrefab;
    [SerializeField] GameObject tigerPrefab;
    [SerializeField] GameObject snakePrefab;
    [SerializeField] GameObject eaglePrefab;
    
    [SerializeField] private ParticleSystem shapeshiftEffect; 

    private GameObject currentModel;
    private CameraController cameraController;

    private void Awake()
    {
        currentModel = this.gameObject;
        cameraController = Camera.main.GetComponent<CameraController>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ShapeshiftToHuman();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ShapeshiftToBear();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ShapeshiftToTiger();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            ShapeshiftToSnake();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            ShapeshiftToEagle();
        }
    }

    public void ShapeshiftToHuman()
    {
        Shapeshift(humanPrefab);
    }

    public void ShapeshiftToBear()
    {
        Shapeshift(bearPrefab);
    }

    public void ShapeshiftToTiger()
    {
        Shapeshift(tigerPrefab);
    }

    public void ShapeshiftToSnake()
    {
        Shapeshift(snakePrefab);
    }

    public void ShapeshiftToEagle()
    {
        Shapeshift(eaglePrefab);
    }

    void Shapeshift(GameObject prefab)
    {
        if (prefab != null)
        {
            if (shapeshiftEffect != null)
            {
                ParticleSystem effectInstance = Instantiate(shapeshiftEffect, transform.position, Quaternion.identity);
                effectInstance.Play();
                Destroy(effectInstance.gameObject, effectInstance.main.duration);
            }

            GameObject instance = Instantiate(prefab, transform.position, transform.rotation);
            currentModel.SetActive(false);
            instance.SetActive(true);
            currentModel = instance;
            cameraController.SetTarget(instance.transform);

            if (instance.GetComponent<BearController>())
                instance.GetComponent<BearController>().ActivateBear();

            if (instance.GetComponent<TigerController>())
                instance.GetComponent<TigerController>().ActivateTiger();

            if (instance.GetComponent<SnakeController>())
                instance.GetComponent<SnakeController>().ActivateSnake();

            if (instance.GetComponent<EagleController>())
                instance.GetComponent<EagleController>().ActivateEagle();
        }
    }
}
