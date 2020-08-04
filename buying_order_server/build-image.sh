#!/bin/bash

releaseVersion=$1;

if [ "$releaseVersion" == "SOURCE_BRANCH" ] || [ -z "$releaseVersion" ];
  then
    releaseVersion="0.0.0";
fi

rm -rdf ./dist
echo BUILDING VERSION $releaseVersion
docker build . --no-cache -t buying-order-server:$releaseVersion -t buying-order-server:latest
