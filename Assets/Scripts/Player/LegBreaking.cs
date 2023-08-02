using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegBreaking : MonoBehaviour
{
    public static LegBreaking Instance;

    [SerializeField] private GameObject legs;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject legPivot;
    [SerializeField] private GameObject explodePrefab;
    public bool done = false;
    Rigidbody2D rbody;
    Vector3 eulerAngleVelocity;
    // Start is called before the first frame update
    void Awake()
    {
        legs = GameObject.Find("LegsToExplode");
        player = GameObject.FindWithTag("Player");
        legPivot = GameObject.Find("LegPivot");
        rbody = player.GetComponent<Rigidbody2D>();
        rbody.angularVelocity = 600f;
    }

    private void Start()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        DontDestroyOnLoad(this);
    }
    // Update is called once per frame
    void Update()
    {
        //AudioManager.Instance.PlaySFX("explode", transform.position);
        if (done || !legPivot || !player)
            return;


        if(SimpleController.Instance.groundCheck)
        {
            legs.GetComponent<ExplodeOnAwake>().explode(legs);
            done = true;
            //player.transform.position = legPivot.transform.position;
            GameObject explosion = Instantiate(explodePrefab, player.transform.position, Quaternion.identity);
            Destroy(explosion, explosion.GetComponent<ParticleSystem>().main.duration);

            AudioManager.Instance.PlaySFX("player_damaged" + Random.Range(1, 5), player.transform.position);
            AudioManager.Instance.PlaySFX("explode", player.transform.position);
            gameObject.SetActive(false);
        }
        else
        {
            legPivot.transform.position = player.transform.position;
            legPivot.transform.rotation = player.transform.rotation;
        }
    }
}
