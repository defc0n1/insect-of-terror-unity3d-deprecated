﻿/*
 * BossHero - script for the Hero fighting the boss, that little purple uniformed man of might.
 * 
 */
using UnityEngine;
using System.Collections;

public class BossHero : MonoBehaviour {

    public float get_up_delay = 1.2f;
    public float speed = 1.0f;
    public float fire_delay = 0.2f;
    public float poo_speed = 1.4f;
    public float orig_fire_delay;
    public float orig_poo_speed;

    public GameObject poo_prefab;

    private bool fallen = false;
    private bool allow_fire = true;
    private Sprite my_sprite;

	// Use this for initialization
	void Start () {
        my_sprite = GetComponent<SpriteRenderer>().sprite;
        Vector3 new_pos = new Vector3(InterestingGameStuff.left + my_sprite.bounds.size.x / 2.0f, 0, 0);
        transform.position = new_pos;

        orig_fire_delay = fire_delay;
        orig_poo_speed = poo_speed;
	}
	
	void FixedUpdate () {
        if (!fallen)
        {
            Vector3 mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouse_pos.z = 0;
            transform.position = Vector3.MoveTowards(transform.position, mouse_pos, speed * Time.fixedDeltaTime);
        }
	}

    void Update()
    {
        if (Input.GetButton("Fire1"))
        {
            StartCoroutine("Fire");
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Enemy")
        {
            FallDown();
        }
    }

    // Goes into 'paralyzed' fallen down state, invokes a delayed 'get up' functions
    void FallDown()
    {
        fallen = true;

        Quaternion new_rot = Quaternion.LookRotation(new Vector3(5, 0, 0), Vector3.forward);
        new_rot.x = new_rot.y = 0.0f;
        transform.rotation = new_rot;

        CancelInvoke("GetUp"); // start fresh!
        Invoke("GetUp", get_up_delay);
    }

    void GetUp()
    {
        fallen = false;
        transform.rotation = Quaternion.identity;
    }

    IEnumerator Fire()
    {
        if (allow_fire && !fallen)
        {
            allow_fire = false;
            SpawnPoo();
            yield return new WaitForSeconds(fire_delay);
            allow_fire = true;
        }
    }

    void SpawnPoo()
    {
        GameObject new_poo = Instantiate(poo_prefab, transform.position, Quaternion.identity) as GameObject;
        BossPoo script = new_poo.GetComponent<BossPoo>();
        script.Initialize(poo_speed);

        new_poo.transform.position += new Vector3(my_sprite.bounds.size.x / 2.0f, 0, 0);
    }

    public void UpgradeWeapon()
    {
        fire_delay = orig_fire_delay;
        fire_delay *= 0.2f;
        poo_speed = orig_poo_speed;
        poo_speed *= 1.3f;

        const float howlong = 2.0f;
        CancelInvoke("DowngradeWeapon");
        Invoke("DowngradeWeapon", howlong);
    }

    void DowngradeWeapon()
    {
        fire_delay = orig_fire_delay;
        poo_speed = orig_poo_speed;
    }
}
