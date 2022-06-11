<script>
	import '../app.css';
</script>

<div class="flex flex-col">
	<div class="center flex-grow-1 pb-4" style="align-self:center;">
		<h1 class="text-5xl">BookVacation</h1>
	</div>
	<hr />
	<div class="flex-grow-1 flex flew-row divide-x-2">
		<div class="flex flex-col px-10 pt-4">
			<a href="/">Introduction</a>
			<!-- Services -->
			<a href="/services">Services</a>
			<a href="/services/vacation">VacationService</a>
			<a href="/services/flight">FlightService</a>
			<a href="/services/hotel">HotelService</a>
			<a href="/services/car">CarService</a>
			<a href="/services/ticket">TicketService</a>
			<a href="/services/tracking">TrackingService</a>
			<!-- DevOps -->
			<a href="/devops">DevOps</a>
			<a href="/devops/terraform">Terraform</a>
			<a href="/devops/grafana">Grafana</a>
			<a href="/devops/ci">Continous Integration</a>
			<a href="/devops/cd">Continous Deployment</a>
			<!-- Test -->
			<a href="/test">Test</a>
			<a href="/test/integration">Integration TestConsole</a>
			<a href="/test/load">Load TestConsole</a>
			<!-- Packages -->
			<a href="/packages">Packages</a>
			<a href="/packages/documentclient">DocumentClient</a>
			<a href="/packages/healthcheck">HealthCheck</a>
			<a href="/packages/logging">Logging</a>
			<a href="/packages/eventdispatcher">EventDispatcher</a>
			<a href="/packages/mediator">Mediator</a>
		</div>

		<div class="pl-4 pt-4">
			<slot />
		</div>
	</div>
</div>
