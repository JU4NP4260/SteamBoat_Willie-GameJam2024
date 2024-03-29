using UnityEngine;
using Unity.Mathematics;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;
using System.Collections;

public class CatBomb : MonoBehaviour
{
    [SerializeField] private GameObject bomb;
    [SerializeField] private GameObject box;
    [SerializeField] private GameObject bombom;// Assign your prefab in the Inspector
    [SerializeField] private float firerate;
    [SerializeField] private float boxFirerate;
    private float timer;
    private float boxTimer;
    public LayerMask wallLayer;
    public bool isShredded = false;
    [SerializeField] private Image cDBombBG;
    [SerializeField] private Image cDSuperBombBG;
    [SerializeField] private Image cDBoxBG;

    private AudioManager audioManager;

    private void Start()
    {
        timer = firerate;
        boxTimer = boxFirerate;

        audioManager = FindFirstObjectByType<AudioManager>();
    }
    void Update()
    {
        timer -= Time.deltaTime;
        boxTimer -= Time.deltaTime;

        if (timer <= 0 && Input.GetMouseButtonDown(1))
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = 10f; // Set this to be the distance you want the object to be placed in front of the camera
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

            audioManager.Play_click_SFX();

            //Debug.Log(IsOverlappingWithObstacles(worldPosition, wallLayer));
            if (!IsOverlappingWithObstacles(worldPosition, wallLayer)) // Right click
            {               
                if (isShredded)
                {
                    Instantiate(bombom, worldPosition, Quaternion.identity);
                    audioManager.Play_catSuperBomb_drop_SFX();
                }
                else
                {
                    Instantiate(bomb, worldPosition, Quaternion.identity);
                    audioManager.Play_CatBomb_Drop();
                    StartCoroutine(WaitForExplosion(1.4f));

                }
            }
            
            timer = firerate;
        }

        if (timer > 0 && !isShredded)
        {
            cDBombBG.fillAmount = 1-(timer / firerate);
        }
        else if (timer > 0 && isShredded)
        {
            cDSuperBombBG.fillAmount = 1-(timer / firerate);
        }

        if (boxTimer <= 0 && Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = 10f; // Set this to be the distance you want the object to be placed in front of the camera
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            audioManager.Play_click_SFX();

            if (!IsOverlappingWithObstacles(worldPosition, wallLayer) && isShredded) // Right click
            {
                Instantiate(box, worldPosition, Quaternion.identity);
                audioManager.Play_catBox_drop_SFX();
            }           
            boxTimer = boxFirerate;
        }
        if (boxTimer > 0)
        {
            cDBoxBG.fillAmount = 1-(boxTimer / boxFirerate);
        }
    }

    bool IsOverlappingWithObstacles(Vector2 position, LayerMask currentLayer)
    {
        // Check if there are any colliders at the specified position
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, 0.5f, currentLayer);

        // If there are no colliders, return false (no overlap)
        return colliders.Length > 0;
    }

    private Vector3 AproximatePosition(Vector3 currentPosition)
    {
        // Aqu� es donde se usa el m�todo math.round para redondear a el n�mero entero m�s cercano

        Vector3 aproximatePos = new Vector3(math.round(currentPosition.x), math.round(currentPosition.y));

        return aproximatePos;
    }

    private IEnumerator WaitForExplosion(float time)
    {
        yield return new WaitForSeconds(time);
        audioManager.Play_bombExplotion_SFX();

    }
}
