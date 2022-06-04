using CarService.Contracts.RentalDeal;
using FlightService.Contracts.FlightReservation;
using HotelService.Contracts.HotelRoomReservationActivity;
using MassTransit;
using MassTransit.Courier.Contracts;
using MediatR;
using TicketService.Contracts.CreateVacationTickets;

namespace VacationService.Infrastructure.Requests.CreateVacation;

public class CreateVacationRequestHandler : IRequestHandler<CreateVacationRequest>
{
    private readonly IBusControl _busControl;

    public CreateVacationRequestHandler(IBusControl busControl)
    {
        _busControl = busControl;
    }
    
    public async Task<Unit> Handle(CreateVacationRequest request, CancellationToken cancellationToken)
    {
        var builder = new RoutingSlipBuilder(NewId.NextGuid());

        builder.AddActivity("BookFlight",
            new Uri("exchange:flight-reservation_execute"),
            new FlightReservationArgument
            {
                FlightId = request.FlightId,
                SeatId = request.FlightSeatId
            });

        builder.AddActivity("BookHotel",
            new Uri("exchange:hotel-room-reservation_execute"),
            new HotelRoomReservationArgument
            {
                HotelId = request.HotelId,
                From = request.HotelFrom,
                To = request.HotelTo,
                RoomId = request.HotelRoomId
            });

        builder.AddActivity("RentCar",
            new Uri("exchange:rental-deal_execute"),
            new RentalDealArgument
            {
                RentalCarId = request.RentalCarId,
                RentFrom = request.RentalCarFrom,
                RentTo = request.RentalCarTo
            });

        builder.AddActivity("CreateVacationTicket",
            new Uri("exchange:create-vacation-ticket_execute"),
            new CreateVacationTicketArgument
            {
                FlightId = request.FlightId,
                HotelId = request.HotelId,
                RoomId = request.HotelRoomId,
                CarId = request.RentalCarId,
                RentingCompanyName = request.RentingCompanyName
            });

        builder.AddSubscription(
            new Uri("exchange:routing-slip-event"),
            RoutingSlipEvents.All, RoutingSlipEventContents.None);

        var routingSlip = builder.Build();

        await _busControl.Execute(routingSlip);
        
        return Unit.Value;
    }
}