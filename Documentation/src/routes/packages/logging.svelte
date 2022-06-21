<script>
    import PageTitle from "../../components/PageTitle.svelte";
    import Section from "../../components/Section.svelte";
</script>

<svelte:head>
    <title>Logging</title>
</svelte:head>

<PageTitle title="Logging">
    Project consisting of a single NuGet package. It consists of an easy setup of an advanced and expandable logging.
</PageTitle>

<Section title="Sinks">
    Each sink is setup as a Serilog sublogger. Meaning, it has its own pipeline consisting of Filtering on event levels.
    There is support for Console and Seq as sinks.
</Section>

<Section title="Configuration">
    LogToConsole: true,
    LogToSeq: true,
    SeqUri: http://localhost:5341,
    SeqMinimumLogLevel: Information,
    ConsoleMinimumLogLevel: Information,
    Servicename: api
    GlobalOverrides: &rbrace;&lbrace;
    SeqOverrides: &rbrace;&lbrace;
    ConsoleOverrides: &rbrace;&lbrace;
</Section>

<Section title="Meta">
    Meta consists of data which is transferred between microservices regardless of protocol.
    Meta is populated when receiving messages through Masstransit,
    incoming requests through ASPNET middleware, sent along requests in ASPNET delegatehandlers
    and automatically sent along Sends and Publishes through Masstransit.
    It is then logged as properties in every log.

    A special remark; The Meta object is stored in an AsyncLocal to simulate Singleton behaviour.
    Since Serilog's implementation of enrichers, is utilizing the singleton pattern.
    Therefore, when logging Meta properties, the enrichers use the singleton Meta object.
</Section>

<Section title="CorrelationId">
    Get a CorrelationId from the header of an incoming HTTP request or AMQP message.
    It is then logged in every log as a property.
</Section>

<Section title="RequestId">
    Get a RequestId from the header of an incoming HTTP request.
    It is then logged in every log as a property.
</Section>

<Section title="MessageId">
    Get a MessageId from the header of an incoming AMQP message.
    It is then logged in every log as a property.
</Section>

<Section title="Serilog Pipeline">
    Setup to allow logging to be extended by the developer.
    Each project and service allows logs from Information level.
    There are also capabilities to override namespaces on a global level and on each individual sink.
    At last metrics and healthchecks are only logged from Warning level.
</Section>

<Section title="Startup Logging">
    The project allows for the developer to output logs during startup as well.
    Remember to use Serilog.Log, and to also set the context with Serilog.Log.ForContext&lt;NAMESPACE&gt;()
    When the service has built the serviceprovider, it is ready to create the advanced pipeline.
    It adds services from the serviceprovider on top of the already built startup logging pipeline.
    These include the metacontextaccessor object.
</Section>