#!/usr/bin/env bash

# set -x
shelldir=$(cd `dirname $0`; pwd)
cd "$shelldir"

./flatc-1.6.0 -b --no-prefix --schema -o "../BundleRes/proto/bfbs/" `find . -name "*.fbs"`

find "../BundleRes/proto/bfbs" -name "*.bfbs" -exec mv "{}" "{}.txt" \;

\rm -rf $shelldir/../../skynet/proto
ln -fs $shelldir/../BundleRes/proto $shelldir/../../skynet/proto