mongosh
mongosh --username <username> --password <password> --authenticationDatabase <authDB>
mongosh --username admin --password password --authenticationDatabase admin

show dbs

use 123Completed

show collections

db.createCollection("events")

db.events.find({})

db.events.findOne({ ReporterId: { $gte: 10 } })

$gte, $lte, $gt, $lt

db.events.insertOne({title: "just a title"})

db.events.insertMany([{title: "eliav maimon", age: 23}, {title: "Moshe", age: 65}])

db.events.updateOne({title: "Moshe"}, { $set: {title: "David Avinu", age: 43} })

db.events.deleteOne({title: "just a title"})

db.events.drop()

db.dropDatabase()

exit