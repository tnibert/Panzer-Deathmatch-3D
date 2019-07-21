#! /bin/bash

VERSION="0.7"
LINFILE="panzerdeathmatch3d-linux"
WINFILE="panzerdeathmatch3d-windows"
tar -cf $LINFILE-$VERSION.tar $LINFILE.x86_64 $LINFILE.pck README.txt
gzip $LINFILE-$VERSION.tar
zip $WINFILE-$VERSION.zip $WINFILE.exe $WINFILE.pck README.txt
mkdir -p pkg_artifacts
mv $LINFILE-$VERSION.tar.gz pkg_artifacts/
mv $WINFILE-$VERSION.zip pkg_artifacts/
