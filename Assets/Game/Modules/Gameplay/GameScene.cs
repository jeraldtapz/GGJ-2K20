using System;
using System.Collections.Generic;
using Core.Common;
using Game.Modules;
using Gravity;
using Modules.General;
using Modules.Towers;
using Modules.Units;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Scene = Modules.SceneSystem.Scene;

namespace Modules.Game
{
    public class GameScene : Scene
    {
        [SerializeField] private TextMeshProUGUI goldValue = null;
        [SerializeField] private SpawnManager spawnManager = null;
        [SerializeField] private TowerManager towerManager = null;
        [SerializeField] private string debugUnitId = string.Empty;
        [SerializeField] private string debugAttackUnitId = "Attacker";
        [SerializeField] private int initialGoldValue = 100;
        [SerializeField] private Transform enemyNexusTransform = null;
        [SerializeField] private Dictionary<string, UnitData> unitDataMap = null;
        [SerializeField] private TextMeshProUGUI winLabel = null;
        [SerializeField] private GameObject winPanel = null;

        private List<BaseUnit> leftUnits;
        private List<BaseUnit> rightUnits;
        
        private GoldManager goldManager = null;
        private bool shouldSpawn = true;
        private float spawnTimer = 0f;

        private async void Awake()
        {
            await InitializeAsync();
            SpawnInIntervals();
        }
        
        private async void SpawnInIntervals()
        {
            float randomTimer = Random.Range(7, 9f);
            SpawnAttackUnitsLeft(2);
            SpawnAttackUnitsRight(1);
            
            while (shouldSpawn)
            {
                spawnTimer += Time.deltaTime;

                if (spawnTimer >= randomTimer)
                {
                    spawnTimer = 0f;
                    randomTimer = Random.Range(6f, 8f);
                    int quantity = Random.Range(0, 2);
                    for (int i = 0; i < quantity; i++)
                    {
                        if (Random.Range(0f, 1f) > 0.5f)
                        {
                            SpawnAttackUnitsRight(1);
                        }
                        else
                        {
                            SpawnAttackUnitsLeft(1);
                        }

                        await new WaitForSeconds(0.5f);
                    }
                }
                await new WaitForUpdate();
            }
        }

        public override void Initialize(object data = null)
        {
            base.Initialize(data);
            
            leftUnits  = new List<BaseUnit>();
            rightUnits = new List<BaseUnit>();
            goldManager = new GoldManager(initialGoldValue, 4f);
            
            Disposables.Add(towerManager.OnTowerDestroyed.Subscribe(x =>
            {
                if (towerManager.AreAllTowersDestroyed())
                {
                    Lose();
                    return;
                }
                else if (towerManager.AreAllTowersRepaired())
                {
                    Win();
                    return;
                }
                
                switch (x.TowerId)
                {
                    case "TowerA":
                        FlipRightAttackingUnits();
                        break;
                    
                    case "TowerB":
                        ForwardRightAttackingUnits();
                        break;
                    
                    case "TowerC":
                        FlipLeftAttackingUnits();
                        break;
                    
                    case "TowerD":
                        ForwardLeftAttackingUnits();
                        break;
                }
            }));
            
            Disposables.Add(towerManager.OnTowerRepaired.Subscribe(x =>
            {
                if (towerManager.AreAllTowersDestroyed())
                {
                    Lose();
                    return;
                }
                else if (towerManager.AreAllTowersRepaired())
                {
                    Win();
                    return;
                }

                switch (x.TowerId)
                {
                    case "TowerA":
                        ForwardLeftRepairingUnits();
                        break;
                
                    case "TowerB":
                        FlipLeftRepairingUnits();
                        break;
                
                    case "TowerC":
                        ForwardRightRepairingUnits();
                        break;
                
                    case "TowerD":
                        FlipRightRepairingUnits();
                        break;
                }
            }));
            
        }

        private void FlipRightAttackingUnits()
        {
            List<BaseUnit> unitsToRemove = new List<BaseUnit>();
            
            foreach (BaseUnit baseUnit in rightUnits)
            {
                if (baseUnit.Role == UnitRole.Attack)
                {
                    TowerTargetingUnit rightUnit = (TowerTargetingUnit) baseUnit;
                    rightUnit.SetDirection(Direction.Left);
                    rightUnit.SetTargetTower(towerManager.GetTowerToAttack(Direction.Left));
                    unitsToRemove.Add(baseUnit);
                }
            }
            
            foreach (BaseUnit baseUnit in unitsToRemove)
            {
                rightUnits.Remove(baseUnit);
                leftUnits.Add(baseUnit);
            }
        }
        
