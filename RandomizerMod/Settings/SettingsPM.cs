using RandomizerCore.StringLogic;

namespace RandomizerMod.Settings
{
    /// <summary>
    /// ProgressionManager for interpreting logic for GenerationSettings
    /// </summary>
    public class SettingsPM : StringPM
    {
        public readonly GenerationSettings GS;

        public SettingsPM(GenerationSettings gs) => GS = gs;

        public delegate bool BoolTermResolver(string term, out bool result);
        public static event BoolTermResolver OnResolveBoolTerm;
        public delegate bool IntTermResolver(string term, out int result);
        public static event IntTermResolver OnResolveIntTerm;

        public override bool IsDefined(string atom)
        {
            if (OnResolveBoolTerm != null)
            {
                foreach (BoolTermResolver d in OnResolveBoolTerm.GetInvocationList().Cast<BoolTermResolver>())
                {
                    if (d(atom, out _)) return true;
                }
            }

            if (OnResolveIntTerm != null)
            {
                foreach (IntTermResolver d in OnResolveIntTerm.GetInvocationList().Cast<IntTermResolver>())
                {
                    if (d(atom, out _)) return true;
                }
            }

            return GetBaseField(atom).HasValue;
        }

        public override int EvaluateToInt(string atom)
        {
            return GetInt(atom);
        }

        public int GetInt(string name)
        {
            if (int.TryParse(name, out int value)) return value;
            if (OnResolveIntTerm != null)
            {
                foreach (IntTermResolver d in OnResolveIntTerm.GetInvocationList().Cast<IntTermResolver>())
                {
                    if (d(name, out int result)) return result;
                }
            }

            return GetBaseField(name) ?? throw new ArgumentException($"Unrecognized term in SettingsPM: {name}");
        }

        public bool GetBool(string name)
        {
            if (OnResolveBoolTerm != null)
            {
                foreach (BoolTermResolver d in OnResolveBoolTerm.GetInvocationList().Cast<BoolTermResolver>())
                {
                    if (d(name, out bool result)) return result;
                }
            }

            return (GetBaseField(name) ?? throw new ArgumentException($"Unrecognized term in SettingsPM: {name}")) != 0;
        }

        private int? GetBaseField(string name)
        {
            bool? flag = name switch
            {
                "PRECISEMOVEMENT" => GS.SkipSettings.PreciseMovement,
                "PROFICIENTCOMBAT" => GS.SkipSettings.ProficientCombat,
                "BACKGROUNDPOGOS" => GS.SkipSettings.BackgroundObjectPogos,
                "ENEMYPOGOS" => GS.SkipSettings.EnemyPogos,
                "OBSCURESKIPS" => GS.SkipSettings.ObscureSkips,
                "SHADESKIPS" => GS.SkipSettings.ShadeSkips,
                "INFECTIONSKIPS" => GS.SkipSettings.InfectionSkips,
                "ACIDSKIPS" => GS.SkipSettings.AcidSkips,
                "FIREBALLSKIPS" => GS.SkipSettings.FireballSkips,
                "SPIKETUNNELS" => GS.SkipSettings.SpikeTunnels,
                "DARKROOMS" => GS.SkipSettings.DarkRooms,

                "DAMAGEBOOSTS" => GS.SkipSettings.DamageBoosts,
                "DANGEROUSSKIPS" => GS.SkipSettings.DangerousSkips,
                "COMPLEXSKIPS" => GS.SkipSettings.ComplexSkips,
                "DIFFICULTSKIPS" => GS.SkipSettings.DifficultSkips,

                "ITEMRANDO" => GS.TransitionSettings.Mode == TransitionSettings.TransitionMode.None,
                "MAPAREARANDO" => GS.TransitionSettings.Mode == TransitionSettings.TransitionMode.MapAreaRandomizer,
                "FULLAREARANDO" => GS.TransitionSettings.Mode == TransitionSettings.TransitionMode.FullAreaRandomizer,
                "AREARANDO" => GS.TransitionSettings.Mode == TransitionSettings.TransitionMode.FullAreaRandomizer
                || GS.TransitionSettings.Mode == TransitionSettings.TransitionMode.MapAreaRandomizer,
                "ROOMRANDO" => GS.TransitionSettings.Mode == TransitionSettings.TransitionMode.RoomRandomizer,

                "SWIM" => !GS.NoveltySettings.RandomizeSwim,
                "ELEVATOR" => !GS.NoveltySettings.RandomizeElevatorPass,

                "2MASKS" => GS.CursedSettings.CursedMasks < 4,

                "VERTICAL" => GS.StartItemSettings.VerticalMovement != StartItemSettings.StartVerticalType.None
                    && GS.StartItemSettings.VerticalMovement != StartItemSettings.StartVerticalType.ZeroOrMore,
                _ => null,
            };
            return flag.HasValue ? flag.Value ? 1 : 0 : null;
        }
    }
}
