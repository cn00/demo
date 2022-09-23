

subbrname=${1}
thisdir="${PWD}"

git submodule foreach --recursive git remote -v | grep -e '^origin.*fetch' | awk '{print $2}' | while read l; do
    local baredir="${l/*\/}"
    if [[ ! -d "../bare/$baredir" ]];then
        pushd "../bare"
        git clone --bare $l $baredir
        popd
    fi
done

set -ex
git submodule | gsed -e 's/^[ +-]//g' | while read l; do
    eval `echo $l | awk -F'[ ]' '{printf("hash=%s;subpath=%s", $1, $2)}'`
    # hash="${l/* }"
    # if [[ x$hash == 'x' ]];then hash='master';fi
    # subpath="${l/* }"
    gitname="${subpath/*\/}.git"
    if [[ -d /Users/cn/ws/bare/${gitname} ]]; then 
        echo "${l}: ${gitname}:${subpath} ==> ${thisdir}/../bare/${gitname}";
        if [[ -d ${subpath} ]]; then rm -rf ./${subpath}; fi
        pushd "${thisdir}/../bare/${gitname}"
        git br -f "${subbrname}" "${hash}"
        git worktree add -f "${thisdir}/${subpath}" "${subbrname}"
        popd
    fi;
done