#!/bin/bash

# enable strict mode
set -euo pipefail

script_dir="$(/usr/bin/realpath "$(/usr/bin/dirname "${BASH_SOURCE[0]}")")"
build_dir="${script_dir}/build"
output_dir="${script_dir}/output"

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
/usr/bin/docker build --target export -o "${output_dir}" "${work_dir}"

# remove build directory
/usr/bin/rm -r "${build_dir}"

# check for root permissions
if [ "$(id -u)" -ne 0 ]; then
    echo 'To install smartmon-exporter on this system, you need to run this script as root.'
    exit 1
fi

# install to host
# binaries go to /usr/local/bin
/usr/bin/cp "${output_dir}/SmartmonExporter" '/usr/local/bin/smartmon-exporter'
/usr/bin/chmod +x '/usr/local/bin/smartmon-exporter'
# config goes to /usr/local/etc
/usr/bin/cp "${script_dir}/config-templates/host.settings.json" '/usr/local/etc/smartmon-exporter.json'
# metrics output goes to /usr/local/share, readable only by root
/usr/bin/mkdir -p '/usr/local/share/smartmon-exporter'
/usr/bin/chown root:root '/usr/local/share/smartmon-exporter'
/usr/bin/chmod 700 '/usr/local/share/smartmon-exporter'
# cron job goes to /etc/cron.d
cron_job_file="# /etc/cron.d/smartmon-exporter

SHELL=/bin/sh
PATH=/usr/local/sbin:/usr/local/bin:/sbin:/bin:/usr/sbin:/usr/bin

# run smartmon-exporter every 5 minutes
*/5 * * * * root /usr/local/bin/smartmon-exporter --config-path /usr/local/etc/smartmon-exporter.json"
echo "${cron_job_file}" > '/etc/cron.d/smartmon-exporter'

# clean up output directory
/usr/bin/rm -r "${output_dir}"

# done
echo 'Smartmon-Exporter has successfully been installed on your system.'
echo 'Please edit the configuration file at /usr/local/etc/smartmon-exporter.json to your needs.'
echo 'By default, the exporter will run every 5 minutes and write metrics to /usr/local/share/smartmon-exporter.'
echo 'You can start the exporter manually by running /usr/local/bin/smartmon-exporter --config-path /usr/local/etc/smartmon-exporter.json.'