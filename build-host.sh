#!/bin/bash

# remove old build directory
script_dir="$(/usr/bin/realpath "$(/usr/bin/dirname "${BASH_SOURCE[0]}")")"
build_dir="${script_dir}/build"
output_dir="${script_dir}/output"
/usr/bin/rm -r "${build_dir}"

# create new build directory
/usr/bin/mkdir -p "${build_dir}"
# copy source files to build directory
/usr/bin/cp -r "${script_dir}/SmartmonExporter" "${build_dir}"
work_dir="${build_dir}/SmartmonExporter"
# add new export stage to Dockerfile:
echo "\n" >> "${work_dir}/Dockerfile"
echo "FROM scratch AS export" >> "${work_dir}/Dockerfile"
echo "COPY --from=publish /app/publish /" >> "${work_dir}/Dockerfile"

# build docker image with multi-stage build and export stage to output directory
/usr/bin/docker build --target export -o "${output_dir}" "${work_dir}"
# remove debug symbols from output directory (*.pdb, *.dbg)
/usr/bin/find "${output_dir}" -type f -name "*.pdb" -exec /usr/bin/rm {} \;
/usr/bin/find "${output_dir}" -type f -name "*.dbg" -exec /usr/bin/rm {} \;
# copy settings.json from config directory to output directory
/usr/bin/cp "${script_dir}/conf/settings.json" "${output_dir}/settings.json"

# remove build directory
/usr/bin/rm -r "${build_dir}"