using FluentAssertions;
using RandomizerCore.Logic;
using RandomizerMod.RandomizerData;
using RandomizerMod.RC;
using RandomizerMod.Settings;
using Xunit.Abstractions;

namespace RandomizerModTests
{
    [Collection("Logic Collection")]
    public class LogicTests(LogicFixture Fix, ITestOutputHelper Output)
    {
        public LogicFixture Fix { get; } = Fix;
        public ITestOutputHelper Output { get; } = Output;

        [Fact]
        public void DashmasterFromWestWaterwaysStartTest()
        {
            RandoModContext ctx = Fix.GetContext("West Waterways");
            LogicManager lm = ctx.LM;
            ProgressionManager pm = new(lm, ctx);
            Fix.UpdateWaypointsAndVanillaTransitions(pm);
            lm.GetLogicDefStrict("Dashmaster").CanGet(pm).Should().BeTrue();
        }

        [Fact]
        public void CanStagFromWestWaterwaysStartTest()
        {
            GenerationSettings gs = new();
            gs.StartLocationSettings.StartLocation = "West Waterways";
            gs.SkipSettings.EnemyPogos = true;
            gs.SkipSettings.ShadeSkips = true;
            gs.SkipSettings.PreciseMovement = true;
            RandoModContext ctx = Fix.GetContext(gs);
            LogicManager lm = ctx.LM;
            ProgressionManager pm = new(lm, ctx);
            Fix.UpdateWaypointsAndVanillaTransitions(pm);
            lm.GetLogicDefStrict("Can_Stag").CanGet(pm).Should().BeTrue();
        }
    }
}
