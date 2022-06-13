<script>
	import "../app.css";
	import mermaid from "mermaid";
	mermaid.initialize({ startOnLoad: true });
</script>

<div class="flex-grow-1 flex flew-row divide-x-2">
	<div class="flex flex-col px-10 pt-4">
		<h1 class="text-3xl">BookVacation</h1>
		<hr class="pb-4" />
		<ul class="list-disc">
			<li><a href="./" class="text-lg">Introduction</a></li>
			<li>
				<a href="/services" class="text-lg">Services</a>
				<ul class="pl-4 list-disc">
					<li><a href="/services/vacation">VacationService</a></li>
					<li><a href="/services/flight">FlightService</a></li>
					<li><a href="/services/hotel">HotelService</a></li>
					<li><a href="/services/car">CarService</a></li>
					<li><a href="/services/ticket">TicketService</a></li>
					<li><a href="/services/tracking">TrackingService</a></li>
				</ul>
			</li>
			<li>
				<a href="/devops" class="text-lg">DevOps</a>
				<ul class="pl-4 list-disc">
					<li><a href="/devops/terraform">Terraform</a></li>
					<li><a href="/devops/grafana">Grafana</a></li>
					<li><a href="/devops/ci">Continous Integration</a></li>
					<li><a href="/devops/cd">Continous Deployment</a></li>
				</ul>
			</li>
			<li>
				<a href="/test" class="text-lg">Test</a>
				<ul class="pl-4 list-disc">
					<li><a href="/test/integration">Integration</a></li>
					<li><a href="/test/load">Load</a></li>
				</ul>
			</li>
			<li>
				<a href="/packages" class="text-lg">Packages</a>
				<ul class="pl-4 list-disc">
					<li><a href="/packages/documentclient">DocumentClient</a></li>
					<li><a href="/packages/healthcheck">HealthCheck</a></li>
					<li><a href="/packages/logging">Logging</a></li>
					<li><a href="/packages/eventdispatcher">EventDispatcher</a></li>
					<li><a href="/packages/mediator">Mediator</a></li>
				</ul>
			</li>
		</ul>
	</div>

	<div class="px-4 pt-4">
		<slot />
	</div>
</div>
