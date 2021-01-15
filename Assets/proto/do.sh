#!/usr/bin/env bash

shelldir=$(cd `dirname $0`; pwd)
cd "$shelldir"

flatc --lua -o "../BundleRes/proto" `find . -name "*.fbs"`