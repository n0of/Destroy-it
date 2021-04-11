using UnityEngine;
using UnityEngine.UI;

public class CannonScript : MonoBehaviour
{
    public GameObject vfx;
    public GameObject shotPower;

    private GameScript gameScript;
    private Image shotPowerImg;
    private bool charging = false;
    private float modifier = 1;

    void Start()
    {
        shotPower.GetComponent<Canvas>().enabled = false;
        shotPowerImg = GameObject.FindGameObjectsWithTag("ShotPowerImg")[0].GetComponent<Image>();
        gameScript = GameObject.Find("GameObject").GetComponent<GameScript>();
    }

    void Update()
    {
        if (!charging)
        {
            return;
        }
        var newScale = shotPowerImg.transform.localScale.x + modifier * Time.deltaTime;
        if (newScale >= 1)
        {
            newScale = 1;
            modifier *= -1;
        }
        else if (newScale <= 0)
        {
            newScale = 0;
            modifier *= -1;
        }
        shotPowerImg.transform.localScale += new Vector3(newScale - shotPowerImg.transform.localScale.x, 0, 0);

    }

    void OnAnimationFinished()
    {
        GetComponent<Animator>().SetTrigger("Shoot");
        TriggerShoot();
    }

    private void OnMouseDown()
    {
        if (gameScript.IsReload)
        {
            return;
        }
        shotPower.GetComponent<Canvas>().enabled = true;
        charging = true;
    }

    private void OnMouseUp()
    {
        if (gameScript.IsReload)
        {
            return;
        }
        shotPower.GetComponent<Canvas>().enabled = false;
        charging = false;
        modifier = 1;
        gameScript.Shoot(shotPowerImg.transform.localScale.x);
        shotPowerImg.transform.localScale = new Vector3(0, 1, 1);
        TriggerShoot();
        Destroy(Instantiate(vfx, new Vector3(transform.position.x - 2, transform.position.y + 1, transform.position.z), Quaternion.identity), 1);
    }

    private void TriggerShoot()
    {
        GetComponent<Animator>().SetTrigger("Shoot");
    }
}