        private void FlipLeftAttackingUnits()
        {
            List<BaseUnit> unitsToRemove = new List<BaseUnit>();
            
            foreach (BaseUnit baseUnit in leftUnits)
            {
                if (baseUnit.Role == UnitRole.Attack)
                {
                    TowerTargetingUnit leftUnit = (TowerTargetingUnit) baseUnit;
                    leftUnit.SetDirection(Direction.Right);
                    leftUnit.SetTargetTower(towerManager.GetTowerToAttack(Direction.Right));
                    unitsToRemove.Add(baseUnit);
                }
            }

            foreach (BaseUnit baseUnit in unitsToRemove)
            {
                leftUnits.Remove(baseUnit);
                rightUnits.Add(baseUnit);
            }
        }

        private void ForwardRightAttackingUnits()
        {
            foreach (BaseUnit baseUnit in rightUnits)
            {
                if (baseUnit.Role == UnitRole.Attack)
                {
                    TowerTargetingUnit rightUnit = (TowerTargetingUnit) baseUnit;
                    rightUnit.SetDirection(Direction.Right);
                    rightUnit.SetTargetTower(towerManager.GetTowerToAttack(Direction.Right));
                }
            }
        }

        private void ForwardLeftAttackingUnits()
        {
            foreach (BaseUnit baseUnit in leftUnits)
            {
                if (baseUnit.Role == UnitRole.Attack)
                {
                    TowerTargetingUnit leftUnit = (TowerTargetingUnit) baseUnit;
                    leftUnit.SetDirection(Direction.Left);
                    leftUnit.SetTargetTower(towerManager.GetTowerToAttack(Direction.Left));
                }
            }
        }

        private void FlipRightRepairingUnits()
        {
            List<BaseUnit> unitsToRemove = new List<BaseUnit>();
            
            foreach (BaseUnit baseUnit in rightUnits)
            {
                if (baseUnit.Role == UnitRole.Repair)
                {
                    TowerTargetingUnit rightUnit = (TowerTargetingUnit) baseUnit;
                    rightUnit.SetDirection(Direction.Left);
                    rightUnit.SetTargetTower(towerManager.GetTowerToDefend(Direction.Left));
                    unitsToRemove.Add(baseUnit);
                }
            }

            foreach (BaseUnit baseUnit in unitsToRemove)
            {
                rightUnits.Remove(baseUnit);
                leftUnits.Add(baseUnit);
            }
        }
        
        private void FlipLeftRepairingUnits()
        {
            List<BaseUnit> unitsToRemove = new List<BaseUnit>();
            
            foreach (BaseUnit baseUnit in leftUnits)
            {
                if (baseUnit.Role == UnitRole.Repair)
                {
                    TowerTargetingUnit leftUnit = (TowerTargetingUnit) baseUnit;
                    leftUnit.SetDirection(Direction.Right);
                    leftUnit.SetTargetTower(towerManager.GetTowerToDefend(Direction.Right));
                    unitsToRemove.Add(baseUnit);
                }
            }
            
            foreach (BaseUnit baseUnit in unitsToRemove)
            {
                leftUnits.Remove(baseUnit);
                rightUnits.Add(baseUnit);
            }
        }

        private void ForwardRightRepairingUnits()
        {
            foreach (BaseUnit baseUnit in rightUnits)
            {
                if (baseUnit.Role == UnitRole.Repair)
                {
                    TowerTargetingUnit rightUnit = (TowerTargetingUnit) baseUnit;
                    rightUnit.SetDirection(Direction.Right);
                    rightUnit.SetTargetTower(towerManager.GetTowerToDefend(Direction.Right));
                }
            }
        }

        private void ForwardLeftRepairingUnits()
        {
            foreach (BaseUnit baseUnit in leftUnits)
            {
                if (baseUnit.Role == UnitRole.Repair)
                {
                    TowerTargetingUnit leftUnit = (TowerTargetingUnit) baseUnit;
                    leftUnit.SetDirection(Direction.Left);
                    leftUnit.SetTargetTower(towerManager.GetTowerToDefend(Direction.Left));
                }
            }
        }

        private async void Lose()
        {
            DestroyEverything();
            winPanel.SetActive(true);
            winLabel.text = "You Lose!";
            print($"Lose!");
            await new WaitForSeconds(3f);
            SceneManager.LoadScene(0);
        }

        private async void Win()
        {
            DestroyEverything();
            winPanel.SetActive(true);
            winLabel.text = "You Win!!";
            print($"Win!");

            await new WaitForSeconds(3f);
            SceneManager.LoadScene(0);
        }

