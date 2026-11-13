# PIKMATH

Videojuego educativo 2D para practicar sumas y restas, dirigido a alumnado de primaria. Proyecto de la asignatura **Desarrollo de Videojuegos** (curso 2026-2027).

El juego se desarrolla en Unity y se publica como build web en Vercel, para que se pueda jugar desde cualquier navegador.

---

## 📋 Requisitos

| Herramienta | Versión / detalle |
|---|---|
| **Unity** | **6000.3.19f1** (exactamente esta versión) |
| Módulo de Unity | **Web Build Support** |
| Git | [GitHub Desktop](https://desktop.github.com/) (recomendado) |

> ⚠️ **Todo el equipo debe usar la misma versión de Unity.** Si se abre el proyecto con otra versión, Unity lo convierte y se generan conflictos en el repositorio.

### Comprobar que tienes el módulo web

1. En Unity Hub, ve a **Installs**.
2. En la versión 6000.3.19f1, pulsa el icono de la **rueda dentada ⚙** (o **Manage**) → **Add modules**.
3. Marca **Web Build Support** e instálalo.

También puedes comprobarlo desde Unity: **File → Build Profiles → Web**. Si aparece el botón *Install with Unity Hub*, falta el módulo; si aparece *Switch Platform*, ya lo tienes.

---

## 🚀 Primeros pasos

1. **Clona el repositorio** con GitHub Desktop: *File → Clone repository*.
2. En **Unity Hub**, pulsa **Add → Add project from disk** y selecciona la carpeta del repositorio.
3. Abre el proyecto. La primera vez tardará un rato porque Unity genera la carpeta `Library/`.
4. Configura la vista **Game** a 1920×1080 (ver abajo).

### Configurar la vista Game a 1920×1080

Esta opción **no se guarda en el repositorio**, así que cada persona debe configurarla en su ordenador:

1. Abre la pestaña **Game**.
2. Despliega el menú que pone **Free Aspect**.
3. Si aparece **1920x1080** o **Full HD**, selecciónalo. Si no, pulsa **+** abajo del todo:
   - **Label**: `Full HD`
   - **Type**: *Fixed Resolution*
   - **Width & Height**: `1920` × `1080`
4. Pulsa **OK** y selecciónala.

Así veremos las pantallas con las mismas proporciones que los diseños del GDD.

---

## 📁 Estructura del proyecto

```
repositorio/
├── .gitignore
├── README.md
├── Assets/
│   └── _PIKMATH/
│       ├── Art/
│       │   ├── Backgrounds/   (prado verde, otoñal, nocturno)
│       │   ├── Characters/
│       │   ├── Avatars/
│       │   └── UI/            (botones, reloj, contador, paneles)
│       ├── Audio/
│       ├── Fonts/
│       ├── Prefabs/
│       ├── Scenes/
│       └── Scripts/
│           ├── Gameplay/
│           ├── UI/
│           └── Data/
├── Packages/
├── ProjectSettings/
└── web/                       (build WebGL que publica Vercel)
    ├── vercel.json
    ├── index.html
    ├── Build/
    └── TemplateData/
```

Todo nuestro contenido va dentro de `Assets/_PIKMATH/`. Las carpetas `Library/`, `Temp/`, `Logs/` y `UserSettings/` las genera Unity y **no se suben** (están en el `.gitignore`).

### Escenas

Orden en **File → Build Profiles → Scene List**:

| Índice | Escena | Contenido |
|---|---|---|
| 0 | `Inicio` | Pantalla de inicio |
| 1 | `Historia` | Historieta con botón «saltar» |
| 2 | `MenuPrincipal` | Menú + paneles de Ajustes, Usuario y Ranking |
| 3 | `SeleccionNiveles` | Selección de los tres niveles |
| 4 | `Juego` | Una sola escena para los 3 niveles y los 2 modos, con el menú de pausa como panel |

### Importar imágenes

Al importar un PNG, en el Inspector pon **Texture Type: Sprite (2D and UI)** y pulsa **Apply**.

---

## 👥 Reparto de tareas

| Persona | Tarea principal | Trabaja sobre todo en |
|---|---|---|
| **Raquel** | Mecánica principal y operaciones matemáticas | Escena `Juego`, `Scripts/Gameplay` |
| **Carme** | Interfaz gráfica y elementos visuales | `Art/`, prefabs de UI |
| **Irene** | Menús | Escenas de menús, `Scripts/UI` |
| **Diego** | GDD, pruebas y corrección de errores | Builds de prueba |

---

## 🤝 Normas para trabajar en equipo

- **Antes de empezar a trabajar:** en GitHub Desktop, pulsa **Fetch origin** y después **Pull origin** para tener la última versión.
- **Al terminar:** haz **commit** con un mensaje claro (por ejemplo, «Añadido menú de pausa») y **Push origin**.
- **No editéis la misma escena a la vez.** Los conflictos en escenas son muy difíciles de resolver. Si necesitas tocar una escena de otra persona, avisa primero por el grupo.
- Usad **prefabs** siempre que se pueda: así cada uno puede trabajar en su parte sin tocar la escena.
- **Subid siempre los archivos `.meta`** junto a los assets. Unity los necesita para mantener las referencias; si falta uno, se rompen enlaces entre objetos.
- No subáis la carpeta `Library/`. Si GitHub Desktop os muestra archivos de `Library/` o `Temp/`, avisad: el `.gitignore` no está funcionando.

---

## 🌐 Publicar en la web (Vercel)

Vercel no compila Unity: solo publica los archivos de la carpeta `web/`. Por eso hay que generar la build y subirla al repositorio.

### Generar la build

1. **File → Build Profiles → Web**.
2. Comprueba en **Player Settings → Publishing Settings** que **Compression Format** está en **Brotli**.
3. Pulsa **Build** y selecciona como destino la carpeta **`web/`** del repositorio.
4. Comprueba que el archivo `web/vercel.json` sigue ahí después de la build. Si ha desaparecido, vuelve a copiarlo.
5. Haz commit y push. **Vercel publicará la nueva versión automáticamente.**

### Configuración de Vercel (ya hecha, solo como referencia)

En *Settings → Build and Deployment*:

- **Root Directory:** `web`
- **Framework Preset:** Other
- **Build Command:** vacío

### Si el juego no carga en el navegador

Si aparece un error sobre *Content-Encoding* o compresión:

1. Ve a **Player Settings → Publishing Settings**.
2. Activa **Decompression Fallback**.
3. Vuelve a generar la build y súbela.

---

## 📄 Documentación

El diseño completo del juego (mecánicas, niveles, personajes, pantallas, HUD y planificación) está en el **GDD de PIKMATH**.