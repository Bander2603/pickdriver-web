# Reglas del juego - PickDriver (API)

Este documento describe las reglas vigentes tal como estan implementadas en la API actual.

## Objetivo
Compite en una liga seleccionando pilotos de Formula 1 por carrera. Ganas puntos segun el rendimiento real del piloto, y la liga se define por el mayor puntaje acumulado en la temporada.

## Ligas
- Requiere temporada activa (`season.active = true`).
- La liga se crea con status `pending`.
- El creador queda como `owner` y tambien se agrega como miembro.
- `maxPlayers` define el limite de miembros.
- Para unirse: la liga debe estar en `pending`, no puedes ser miembro ya existente, y la liga no puede estar llena (`memberCount >= maxPlayers`).

### Permisos del owner
- Asignar orden de picks (`assign-pick-order`).
- Iniciar el draft (`start-draft`).
- Eliminar la liga (solo si esta en `pending`).

## Opciones de liga
- `teamsEnabled`, `bansEnabled`, `mirrorEnabled` se aceptan sin validaciones adicionales al crear la liga.

## Equipos (Teams)
Solo aplican si `teamsEnabled = true`.

### Reglas de habilitacion
- La liga debe estar en `pending`.
- La liga debe estar completa (`memberCount == maxPlayers`).

### Reglas de tamanos
- Tamano minimo por equipo: 2.
- Maximo de equipos = min(totalPlayers / 2, cantidad de equipos F1 en la temporada).
- La distribucion debe ser factible y balanceada:
  - cada equipo debe quedar entre floor(totalPlayers / k) y ceil(totalPlayers / k)
  - no se permiten equipos por debajo del minimo

### Reglas de membresia
- No se permiten usuarios duplicados dentro de un mismo equipo.
- Un usuario no puede estar en multiples equipos.
- Solo miembros de la liga pueden estar en equipos.

## Draft
### Activacion (start-draft)
- Solo `owner`.
- Liga en `pending`.
- Liga completa (`members == maxPlayers`).
- Si `teamsEnabled = true`, todos los jugadores deben estar asignados a equipos.

### Orden de picks y rotacion
- Si existe `pickOrder` asignado para todos los miembros, se respeta.
- Si no existe, se calcula un orden aleatorio:
  - sin equipos: shuffle directo
  - con equipos: shuffle por equipo y round-robin entre equipos
- Por cada carrera futura desde `initialRaceRound`, se rota el orden.
- Si `mirrorEnabled = true`, el orden se duplica en espejo (rotated + reversed).

## Deadlines de picks
- `firstHalfDeadline = fp1Time - 36h`
- `secondHalfDeadline = fp1Time`

## Autopick
- Cada usuario puede guardar una lista ordenada de drivers (`driverIDs`) por liga.
- Se eliminan duplicados manteniendo orden.
- La lista debe coincidir con drivers de la temporada de la liga.
- Si se envia lista vacia, se borra la configuracion.
- Cuando expira un turno, se intenta autopick con el primer driver disponible:
  - no baneado por el usuario
  - no pickeado por otro jugador
- Si no hay autopick valido, el turno se considera expirado y se avanza igual.

## Picks
### Reglas de acceso
- Solo el usuario del turno puede pickear.
- Si `teamsEnabled = true` y falta menos de 1h para `fp1`, un companero puede pickear por el turno actual.

### Validaciones
- No se permite pick/ban si la carrera ya comenzo o esta completada:
  - `race.completed == true` o `race.raceTime < now`
- El driver debe existir y pertenecer a la temporada de la carrera.
- No se permite pickear un driver ya pickeado (global en el draft).
- No se permite pickear un driver que el usuario tenga baneado.
- Solo un pick por usuario y por "mirror slot" (`is_mirror_pick`).

### Efectos
- Inserta pick y avanza `currentPickIndex`.
- Notifica al siguiente jugador cuando corresponde.

## Bans
### Reglas de acceso
- Solo si `bansEnabled = true`.
- Solo se puede banear el pick inmediatamente anterior.
- No se puede banear al ultimo jugador del orden (salvo que tambien sea el primero).
- Ligas sin equipos:
  - cada usuario solo puede banear una vez por carrera.
  - un jugador solo puede ser baneado una vez por carrera.
- Ligas con equipos:
  - cada equipo solo puede banear una vez por carrera.
- Permisos:
  - sin equipos: solo el usuario del turno
  - con equipos: el usuario del turno o un companero

### Cantidad de bans
- Sin equipos: 2 bans por usuario.
- Con equipos: 3 bans por equipo.
- Restriccion por carrera: cada usuario/equipo solo puede usar 1 ban por carrera; en ligas sin equipos, un jugador solo puede ser baneado una vez por carrera.

### Efectos
- Marca el pick como baneado (`is_banned = true`) y guarda `banned_by`.
- Retrocede `currentPickIndex` al pick anterior para que el usuario re-pickee.
- Notifica al siguiente jugador despues del cambio.

## Standings y puntuacion
- Solo se consideran picks no baneados.
- Picks hechos por autopick valen 50% de los puntos del driver.
- Los standings se calculan sobre carreras completadas.
- En picks con mirror, el sistema calcula la posicion considerando el orden espejo.

## Notificaciones del draft
- Al iniciar draft se notifica al primer usuario del orden.
- Al completar un pick se notifica al siguiente usuario.
- Al publicar resultados se generan notificaciones asociadas a la carrera.
