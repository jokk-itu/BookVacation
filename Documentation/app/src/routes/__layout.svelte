<script>
	import '../app.css';
</script>

<div class="flex flex-col">
	<div class="center flex-grow-1" style="align-self:center;">
		<h1>BookVacation</h1>
	</div>
	<div class="flex-grow-1 flex flew-row">
		<div class="flex flex-col px-10 bg-gray-400">
			<a href="/">Introduction</a>
			<a href="/vacationservice">VacationService</a>
			<a href="/flightservice">FlightService</a>
			<a href="/hotelservice">HotelService</a>
			<a href="/carservice">CarService</a>
			<a href="/ticketservice">TicketService</a>
			<a href="/trackingservice">TrackingService</a>
			<a href="/terraform">Terraform</a>
			<a href="/grafana">Grafana</a>
			<a href="/ci">Continous Integration</a>
			<a href="/cd">Continous Deployment</a>
		</div>
		<div>
			<slot />
		</div>
	</div>
</div>

<!-- Active links with underbar -->
<!-- Links can expand, show children on its right side (small indentation) -->
