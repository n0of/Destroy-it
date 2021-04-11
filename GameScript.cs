using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameScript : MonoBehaviour
{
    public GameObject reloadCamera;
    public GameObject shotCamera;
    public GameObject bullet;
    public GameObject enemy1;
    public GameObject enemy2;
    public GameObject enemy3;
    public GameObject floor;
    public GameObject cannon;
    public GameObject menuUI;
    public GameObject title;
    public bool IsReload { get; private set; }

    private List<GameObject> bullets = new List<GameObject>();
    private GameObject enemy;
    private bool finished;

    void Start()
    {
        Restart();
    }

    void Update()
    {
        if (finished)
        {
            return;
        }
        var hit = false;
        var inactiveBullets = 0;
        foreach (var bullet in bullets)
        {
            if (enemy.GetComponent<BoxCollider>().bounds.Intersects(bullet.GetComponent<SphereCollider>().bounds))
            {
                hit = true;
            }
            if (bullet.GetComponent<BulletScript>().IsInactive())
            {
                inactiveBullets++;
            }
        }
        if (hit || inactiveBullets == 3)
        {
            finished = true;
            title.GetComponent<Text>().text = hit ? "Victory!" : "Defeat!";
            if (hit)
            {
                foreach (var block in GameObject.FindGameObjectsWithTag("SimpleCollectible"))
                {
                    if (block.GetComponent<Rigidbody>() == null)
                    {
                        block.AddComponent<BoxCollider>();
                        var gameObjectsRigidBody = block.AddComponent<Rigidbody>();
                        gameObjectsRigidBody.mass = 5;
                    }
                }
            }
            menuUI.SetActive(true);
        }
    }

    public void ToReload()
    {
        if (finished)
        {
            return;
        }
        IsReload = true;
        reloadCamera.GetComponent<Camera>().enabled = true;
        shotCamera.GetComponent<Camera>().enabled = false;
    }

    public void ToShot()
    {
        IsReload = false;
        shotCamera.GetComponent<Camera>().enabled = true;
        reloadCamera.GetComponent<Camera>().enabled = false;
    }

    public void Shoot(float strength)
    {
        foreach (var bullet in bullets)
        {
            var script = bullet.GetComponent<BulletScript>();
            if (script.IsLoaded())
            {
                script.Shoot(strength);
            }
        }
    }

    public GameObject GetFloor()
    {
        return floor;
    }

    public GameObject GetCannon()
    {
        return cannon;
    }

    public void Restart()
    {
        finished = false;
        var random = new System.Random();
        var enemyId = random.Next(3);
        foreach (var bullet in bullets)
        {
            Destroy(bullet);
        }
        Destroy(enemy);
        bullets = new List<GameObject>();
        enemy = null;
        bullets.Add(Instantiate(bullet, new Vector3(11.5f, 0.9f, 2f), Quaternion.identity));
        bullets.Add(Instantiate(bullet, new Vector3(9.5f, 0.9f, 2f), Quaternion.identity));
        bullets.Add(Instantiate(bullet, new Vector3(10.5f, 0.9f, 2.5f), Quaternion.identity));
        enemy = Instantiate(enemyId == 0 ? enemy1 : enemyId == 1 ? enemy2 : enemy3, new Vector3(-10f, 0.5f, 0f), Quaternion.AngleAxis(-90, new Vector3(0, 1, 0)));
        ToReload();
        menuUI.SetActive(false);
    }
}
