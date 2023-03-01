using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Attackable : Interactable
{
    public int Health;
    public int MaxHealth;
    public Vector3 uiOffset;
    private GameObject _healthDisplay;
    private HealthBar _healthBar;
    public List<Radmin> RadminAssigned = new List<Radmin>();
    public List<Radmin> RadminAttacking = new List<Radmin>();
    bool IsAlive = true;
    public List<GameObject> ThingsToSpawnOnDeath;
    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        UpdateDisplayedHealth();
        base.Update();
    }
    private void OnTriggerEnter(Collider other)
    {
        Radmin radmin = other.gameObject.GetComponent<Radmin>();
        if (radmin != null && (radmin.State == RadminState.MovingToAttack || radmin.State == RadminState.InAir))
        {
            StartAttacking(radmin);
        }
        else if (radmin != null && radmin.State == RadminState.Idle)
        {
            StartAttacking(radmin);
        }
    }
    private void UpdateDisplayedHealth()
    {
        if (Health != MaxHealth && Health != 0)
        {
            if (_healthDisplay == null)
            {
                _healthDisplay = PrefabRetrieverScript.BasicHealthBar;
                _healthDisplay = Instantiate(_healthDisplay, PrefabRetrieverScript.Canvas);
                _healthDisplay.SetActive(true);
                _healthBar = _healthDisplay.GetComponent<HealthBar>();
            }
            _healthBar.MaxHealth = MaxHealth;
            _healthBar.Health = Health;
            _healthBar.transform.position = Camera.main.WorldToScreenPoint(transform.position + uiOffset);
        }
        else if (_healthDisplay != null)
        {
            Destroy(_healthDisplay);
        }
    }
    public override void FixedUpdate()
    {
        if (Health > 0)
        {
            foreach (Radmin radmin in RadminAttacking)
            {
                if (radmin.Ticker % radmin.AttackSpeed == 0)
                {
                    Health -= radmin.Damage;
                }
            }
        }
        else if (RadminAssigned.Count > 0)
        {
            ReleaseRadmin();
        }
        if (Health <= 0 && IsAlive)
            Killed();
        base.FixedUpdate();
    }

    public override void AssignRadmin(Radmin radmin)
    {
        base.AssignRadmin(radmin);
        if (RadminAssigned.Contains(radmin))
            return;
        RadminAssigned.Add(radmin);
        radmin.State = RadminState.MovingToAttack;
        radmin.agent.SetDestination(transform.position);
        radmin.agent.stoppingDistance = 0;
    }
    private void StartAttacking(Radmin radmin)
    {
        if (!RadminAttacking.Contains(radmin))
        {
            if (!RadminAssigned.Contains(radmin))
                RadminAssigned.Add(radmin);
            radmin.Objective = this;
            radmin.State = RadminState.Attacking;
            RadminAttacking.Add(radmin);
            radmin.SetToTransform(transform);
            radmin.transform.DOLookAt(new Vector3(transform.position.x, radmin.transform.position.y, transform.position.z), .2f);
        }
    }

    public override void ReleaseRadmin()
    {
        RadminAttacking.Clear();
        while (RadminAssigned.Count > 0)
        {
            Radmin radmin = RadminAssigned[0];
            RadminAssigned.Remove(radmin);
            radmin.SetIdle();
        }
        base.ReleaseRadmin();
    }

    public override void ReleaseRadmin(Radmin radmin)
    {
        if (RadminAttacking.Contains(radmin))
            RadminAttacking.Remove(radmin);
        if (RadminAssigned.Contains(radmin))
        {
            RadminAssigned.Remove(radmin);
            radmin.SetIdle();
        }
        base.ReleaseRadmin(radmin);
    }

    public override bool IsInteractable()
    {
        return IsAttackable();
    }

    public override void Initialize()
    {
        base.Initialize();
    }

    public virtual void Killed()
    {
        Health = 0;
        IsAlive = false;
        foreach (GameObject obj in ThingsToSpawnOnDeath)
        {
            var spawnedObj = Instantiate(obj, new Vector3(transform.position.x, transform.position.y + 2, transform.position.z), new Quaternion());
            spawnedObj.SetActive(true);
        }
        ReleaseRadmin();
        DestroyImmediate(_healthDisplay);
        DestroyImmediate(gameObject);
    }

    public virtual bool IsAttackable()
    {
        return IsAlive;
    }
}