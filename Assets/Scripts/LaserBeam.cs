using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LaserBeam : MonoBehaviour
{
    [SerializeField] GameObject player;
    private GameObject closestWp;
    private bool isTeleported;

    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] GameManager gm;
    [SerializeField] Camera playerCamera;
    [SerializeField] Transform laserSpawn;

    [SerializeField] AudioSource tpRealSound;
    [SerializeField] AudioSource tpDarkSound;

    public List<Transform> realTP = new List<Transform>();
    public List<Transform> upsideTP = new List<Transform>();
    public int tpCounter;
    public bool playerOnUpside;

    public static LaserBeam Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        //Disparar poder
        //if (Input.GetMouseButtonDown(0) && _fireTimer > fireRate && gm.canShoot)
        //{
        //    //ActivatePower();
        //}
    }

    void ActivatePower()
    {
        switch (gm.state)
        {
            case PowerStates.OnLaser:
                //ShootLaser();
                break;
            case PowerStates.OnDimension:
                NewTeleport();
                break;
            default:
                break;
        }
    }

    void NewTeleport()
    {
        //if (!GameManager.Instance.ableToTeleport || isTeleporting)
        //    return;
        StartCoroutine(TeleportCooldown());

        var currentPos = player.transform.position;

        Vector3 newPos = new Vector3(currentPos.x, currentPos.y, currentPos.z + (isTeleported ? -50 : 50));

        Transform closestWp = null;
        float closestDistance = Mathf.Infinity;

        foreach (var wp in GameManager.Instance.TPWaypoints)
        {
            float distance = Vector3.Distance(newPos, wp.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestWp = wp.transform;
            }
        }

        if (closestWp != null)
        {
            player.transform.position = closestWp.position;
        }
        else
        {
            player.transform.position = newPos;
        }

        isTeleported = !isTeleported;

        StartCoroutine(ResetTeleporting());
    }

    IEnumerator ResetTeleporting()
    {
        yield return new WaitForSeconds(0.2f);
    }

    void TeleportPlayer()
    {
        //if (!GameManager.Instance.ableToTeleport)
        //    return;

        StartCoroutine(TeleportCooldown());

        if (!playerOnUpside)
        {
            transform.parent.position = upsideTP[tpCounter].position;
            tpRealSound.Play();
        }
        else
        {
            transform.parent.position = realTP[tpCounter].position;
            tpDarkSound.Play();
        }

        playerOnUpside = !playerOnUpside;
    }

    IEnumerator TeleportCooldown()
    {
        //GameManager.Instance.ableToTeleport = false;
        yield return new WaitForSeconds(1f);
        //GameManager.Instance.ableToTeleport = true;
    }

    //void ShootLaser()
    //{
    //    laserSound.Play();
    //    Vector3 rayOirigin = playerCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
    //    RaycastHit hit;

    //    if (Physics.Raycast(rayOirigin, playerCamera.transform.forward, out hit, gunRange, ~limit4th))
    //    {
    //        var interactor = hit.collider.GetComponent<Interactor>();
    //        if (interactor != null)
    //        {
    //            interactor.Interact();
    //        }

    //        var teleportable = hit.collider.GetComponent<ITeleportable>();
    //        if (teleportable != null)
    //        {
    //            teleportable.Interact();
    //        }
    //    }
    //    else
    //    {
    //        hit.point = rayOirigin + playerCamera.transform.forward * 20f;
    //    }

    //    StartCoroutine(ShootLaserCor(hit.point));
    //    StartCoroutine(ShootTimer());
    //}

    //IEnumerator ShootLaserCor(Vector3 hit)
    //{
    //    float elapsedTime = 0;
    //    lineRenderer.enabled = true;
    //    while (elapsedTime <= 0.05f)
    //    {
    //        lineRenderer.SetPosition(0, laserSpawn.position);
    //        Vector3 rayOirigin = playerCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
    //        lineRenderer.SetPosition(1, hit);
    //        yield return new WaitForEndOfFrame();
    //        elapsedTime += Time.deltaTime;
    //    }
    //    lineRenderer.enabled = false;
    //}
}
