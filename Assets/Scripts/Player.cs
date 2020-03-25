﻿using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int id;
    public string username;
    public Transform shootOrigin;
    public float moveSpeed = 5f;
    public float health;
    public float maxHealth = 100f;
    public int itemAmount = 0;
    public int maxItemAmount = 3;

    private float[] inputs;

    public void Initialize(int _id, string _username)
    {
        id = _id;
        username = _username;
        health = maxHealth;

        inputs = new float[2];
    }

    /// <summary>Processes player input and moves the player.</summary>
    public void FixedUpdate()
    {
        if (health <= 0f)
        {
            return;
        }

        MoveTransform(inputs[0], inputs[1]);
    }

    /// <summary>Calculates the player's desired movement direction and moves him.</summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    public void MoveTransform(float x, float z)
    {
        var newPosition = transform.position + (new Vector3(x, 0, z) * moveSpeed * Time.deltaTime);
        transform.position = newPosition;

        ServerSend.PlayerPosition(this);
    }

    /// <summary>Updates the player input with newly received input.</summary>
    /// <param name="_inputs">The new key inputs.</param>
    public void SetInput(float[] _inputs)
    {
        inputs = _inputs;
    }

    public void Shoot(Vector3 _viewDirection)
    {
        if (Physics.Raycast(shootOrigin.position, _viewDirection, out RaycastHit _hit, 25f))
        {
            if (_hit.collider.CompareTag("Player"))
            {
                _hit.collider.GetComponent<Player>().TakeDamage(50f);
            }
        }
    }

    public void TakeDamage(float _damage)
    {
        if (health <= 0f)
        {
            return;
        }

        health -= _damage;
        if (health <= 0f)
        {
            health = 0f;
            transform.position = new Vector3(0f, 25f, 0f);
            ServerSend.PlayerPosition(this);
            StartCoroutine(Respawn());
        }

        ServerSend.PlayerHealth(this);
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(5f);

        health = maxHealth;
        ServerSend.PlayerRespawned(this);
    }

    public bool AttemptPickupItem()
    {
        if (itemAmount >= maxItemAmount)
        {
            return false;
        }

        itemAmount++;
        return true;
    }
}
