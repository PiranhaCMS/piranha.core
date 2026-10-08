#!/usr/bin/env bash#!/bin/bash

dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:ExcludeByAttribute="NoCoverage"