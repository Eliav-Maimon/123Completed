# Create the images without the cache
docker build --no-cache -t kafka-producer:latest .

# Create Image
docker build -t kafka-producer .

# Running container
docker run -d --name kafka-producer kafka-producer

# See the running containers
docker ps

# Run kafka and zookeeper
docker-compose -f Kafka.yaml up -d

# Stop kafka and zookeeper
docker-compose -f Kafka.yaml stop

# Stop kafka and zookeeper and delete the containers
docker-compose -f Kafka.yaml down

# Create new topic
docker-compose -f Kafka.yaml down
docker exec 123completed-kafka-1 /usr/bin/kafka-topics --create --topic events --bootstrap-server localhost:9092 --partitions 1 --replication-factor 1

# Delete topic from kafka
docker exec 123completed-kafka-1 /usr/bin/kafka-topics --delete --topic events --bootstrap-server localhost:9092

# See the topic list
docker exec 123completed-kafka-1 /usr/bin/kafka-topics --list --bootstrap-server localhost:9092


# Start kafka producer
docker start kafka-producer

# Stop kafka producer
docker stop kafka-producer

# Read kafka consumer
docker exec -it 123completed-kafka-1 kafka-console-consumer --topic events --bootstrap-server localhost:9092 --from-beginning


# Start and Stop mongo server
run as administrator the CMD
net start MongoDB
net stop MongoDB


# Start mongo container
docker start mongodb

# Stop mongo container
docker stop mongodb


# Create 123Completed with events collection
mongosh
mongosh --username admin --password password --authenticationDatabase admin
use 123Completed
db.createCollection("events")
exit


# Start kafka consumer
docker start kafka-consumer

# Stop kafka consumer
docker stop kafka-consumer


# Start redis container
docker start redis-server

# Stop redis container
docker stop redis-server


# Get into the CLI of redis
docker exec -it redis-server redis-cli

# See all keys in redis
Keys *

# See value of certin key in redis
GET

# Delete all keys in redis
FLUSHALL

# Exit from the CLI of redis
exit


# Start ETL container
docker start mongoToRedis

# Stop ETL container
docker stop mongoToRedis


# See the logs
docker logs -f kafka-producer
docker logs -f kafka-consumer
docker logs -f mongodb
docker logs -f mongo-to-redis
docker logs -f 123completed-kafka-1