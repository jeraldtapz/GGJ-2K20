using UnityEngine;

namespace Game.Modules
{
    public class GoldManager
    {
        public int Gold { get; private set; }

        private readonly float goldPerSecond;

        private bool shouldGenerateGold = true;
        private float goldBuffer = 0f;
        private readonly WaitForUpdate waitForUpdate = null;

        public GoldManager(int initialValue, float goldPerSecond)
        {
            Gold = initialValue;
            this.goldPerSecond = goldPerSecond;
            waitForUpdate = new WaitForUpdate();
            
            AutoGenerate();
        }

        private async void AutoGenerate()
        {
            while (true)
            {
                if (shouldGenerateGold)
                {
                    goldBuffer += goldPerSecond * Time.deltaTime;

                    if (goldBuffer >= 1)
                    {
                        goldBuffer -= 1;
                        Gold++;
                    }
                }
                else
                {
                    return;
                }
                await waitForUpdate;
            }
        }

        public void Take(int value)
        {
            Gold -= value;
        }

        public void DisableGoldGeneration()
        {
            shouldGenerateGold = false;
        }

        public void EnableGoldGeneration()
        {
            shouldGenerateGold = true;
        }
    }
}