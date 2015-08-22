#!/bin/sh

ROOT_DIR=$PWD
SOURCE_DIR=$PWD/src
NUGET_DIR=$SOURCE_DIR/.nuget
PACKAGE_DIR=$SOURCE_DIR/packages
NUGET_BIN=$NUGET_DIR/NuGet.exe
FAKE_BIN=$PACKAGE_DIR/FAKE/tools/FAKE.exe

chmod +x $NUGET_BIN
[ -f "$FAKE_BIN" ] || mono $NUGET_BIN Install FAKE -OutputDirectory $PACKAGE_DIR -ExcludeVersion
chmod +x $FAKE_BIN
mono $FAKE_BIN build.fsx
exitcode=$?
git checkout $NUGET_BIN
exit $exitcode
