#! /bin/sh

curl -X POST https://occupancydetection.software/car/api/v1/rentalcar -H 'Content-Type: application/json'  -d '{"carModelNumber": "3fa85f64-5717-4562-b3fc-2c963f66afa6", "carCompanyName": "string", "rentingCompanyName": "string", "dayPrice": 10, "color": "string" }'
