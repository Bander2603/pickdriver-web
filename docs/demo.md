# Demo / Mock Mode

El cliente puede ejecutarse sin API real habilitando el modo mock.

## Activar

En `appsettings.json` o `appsettings.Development.json`:

```json
{
  "Api": {
    "BaseUrl": "https://api.example.com/api/",
    "UseMock": true
  }
}
```

## Que hace

- Intercepta todas las llamadas HTTP y devuelve respuestas mock.
- Devuelve listas y objetos minimos para que la UI pueda cargar sin backend.
- Algunos flujos escriben datos (crear liga/equipo, picks) pero no persisten: se responde con datos demo.

## Desactivar

Poner `UseMock` en `false` y usar la URL real del backend.
