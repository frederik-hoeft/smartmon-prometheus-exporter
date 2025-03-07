# Smartmon Prometheus Exporter

Smartmon Prometheus Exporter is a tool to export metrics from smartmontools to Prometheus format. This repository provides both containerized and local deployment options.

## Installation

### Containerized Deployment

To deploy the Smartmon Prometheus Exporter using Docker, follow these steps:

1. **Clone the repository:**
    ```sh
    git clone https://github.com/frederik-hoeft/smartmon-prometheus-exporter.git
    cd smartmon-prometheus-exporter
    ```

2. **Edit the configuration files (optional):**
    Edit the `docker-compose.yml` file and the `conf/settings.json` file to customize the exporter's behavior.

2. **Build and run the Docker containers:**
    ```sh
    docker compose up -d
    ```

This will build the Docker image and start the `smartctl-exporter` and `node-exporter` services. The `smartctl-exporter` service will run every 5 minutes and export metrics to the data directory. The `node-exporter` service will provide the host metrics, including the collected smartctl metrics, to Prometheus on port 9100.

### Local Deployment (Linux, system-wide)

To install and run the Smartmon Prometheus Exporter locally, follow these steps:

1. **Clone the repository:**
    ```sh
    git clone https://github.com/frederik-hoeft/smartmon-prometheus-exporter.git
    cd smartmon-prometheus-exporter
    ```

2. **Run the installation script:**
    ```sh
    chmod +x build.sh
    chmod +x install.sh
    sudo ./install.sh
    ```

This script will build the Smartmon Prometheus Exporter, install it to `/usr/local/bin`, and set up the necessary configuration and cron job. To uninstall the exporter, run `sudo ./uninstall.sh`.

### Local Deployment (Linux, build-only)

To build the Smartmon Prometheus Exporter locally without installing it, run:

```sh
chmod +x build.sh
./build.sh
```

The final binary will be located at `./bin/smartmon-exporter`.

### Local Deployment (Windows)

To build the Smartmon Prometheus Exporter locally on Windows, you will need to install the latest [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0). Then, run the following commands in PowerShell:

```powershell
git clone https://github.com/frederik-hoeft/smartmon-prometheus-exporter.git
cd smartmon-prometheus-exporter
dotnet publish .\SmartmonExporter\SmartmonExporter\SmartmonExporter.csproj --arch x64 --configuration Release --os win --output .\bin\
```

The final binary will be located at `.\bin\SmartmonExporter.exe`. Change the `--arch` and `--os` options to build for different platforms.

## Usage (Local Deployment)

The Smartmon Prometheus Exporter is installed to `/usr/local/bin/smartmon-exporter`. You can run it manually with the following command:

```sh
smartmon-exporter export --config-path /usr/local/etc/smartmon-exporter.json
```

The configuration file is located at `/usr/local/etc/smartmon-exporter.json`. You can edit this file to customize the exporter's behavior.

## Configuration

The configuration file for the Smartmon Prometheus Exporter is a JSON file. Here is an example configuration:

```json
{
  "DebugMode": false,
  "SmartctlPath": "/usr/sbin/smartctl",
  "OutputPath": "./smart-metrics.prom",
  "PrometheusNamespace": null,
  "WriteToConsole": false,
  "Devices": [
    "/dev/sda",
    "/dev/sdc",
    "/dev/sdd"
  ]
}
```

The configuration options are as follows:
- `DebugMode` (bool): Print every command executed by the exporter.
- `SmartctlPath` (string): The path to the smartctl binary.
- `OutputPath` (string): The path to write the Prometheus metrics to.
- `PrometheusNamespace` (string): The namespace to use for the Prometheus metrics. This will be prepended to all metric names.
- `WriteToConsole` (bool): Write the Prometheus metrics to the console instead of a file. This will override the `OutputPath` option and disable the `DebugMode` option, if set.
- `Devices` (array of strings): The list of devices to collect metrics for, or `null` to let smartctl auto-detect devices.

> [!WARNING]
> On windows, you will need to specify the device paths as `C:`, `D:`, etc. Auto-discovery through smartctl is ***not*** supported on Windows.