#! /bin/bash
# clone repo
git clone https://github.com/godotengine/godot/
cd godot

# Build temporary binary
scons p=x11 tools=yes module_mono_enabled=yes mono_glue=no
# Generate glue sources
bin/godot.x11.tools.64.mono --generate-mono-glue modules/mono/glue

### Build binaries normally
# Editor
scons p=x11 target=release_debug tools=yes module_mono_enabled=yes mono_glue=yes
# Export templates
scons p=x11 target=debug tools=no module_mono_enabled=yes
scons p=x11 target=release tools=no module_mono_enabled=yes

# place build templates
mkdir -p ~/.local/share/godot/templates/3.1.beta.mono/
cp -r bin/* ~/.local/share/godot/templates/3.1.beta.mono/

cd ~/.local/share/godot/templates/3.1.beta.mono/

ln -s godot.x11.debug.64.mono linux_x11_64_debug
ln -s godot.x11.opt.64.mono linux_x11_64_release
