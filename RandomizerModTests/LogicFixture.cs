using RandomizerCore.Logic;
using RandomizerCore.Logic.StateLogic;
using RandomizerMod.RandomizerData;
using RandomizerMod.RC;
using RandomizerMod.Settings;

namespace RandomizerModTests
{
    public class LogicFixture
    {
        public LogicManager LM { get; }

        private RandoModContext Default_CTX { get; }

        public LogicFixture()
        {
            Data.Load();
            Default_CTX = new(new(), Data.GetStartDef("King's Pass"));
            Default_CTX.notchCosts.AddRange(CharmNotchCosts._vanillaCosts);
            LM = Default_CTX.LM;
        }

        public RandoModContext GetContext()
        {
            return new(Default_CTX);
        }

        public RandoModContext GetContext(string startName)
        {
            StartDef start = Data.GetStartDef(startName) ?? throw new KeyNotFoundException($"Start {startName} does not exist.");
            GenerationSettings gs = new();
            gs.StartLocationSettings.StartLocation = startName;
            RandoModContext ctx = new(gs, start);
            ctx.notchCosts.AddRange(CharmNotchCosts._vanillaCosts);
            return ctx;
        }

        public RandoModContext GetContext(GenerationSettings gs)
        {
            StartDef start = Data.GetStartDef(gs.StartLocationSettings.StartLocation) ?? throw new KeyNotFoundException($"Start {gs.StartLocationSettings.StartLocation} does not exist.");
            RandoModContext ctx = new(gs, start);
            ctx.notchCosts.AddRange(CharmNotchCosts._vanillaCosts);
            return ctx;
        }

        public ProgressionManager GetProgressionManager()
        {
            return new(LM, new RandoModContext(Default_CTX));
        }

        public ProgressionManager GetProgressionManager(Dictionary<string, int> pmFieldValues)
        {
            ProgressionManager pm = GetProgressionManager();
            foreach (var kvp in pmFieldValues) pm.Set(kvp.Key, kvp.Value);
            return pm;
        }

        public void UpdateWaypointsAndVanillaTransitions(ProgressionManager pm)
        {
            LogicManager lm = pm.lm;
            pm.mu.AddWaypoints(lm.Waypoints);
            pm.mu.AddTransitions(lm.TransitionLookup.Values);
            pm.mu.AddPlacements(Data.Transitions.Where(kvp => kvp.Value.VanillaTarget is not null).Select(kvp => new RandomizerCore.GeneralizedPlacement(lm.GetTransitionStrict(kvp.Value.VanillaTarget), lm.GetTransitionStrict(kvp.Key))));
            pm.mu.StartUpdating();
        }

        public LazyStateBuilder GetState(Dictionary<string, int> stateFieldValues)
        {
            LazyStateBuilder lsb = new(LM.StateManager.DefaultState);
            foreach (var kvp in stateFieldValues)
            {
                StateField sf = LM.StateManager.FieldLookup[kvp.Key];
                if (sf is StateBool) lsb.SetBool(sf, kvp.Value == 1);
                else lsb.SetInt(sf, kvp.Value);
            }
            return lsb;
        }

        public LazyStateBuilder GetStateP(params (string, int)[] vals) => GetState(vals.ToDictionary(p => p.Item1, p => p.Item2));

    }

    [CollectionDefinition("Logic Collection")]
    public class LMCollection : ICollectionFixture<LogicFixture>
    {

    }
}