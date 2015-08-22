#!/bin/sh

ROOT_DIR=$PWD
SOURCE_DIR=$PWD/src
NUGET_DIR=$SOURCE_DIR/.nuget
PACKAGE_DIR=$SOURCE_DIR/packages
NUGET_BIN=$NUGET_DIR/NuGet.exe
FAKE_BIN=$PACKAGE_DIR/FAKE/tools/FAKE.exe

chmod +x $NUGET_BIN
[ -f "$FAKE_BIN" ] || $NUGET_BIN Install FAKE -OutputDirectory $PACKAGE_DIR -ExcludeVersion
chmod +x $FAKE_BIN
$FAKE_BIN build.fsx
git checkout $NUGET_BIN
