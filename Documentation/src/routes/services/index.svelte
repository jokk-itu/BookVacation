<script>
    import PageTitle from "../../components/PageTitle.svelte";
    import Section from "../../components/Section.svelte";
</script>

<svelte:head>
    <title>Services</title>
</svelte:head>

<PageTitle title="Services">
    All services are microservices and adopt a clean architecture. Each microservice is updated to the
    latest dotnet version. A microservice contains an Api project, Infrastructure project, Contracts
    project, Domain project and Test project.
</PageTitle>

<Section title="API">
    It is a simple Api receiving requests by HTTP and serves as main assembly of the microservice.
</Section>

<Section title="Infrastructure">
    It consists of infrastructure services, such as communication with other services eg. the Broker. It
    also uses the Mediator pattern which serves as the main pattern in a microservice.
</Section>

<Section title="Contracts">
    It consists of requests and responses which are used in the Api. It also consists of events which
    are distributed through the broker.
</Section>

<Section title="Domain">
    Consists of POCO's which illustrate documents of the datamodel.
</Section>

<Section title="Tests">
    Consists only of Unit tests.
</Section>