#! /bin/bash

# path to the godot executable
GODOT=/home/tim/bin/Godot_v3.1.1-stable_mono_x11_64/Godot_v3.1.1-stable_mono_x11.64

$GODOT --export "Linux/X11" panzerdeathmatch3d-linux.x86_64
$GODOT --export "Windows Desktop" panzerdeathmatch3d-windows.exe

