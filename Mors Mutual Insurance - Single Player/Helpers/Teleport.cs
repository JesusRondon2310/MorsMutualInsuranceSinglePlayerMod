using GTA;
using GTA.Math;
using MMI_SP.Helpers.Spawn.Coordinates;

namespace MMI_SP.Helpers.Teleport
{
	public static class Teleport
	{
		// ==========================================
		// Métodos públicos
		// ==========================================
		public static void BehindPlayer(Vehicle veh)
		{
			if (veh == null || !veh.Exists()) return;

			float currentDistance = Game.Player.Character.Position.DistanceTo(veh.Position);
			if (currentDistance <= Constants.TELEPORT_MIN_DISTANCE) return;

			Vector3 camPos = GameplayCamera.Position;
			Vector3 camForward = GameplayCamera.Direction;
			Vector3 candidatePos = camPos - (camForward * Constants.DELIVERY_DISTANCE_BEHIND_PLAYER);

			// Obtener un nodo de carretera seguro cerca de la posición candidata
			var roadResult = SpawnHandler.GetValidRoad(candidatePos.X, candidatePos.Y, candidatePos.Z);
			Vector3 finalPos = roadResult.unwrap_or(candidatePos);

			veh.Position = finalPos;
			veh.Heading = Game.Player.Character.Heading;
		}

		// Teletransporta el vehículo delante del jugador (sin validación de carretera)
		public static void InFrontOfPlayer(Vehicle veh)
		{
			if (veh == null || !veh.Exists()) return;

			EntityPosition pos = SpawnHandler.InstantDeliverySpawn(Game.Player.Character.Position);
			veh.Position = pos.Position;
			veh.Heading = pos.Heading;
		}

		// Teletransporta el vehículo al nodo de carretera más cercano (sin filtrar heading)
		public static void ToRoad(Vehicle veh)
		{
			if (veh == null || !veh.Exists()) return;

			var nodeResult = SpawnHandler.GetValidRoad(veh.Position.X, veh.Position.Y, veh.Position.Z);
			veh.Position = nodeResult.unwrap_or(veh.Position);
		}
	}
}