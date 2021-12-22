CREATE (flight1:Flight {id: "4364dece-81a3-4da1-8f32-00efb345d752", fromDate: datetime('2021-11-12T00:00:00'), toDate: datetime('2021-11-12T00:00:00')})
CREATE (flight2:Flight {id: "d5639198-17b1-42cc-abc4-08c454650061", fromDate: datetime('2021-11-12T00:00:00'), toDate: datetime('2021-11-12T00:00:00')})



CREATE (airplane1:Airplane {id: "83d49015-e93a-48a8-84de-479c63b2a12c", seats: 120, name: 'Boeing737'})
CREATE (airplane2:Airplane {id: "bc55a62d-09ee-4c01-a79f-4170c0ace346", seats: 220, name: 'Boeing747'})



CREATE (airline1:Airline {id: "7218ba3e-50d7-4932-8138-10a14d3a31cb", name: "Norwegian"})
CREATE (airline2:Airline {id: "1282476c-07d1-408e-a5d2-892824692e0b", name: "SAS"})



CREATE (s1:Seat {id: 1, class: 'First'})
CREATE (s2:Seat {id: 2, class: 'First'})
CREATE (s3:Seat {id: 3, class: 'First'})
CREATE (s4:Seat {id: 4, class: 'First'})
CREATE (s5:Seat {id: 5, class: 'First'})



CREATE (airport1:Airport {id: "056747de-cba4-4f54-b9dd-8a44f61a6cd3", name: 'JFK Airport'})
CREATE (airport2:Airport {id: "74a1eaa8-cfb8-4a6f-a842-6a217e88e30d", name: 'Copenhagen Airport'})



CREATE (flight1)-[:From]->(airport1)
CREATE (flight1)-[:To]->(airport2)
CREATE (flight2)-[:From]->(airport2)
CREATE (flight2)-[:To]->(airport1)



CREATE (flight1)-[:With]->(airplane1)
CREATE (flight2)-[:With]->(airplane2)



CREATE (airplane1)-[:Has]->(s1)
CREATE (airplane1)-[:Has]->(s2)
CREATE (airplane2)-[:Has]->(s3)
CREATE (airplane2)-[:Has]->(s4)
CREATE (airplane2)-[:Has]->(s5)



CREATE (airline1)-[:Owns {id: "bf5f2ac4-186b-4c0a-91b4-3e7dd53280b6"}]->(airplane1)
CREATE (airline1)-[:Owns {id: "2e90b2fe-3a4e-4b1c-b3c0-321c95b4312d"}]->(airplane2)
CREATE (airline2)-[:Owns {id: "7e61b511-8e2f-4713-9f4b-9afeb3ba1bc3"}]->(airplane1)
CREATE (airline2)-[:Owns {id: "49f63d58-ce8b-40f5-b6be-c980b663bada"}]->(airplane2)



CREATE (room1:Room {id: "6e973629-2ebf-410d-b7a2-32be29797673", people: 2, floor: 2, dayPrice: 500})
CREATE (room2:Room {id: "fa687723-4901-4219-9d6f-dc5fec6caf68", people: 2, floor: 3, dayPrice: 1000})
CREATE (room3:Room {id: "376ffe6e-4e26-4f7c-9941-4dfd14be228a", people: 3, floor: 4, dayPrice: 2000})



CREATE (hotel1:Hotel {id: "ddcb03aa-eb58-4d34-8d95-092003e55384", stars: 5})
CREATE (hotel2:Hotel {id: "47a44df2-3d79-4baa-8602-347cca8a5a3a", stars: 2})
CREATE (hotel3:Hotel {id: "5b233f2b-c8db-4417-a7f7-189fbdc37604", stars: 4})



CREATE (hotel1)-[:Has]->(room1)
CREATE (hotel1)-[:Has]->(room2)
CREATE (hotel1)-[:Has]->(room3)
CREATE (hotel2)-[:Has]->(room1)
CREATE (hotel2)-[:Has]->(room2)
CREATE (hotel2)-[:Has]->(room3)
CREATE (hotel3)-[:Has]->(room1)
CREATE (hotel3)-[:Has]->(room2)
CREATE (hotel3)-[:Has]->(room3)



CREATE (carCompany1:CarCompany {id: "e25d0d35-86f9-4509-8458-8436507dba7d", name: "Audi"})
CREATE (carCompany2:CarCompany {id: "3504af3c-f20d-4761-b950-ae0af3055fdf", name: "BMW"})



CREATE (car1:Car {id: "003e1f8f-ad9c-4f30-b80c-d591428313a8", name: "R8", seats: 2, color: "Black", year: 2009})
CREATE (car2:Car {id: "6939a76f-bc64-4082-bd18-9c038c6209a0", name: "M3", seats: 5, color: "Black", year: 2007})



CREATE (rentingCompany1:RentingCompany {id: "a7b52f6e-54d3-46b7-994c-bd2936cbf87d", name: "Hertz"})
CREATE (rentingCompany2:RentingCompany {id: "fc718eff-5f78-4c9d-adbe-088419f4f271", name: "Europcar"})



CREATE (carCompany1)-[:Made]->(car1)
CREATE (carCompany2)-[:Made]->(car2)



CREATE (rentingCompany1)-[:Owns {dayPrice: 10000}]->(car1)
CREATE (rentingCompany1)-[:Owns {dayPrice: 10000}]->(car2)
CREATE (rentingCompany2)-[:Owns {dayPrice: 10000}]->(car1)
CREATE (rentingCompany2)-[:Owns {dayPrice: 10000}]->(car2)