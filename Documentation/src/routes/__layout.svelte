<script>
    import "../app.css";
    import mermaid from "mermaid";

    mermaid.initialize({startOnLoad: true});
</script>

<div class="flex-grow-1 flex flew-row divide-x-2">
    <div class="flex flex-col px-10 pt-4">
        <h1 class="text-3xl">BookVacation</h1>
        <hr class="pb-4"/>
        <ul class="list-disc">
            <li><a class="ref-link text-lg" href="/">Introduction</a></li>
            <li>
                <a class="ref-link text-lg" href="/services">Services</a>
                <ul class="pl-4 list-disc">
                    <li>
                        <a class="ref-link" href="/services/vacation">VacationService</a>
                    </li>
                    <li><a class="ref-link" href="/services/flight">FlightService</a></li>
                    <li><a class="ref-link" href="/services/hotel">HotelService</a></li>
                    <li><a class="ref-link" href="/services/car">CarService</a></li>
                    <li><a class="ref-link" href="/services/ticket">TicketService</a></li>
                    <li>
                        <a class="ref-link" href="/services/tracking">TrackingService</a>
                    </li>
                </ul>
            </li>
            <li>
                <a class="ref-link text-lg" href="/devops">DevOps</a>
                <ul class="pl-4 list-disc">
                    <li><a class="ref-link" href="/devops/terraform">Terraform</a></li>
                    <li><a class="ref-link" href="/devops/grafana">Grafana</a></li>
                    <li>
                        <a class="ref-link" href="/devops/ci">Continous Integration</a>
                    </li>
                    <li>
                        <a class="ref-link" href="/devops/cd">Continous Deployment</a>
                    </li>
                </ul>
            </li>
            <li>
                <a class="ref-link text-lg" href="/test">Test</a>
                <ul class="pl-4 list-disc">
                    <li><a class="ref-link" href="/test/integration">Integration</a></li>
                    <li><a class="ref-link" href="/test/load">Load</a></li>
                </ul>
            </li>
            <li>
                <a class="ref-link text-lg" href="/packages">Packages</a>
                <ul class="pl-4 list-disc">
                    <li>
                        <a class="ref-link" href="/packages/documentclient"
                        >DocumentClient</a
                        >
                    </li>
                    <li>
                        <a class="ref-link" href="/packages/healthcheck">HealthCheck</a>
                    </li>
                    <li><a class="ref-link" href="/packages/logging">Logging</a></li>
                    <li>
                        <a class="ref-link" href="/packages/eventdispatcher"
                        >EventDispatcher</a
                        >
                    </li>
                    <li><a class="ref-link" href="/packages/mediator">Mediator</a></li>
                    <li><a class="ref-link" href="/packages/meta">Meta</a></li>
                    <li>
                        <a class="ref-link" href="/packages/blobstorage">BlobStorage</a>
                    </li>
                </ul>
            </li>
        </ul>
    </div>

    <div class="px-4 pt-4">
        <slot/>
    </div>
</div>