        private void DestroyEverything()
        {
            foreach (BaseUnit rightUnit in rightUnits)
            {
                rightUnit.gameObject.SetActive(false);
            }

            rightUnits.Clear();

            foreach (BaseUnit leftUnit in leftUnits)
            {
                leftUnit.gameObject.SetActive(false);
            }

            leftUnits.Clear();
        }

        private void Update()
        {
            goldValue.text = goldManager.Gold.ToString();
            
            if(Input.GetKeyDown(KeyCode.J))
                SpawnDebugUnitsLeft(1);
            if(Input.GetKeyDown(KeyCode.K))
                SpawnDebugUnitsRight(1);
            
            if(Input.GetKeyDown(KeyCode.X))
                SpawnAttackUnitsRight(1);
            
            if(Input.GetKeyDown(KeyCode.Z))
                SpawnAttackUnitsLeft(1);
        }

        public void SpawnLeft()
        {
            if (goldManager.Gold > 20)
            {
                SpawnDebugUnitsLeft(1);
                goldManager.Take(20);
            }
        }

        public void SpawnRight()
        {
            if (goldManager.Gold > 20)
            {
                SpawnDebugUnitsRight(1);
                goldManager.Take(20);
            }
        }


        [Button]
        private async void SpawnDebugUnitsLeft(int quantity)
        {
            for (int i = 0; i < quantity; i++)
            {
                TowerTargetingUnit towerTargetingUnit = spawnManager.Spawn(debugUnitId).GetComponent<TowerTargetingUnit>();
                towerTargetingUnit.SetData(unitDataMap[towerTargetingUnit.UnitId]);
                towerTargetingUnit.Initialize();
                towerTargetingUnit.SetDirection(Direction.Left);
                towerTargetingUnit.SetTargetTower(towerManager.GetTowerToDefend(Direction.Left));
                towerTargetingUnit.GetComponent<UnitGravityBody>().SetReference(transform);
                towerTargetingUnit.SetTowerManager(towerManager);
                
                leftUnits.Add(towerTargetingUnit);
                await new WaitForSeconds(0.5f);
            }
        }
        
        [Button]
        private async void SpawnDebugUnitsRight(int quantity)
        {
            for (int i = 0; i < quantity; i++)
            {
                TowerTargetingUnit towerTargetingUnit =spawnManager.Spawn(debugUnitId).GetComponent<TowerTargetingUnit>();
                
                towerTargetingUnit.SetData(unitDataMap[towerTargetingUnit.UnitId]);
                towerTargetingUnit.Initialize();
                towerTargetingUnit.SetDirection(Direction.Right);
                towerTargetingUnit.SetTargetTower(towerManager.GetTowerToDefend(Direction.Right));
                towerTargetingUnit.GetComponent<UnitGravityBody>().SetReference(transform);
                towerTargetingUnit.SetTowerManager(towerManager);
                
                rightUnits.Add(towerTargetingUnit);
                await new WaitForSeconds(0.5f);
            }
        }
        
        [Button]
        private async void SpawnAttackUnitsRight(int quantity)
        {
            for (int i = 0; i < quantity; i++)
            {
                TowerTargetingUnit towerTargetingUnit =spawnManager.Spawn(debugAttackUnitId, false).GetComponent<TowerTargetingUnit>();
                
                towerTargetingUnit.SetData(unitDataMap[towerTargetingUnit.UnitId]);
                towerTargetingUnit.Initialize();
                towerTargetingUnit.SetDirection(Direction.Right);
                towerTargetingUnit.SetTargetTower(towerManager.GetTowerToAttack(Direction.Right));
                towerTargetingUnit.GetComponent<UnitGravityBody>().SetReference(transform);
                towerTargetingUnit.SetTowerManager(towerManager);
                
                rightUnits.Add(towerTargetingUnit);
                await new WaitForSeconds(0.5f);
            }
        }
        
        [Button]
        private async void SpawnAttackUnitsLeft(int quantity)
        {
            for (int i = 0; i < quantity; i++)
            {
                TowerTargetingUnit towerTargetingUnit =spawnManager.Spawn(debugAttackUnitId, false).GetComponent<TowerTargetingUnit>();
                
                towerTargetingUnit.SetData(unitDataMap[towerTargetingUnit.UnitId]);
                towerTargetingUnit.Initialize();
                towerTargetingUnit.SetDirection(Direction.Left);
                towerTargetingUnit.SetTargetTower(towerManager.GetTowerToAttack(Direction.Left));
                towerTargetingUnit.GetComponent<UnitGravityBody>().SetReference(transform);
                towerTargetingUnit.SetTowerManager(towerManager);

                leftUnits.Add(towerTargetingUnit);
                await new WaitForSeconds(0.5f);
            }
        }
    }
}
