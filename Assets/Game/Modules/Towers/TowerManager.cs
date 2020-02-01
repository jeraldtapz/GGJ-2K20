using System;
using Core.Common;
using Modules.General;
using UnityEngine;

namespace Modules.Towers
{
    public class TowerManager : CompositeInitializableBehaviour
    {
        [SerializeField] private TowerBehaviour towerA = null;
        [SerializeField] private TowerBehaviour towerB = null;
        [SerializeField] private TowerBehaviour towerC = null;
        [SerializeField] private TowerBehaviour towerD = null;

        private Subject<TowerBehaviour> onTowerDestroyed;
        private Subject<TowerBehaviour> onTowerRepaired;

        public IObservable<TowerBehaviour> OnTowerDestroyed => onTowerDestroyed;
        public IObservable<TowerBehaviour> OnTowerRepaired => onTowerRepaired;

        public override void Initialize(object data = null)
        {
            base.Initialize(data);
            
            onTowerDestroyed = new Subject<TowerBehaviour>();
            onTowerRepaired = new Subject<TowerBehaviour>();
            
            Disposables.Add(towerA.OnHealthChanged.Subscribe(x =>
            {
                if(x >= 100)
                    onTowerRepaired.OnNext(towerA);
                else if(x <= 0)
                    onTowerDestroyed.OnNext(towerA);
            }));
            
            Disposables.Add(towerB.OnHealthChanged.Subscribe(x =>
            {
                if(x >= 100)
                    onTowerRepaired.OnNext(towerB);
                else if(x <= 0)
                    onTowerDestroyed.OnNext(towerB);
            }));
            
            Disposables.Add(towerC.OnHealthChanged.Subscribe(x =>
            {
                if(x >= 100)
                    onTowerRepaired.OnNext(towerC);
                else if(x <= 0)
                    onTowerDestroyed.OnNext(towerC);
            }));
            
            Disposables.Add(towerD.OnHealthChanged.Subscribe(x =>
            {
                if(x >= 100)
                    onTowerRepaired.OnNext(towerD);
                else if(x <= 0)
                    onTowerDestroyed.OnNext(towerD);
            }));
        }


        public TowerBehaviour GetTowerToAttack(Direction dir)
        {
            if (dir == Direction.Left)
            {
                if (towerD.CurrentHealth > 0)
                    return towerD;

                if (towerC.CurrentHealth > 0)
                    return towerC;

                if (towerB.CurrentHealth > 0)
                    return towerB;

                if (towerA.CurrentHealth > 0)
                    return towerA;
                
                Debug.Log($"All towers are dead! Enemies Win!");
                return null;
            }
            else
            {
                
                if (towerB.CurrentHealth > 0)
                    return towerB;

                if (towerA.CurrentHealth > 0)
                    return towerA;
                
                if (towerD.CurrentHealth > 0)
                    return towerD;

                if (towerC.CurrentHealth > 0)
                    return towerC;
                
                Debug.Log($"All towers are dead! Enemies Win!");
                return null;
            }
        }

        public TowerBehaviour GetTowerToDefend(Direction dir)
        {
            if (dir == Direction.Left)
            {
                if (towerA.CurrentHealth < 100)
                    return towerA;

                if (towerB.CurrentHealth < 100)
                    return towerB;

                if (towerC.CurrentHealth < 100)
                    return towerC;

                if (towerD.CurrentHealth < 100)
                    return towerD;
                
                Debug.Log($"You have won! All towers");
                return null;
            }
            else
            {
                if (towerC.CurrentHealth < 100)
                    return towerC;

                if (towerD.CurrentHealth < 100)
                    return towerD;
                
                if (towerA.CurrentHealth < 100)
                    return towerA;

                if (towerB.CurrentHealth < 100)
                    return towerB;
                
                Debug.Log($"You have won! All towers");
                return null;
            }
        }

        public bool AreAllTowersDestroyed()
        {
            return towerA.CurrentHealth <= 0 && towerB.CurrentHealth <= 0
                                             && towerC.CurrentHealth <= 0 && towerD.CurrentHealth <= 0;
        }
        
        public bool AreAllTowersRepaired()
        {
            return towerA.CurrentHealth >= 100 && towerB.CurrentHealth >= 100
                                             && towerC.CurrentHealth >= 100 && towerD.CurrentHealth >= 100;
        }
        
    }
}