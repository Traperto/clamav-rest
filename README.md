# ClamAV-Rest

## Overview

Simple [ClamAV](https://www.clamav.net/) REST proxy. Inspired by [Solita/clamav-rest](https://github.com/solita/clamav-rest), this client adds some more environment configuration.

This source code is build into the docker image [jreinartz/clamav-rest](https://hub.docker.com/repository/docker/jreinartz/clamav-rest).

Your __ClamAV-daemon is in the same docker-compose.yml__? Or shall the __service of the Docker host__ be used? Change these: 

- HOST or IP
- PORT of ClamAV-daemon

You want to customize the __maximal size of scanable file__? This is your parameter: 

- MAX_STREAM_SIZE  (in Bytes)

## docker-compose.yml with docker image

Simply use the `docker-compose.yml` of this repository for a working REST-API. It contains the ClamAV server and uses the image [jreinartz/clamav-rest](https://hub.docker.com/repository/docker/jreinartz/clamav-rest).

Run `docker compose up` to start the service.

Scan any file using `curl -F "file=@./eicar.txt" localhost:8080/scan` where `eicar.txt` is the file to be scanned.

The answer will be either `Everything ok : false` for malicious files or otherwise `Everything ok : true`.

## docker-compose.yml with source

Clone this source code and use the `docker-compose.yml` of the repository for a working REST-API. It contains the ClamAV server.

```
version: "3.2"

services:
  clamav:
    build: .
    environment:
      HOST: host.docker.internal
    ports:
      - 5000:80
    extra_hosts:
      - "host.docker.internal:host-gateway"
```

Run `docker compose up` to start the service. The existing `Dockerfile` will be used, to package the source code into a Docker image. Therefore some download traffic will occur.

Scan any file using `curl -F "file=@./eicar.txt" localhost:8080/scan` where `eicar.txt` is the file to be scanned.

The answer will be either `Everything ok : false` for malicious files or otherwise `Everything ok : true`.

## Accessing the host ClamAV

_Tested on Ubuntu 20.04_

With this `docker-compose.yml` the service will access the host daemon.

```
version: "3.2"

services:
  clamav:
    image: jreinartz/clamav-rest
    environment:
      HOST: host.docker.internal
    ports:
      - 5000:80
    extra_hosts:
      - "host.docker.internal:host-gateway"
```

There are some actions, that may be required (THESE CHANGES MAY LEAD TO A DEFECTIVE SYSTEM. DO ONLYPERFORM CHANGES, IF YOU KNOW WHAT YOU ARE DOING!):
1. The `clamd.conf` must be changed, so that the service will listen to TCP and not the socket. This command may help you `sudo dpkg-reconfigure clamav-daemon`
1. If a firewall is present (e.g. [ufw](https://wiki.ubuntuusers.de/ufw/)), it must allow traffic from Docker container to the host port.
   Example for opening the firewall wide (what you surely do not want) for all possible docker IPs `sudo ufw allow in from 172.0.0.0/8`

## Hint to the file size

As you can see, this code supports the configuration of a maximal file size for scanned files. Make sure, that the ClamAV service also supports this file size. Otherwise, an error will occur.

### How set the file size for ClamAV?

1. If using the host service, simply change the `/etc/clamav/clamd.conf` to the wished file size. This should be the corresponding settings:
   `MaxScanSize`, `MaxFileSize`, `PCREMaxFileSize`, `StreamMaxLength`
1. If using the dockerized ClamAV image, you want to overwrite the conf using a volume link to a more fitting clamd.conf

```
    …
    volumes:
      - ./clamd.conf:/etc/clamav/clamd.conf
    …
```

## Thanks

Thanks to [Solita](https://github.com/solita) for the inspiration.

## Contact

Contact us via [traperto.com](https://traperto.com).