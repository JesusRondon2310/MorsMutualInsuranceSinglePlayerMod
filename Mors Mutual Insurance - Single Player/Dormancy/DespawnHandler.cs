using GTA;
using GTA.Math;
using MMI_SP.DB;
using MMI_SP.Helpers;
using MMI_SP.Insurance.Observer;
using MMI_SP.PatternMatching;
using System.Collections.Generic;
using System.Linq;

namespace MMI_SP.Dormancy
{
    internal static class DespawnHandler
    {
        internal static Result<bool> Execute(string vehId, Vehicle veh, List<DormantVehicle> dormantList)
        {
            // Validaciones de entrada
            if (string.IsNullOrEmpty(vehId)) return new Err<bool>("ID de vehículo no válido.");
            if (veh == null || !veh.Exists()) return new Err<bool>("El vehículo no existe.");
            if (dormantList == null) return new Err<bool>("Lista de dormantes nula.");

            // Obtener datos de la base de datos
            var dataOption = DB.Core.FindVehicle(vehId);
            if (dataOption.is_none()) return new Err<bool>("Vehículo no encontrado en la base de datos.");

            // Cast directo después de is_none() (excepción controlada)
            VehicleData data = ((Some<VehicleData>)dataOption).Value;

            // Comprobar estados que impiden dormir
            if (data.IsLocked) return new Err<bool>("Vehículo bloqueado, no se puede dormir.");

            if (data.IsInGarage) return new Err<bool>("Vehículo en garaje, no se puede dormir.");

            // Distancia desde la posición guardada en BD
            Vector3 dbPos = new Vector3(data.PosX, data.PosY, data.PosZ);
            if (dbPos.DistanceTo(Game.Player.Character.Position) < Constants.DORMANCY_THRESHOLD)
                return new Err<bool>("La posición guardada del vehículo está cerca del jugador, no se duerme.");

            // Vehículo recién recuperado (no dormir hasta que el jugador lo monte)
            string recoveryKey = VehicleKey.FullKeyFrom(data);
            if (Manager.RecoveredVehicleKeys.Contains(recoveryKey) && !Game.Player.Character.IsInVehicle(veh))
                return new Err<bool>("Vehículo recién reclamado, no se puede dormir hasta que el jugador lo recoja.");

            // Posición actual del vehículo inválida
            if (veh.Position == Vector3.Zero || veh.Position.Length() < Constants.MIN_VALID_POSITION_LENGTH)
                return new Err<bool>("Posición del vehículo inválida.");

            // Distancia actual al jugador
            if (veh.Position.DistanceTo(Game.Player.Character.Position) < Constants.DORMANCY_THRESHOLD)
                return new Err<bool>("Vehículo demasiado cerca del jugador para dormir.");

            // Evitar duplicados en la lista de dormantes
            if (dormantList.Any(d => d.Data?.Plate == data.Plate && d.Data?.ModelName == data.ModelName))
                return new Err<bool>("El vehículo ya está en estado dormante.");

            // Crear datos dormantes y actualizar BD
            Vector3 lastPosition = veh.Position;
            var dormantData = data.With(d => {
                d.PosX = lastPosition.X;
                d.PosY = lastPosition.Y;
                d.PosZ = lastPosition.Z;
                d.IsDormant = true;
                d.IsInGarage = false;
            });

            var updateResult = DB.Core.Update(dormantData);
            if (updateResult.is_err()) return new Err<bool>(((Err<bool>)updateResult).Message);

            // Añadir a la lista de dormantes
            dormantList.Add(new DormantVehicle { Data = dormantData });
            return new Ok<bool>(true);
        }
    }
}