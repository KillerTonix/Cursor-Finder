# 🖱️ Cursor Finder -> Pulse Double-Ctrl

A lightweight Windows utility built with C# and .NET 4.8 that helps you locate your cursor instantly. Just tap the **Control** key twice, and the screen dims while a pulse emanates from your cursor.



## ✨ Features

- **Double-Ctrl Detection**: Minimalist trigger that doesn't interfere with your workflow.
- **Multi-Monitor Support**: Dims all connected screens simultaneously to make the cursor pop.
- **Spotlight Effect**: Uses a transparent overlay to create a "searchlight" visual around the mouse.
- **Click-Through Transparency**: The overlays use `WS_EX_TRANSPARENT` so you can trigger the pulse and keep clicking your apps without losing focus.
- **System Tray Integration**: Runs quietly in the background with a tray icon to exit or change settings.
- **Single Instance Protection**: Uses a System Mutex to prevent multiple copies from running.

## 🚀 How It Works

The app utilizes a **Low-Level Keyboard Hook** (`WH_KEYBOARD_LL`) to listen for `VK_LCONTROL` and `VK_RCONTROL`. When a double-tap is detected within 400ms:
1. It spawns `DarkenerForms` across all detected `Screen.AllScreens`.
2. It triggers a `RippleForm` that draws a growing, fading circle using GDI+.
3. It automatically cleans up resources after the animation finishes.



## 🛠️ Installation & Usage

1. **Clone & Build**: Open the solution in Visual Studio and build for `Release`.
2. **Run**: Launch the `.exe`. You will see an icon appear in your System Tray.
3. **Auto-Start**: To have it start with Windows, press `Win+R`, type `shell:startup`, and paste a shortcut to the executable there.

## 📝 Technical Requirements
- .NET Framework 4.8
- Windows OS (7, 10, or 11)
- Administrator privileges may be required if you want the pulse to show over other Admin-run windows (like Task Manager).

## 🛠️ Development Setup
The project uses `user32.dll` and `kernel32.dll` for:
- `SetWindowsHookEx` (Keyboard Interception)
- `SetWindowLong` (Click-through functionality)
- `GetModuleHandle` (Hook management)

---
*Created as a "Quality of Life" utility for power users with large multi-monitor setups.*
