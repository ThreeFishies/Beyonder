using System;
using Trainworks.Managers;
using Void.Champions;
using Void.Chaos;
using Void.Init;
using Void.Monsters;

namespace ShinyShoe.Loading
{
    // Token: 0x02000649 RID: 1609
    public class LoadChaos : LoadingTask
    {
        // Token: 0x0600386B RID: 14443 RVA: 0x000D5215 File Offset: 0x000D3415
        public LoadChaos(LoadingScreen.DisplayStyle screenStyle, Action doneCallback) : base(screenStyle, doneCallback)
        {
        }

        // Token: 0x0600386C RID: 14444 RVA: 0x000D533C File Offset: 0x000D353C
        public override void StartTask(LoadingScreen loadingScreen)
        {
            base.StartTask(loadingScreen);
            if (ProviderManager.SaveManager.HasMainClass() && (ProviderManager.SaveManager.GetMainClass() == Beyonder.BeyonderClanData || ProviderManager.SaveManager.GetSubClass() == Beyonder.BeyonderClanData))
            {
                ChaosManager.Shuffle(RngId.NonDeterministic);
                LocoMotive.BuildTreeForNewRun(RngId.NonDeterministic);
                Epidemial.BuildTreeForNewRun(RngId.NonDeterministic);
                BeyonderSaveManager.SaveData();
            }
            base.SetDone();
            loadingScreen.TryToDoNextTask();
        }
    }

    public class ReplayChaos : LoadingTask
    {
        // Token: 0x0600386B RID: 14443 RVA: 0x000D5215 File Offset: 0x000D3415
        public ReplayChaos(LoadingScreen.DisplayStyle screenStyle, Action doneCallback) : base(screenStyle, doneCallback)
        {
        }

        // Token: 0x0600386C RID: 14444 RVA: 0x000D533C File Offset: 0x000D353C
        public override void StartTask(LoadingScreen loadingScreen)
        {
            base.StartTask(loadingScreen);
            if (ProviderManager.SaveManager.HasMainClass() && (ProviderManager.SaveManager.GetMainClass() == Beyonder.BeyonderClanData || ProviderManager.SaveManager.GetSubClass() == Beyonder.BeyonderClanData))
            {
                BeyonderSaveManager.SaveData();
                ChaosManager.UpdateStartingUpgrades(BeyonderSaveManager.CurrentRunSetupData.BoonsBanesData);
                LocoMotive.BuildTreeForNewRun(RngId.NonDeterministic, false);
                Epidemial.BuildTreeForNewRun(RngId.NonDeterministic, false);
            }
            base.SetDone();
            loadingScreen.TryToDoNextTask();
        }
    }
}
