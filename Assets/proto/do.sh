#!/usr/bin/env bash

shelldir=$(cd `dirname $0`; pwd)
cd "$shelldir"

./flatc -l -o "../BundleRes/proto" `find . -name "*.fbs"`