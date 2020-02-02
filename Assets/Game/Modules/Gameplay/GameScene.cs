using System;
using System.Collections.Generic;
using Core.Common;
using Game.Modules;
using Gravity;
using Modules.General;
using Modules.SceneSystem;
using Modules.Towers;
using Modules.Units;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Modules.Game
{
    public class GameScene : Scene
    {
        [SerializeField] private SpawnManager spawnManager = null;
        [SerializeField] private TowerManager towerManager = null;
        [SerializeField] private string debugUnitId = string.Empty;
        [SerializeField] private string debugAttackUnitId = "Attacker";
        [SerializeField] private int initialGoldValue = 100;
        [SerializeField] private Transform enemyNexusTransform = null;
        [SerializeField] private Dictionary<string, UnitData> unitDataMap = null;

        private List<BaseUnit> leftUnits;
        private List<BaseUnit> rightUnits;
        
        private GoldManager goldManager = null;

        private async void Awake()
        {
            await InitializeAsync();
        }

        public override void Initialize(object data = null)
        {
            base.Initialize(data);
            
            leftUnits  = new List<BaseUnit>();
            rightUnits = new List<BaseUnit>();
            goldManager = new GoldManager(initialGoldValue, 7.5f);
            
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

        private void Lose()
        {
            print($"Lose!");
        }

        private void Win()
        {
            print($"Win!");
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.J))
                SpawnDebugUnitsLeft(1);
            if(Input.GetKeyDown(KeyCode.K))
                SpawnDebugUnitsRight(1);
            
            if(Input.GetKeyDown(KeyCode.X))
                SpawnAttackUnitsRight(1);
            
            if(Input.GetKeyDown(KeyCode.Z))
                SpawnAttackUnitsLeft(1);
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

                rightUnits.Add(towerTargetingUnit);
                await new WaitForSeconds(0.5f);
            }
        }
    }
}
