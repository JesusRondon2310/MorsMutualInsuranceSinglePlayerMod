# Mors Mutual Insurance Single Player Mod

_Por Platanito22_
Un derivado basado en el trabajo original de Bob74 (2022)

[![Vídeo del tráiler original](https://user-images.githubusercontent.com/9498543/162617439-42459c98-9915-4a43-b476-c339192e307a.png)](https://www.youtube.com/watch?v=WATdK3aOdGk)

## 🔍 Descripcion original

¿Cansado de perder tu vehículo tuneado de 500,000 Bs.F porque se te quedó por una misión o decidiste probar si era sumergible? ¡No esperes más y asegura tu vehículo ahora en Mors Mutual Insurance!

La descarga está disponible [aquí](https://github.com/JesusRondon2310/MMI-SP-Rewrite-WIP/releases/tag/0.3)

---

## 🦜 Mi descripción

Hey, ¿qué tal? Aquí Platanito22, pero esta vez en ¿GTA? y ¿GTA V Legacy? Pues sí, así como lo ves, ese modder de NFS ahora hizo un mod para GTA V. ¿Sorprendido? ¿No? Ok, no importa.

El punto es que les quiero presentar mi última y más refinada creación Un derivado/reescritura completa del mod Mors Mutual Insurance - Single Player (MMI-SP) de Bob74.

¿Por qué me decidí a hacer esto? Bueno, si tú ya has descargado mis otros mods, sabes que me caracterizo por rehacer cosas que no me gustan y hacer cosas que nadie ha hecho —como pasar el rendimiento de todos los autos de un juego al otro—. Entonces, con este mod no fue distinto. El mod no estaba mal conceptualmente y en la comunidad no hay mods muy buenos que repliquen bien a Mors Mutual y al mecánico de GTAO en modo historia, y sabiendo lo bueno que era el mod, pensé: "Yo no puedo dejar morir este mod así, es demasiado bueno", y me descargué el código fuente y empecé a reescribirlo ¡desde 0! hasta llegar a donde estamos ahora.

Entonces, viendo que es la Rev 1.0, ¿eso implica más cambios o soporte a futuro? No.

No porque lo que quería hacer era una simple —y agobiante— reescritura, pero cambié tanto el código fuente que ahora es, más bien, un derivado que no tiene nada que ver con la materia prima. A lo que voy, mi objetivo con esto es dejar una versión única, definitiva, probada exhaustivamente —nos pusimos mi hermano de 11 años y yo a jugar el modo historia en mi GTA supermodeado, 2 veces (1 vez por partida) de 0 a terminar la historia y, entre cambios de savegames, íbamos cambiando los archivos `db.json` entre los de él y los míos, así que sí, probado— y con el código fuente libre en mi repositorio, para que no estén reportando bugs o una feature que quieren agregar; no lo hagan, porque ese tipo de peticiones no las voy a contestar. Se los digo desde ya para que se las ahorren y no me estén molestando con eso. Si quieren agregar features, arreglar más bugs o darle soporte a enhanced, bueno, ahí está el código, agárrenlo, caiganse a coñazos con RAGE y ustedes mismos reparan lo que quieran reparar, agregan lo que quieren agregar y suben lo que quieran subir; no me digan a mí, ya les hice el trabajo sucio de tomar un "ugly source code" —según Bob—, meterlo en sala de emergencias, diseccionarlo y revivirlo, así que de nada.

Me despido, disfruten, nos pillamos por ahí.

---

## 📚 Requisitos y 💻 Compatibilidad

- GTA V Legacy (3725, no tengo pensado dar soporte a la enhanced, pero sí tiene soporte con el mod [Enhanced Content for Legacy](https://www.nexusmods.com/gta5/mods/1267))

### ⚙️ ¿Cómo lo instalo?

Con las últimas versiones de las dependencias importantes instaladas y actualizadas, las cuales son:

- [NativeUI 1.9.1](https://github.com/Guad/NativeUI/releases/)
- [ScriptHookVDotNet Nightly (3.7.0.162 - en adelante)](https://github.com/scripthookvdotnet/scripthookvdotnet-nightly/releases)
- [iFruitAddon2 3.1.1](https://github.com/Bob74/iFruitAddon2)
- [Newtonsoft.Json (ya integrado en el .zip)](https://www.newtonsoft.com/json)

Copian todo lo que está dentro del `.zip` y lo pegan en su carpeta `/scripts` dentro de su carpeta raíz de GTA V.

---

## Aja, y… ¿Qué cambió? 🤔

### 🔧 Mejoras técnicas (código fuente)

- **Reescritura completa del código:** Se ha pasado de una estructura monolítica a una arquitectura moderna y modular, lo que facilita el mantenimiento, la corrección de errores y futuras ampliaciones.<br><br>

- **Actualización de dependencias:** Se han resuelto los conflictos con versiones recientes de iFruitAddon2 y ScriptHookVDotNet, eliminando los fallos de compatibilidad que afectaban al mod original.<br><br>

- **Corrección de 45 bugs documentados:** Entre ellos, errores críticos como la pérdida de mejoras de Los Santos Customs, fallos en la persistencia de vehículos, duplicación de coches y el mal funcionamiento del conductor mensajero (mecánico).<br><br>

- **Base de datos robusta:** Se ha mejorado el sistema de guardado (db.json) para que soporte ediciones manuales y sea resistente a corrupciones, probado antes de iniciar partidas.<br><br>

- **Gestión de persistencia dedicada:** Nueva clase PersistenceManager que asegura que los vehículos asegurados mantengan su estado, posición y modificaciones de forma fiable.<br><br>

- **Refactorización del sistema telefónico:** El antiguo PhoneService ha sido reemplazado por un módulo UI más limpio y eficiente, que también permite la configuración completa del mod desde el teléfono del juego (sin necesidad de editar archivos .ini manualmente).<br><br>

### 🎮 Mejoras en la experiencia de juego (in-game)

- **Estabilidad absoluta:** El mod ha superado una fase de beta testing exhaustiva que incluyó dos partidas completas (con dos jugadores diferentes) y pruebas de manipulación directa de la base de datos. Está listo para producción.<br><br>

- **Funcionalidades clave funcionando al 100%:** A diferencia del original (cuyos comentarios reportaban fallos en la recuperación de vehículos), aquí asegurar, recuperar y que te traigan el coche funciona sin errores.<br><br>

- **Conductor mensajero (mecánico) fiable:** El servicio de "tráeme mi vehículo" ahora opera correctamente.
**Nota:** Me parece increíble que la característica principal del mod original fuera lo que menos servía y que tenga que poner esto como una mejora... Qué arrecho.<br><br>

- **Compatibilidad garantizada:** Al eliminar las dependencias obsoletas y los conflictos con otros scripts, este mod funciona sin problemas junto a otros mods populares actuales.<br><br>

- **Configuración desde el móvil:** Ajusta precios, opciones de persistencia y más directamente desde el teléfono del juego, sin salir de la partida.<br><br>

- **Persistencia real de los vehículos:** Los coches tuneados no desaparecen al cambiar de misión ni se duplican. Su estado (daños, mejoras, ubicación) se guarda de forma consistente.

---

## 🚀 Optimización Extrema & Control de Calidad Indestructible

- **Apto para "Tostadoras" (Canaima Friendly)**: El mod está optimizado línea por línea. Fue diseñado pensando en escenarios extremos: PCs de muy bajos recursos corriendo el juego con 100 mods encima a 15 FPS. Si tu PC apenas puede con el juego, este script no le quitará ni un solo cuadro por segundo gracias a su sistema inteligente de dormancia.<br><br>

- **Sello de Calidad "Anti-Niños"**: El control de calidad (QA) principal estuvo a cargo de mi hermano de 11 años. Su único trabajo fue jugar al caos absoluto, destruir vehículos de mil formas creativas, romper misiones y forzar el juego al límite. Si un niño de 11 años no pudo corromper la base de datos ni romper el sistema de seguros, ten por seguro que tus autos están 100% a salvo.

---

## 🗿 Créditos

- **Desarrollo y Arquitectura:** [JesusRondon2310/Platanito22](https://github.com/JesusRondon2310).<br><br>
- **Coder y Solucionador de bugs:** [DeepSeek: hizo el 99.95% del codigo del proyecto](https://www.deepseek.com/en/) y [Claude: solucionador de bugs criticos y dificiles de encontrar](https://claude.ai/).<br><br>
- **Colaborador:** [Ricardo Vera](https://github.com/ricardovera76).<br><br>
- **Información necesaria para llevar a cabo el Rust Simulated Pattern Matching:** [Trevor Sullivan](https://www.youtube.com/trevorsullivan).<br><br>
- **Aprendizaje de C#:** [hdeleon.net](https://www.youtube.com/@hdeleonnet).<br><br>
- **Beta tester como sello de calidad:** Mi hermano de 11 años.
