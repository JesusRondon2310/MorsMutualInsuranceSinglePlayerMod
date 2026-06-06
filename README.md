# MMI-SP-Rewrite

_Un mod para GTA V_
**Mors Mutual Insurance - Single Player**

### Basado en el trabajo original de Bob74 (2022)

[![Vídeo del tráiler](https://user-images.githubusercontent.com/9498543/162617439-42459c98-9915-4a43-b476-c339192e307a.png)](https://www.youtube.com/watch?v=WATdK3aOdGk)

¿Cansado de perder tu vehículo tuneado de $500,000 porque se te quedó por una misión o decidiste probar si era sumergible? ¡No esperes más y asegura tu vehículo ahora en Mors Mutual Insurance!

La descarga está disponible [aquí](https://github.com/JesusRondon2310/MMI-SP-Rewrite-WIP/releases/tag/0.3)

# Historial de Cambios

# Changelog — MMI-SP-Rewrite

## Beta 2 (24/05/2026)

- **Módulo iFruitAddon2 3.1.1 completo.** Contactos MMI, Mecánico y Configuración funcionales en el teléfono.
- **Mecánico con NPC conductor.** Entrega de vehículos con sistema de TP a 100m para ahorro de recursos (>200m).
- **Validación de spawn.** `SpawnPositionValidator` busca nodos de carretera seguros y valida altura del suelo.
- **Notificaciones SMS.** Formato correcto con icono del contacto, remitente, asunto y mensaje (`Notification.ShowMMI` / `ShowMechanic`).
- **Voces del mecánico.** Saludos, afirmaciones y negaciones extraídas de `SS_GM.rpf` y reproducidas con `AudioPlayer`.
- **Límite de 1 entrega activa simultánea.** Previene saturación del sistema de entregas.
- **Colores en menús y notificaciones.** Nombres de vehículos en azul, precios en verde, errores en rojo, placas en amarillo.
- **Modularización de Config.** `Config.cs` dividido en `ModSettings.cs` + `Persistence.cs` dentro de `Config/`.
- **Centralización de lógica de spawn.** `SpawnPositionValidator` movido a `Observer/Delivery/`, `DeliveryHelper.GetSafeTeleportPosition` encapsula TP.
- **Extracción de `GetVehicleLabel` duplicado.** Centralizado en `VehicleIdentifier.GetLocalizedDisplayNameFromId`.
- **Refactorización de `Core.cs`.** Bajado de 138 a 118 líneas extrayendo `ExecuteSafe` y compactando contactos.
- **Corrección de orden en `ForceArrivalIfNear`.** `Wander()` antes de `SET_BLOCKING_OF_NON_TEMPORARY_EVENTS` para evitar conductor parado.

## Beta 1 (21/05/2026)

- **Contacto MMI en el teléfono.** Menú con opción de Reclamar vehículo destruido.
- **Bloqueo de controles del teléfono.** `_iFruit.Close()` para evitar doble input con NativeUI.
- **Voces de la recepcionista.** `Properties.Resources` + `AudioPlayer` + `SoundPlayer.Play()` asíncrono.
- **Notificaciones SMS para MMI.** `Notification.ShowMMI` con icono `char_mp_mors_mutual`.
- **Migración de `BlipManager` a `Insurance/`.** Independiente del Observer.
- **Corrección de bugs en DB.** `PlateStyle`, `CustomTires`, orden de aplicación de rines.
- **Límite de 30 vehículos asegurados.**
- **Límite de 5 vehículos reclamables.** `DestroyedAt`, `MarkAsDestroyed`, `GetDestroyedList`.
- **Sistema de bloqueo con tecla L.** `LockVehicle`, `LockFeedback`, `LockPersistenceHelper`.

## Alpha 0.4.0 (20/05/2026)

- **Migración completa a módulo `DB/`.** `VehicleData`, `VehicleDataBuilder`, `VehicleSpawnManager`, `JSONHandler`, `InsuredVehiclesData`, `Core`.
- **Separación Backend/Frontend.** `DB/` como capa de datos pura, `Insurance/` como lógica de negocio.
- **Eliminación de `try/catch`** fuera de `Repository` y `EnterSequence` en todos los archivos.
- **Reemplazo de `Game.GenerateHash`** por `VehicleHash` enum.
- **Pattern Matching obligatorio.** `Result<T>`, `Option<T>`, `match<T>`, `and_then<T>` en todo el proyecto.

## Alpha 0.3.4 (18/05/2026)

- **Refactorización final y Patrón de Coincidencia.** Se modularizó la lógica del asegurador, se mejoró el manejo de errores en el monitoreo de vehículos y se unificaron las funciones de fábrica.

## Alpha 0.3.3 (18/05/2026)

- **Corrección de errores críticos.** Se arregló un error que provocaba salir de la oficina al recuperar un vehículo, se estabilizó el estado "Destruido" y la actualización del submenú de recuperación.

## Alpha 0.3.2 (17/05/2026)

- **Módulo de Recuperación y su menú.** Se añadió la funcionalidad de reclamar vehículos destruidos a través de un nuevo módulo y submenú. El vehículo aparecerá en el depósito.

## Alpha 0.3.1 (17/05/2026)

- **Refactorización inicial y preparación.** Se reescribió gran parte de la lógica del seguro usando patrones funcionales para hacer el código más robusto y fácil de mantener.

## Alpha 0.3.0 (16/05/2026)

- **Persistencia Real.** ¡Tus coches sobreviven al cerrar y cargar la partida! Se añadió un sistema para guardar la ubicación exacta y recrear los vehículos asegurados al iniciar el juego.

## Alpha 0.2.3 (15/05/2026)

- Migración a `Result<T>` y pattern matching en todos los métodos críticos.
- Resueltos 15 errores de compilación por APIs obsoletas de SHVDN3 (`CS0618`).
- Base de datos JSON con Newtonsoft (`db.json`).

## Alpha 0.2.1 (14/05/2026)

- Refactorización inicial aplicando SOM (Sistema de Orquestación Modular).
- Eliminados archivos obsoletos (`InsuredVehicles.cs`, `Action.cs`, `Create.cs` duplicados).
- Creadas las fábricas `Buttons/Build.cs` y `Buttons/Fill.cs`.

## Alpha 0.2.0 (13/05/2026)

- Menú principal con los botones "Asegurar" y "Cancelar seguro".
- Solucionados bugs de salida de oficina, descripciones cortadas, orden de botones y `NullReferenceException` en NativeUI.

## Alpha 0.1.0 (12/05/2026)

- Migración del código original de Bob74 a SHVDN3.
- Entrada a la oficina y estructura básica del mod.

---

## Notas de Instalación

Recuerda dar los permisos de "Modificar" a tu usuario en la carpeta del juego para que ScriptHook y el mod puedan funcionar correctamente.

## Créditos

- **Desarrollo y Arquitectura:** [JesusRondon2310/Platanito22](https://github.com/JesusRondon2310)
- **Coder y Solucionador de bugs:** [DeepSeek](https://www.deepseek.com/en/) y [Claude](https://claude.ai/)
- **Colaborador:** [Ricardo Vera](https://github.com/ricardovera76)
