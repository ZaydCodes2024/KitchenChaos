# 🍳 Kitchen Chaos

> A multiplayer cooking chaos game built with **Unity 6** — cook recipes, cut vegetables, fry patties, and deliver plates under time pressure, together with up to 4 players over the internet.

Kitchen Chaos is my take on the [Code Monkey multiplayer course](#credits) project, rebuilt with the latest Unity and Netcode versions. It is a complete, working multiplayer game: matchmaking through Unity Lobbies, connection via Relay (no port forwarding needed), a lobby/character-select flow, and a full round-based game loop.

---

## 📸 Screenshots

> **Adding your screenshots:** create a folder called `docs/images/` at the project root, save your screenshots in it with the file names below (PNG or JPG), and every image will show up automatically in this README. Until the files exist the links will show as broken thumbnails — that's expected. Tip: keep screenshots the same aspect ratio and around 1280×720 for a clean gallery.

<table>
  <tr>
    <td align="center">
      <img src="docs/images/gameplay.png" alt="Gameplay" width="600"/><br/>
      <em>In-game: two chefs rushing orders</em>
    </td>
    <td align="center">
      <img src="docs/images/gameplay-2.png" alt="Gameplay 2" width="600"/><br/>
      <em>Cutting and frying stations in action</em>
    </td>
  </tr>
  <tr>
    <td align="center">
      <img src="docs/images/main-menu.png" alt="Main menu" width="600"/><br/>
      <em>Main menu</em>
    </td>
    <td align="center">
      <img src="docs/images/lobby.png" alt="Lobby" width="600"/><br/>
      <em>Lobby list and joining</em>
    </td>
  </tr>
  <tr>
    <td align="center">
      <img src="docs/images/character-select.png" alt="Character select" width="600"/><br/>
      <em>Character selection (color + name)</em>
    </td>
    <td align="center">
      <img src="docs/images/orders-ui.png" alt="Orders UI" width="600"/><br/>
      <em>Waiting orders on the delivery side</em>
    </td>
  </tr>
  <tr>
    <td align="center">
      <img src="docs/images/pause.png" alt="Pause + options" width="600"/><br/>
      <em>Pause and options (key rebinding)</em>
    </td>
    <td align="center">
      <img src="docs/images/game-over.png" alt="Game over" width="600"/><br/>
      <em>Scores at the end of a 90-second round</em>
    </td>
  </tr>
</table>

Single-image version (use this if you prefer a classic layout over the table):

```markdown
![Gameplay](docs/images/gameplay.png)
```

---

## ✨ Features

### Gameplay
- **Round-based cooking loop** — 90-second rounds with a countdown start, live timer bar, and a game-over scoring screen.
- **Full recipe pipeline** — order panels spawn periodically (server-authoritative), and each delivered plate is validated ingredient-by-ingredient against the recipe.
- **Data-driven design** — every recipe, kitchen object, cutting/frying/burning step is a `ScriptableObject`, so you can add new recipes or items *without touching any code*.
- **Kitchen station types:**
  - `ClearCounter` — plain interaction counters
  - `ContainerCounter` — ingredient dispensers
  - `CuttingCounter` — chop tomatoes, cheese, cabbage with progress bar and animation
  - `StoveCounter` — fry meat with 3 states (raw → cooked → burned!) and sizzle sound that fades with progress
  - `PlatesCounter` — stack and grab plates for multi-ingredient assembly
  - `DeliveryCounter` — hand in finished plates
  - `TrashCounter` — destroy items you regret picking up
- **Character animations** — walking, cutting, and ice-pick delivery animations driven by the movement/held-object state; visuals are fully separate from the player input logic so each client only controls its own character.
- **Game feel** — Cinemachine camera, music/sound managers, sounds for every interaction, recipes succeed/fail feedback plates.

### Multiplayer
- **Up to 4 players** with connection approval (rejecting full games / mid-game joins).
- **Internet play without port forwarding** — Unity **Relay** with DTLS encryption handles connectivity.
- **Unity Lobby integration** — create private/Public lobbies, quick join, join by code or by ID, periodic lobby listing, heartbeat keep-alive, host kick, and leave.
- **In-multiplayer pause policy** — the game pauses only if *any* connected player pauses (and stays paused until they resume); `Time.timeScale` handled via `NetworkVariable`.
- **Distributed state** — all game state (phase, counters, timers, spawned kitchen objects) lives on the server and is replicated with `NetworkVariable`s, ClientRpcs and a `NetworkList` of player data. Clients only ever send input; the host computes and replicates.
- **Character select scene** — pick your character color (server-validated so no duplicates) and player name, with a "ready" handshake that gates the countdown for everyone.
- **Graceful disconnection UI** — host-left detection, failed-join messaging, multiplayer pause overlay.

### Polish & UX
- **Remappable controls** — interactive rebinding for keyboard *and* gamepad, persisted to `PlayerPrefs` as JSON.
- **Pause menu + options** with dynamic binding text.
- **Loading scene** with animated progress bar between scenes (including networked scene changes).
- **Input System** (keyboard + gamepad), URP rendering, TMP everywhere.

---

## 🎮 Controls

| Action | Keyboard | Gamepad |
|---|---|---|
| Move | WASD / arrow keys | Left stick |
| Interact / pick up | `E` | Y / Triangle |
| Interact alternate (chop, fry, slice-bread…) | `F` | X / Square |
| Pause | `Esc` | Start |

All bindings are **remappable in-game** via the Options menu.

---

## 🛠️ Tech Stack

| Layer | Choice |
|---|---|
| Engine | Unity **6000.3.8f1** (Unity 6.3) |
| Render pipeline | Universal Render Pipeline (URP) 17 |
| Networking | **Netcode for GameObjects 2.9.2** + Unity Transport (UTP, Relay DTLS) |
| Services | Unity Gaming Services — **Lobby**, **Relay**, **Authentication** (anonymous sign-in) |
| Input | new **Input System** 1.18 (`PlayerInputActions` generated class) |
| Camera | Cinemachine 3.1 |
| UI | TextMeshPro + uGUI |
| AI/Nav | AI Navigation package |

> **Note** — most core scripts are `NetworkBehaviour`s doing ServerRpc/ClientRpc dance; there is intentionally no separate server build: the host peer is the authority.

---

## 🚀 Getting Started

### Prerequisites
- Unity Hub + Unity **6000.3.8f1** (or a compatible Unity 6.x patch) with the **Windows/Mac (Standalone)** module.
- A (free) **Unity account** — needed only for *online* multiplayer.

### Open & run (single player)
1. Clone the repo:
   ```bash
   git clone <your-repo-url> KitchenChaos
   cd KitchenChaos
   ```
2. Open the project in Unity (open the `KitchenChaos` folder directly, not the solution).
3. Let the editor import packages and compile (first open can take a while).
4. From the editor build settings menu or the main scene `Assets/Scenes/MainMenuScene`, press **Play** and choose **Single player** — it starts a local host on `127.0.0.1:7777`.

### Configure Unity Gaming Services (needed for online multiplayer)
The lobby/relay flow uses UGS and needs your own project linkage:

1. In Unity, open **Window → Package Manager** — ensure `Unity Services Core Editor` is present (it comes with the multiplayer packages in this project).
2. Open the **Services window** (`Services` tab next to `Inspector` in newer Unity), sign in with your Unity account, and **link this project to a Unity Cloud project**.
3. In the Unity Dashboard, open your project and **activate** these products (all have free tiers):
   - **Authentication** (anonymous sign-in is used by `KitchenGameLobby.InitializeUnityAuthentication`)
   - **Lobby**
   - **Relay**
4. Enter play mode again — now **Create lobby / Quick join / Join by code** all work over the internet.

### Play-testing multiplayer locally in-editor
- Use **two machines**, or run two instances of a built player + one editor instance.
- Each Unity profile/account signs in anonymously; use the Lobby scene to host/join.
- `Assets/ScriptableObjects/` ships with the full recipe/kitchen-object data set, so no extra setup is required.

### Building
- Target platform: Standalone (Windows/Mac/Linux). Web **is not supported** by Netcode for GameObjects as a dedicated transport; the WebGL build in the player settings is a leftover template.

---

## 🧠 Architecture Overview

```
Assets/Scripts/
│  GameManager.cs            # Game state machine (NetworkVariable<State>) and pause policy
│  DeliveryManager.cs        # Server spawns orders; validates delivered plates
│  KitchenGameMultiplayer.cs # Connection approval, player data NetworkList, item spawn/destroy
│  KitchenGameLobby.cs       # UGS Lobby + Relay matchmaking (create/quick join/join by code/ID)
│  GameInput.cs              # Input System wrapper + rebinding persistence
│  Loader.cs / Player.cs     # Scene loader helper / player movement & interaction
│  SoundManager.cs / MusicManager.cs
CharacterSelection/          # Character select scene scripts
Counter visuals + animator
UI/                          # Game HUD, delivery UI, pause/options, lobby UI, character select UI
Counters/                    # One class per station (Base + Clear/Container/Cutting/Stove/Plates/Delivery/Trash)
ScriptableObjects/           # KitchenObjectSO, ReceipeSO, CuttingReceipeSO, FryingReceipeSO, BurningReceipeSO, lists
Player/                      # Player.cs, PlayerVisual.cs, PlayerAnimator.cs
```

### Key patterns
- **Singleton + event-decoupled UI:** managers expose `event EventHandler`s; UI scripts subscribe instead of polling.
- **Server-authoritative:** every mutating action goes through an `[Rpc(SendTo.Server)]` call and is fanned back out with `[ClientRpc]`/`NetworkVariable`.
- **Static-data via `ScriptableObject`s:** `ReceipeListSO` (order pool), `KitchenObjectListSO` (all ingredients), process recipes for cutting/frying/burning. The network layer exchanges only SO *indices*, not objects.
- **Scene-flow enum:** `Loader.Scene` decides the flow `MainMenu → Lobby → CharacterSelect → GameScene` with a loading scene in between.

---

## 🔧 Troubleshooting

Common gotchas:
- **`Could not initialize Unity Services`** — you haven't linked the project to a UGS project (see the configure step above) or the Lobby/Relay products aren't active.
- **Client can't connect through the lobby** — Relay join code is a lobby *data object* visible to members only; make sure invitees are in the lobby *before* the game scene loads (create → invite → go).
- **Editor-only testing** — running host + client in the *same* editor instance isn't supported; use a second copy / build.
- **Scripts report missing `KitchenObjectSO` icons** — code references icons the import pack doesn't include; they are optional and only used in the editor inspector.

---

## 🗺️ Roadmap / Ideas

- [ ] Matchmaker score / rounds persistence between matches
- [ ] Sprint/shuffle mechanism for competitive play
- [ ] Gamepad-only boost & dedicated server build (headless)
- [ ] More recipes (smoothies?) and a second map

---

## 🙏 Credits

- **[Code Monkey](https://www.youtube.com/@CodeMonkeyUnity)** — the original *Kitchen Chaos* design and the multiplayer course this project is based on.
- Unity's official documentation for Netcode for GameObjects, Relay, and Lobby.

---

## 📄 License

This project currently has **no license attached**. If you fork or build on it, keep in mind the art/audio assets came with Code Monkey's course and are only for private/educational use.
