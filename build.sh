#!/bin/sh

ROOT_DIR=$PWD
SOURCE_DIR=$PWD/src
PAKET_DIR=$SOURCE_DIR/.paket
PACKAGE_DIR=packages/
PAKET_BOOTSTRAPPER_BIN=$PAKET_DIR/paket.bootstrapper.exe
PAKET_BIN=$PAKET_DIR/paket.exe
FAKE_BIN=$PACKAGE_DIR/FAKE/tools/FAKE.exe

chmod +x $PAKET_BOOTSTRAPPER_BIN
chmod +x $PAKET_BIN
mono $PAKET_BOOTSTRAPPER_BIN
mono $PAKET_BIN restore
chmod +x $FAKE_BIN
mono $FAKE_BIN build.fsx
exitcode=$?
git checkout $PAKET_BOOTSTRAPPER_BIN
exit $exitcode
