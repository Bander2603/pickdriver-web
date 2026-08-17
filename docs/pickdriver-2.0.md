# PickDriver 2.0 en el cliente web

El cliente web mantiene dos experiencias de draft en la misma pagina. La seleccion se hace exclusivamente con `RaceDraft.gameplayVersion`; las fechas nunca se usan para decidir si un draft es legacy o V2.

## Compatibilidad

- `legacy`: conserva picks por turnos, edicion del ultimo pick, ban del turno y autopick.
- `v2`: elimina turnos y picks manuales. Usa una lista privada reusable, publica el resultado por slot y habilita bans durante su ventana.
- Los DTO aceptan los campos V2 como aditivos y mantienen valores seguros si una respuesta legacy no los incluye.
- El autopick legacy no se consulta ni se copia al abrir un draft V2.

## Lista privada

La accion `Picks` usa:

- `GET /api/leagues/{leagueID}/pick-preferences`
- `PUT /api/leagues/{leagueID}/pick-preferences`

La lista admite cero o más pilotos, conserva el orden y sólo pertenece al usuario autenticado. La UI permite añadir, quitar, vaciar y reordenar con drag-and-drop o controles de teclado. Sólo se puede editar mientras el draft está en `collecting` y `now < submissionDeadline`. Desde el instante exacto del deadline, el botón `Picks` queda deshabilitado hasta que la vista cargue la siguiente carrera o draft en estado `collecting`.

Los avisos bajo las acciones se evalúan en este orden:

1. `Lista de picks vacia!` si nunca se envió una lista o la lista guardada está vacía.
2. `Lista de picks incompleta` si contiene menos pilotos que slots tiene el draft.
3. `Lista aun no actualizada este GP` si una lista completa no se actualizó después de la carrera anterior.

Todos los textos nuevos tienen traducción inglesa en `Localization/AppStrings.cs`.

## Resultado público

`pickedDriverIDs` se renderiza por `pickIndex`, alineado con `pickOrder`, mostrando junto a cada jugador el piloto que le corresponde. No se crea un diccionario por usuario porque mirror puede repetir `userID`. Un valor `null` se muestra como `Missed pick` sólo cuando el resultado es público (`resolved` o `finalized`). Durante `collecting` se muestra el orden sin resultados; `cancelled` no ofrece acciones.

La pantalla sustituye siempre el DTO completo cuando refresca. Esto es necesario porque un ban incrementa `resolutionRevision` y puede recalcular el target y todos los slots posteriores.

## Deadlines

En V2 se usan `submissionDeadline` y `banWindowClosesAt`:

- sin bans: se muestra un único deadline de picks, en FP1;
- con bans: se muestran el deadline de picks (FP1−24h) y el cierre de bans (FP1).

Las fechas se reciben en UTC y se muestran en la zona local del navegador. `firstHalfDeadline` y `secondHalfDeadline` se conservan únicamente para legacy y retrocompatibilidad.

## Bans V2

La accion usa `POST /api/leagues/{leagueID}/draft/{raceID}/v2/ban` con `targetUserID` y `driverID`. El botón sólo existe si la liga tiene bans y permanece deshabilitado fuera de `resolved` o de la ventana `[submissionDeadline, banWindowClosesAt)`.

La selección se construye por slot y excluye:

- picks `null`;
- el usuario autenticado;
- compañeros de equipo;
- cualquier target que ya recibió un ban en esa carrera;
- la accion completa si el usuario/team ya baneó en ese draft o agotó el presupuesto estacional.

El presupuesto autoritativo viene de `bansUsedByUserID` o `bansUsedByTeamID`; `banLimitPerActor` vale 2 sin teams y 3 compartido con teams. Antes de enviar se presenta una confirmación irreversible. Los errores `400`, `403` y `409` muestran el `reason` del backend. Después de un `200` se vuelve a pedir el detalle completo.

## Refresco

Mientras la pestaña Draft está activa, el detalle se refresca cada 60 segundos. También se recarga inmediatamente antes de abrir el selector de ban y después de ejecutar un ban.

## Pruebas

`DraftV2ContractTests` cubre DTOs legacy/V2, listas vacías y parciales, missed picks, mirror, deadlines, teams y contadores. `LeagueDraftV2ComponentTests` cubre la UI legacy, collecting sin filtración de resultados, resultado por slots, ban V2, errores del backend y recarga por `resolutionRevision`.

Ejecutar:

```bash
dotnet test pickdriver-web.sln
```
