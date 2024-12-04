using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;

namespace RoutineCallouts.Callouts
{
	[CalloutInfo("HighSpeedChase", CalloutProbability.Medium)]
	public class HighSpeedChase : Callout
	{
		private Ped? Suspect;
		private Vehicle? SuspectVehicle;
		private Blip? SuspectBlip;
		private LHandle? Pursuit;
		private Vector3 SpawnPoint;
		private bool PursuitCreated = false;

		public override bool OnBeforeCalloutDisplayed()
		{
			SpawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(500f));
			ShowCalloutAreaBlipBeforeAccepting(SpawnPoint, 30f);
			AddMinimumDistanceCheck(100f, SpawnPoint);
			CalloutMessage = "High Speed Chase";
			CalloutPosition = SpawnPoint;
			Functions.PlayScannerAudioUsingPosition("WE_HAVE CRIME_SUSPECT_RESISTING_ARREST_01 IN_OR_ON_POSITION", SpawnPoint);

			return base.OnBeforeCalloutDisplayed();
		}

		public override bool OnCalloutAccepted()
		{
			SuspectVehicle = new Vehicle("BANSHEE", SpawnPoint);
			if (!SuspectVehicle.Exists())
			{
				End();
				return false;
			}
			SuspectVehicle.IsPersistent = true;

			Suspect = SuspectVehicle.CreateRandomDriver();
			if (!Suspect.Exists())
			{
				End();
				return false;
			}
			Suspect.IsPersistent = true;
			Suspect.BlockPermanentEvents = true;

			SuspectBlip = Suspect.AttachBlip();
			SuspectBlip.IsFriendly = false;
			SuspectBlip.Color = System.Drawing.Color.Red;
			SuspectBlip.IsRouteEnabled = true;

			PursuitCreated = false;

			return base.OnCalloutAccepted();
		}

		public override void Process()
		{
			base.Process();

			if (Suspect == null || SuspectVehicle == null || !Suspect.Exists() || !SuspectVehicle.Exists() || Suspect.IsDead || Functions.IsPedArrested(Suspect))
			{
				End();
				return;
			}

			if (!PursuitCreated && Game.LocalPlayer.Character.DistanceTo(Suspect) < 100f)
			{
				Pursuit = Functions.CreatePursuit();
				Functions.AddPedToPursuit(Pursuit, Suspect);
				Functions.SetPursuitIsActiveForPlayer(Pursuit, true);
				PursuitCreated = true;
			}

			if (PursuitCreated && !Functions.IsPursuitStillRunning(Pursuit))
			{
				End();
			}
		}

		public override void End()
		{
			if (SuspectBlip != null && SuspectBlip.Exists()) SuspectBlip.Delete();
			if (Suspect != null && Suspect.Exists()) Suspect.Dismiss();
			if (SuspectVehicle != null && SuspectVehicle.Exists()) SuspectVehicle.Dismiss();

			base.End();
		}
	}
}
