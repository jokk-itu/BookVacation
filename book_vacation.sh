#! /bin/sh

curl -X 'POST' \
  'http://localhost:5000/api/v1/Vacation' \
  -H 'accept: */*' \
  -H 'Content-Type: application/json' \
  -d '{
  "flightId": "4364dece-81a3-4da1-8f32-00efb345d752",
  "seatId": 1,
  "hotelId": "ddcb03aa-eb58-4d34-8d95-092003e55384",
  "rentHotelDays": 5,
  "roomId": "6e973629-2ebf-410d-b7a2-32be29797673",
  "carId": "003e1f8f-ad9c-4f30-b80c-d591428313a8",
  "rentingCompanyId": "a7b52f6e-54d3-46b7-994c-bd2936cbf87d",
  "rentCarDays": 5
}'
