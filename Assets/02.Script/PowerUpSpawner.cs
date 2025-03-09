using Fusion;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpSpawner : SimulationBehaviour
{
    private void Start()
    {
        RegisterOnRunner(); // ✅ 자동으로 `NetworkRunner`에 등록

    }
    // Local list of prefabs for the available power ups to be spawned.
    [SerializeField] private List<NetworkObject> _availablePowerUps = new List<NetworkObject>();

    private float _spawnDelay = 3f;

    public void RegisterOnRunner()
    {
        // Find the network runner for this gameobject scene. This is useful on a scene object.
        var runner = NetworkRunner.GetRunnerForGameObject(gameObject);

        // Make sure the network runner is started and running.
        if (runner.IsRunning)
        {
            runner.AddGlobal(this);
        }
    }

    public void RemoveFromRunner()
    {
        // The same process can be done to remove the SimulationBehaviour.
        var runner = NetworkRunner.GetRunnerForGameObject(gameObject);
        if (runner.IsRunning)
        {
            runner.RemoveGlobal(this);
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (Runner.Tick % Mathf.RoundToInt(Runner.TickRate * _spawnDelay) == 0)
        {
            // Generate a random index to select a power up from the list.
            int randomIndex = Random.Range(0, _availablePowerUps.Count);
            // Spawn the selected power up.
            Runner.Spawn(_availablePowerUps[randomIndex]);
        }
    }
}