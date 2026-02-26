# 🌍 Godot World Generator

Ein prozeduraler Weltengenerator für Spieleentwicklung mit **Godot 4** und **C#**.
<img width="1145" height="648" alt="image" src="https://github.com/user-attachments/assets/67d5f9f3-93ce-4811-9587-9672edb251db" />

---

## ✨ Features

### 🌱 Seed-basierte Weltgenerierung
Jede Welt wird über einen einzigartigen Seed generiert – reproduzierbar und einzigartig. Die Noise-Funktion sorgt für natürlich wirkendes, abwechslungsreiches Terrain.

### 🎨 Anpassbarer Textur-Shader
Jedes Terrain-Tile erhält eine prozedurale Textur, die sich dynamisch an die Geländeform anpasst und vollständig anpassbar ist.

### 💾 SQLite-Speicherung
Position und Rotation aller Terrain-Objekte werden in einer lokalen SQLite-Datenbank gespeichert – die Welt bleibt beim nächsten Start exakt so wie sie war.

### 🧍 First-Person Player
Direkt spielbarer First-Person Controller mit WASD, Maussteuerung und Springen – ready to go.

---

## 🛠️ Stack

- **Godot 4** + **C#**
- **SQLite** (`Microsoft.Data.Sqlite`)
- **FastNoiseLite** für Terrain & Texturen

---

## 📄 Lizenz

MIT License
