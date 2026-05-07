# EcoTrack Backend

Este documento detalla la configuración necesaria para el entorno de desarrollo local.

## Onboarding

Para el correcto funcionamiento del proyecto, es obligatorio que las siguientes variables de entorno estén configuradas a nivel de sistema operativo. Esto permite que el IDE (Rider, VS Code, etc.) herede las credenciales automáticamente al ejecutar el proceso sin depender de archivos de configuración locales.

### 1. Variables de Entorno Requeridas

| Variable | Descripción |
| :--- | :--- |
| **DB_USER** | Nombre de usuario administrativo para la base de datos. |
| **DB_PASSWORD** | Contraseña administrativa para la base de datos. |
| **JWT_SECRET** | Clave criptográfica para la firma de tokens JWT. |

---

### 2. Configuración del Sistema

Las variables deben definirse de forma que sean persistentes y visibles para los procesos del entorno gráfico.

#### Linux

En sistemas como Fedora con KDE, se recomienda el uso de archivos de configuración de sesión.

1. Crear el directorio: mkdir -p ~/.config/environment.d/
2. Crear el archivo: nano ~/.config/environment.d/ecotrack.conf
3. Añadir el contenido:
    ```
    DB_USER=tu_usuario
    DB_PASSWORD=tu_password
    JWT_SECRET=tu_clave_secreta
    ```
4. Reiniciar la sesión de usuario para aplicar los cambios.

#### Windows

1. Buscar "Editar las variables de entorno del sistema" en el menú Inicio.
2. Acceder a "Variables de entorno".
3. En "Variables de usuario", crear entradas nuevas para DB_USER, DB_PASSWORD y JWT_SECRET.
4. Reiniciar el IDE para refrescar el entorno.

#### Otros sistemas operativos

Si lo anterior no te funciona o usas alguna fumada como ZorinOS o MacOS ni idea bro, abre Google o ChatGPT un rato.
