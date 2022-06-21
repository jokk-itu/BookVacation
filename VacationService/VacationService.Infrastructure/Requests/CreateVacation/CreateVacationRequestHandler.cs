using CarService.Contracts.RentalDeal;
using FlightService.Contracts.FlightReservation;
using HotelService.Contracts.HotelRoomReservationActivity;
using MassTransit;
using MassTransit.Courier.Contracts;
using Mediator;
using MediatR;
using TicketService.Contracts.CreateVacationTickets;

namespace VacationService.Infrastructure.Requests.CreateVacation;

public class CreateVacationRequestHandler : ICommandHandler<CreateVacationCommand, Unit>
{
    private readonly IBusControl _busControl;

    public CreateVacationRequestHandler(IBusControl busControl)
    {
        _busControl = busControl;
    }

    public async Task<Mediator.Response<Unit>> Handle(CreateVacationCommand command,
        CancellationToken cancellationToken)
    {
        var builder = new RoutingSlipBuilder(NewId.NextGuid());

        builder.AddActivity("BookFlight",
            new Uri("exchange:flight-reservation_execute"),
            new FlightReservationArgument
            {
                FlightId = command.FlightId,
                SeatId = command.FlightSeatId
            });

        builder.AddActivity("BookHotel",
            new Uri("exchange:hotel-room-reservation_execute"),
            new HotelRoomReservationArgument
            {
                HotelId = command.HotelId,
                From = command.HotelFrom,
                To = command.HotelTo,
                RoomId = command.HotelRoomId
            });

        builder.AddActivity("RentCar",
            new Uri("exchange:rental-deal_execute"),
            new RentalDealArgument
            {
                RentalCarId = command.RentalCarId,
                RentFrom = command.RentalCarFrom,
                RentTo = command.RentalCarTo
            });

        builder.AddActivity("CreateVacationTicket",
            new Uri("exchange:create-vacation-ticket_execute"),
            new CreateVacationTicketArgument
            {
                FlightId = command.FlightId,
                HotelId = command.HotelId,
                RoomId = command.HotelRoomId,
                CarId = command.RentalCarId,
                RentingCompanyName = command.RentingCompanyName
            });

        builder.AddSubscription(
            new Uri("exchange:routing-slip-event"),
            RoutingSlipEvents.All, RoutingSlipEventContents.Itinerary);

        var routingSlip = builder.Build();

        await _busControl.Execute(routingSlip);

        return new Mediator.Response<Unit>();
    }
}