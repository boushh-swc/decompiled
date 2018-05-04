using Net.RichardLord.Ash.Core;
using StaRTS.GameBoard.Components;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Models.Entities
{
	public class SmartEntity : Entity
	{
		public AreaTriggerComponent AreaTriggerComp;

		public ArmoryComponent ArmoryComp;

		public AssetComponent AssetComp;

		public AttackerComponent AttackerComp;

		public BarracksComponent BarracksComp;

		public BoardItemComponent BoardItemComp;

		public BuildingAnimationComponent BuildingAnimationComp;

		public BuildingComponent BuildingComp;

		public ChampionComponent ChampionComp;

		public CivilianComponent CivilianComp;

		public ClearableComponent ClearableComp;

		public DamageableComponent DamageableComp;

		public DefenderComponent DefenderComp;

		public DefenseLabComponent DefenseLabComp;

		public DroidComponent DroidComp;

		public DroidHutComponent DroidHutComp;

		public SquadBuildingComponent SquadBuildingComp;

		public NavigationCenterComponent NavigationCenterComp;

		public FactoryComponent FactoryComp;

		public CantinaComponent CantinaComp;

		public FleetCommandComponent FleetCommandComp;

		public FollowerComponent FollowerComp;

		public GameObjectViewComponent GameObjectViewComp;

		public GeneratorComponent GeneratorComp;

		public GeneratorViewComponent GeneratorViewComp;

		public HealerComponent HealerComp;

		public HealthComponent HealthComp;

		public TroopShieldComponent TroopShieldComp;

		public TroopShieldViewComponent TroopShieldViewComp;

		public TroopShieldHealthComponent TroopShieldHealthComp;

		public HealthViewComponent HealthViewComp;

		public HQComponent HQComp;

		public KillerComponent KillerComp;

		public LootComponent LootComp;

		public MeterShaderComponent MeterShaderComp;

		public OffenseLabComponent OffenseLabComp;

		public PathingComponent PathingComp;

		public SecondaryTargetsComponent SecondaryTargetsComp;

		public ShieldBorderComponent ShieldBorderComp;

		public ShieldGeneratorComponent ShieldGeneratorComp;

		public SizeComponent SizeComp;

		public ShooterComponent ShooterComp;

		public StarportComponent StarportComp;

		public StateComponent StateComp;

		public StorageComponent StorageComp;

		public SupportComponent SupportComp;

		public SupportViewComponent SupportViewComp;

		public TacticalCommandComponent TacticalCommandComp;

		public ChampionPlatformComponent ChampionPlatformComp;

		public TeamComponent TeamComp;

		public TrackingComponent TrackingComp;

		public TrackingGameObjectViewComponent TrackingGameObjectViewComp;

		public TransformComponent TransformComp;

		public TransportComponent TransportComp;

		public TroopComponent TroopComp;

		public TurretBuildingComponent TurretBuildingComp;

		public TurretShooterComponent TurretShooterComp;

		public WalkerComponent WalkerComp;

		public WallComponent WallComp;

		public BuffComponent BuffComp;

		public TrapComponent TrapComp;

		public TrapViewComponent TrapViewComp;

		public HousingComponent HousingComp;

		public ScoutTowerComponent ScoutTowerComp;

		public SpawnComponent SpawnComp;

		protected override Entity AddComponentAndDispatchAddEvent(ComponentBase comp, Type compCls)
		{
			bool flag = false;
			if (comp is AreaTriggerComponent)
			{
				this.AreaTriggerComp = (AreaTriggerComponent)comp;
				flag = true;
			}
			if (comp is ArmoryComponent)
			{
				this.ArmoryComp = (ArmoryComponent)comp;
				flag = true;
			}
			if (comp is AssetComponent)
			{
				this.AssetComp = (AssetComponent)comp;
				flag = true;
			}
			if (comp is AttackerComponent)
			{
				this.AttackerComp = (AttackerComponent)comp;
				flag = true;
			}
			if (comp is BarracksComponent)
			{
				this.BarracksComp = (BarracksComponent)comp;
				flag = true;
			}
			if (comp is BoardItemComponent)
			{
				this.BoardItemComp = (BoardItemComponent)comp;
				flag = true;
			}
			if (comp is BuildingAnimationComponent)
			{
				this.BuildingAnimationComp = (BuildingAnimationComponent)comp;
				flag = true;
			}
			if (comp is BuildingComponent)
			{
				this.BuildingComp = (BuildingComponent)comp;
				flag = true;
			}
			if (comp is ChampionComponent)
			{
				this.ChampionComp = (ChampionComponent)comp;
				flag = true;
			}
			if (comp is CivilianComponent)
			{
				this.CivilianComp = (CivilianComponent)comp;
				flag = true;
			}
			if (comp is ClearableComponent)
			{
				this.ClearableComp = (ClearableComponent)comp;
				flag = true;
			}
			if (comp is DamageableComponent)
			{
				this.DamageableComp = (DamageableComponent)comp;
				flag = true;
			}
			if (comp is DefenderComponent)
			{
				this.DefenderComp = (DefenderComponent)comp;
				flag = true;
			}
			if (comp is DefenseLabComponent)
			{
				this.DefenseLabComp = (DefenseLabComponent)comp;
				flag = true;
			}
			if (comp is DroidComponent)
			{
				this.DroidComp = (DroidComponent)comp;
				flag = true;
			}
			if (comp is DroidHutComponent)
			{
				this.DroidHutComp = (DroidHutComponent)comp;
				flag = true;
			}
			if (comp is SquadBuildingComponent)
			{
				this.SquadBuildingComp = (SquadBuildingComponent)comp;
				flag = true;
			}
			if (comp is NavigationCenterComponent)
			{
				this.NavigationCenterComp = (NavigationCenterComponent)comp;
				flag = true;
			}
			if (comp is FactoryComponent)
			{
				this.FactoryComp = (FactoryComponent)comp;
				flag = true;
			}
			if (comp is CantinaComponent)
			{
				this.CantinaComp = (CantinaComponent)comp;
				flag = true;
			}
			if (comp is FleetCommandComponent)
			{
				this.FleetCommandComp = (FleetCommandComponent)comp;
				flag = true;
			}
			if (comp is FollowerComponent)
			{
				this.FollowerComp = (FollowerComponent)comp;
				flag = true;
			}
			if (comp is GameObjectViewComponent)
			{
				this.GameObjectViewComp = (GameObjectViewComponent)comp;
				flag = true;
			}
			if (comp is GeneratorComponent)
			{
				this.GeneratorComp = (GeneratorComponent)comp;
				flag = true;
			}
			if (comp is GeneratorViewComponent)
			{
				this.GeneratorViewComp = (GeneratorViewComponent)comp;
				flag = true;
			}
			if (comp is HealerComponent)
			{
				this.HealerComp = (HealerComponent)comp;
				flag = true;
			}
			if (comp is TroopShieldComponent)
			{
				this.TroopShieldComp = (TroopShieldComponent)comp;
				flag = true;
			}
			if (comp is TroopShieldViewComponent)
			{
				this.TroopShieldViewComp = (TroopShieldViewComponent)comp;
				flag = true;
			}
			if (comp is TroopShieldHealthComponent)
			{
				this.TroopShieldHealthComp = (TroopShieldHealthComponent)comp;
				flag = true;
			}
			if (comp is HealthComponent)
			{
				this.HealthComp = (HealthComponent)comp;
				flag = true;
			}
			if (comp is HealthViewComponent)
			{
				this.HealthViewComp = (HealthViewComponent)comp;
				flag = true;
			}
			if (comp is HQComponent)
			{
				this.HQComp = (HQComponent)comp;
				flag = true;
			}
			if (comp is KillerComponent)
			{
				this.KillerComp = (KillerComponent)comp;
				flag = true;
			}
			if (comp is LootComponent)
			{
				this.LootComp = (LootComponent)comp;
				flag = true;
			}
			if (comp is MeterShaderComponent)
			{
				this.MeterShaderComp = (MeterShaderComponent)comp;
				flag = true;
			}
			if (comp is OffenseLabComponent)
			{
				this.OffenseLabComp = (OffenseLabComponent)comp;
				flag = true;
			}
			if (comp is PathingComponent)
			{
				this.PathingComp = (PathingComponent)comp;
				flag = true;
			}
			if (comp is SecondaryTargetsComponent)
			{
				this.SecondaryTargetsComp = (SecondaryTargetsComponent)comp;
				flag = true;
			}
			if (comp is ShieldBorderComponent)
			{
				this.ShieldBorderComp = (ShieldBorderComponent)comp;
				flag = true;
			}
			if (comp is ShieldGeneratorComponent)
			{
				this.ShieldGeneratorComp = (ShieldGeneratorComponent)comp;
				flag = true;
			}
			if (comp is SizeComponent)
			{
				this.SizeComp = (SizeComponent)comp;
				flag = true;
			}
			if (comp is ShooterComponent)
			{
				this.ShooterComp = (ShooterComponent)comp;
				flag = true;
			}
			if (comp is StarportComponent)
			{
				this.StarportComp = (StarportComponent)comp;
				flag = true;
			}
			if (comp is StateComponent)
			{
				this.StateComp = (StateComponent)comp;
				flag = true;
			}
			if (comp is StorageComponent)
			{
				this.StorageComp = (StorageComponent)comp;
				flag = true;
			}
			if (comp is SupportComponent)
			{
				this.SupportComp = (SupportComponent)comp;
				flag = true;
			}
			if (comp is SupportViewComponent)
			{
				this.SupportViewComp = (SupportViewComponent)comp;
				flag = true;
			}
			if (comp is TacticalCommandComponent)
			{
				this.TacticalCommandComp = (TacticalCommandComponent)comp;
				flag = true;
			}
			if (comp is ChampionPlatformComponent)
			{
				this.ChampionPlatformComp = (ChampionPlatformComponent)comp;
				flag = true;
			}
			if (comp is TeamComponent)
			{
				this.TeamComp = (TeamComponent)comp;
				flag = true;
			}
			if (comp is TrackingComponent)
			{
				this.TrackingComp = (TrackingComponent)comp;
				flag = true;
			}
			if (comp is TrackingGameObjectViewComponent)
			{
				this.TrackingGameObjectViewComp = (TrackingGameObjectViewComponent)comp;
				flag = true;
			}
			if (comp is TransformComponent)
			{
				this.TransformComp = (TransformComponent)comp;
				flag = true;
			}
			if (comp is TransportComponent)
			{
				this.TransportComp = (TransportComponent)comp;
				flag = true;
			}
			if (comp is TroopComponent)
			{
				this.TroopComp = (TroopComponent)comp;
				flag = true;
			}
			if (comp is TurretBuildingComponent)
			{
				this.TurretBuildingComp = (TurretBuildingComponent)comp;
				flag = true;
			}
			if (comp is TurretShooterComponent)
			{
				this.TurretShooterComp = (TurretShooterComponent)comp;
				flag = true;
			}
			if (comp is WalkerComponent)
			{
				this.WalkerComp = (WalkerComponent)comp;
				flag = true;
			}
			if (comp is WallComponent)
			{
				this.WallComp = (WallComponent)comp;
				flag = true;
			}
			if (comp is BuffComponent)
			{
				this.BuffComp = (BuffComponent)comp;
				flag = true;
			}
			if (comp is TrapComponent)
			{
				this.TrapComp = (TrapComponent)comp;
				flag = true;
			}
			if (comp is TrapViewComponent)
			{
				this.TrapViewComp = (TrapViewComponent)comp;
				flag = true;
			}
			if (comp is HousingComponent)
			{
				this.HousingComp = (HousingComponent)comp;
				flag = true;
			}
			if (comp is ScoutTowerComponent)
			{
				this.ScoutTowerComp = (ScoutTowerComponent)comp;
				flag = true;
			}
			if (comp is SpawnComponent)
			{
				this.SpawnComp = (SpawnComponent)comp;
				flag = true;
			}
			if (!flag && compCls != null)
			{
				Service.Logger.Error("Invalid component add: " + compCls.Name);
			}
			return base.AddComponentAndDispatchAddEvent(comp, compCls);
		}

		public override object Remove(Type compCls)
		{
			if (compCls == typeof(AreaTriggerComponent))
			{
				this.AreaTriggerComp = null;
			}
			else if (compCls == typeof(ArmoryComponent))
			{
				this.ArmoryComp = null;
			}
			else if (compCls == typeof(AssetComponent))
			{
				this.AssetComp = null;
			}
			else if (compCls == typeof(AttackerComponent))
			{
				this.AttackerComp = null;
			}
			else if (compCls == typeof(BarracksComponent))
			{
				this.BarracksComp = null;
			}
			else if (compCls == typeof(BoardItemComponent))
			{
				this.BoardItemComp = null;
			}
			else if (compCls == typeof(BuildingAnimationComponent))
			{
				this.BuildingAnimationComp = null;
			}
			else if (compCls == typeof(BuildingComponent))
			{
				this.BuildingComp = null;
			}
			else if (compCls == typeof(CantinaComponent))
			{
				this.CantinaComp = null;
			}
			else if (compCls == typeof(ChampionComponent))
			{
				this.ChampionComp = null;
			}
			else if (compCls == typeof(CivilianComponent))
			{
				this.CivilianComp = null;
			}
			else if (compCls == typeof(ClearableComponent))
			{
				this.ClearableComp = null;
			}
			else if (compCls == typeof(DamageableComponent))
			{
				this.DamageableComp = null;
			}
			else if (compCls == typeof(DefenderComponent))
			{
				this.DefenderComp = null;
			}
			else if (compCls == typeof(DefenseLabComponent))
			{
				this.DefenseLabComp = null;
			}
			else if (compCls == typeof(DroidComponent))
			{
				this.DroidComp = null;
			}
			else if (compCls == typeof(DroidHutComponent))
			{
				this.DroidHutComp = null;
			}
			else if (compCls == typeof(SquadBuildingComponent))
			{
				this.SquadBuildingComp = null;
			}
			else if (compCls == typeof(NavigationCenterComponent))
			{
				this.NavigationCenterComp = null;
			}
			else if (compCls == typeof(FactoryComponent))
			{
				this.FactoryComp = null;
			}
			else if (compCls == typeof(FleetCommandComponent))
			{
				this.FleetCommandComp = null;
			}
			else if (compCls == typeof(FollowerComponent))
			{
				this.FollowerComp = null;
			}
			else if (compCls == typeof(GameObjectViewComponent))
			{
				this.GameObjectViewComp = null;
			}
			else if (compCls == typeof(GeneratorComponent))
			{
				this.GeneratorComp = null;
			}
			else if (compCls == typeof(GeneratorViewComponent))
			{
				this.GeneratorViewComp = null;
			}
			else if (compCls == typeof(HealerComponent))
			{
				this.HealerComp = null;
			}
			else if (compCls == typeof(HealthComponent))
			{
				this.HealthComp = null;
			}
			else if (compCls == typeof(TroopShieldComponent))
			{
				this.TroopShieldComp = null;
			}
			else if (compCls == typeof(TroopShieldViewComponent))
			{
				this.TroopShieldViewComp = null;
			}
			else if (compCls == typeof(TroopShieldHealthComponent))
			{
				this.TroopShieldHealthComp = null;
			}
			else if (compCls == typeof(HealthViewComponent))
			{
				this.HealthViewComp = null;
			}
			else if (compCls == typeof(HQComponent))
			{
				this.HQComp = null;
			}
			else if (compCls == typeof(KillerComponent))
			{
				this.KillerComp = null;
			}
			else if (compCls == typeof(LootComponent))
			{
				this.LootComp = null;
			}
			else if (compCls == typeof(MeterShaderComponent))
			{
				this.MeterShaderComp = null;
			}
			else if (compCls == typeof(OffenseLabComponent))
			{
				this.OffenseLabComp = null;
			}
			else if (compCls == typeof(PathingComponent))
			{
				this.PathingComp = null;
			}
			else if (compCls == typeof(SecondaryTargetsComponent))
			{
				this.SecondaryTargetsComp = null;
			}
			else if (compCls == typeof(ShieldBorderComponent))
			{
				this.ShieldBorderComp = null;
			}
			else if (compCls == typeof(ShieldGeneratorComponent))
			{
				this.ShieldGeneratorComp = null;
			}
			else if (compCls == typeof(SizeComponent))
			{
				this.SizeComp = null;
			}
			else if (compCls == typeof(ShooterComponent))
			{
				this.ShooterComp = null;
			}
			else if (compCls == typeof(StarportComponent))
			{
				this.StarportComp = null;
			}
			else if (compCls == typeof(StateComponent))
			{
				this.StateComp = null;
			}
			else if (compCls == typeof(StorageComponent))
			{
				this.StorageComp = null;
			}
			else if (compCls == typeof(SupportComponent))
			{
				this.SupportComp = null;
			}
			else if (compCls == typeof(SupportViewComponent))
			{
				this.SupportViewComp = null;
			}
			else if (compCls == typeof(TacticalCommandComponent))
			{
				this.TacticalCommandComp = null;
			}
			else if (compCls == typeof(ChampionPlatformComponent))
			{
				this.ChampionPlatformComp = null;
			}
			else if (compCls == typeof(TeamComponent))
			{
				this.TeamComp = null;
			}
			else if (compCls == typeof(TrackingComponent))
			{
				this.TrackingComp = null;
			}
			else if (compCls == typeof(TrackingGameObjectViewComponent))
			{
				this.TrackingGameObjectViewComp = null;
			}
			else if (compCls == typeof(TransformComponent))
			{
				this.TransformComp = null;
			}
			else if (compCls == typeof(TransportComponent))
			{
				this.TransportComp = null;
			}
			else if (compCls == typeof(TroopComponent))
			{
				this.TroopComp = null;
			}
			else if (compCls == typeof(TurretBuildingComponent))
			{
				this.TurretBuildingComp = null;
			}
			else if (compCls == typeof(TurretShooterComponent))
			{
				this.TurretShooterComp = null;
			}
			else if (compCls == typeof(WalkerComponent))
			{
				this.WalkerComp = null;
			}
			else if (compCls == typeof(WallComponent))
			{
				this.WallComp = null;
			}
			else if (compCls == typeof(BuffComponent))
			{
				this.BuffComp = null;
			}
			else if (compCls == typeof(TrapComponent))
			{
				this.TrapComp = null;
			}
			else if (compCls == typeof(TrapViewComponent))
			{
				this.TrapViewComp = null;
			}
			else if (compCls == typeof(HousingComponent))
			{
				this.HousingComp = null;
			}
			else if (compCls == typeof(SpawnComponent))
			{
				this.SpawnComp = null;
			}
			return base.Remove(compCls);
		}
	}
}
