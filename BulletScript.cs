using UnityEngine;

public class BulletScript : MonoBehaviour
{
    private Vector3 prevInputPos;
    private GameScript gameScript;
    private float maxX;
    private float maxZ;
    private BulletState state = BulletState.Idle;
    private Vector3 inCannonPos;
    private Vector3? endPos;

    private readonly float shotHeight = 0.3f;

    void Start()
    {
        gameScript = GameObject.Find("GameObject").GetComponent<GameScript>();
        maxX = gameScript.GetFloor().GetComponent<BoxCollider>().bounds.size.x / 2;
        maxZ = gameScript.GetFloor().GetComponent<BoxCollider>().bounds.size.z / 2;
        var cannon = gameScript.GetCannon();
        inCannonPos = new Vector3(cannon.transform.position.x, 0.9f, cannon.transform.position.z);
    }

    void Update()
    {
        if (state != BulletState.Shot || !endPos.HasValue)
        {
            return;
        }
        var newX = transform.position.x - 30f * Time.deltaTime;
        var yAdd = Mathf.Sin(Mathf.PI * (newX - endPos.Value.x)/(inCannonPos.x - endPos.Value.x));
        var currentPos = new Vector3(newX, inCannonPos.y + yAdd, transform.position.z);
        transform.position = currentPos;
        if (currentPos.x <= endPos.Value.x)
        {
            endPos = null;
            gameScript.ToReload();
        }
    }

    public void Shoot(float strength)
    {
        state = BulletState.Shot;
        endPos = new Vector3(inCannonPos.x + strength * (-maxX - inCannonPos.x), inCannonPos.y, inCannonPos.z);
    }

    public bool IsLoaded()
    {
        return state == BulletState.Loaded;
    }

    public bool IsInactive()
    {
        return state == BulletState.Shot && !endPos.HasValue;
    }

    private void OnMouseDown()
    {
        if (!gameScript.IsReload || state != BulletState.Idle)
        {
            return;
        }
        transform.position = new Vector3(transform.position.x, transform.position.y + shotHeight, transform.position.z);
        prevInputPos = GetDragPosition();
    }

    private void OnMouseDrag()
    {
        if (!gameScript.IsReload || state != BulletState.Idle)
        {
            return;
        }
        var nextInputPos = GetDragPosition();
        var mouseMove = prevInputPos - nextInputPos;
        mouseMove.y = 0;
        var newPosition = transform.position - mouseMove;
        newPosition.x = Mathf.Max(Mathf.Min(newPosition.x, maxX), -maxX / 2);
        newPosition.z = Mathf.Max(Mathf.Min(newPosition.z, maxZ / 2), -maxZ / 2);
        transform.position = newPosition;
        prevInputPos = nextInputPos;
    }

    private void OnMouseUp()
    {
        if (!gameScript.IsReload || state != BulletState.Idle)
        {
            return;
        }
        if (gameScript.GetCannon().GetComponent<BoxCollider>().bounds.Intersects(GetComponent<SphereCollider>().bounds))
        {
            transform.position = inCannonPos;
            state = BulletState.Loaded;
            gameScript.ToShot();
        }
        else
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - shotHeight, transform.position.z);
        }
    }

    private Vector3 GetDragPosition()
    {
        var plane = new Plane(Vector3.up, transform.position);
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        plane.Raycast(ray, out var distance);
        return ray.GetPoint(distance);
    }

    private enum BulletState
    {
        Idle,
        Loaded,
        Shot
    }
}

