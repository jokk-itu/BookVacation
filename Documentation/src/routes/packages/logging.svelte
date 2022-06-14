<svelte:head>
    <title>Logging</title>
</svelte:head>

<div class="py-4">
    <h2 id="title" class="text-3xl">
        <a class="text-blue-400 header-anchor" href="#title">#</a>Logging
    </h2>
    <hr class="border-blue-400"/>
</div>

Project consisting of a single NuGet package. It consists of an easy setup of an advanced and expandable logging.

<div class="py-4">
    <h2 id="sinks" class="text-xl">
        <a class="text-blue-400 header-anchor" href="#sinks">#</a>Sinks
    </h2>
    <hr class="border-blue-400"/>
</div>

Each sink is setup as a Serilog sublogger. Meaning, it has its own pipeline consisting of Filtering on event levels.
There is support for Console and Seq as sinks.

<div class="py-4">
    <h2 id="configuration" class="text-xl">
        <a class="text-blue-400 header-anchor" href="#configuration">#</a>Configuration
    </h2>
    <hr class="border-blue-400"/>
</div>

LogToConsole: true,
LogToSeq: true,
SeqUri: http://localhost:5341,
SeqMinimumLogLevel: Information,
ConsoleMinimumLogLevel: Information,
Servicename: api

<div class="py-4">
    <h2 id="meta" class="text-xl">
        <a class="text-blue-400 header-anchor" href="#meta">#</a>Meta
    </h2>
    <hr class="border-blue-400"/>
</div>

Meta consists of data which is transferred between microservices regardless of protocol.
Meta is populated when receiving messages through Masstransit, 
incoming requests through ASPNET middleware, sent along requests in ASPNET delegatehandlers 
and automatically sent along Sends and Publishes through Masstransit.
It is then logged as properties in every log.

A special remark; The Meta object is stored in an AsyncLocal to simulate Singleton behaviour. 
Since Serilog's implementation of enrichers, is utilizing the singleton pattern.
Therefore, when logging Meta properties, the enrichers use the singleton Meta object.

<div class="py-4">
    <h2 id="correlationid" class="text-xl">
        <a class="text-blue-400 header-anchor" href="#correlationid">#</a>CorrelationId
    </h2>
    <hr class="border-blue-400"/>
</div>

Get a CorrelationId from the header of an incoming HTTP request or AMQP message.
It is then logged in every log as a property.

<div class="py-4">
    <h2 id="requestid" class="text-xl">
        <a class="text-blue-400 header-anchor" href="#requestid">#</a>RequestId
    </h2>
    <hr class="border-blue-400"/>
</div>

Get a RequestId from the header of an incoming HTTP request.
It is then logged in every log as a property.

<div class="py-4">
    <h2 id="messageid" class="text-xl">
        <a class="text-blue-400 header-anchor" href="#messageid">#</a>MessageId
    </h2>
    <hr class="border-blue-400"/>
</div>

Get a MessageId from the header of an incoming AMQP message.
It is then logged in every log as a property.