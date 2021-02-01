

subbrname=${1}
thisdir="${PWD}"

# set -x
git submodule | gsed -e 's/^[ +-]//g' | 
    awk -F'[ ]' '{print $2, $3}' | while read l; do
        hash="${l/* }"
        gitname="${l/*\/}.git"
        subpath="${l/* }"
        if [[ -d /Volumes/Data/bare/${gitname} ]]; then 
            echo "${l}: ${gitname}:${subpath} ==> /Volumes/Data/bare/${gitname}";
            if [[ -d ${subpath} ]]; then rmdir ${subpath}; fi
            pushd "/Volumes/Data/bare/${gitname}"
            git br "${subbrname}" "${hash}"
            git worktree add "${thisdir}/${subpath}" "${subbrname}"
            popd
        fi;
    done