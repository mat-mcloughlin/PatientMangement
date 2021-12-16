# PatientMangement

This is very basic implementation of the CQRS and ES patterns that I used in my talk "Introduction to CQRS and ES". It is anything but feature complete but should hopefully give you an idea on how to get the started

To get it running you first need to install default installations of both

- [EventStore](https://eventstore.com/)
- [RavenDB](https://ravendb.net/)

Both need to be running. You can use [Docker](https://www.docker.com/products/docker-desktop) to run databases using prepared Docker Compose configuration:

```shell
docker-compose up
```

# Structure
There are 4 projections 

## PatientManagement
This contains the domain and the command side framework

## ProjectionManager
This project contains all the functionality required for writing and producing projections

## SeedGenerator
Small console application that will randomly generate a number of events against the encounter

## TestConsole
Small console application that will attempt to admit, transfer and discharge a patient
