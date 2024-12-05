using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;

namespace RoutineCallouts.Callouts
{
	[CalloutInfo("HighSpeedChase", CalloutProbability.Medium)]
	public class HighSpeedChase : Callout
	{
		private Ped _suspect;
		private Vehicle _suspectVehicle;
		private Blip _suspectBlip;
		private LHandle _pursuit;
		private Vector3 _spawnPoint;
		private bool _pursuitCreated = false;

		public override bool OnBeforeCalloutDisplayed()
		{
			_spawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(500f));
			ShowCalloutAreaBlipBeforeAccepting(_spawnPoint, 30f);
			AddMinimumDistanceCheck(100f, _spawnPoint);
			CalloutMessage = "High Speed Chase";
			CalloutPosition = _spawnPoint;
			Functions.PlayScannerAudioUsingPosition("CITIZENS_REPORT CRIME_SUSPECT_ON_THE_RUN IN_OR_ON_POSITION", _spawnPoint);

			return base.OnBeforeCalloutDisplayed();
		}

		public override bool OnCalloutAccepted()
		{
			_suspectVehicle = new Vehicle("BANSHEE", _spawnPoint);
			if (!_suspectVehicle.Exists())
			{
				End();
				return false;
			}
			_suspectVehicle.IsPersistent = true;

			_suspect = _suspectVehicle.CreateRandomDriver();
			if (!_suspect.Exists())
			{
				End();
				return false;
			}
			_suspect.IsPersistent = true;
			_suspect.BlockPermanentEvents = true;

			_suspectBlip = _suspect.AttachBlip();
			_suspectBlip.IsFriendly = false;
			_suspectBlip.Color = System.Drawing.Color.Red;
			_suspectBlip.IsRouteEnabled = true;

			_pursuitCreated = false;

			return base.OnCalloutAccepted();
		}

		public override void Process()
		{
			base.Process();

			if (_suspect == null || _suspectVehicle == null || !_suspect.Exists() || !_suspectVehicle.Exists() || _suspect.IsDead || Functions.IsPedArrested(_suspect))
			{
				End();
				return;
			}

			if (!_pursuitCreated && Game.LocalPlayer.Character.DistanceTo(_suspect) < 100f)
			{
				_pursuit = Functions.CreatePursuit();
				Functions.AddPedToPursuit(_pursuit, _suspect);
				Functions.SetPursuitIsActiveForPlayer(_pursuit, true);
				_pursuitCreated = true;
			}

			if (_pursuitCreated && !Functions.IsPursuitStillRunning(_pursuit))
			{
				End();
			}
		}

		public override void End()
		{
			if (_suspectBlip != null && _suspectBlip.Exists()) _suspectBlip.Delete();
			if (_suspect != null && _suspect.Exists()) _suspect.Dismiss();
			if (_suspectVehicle != null && _suspectVehicle.Exists()) _suspectVehicle.Dismiss();

			base.End();
		}
	}
}
