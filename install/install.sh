#!/usr/bin/env bash

# create user
username="rustdeskapi"
if [[ $(id -u $username >/dev/null 2>&1; echo $?) -ne 0 ]]; then
    useradd -s /sbin/nologin $username
    echo "User $username created"
else
    echo "User $username already exists"
fi
echo "User $username created"

# set permissions
chown -R $username:$username ./

# create services
yes | cp -rf ./rustdeskapi.service /etc/systemd/system/
echo "Service added"

# enable services
systemctl enable rustdeskapi

# start services
systemctl start rustdeskapi
