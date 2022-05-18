# Workflows

## Continous Integration
Runs on push on every branch

## Continous Deployment Service
Runs on push on /release and it must have a tag to push image
Can run on workflow_dispatch, but only build image

## Continous Deplyoment Nuget
Runs on push on /release and it must have a tag