#!/bin/bash

# enable strict mode
set -euo pipefail

script_dir="$(/usr/bin/realpath "$(/usr/bin/dirname "${BASH_SOURCE[0]}")")"
build_dir="${script_dir}/build"
output_dir="${script_dir}/bin"

# remove old build directory
if [ -d "${build_dir}" ]; then
    /usr/bin/rm -r "${build_dir}"
fi

# create new build directory
/usr/bin/mkdir -p "${build_dir}"
# copy source files to build directory
/usr/bin/cp -r "${script_dir}/SmartmonExporter" "${build_dir}"
work_dir="${build_dir}/SmartmonExporter"
# remove ENTRYPOINT from Dockerfile
/usr/bin/sed -i '/ENTRYPOINT/d' "${work_dir}/Dockerfile"
# add new export stage to Dockerfile:
echo 'FROM scratch AS export' >> "${work_dir}/Dockerfile"
echo 'COPY --from=publish /app/publish /' >> "${work_dir}/Dockerfile"

# build docker image with multi-stage build and export stage to output directory
/usr/bin/docker build --no-cache --target export -o "${output_dir}" "${work_dir}"

# remove build directory
/usr/bin/rm -r "${build_dir}"

# rename binary to smartmon-exporter
/usr/bin/mv "${output_dir}/SmartmonExporter" "${output_dir}/smartmon-exporter"

# done
echo 'smartmon-exporter has been built and exported to the output directory.'
echo 'try running `./bin/smartmon-exporter --help` to see available commands.'
echo 'to install smartmon-exporter on this system and register it as a cron job, run `./install.sh`.'