#! /bin/bash

VERSION="0.8"
LINFILE="panzerdeathmatch3d-linux"
WINFILE="panzerdeathmatch3d-windows"
tar -cf $LINFILE-$VERSION.tar $LINFILE.x86_64 $LINFILE.pck README.txt
gzip $LINFILE-$VERSION.tar
zip $WINFILE-$VERSION.zip $WINFILE.exe $WINFILE.pck README.txt
mkdir -p pkg_artifacts
mv $LINFILE-$VERSION.tar.gz pkg_artifacts/
mv $WINFILE-$VERSION.zip pkg_artifacts/
sha1sum pkg_artifacts/$LINFILE-$VERSION.tar.gz > pkg_artifacts/$LINFILE-$VERSION.tar.gz.sha1
sha1sum pkg_artifacts/$WINFILE-$VERSION.zip > pkg_artifacts/$WINFILE-$VERSION.zip.sha1

