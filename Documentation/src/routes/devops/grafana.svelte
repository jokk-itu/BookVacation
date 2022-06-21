<script>
    import PageTitle from "../../components/PageTitle.svelte";
    import Section from "../../components/Section.svelte";
</script>

<svelte:head>
    <title>Grafana</title>
</svelte:head>

<PageTitle title="Grafana">
    Service to monitor the system as a whole, by illustrating metrics gathered from Prometheus which scrape metrics from the different services.
</PageTitle>

<Section title="Masstransit">
    Exposes metrics which include different data in regards to messaging.
</Section>

<Section title="ASPNET">
    Exposes metrics which include different data in regards to requests and the container.
</Section>