<svelte:head>
    <title>Services</title>
</svelte:head>

<div class="py-4">
    <h2 class="text-3xl" id="title">
        <a class="text-blue-400 header-anchor" href="#title">#</a>Services
    </h2>
    <hr class="border-blue-400"/>
</div>

All services are microservices and adopt a clean architecture. Each microservice is updated to the
latest dotnet version. A microservice contains an Api project, Infrastructure project, Contracts
project, Domain project and Test project.

<div class="py-4">
    <h2 class="text-xl" id="api">
        <a class="text-blue-400 header-anchor" href="#title">#</a>Api
    </h2>
    <hr class="border-blue-400"/>
</div>

It is a simple Api receiving requests by HTTP and serves as main assembly of the microservice.

<div class="py-4">
    <h2 class="text-xl" id="infrastructure">
        <a class="text-blue-400 header-anchor" href="#title">#</a>Infrastructure
    </h2>
    <hr class="border-blue-400"/>
</div>

It consists of infrastructure services, such as communication with other services eg. the Broker. It
also uses the Mediator pattern which serves as the main pattern in a microservice.

<div class="py-4">
    <h2 class="text-xl" id="contracts">
        <a class="text-blue-400 header-anchor" href="#title">#</a>Contracts
    </h2>
    <hr class="border-blue-400"/>
</div>

It consists of requests and responses which are used in the Api. It also consists of events which
are distributed through the broker.

<div class="py-4">
    <h2 class="text-xl" id="domain">
        <a class="text-blue-400 header-anchor" href="#title">#</a>Domain
    </h2>
    <hr class="border-blue-400"/>
</div>

Consists of POCO's which illustrate documents of the datamodel.

<div class="py-4">
    <h2 class="text-xl" id="tests">
        <a class="text-blue-400 header-anchor" href="#title">#</a>Tests
    </h2>
    <hr class="border-blue-400"/>
</div>

Consists only of Unit tests.
