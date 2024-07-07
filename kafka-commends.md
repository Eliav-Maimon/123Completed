# ליצור טופיק חדש
docker exec kafkaproducer-kafka-1 /usr/bin/kafka-topics --create --topic my_topic --bootstrap-server localhost:9092 --partitions 1 --replication-factor 1

# למחוק את הטופיק
docker exec kafkaproducer-kafka-1 /usr/bin/kafka-topics --delete --topic my_topic --bootstrap-server localhost:9092

# ליצור קאפקה פרודוסר
docker exec -it kafkaproducer-kafka-1 kafka-console-producer --topic my-topic --bootstrap-server localhost:9092

# להאזין עם קאפקה קונסומר
docker exec -it kafkaproducer-kafka-1 kafka-console-consumer --topic my-topic --bootstrap-server localhost:9092 --from-beginning

# לראות את רשימת הטופיקים
<!-- docker exec kafkaproducer-kafka-1 kafka-topics.sh --list --bootstrap-server localhost:9092 -->
docker exec kafkaproducer-kafka-1 /usr/bin/kafka-topics --list --bootstrap-server localhost:9092
