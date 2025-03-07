#!/bin/bash

# enable strict mode
set -euo pipefail

script_dir="$(/usr/bin/realpath "$(/usr/bin/dirname "${BASH_SOURCE[0]}")")"
build_dir="${script_dir}/build"
output_dir="${script_dir}/bin"

# build smartmon-exporter
"${script_dir}/build.sh"

# check for root permissions
if [ "$(id -u)" -ne 0 ]; then
    echo 'To install smartmon-exporter on this system, you need to run this script as root.'
    exit 1
fi

# install to host
# binaries go to /usr/local/bin
/usr/bin/cp "${output_dir}/smartmon-exporter" '/usr/local/bin/smartmon-exporter'
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
*/5 * * * * root /usr/local/bin/smartmon-exporter export --config-path /usr/local/etc/smartmon-exporter.json"
echo "${cron_job_file}" > '/etc/cron.d/smartmon-exporter'

# clean up output directory
/usr/bin/rm -r "${output_dir}"

# done
echo 'Smartmon-Exporter has successfully been installed on your system.'
echo 'Please edit the configuration file at /usr/local/etc/smartmon-exporter.json to your needs.'
echo 'By default, the exporter will run every 5 minutes and write metrics to /usr/local/share/smartmon-exporter.'
echo 'You can start the exporter manually by running /usr/local/bin/smartmon-exporter export --config-path /usr/local/etc/smartmon-exporter.json'
echo 'To uninstall smartmon-exporter from this system, run ./uninstall.sh as root.'
echo 'To see available commands, run /usr/local/bin/smartmon-exporter --help.'