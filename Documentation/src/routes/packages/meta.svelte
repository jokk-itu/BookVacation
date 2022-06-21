<script>
    import PageTitle from "../../components/PageTitle.svelte";
</script>

<svelte:head>
    <title>Meta</title>
</svelte:head>

<PageTitle title="Meta">
    Meta is used to contain meta data which is shown as properties in all log statements.
    It is instantiated upon requests and messages being received and it transfers information when communicating with other services.
    For example a CorrelationId used to correlate an entire request through all services.
</PageTitle>