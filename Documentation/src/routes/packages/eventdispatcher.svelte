<script>
    import PageTitle from "../../components/PageTitle.svelte";
</script>

<svelte:head>
    <title>EventDispatcher</title>
</svelte:head>

<PageTitle title="EventDispatcher">
    Project to abstract away the setup of MassTransit.
    There is setup of the bus, rabbitmq transport, filters for logging and metadata, as well as message retries,
    killswitch, circuit breaker, and metrics for prometheus.
</PageTitle>
