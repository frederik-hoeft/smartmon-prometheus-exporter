#!/bin/bash

# enable strict mode
set -euo pipefail

# check for root permissions
if [ "$(id -u)" -ne 0 ]; then
    echo 'To uninstall smartmon-exporter from this system, you need to run this script as root.'
    exit 1
fi

# remove binaries from /usr/local/bin
if [ -f '/usr/local/bin/smartmon-exporter' ]; then
    echo 'deleting /usr/local/bin/smartmon-exporter'
    /usr/bin/rm '/usr/local/bin/smartmon-exporter'
fi

# remove config from /usr/local/etc
if [ -f '/usr/local/etc/smartmon-exporter.json' ]; then
    echo 'deleting /usr/local/etc/smartmon-exporter.json'
    /usr/bin/rm '/usr/local/etc/smartmon-exporter.json'
fi

# remove metrics output directory from /usr/local/share
if [ -d '/usr/local/share/smartmon-exporter' ]; then
    echo 'deleting /usr/local/share/smartmon-exporter'
    /usr/bin/rm -r '/usr/local/share/smartmon-exporter'
fi

# remove cron job from /etc/cron.d
if [ -f '/etc/cron.d/smartmon-exporter' ]; then
    echo 'deleting /etc/cron.d/smartmon-exporter'
    /usr/bin/rm '/etc/cron.d/smartmon-exporter'
fi

# done
echo 'smartmon-exporter has been uninstalled from this system.'