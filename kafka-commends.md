# ליצור טופיק חדש
docker exec 123completed-kafka-1 /usr/bin/kafka-topics --create --topic events --bootstrap-server localhost:9092 --partitions 1 --replication-factor 1

# למחוק את הטופיק
docker exec 123completed-kafka-1 /usr/bin/kafka-topics --delete --topic events --bootstrap-server localhost:9092

# ליצור קאפקה פרודוסר
docker exec -it 123completed-kafka-1 kafka-console-producer --topic events --bootstrap-server localhost:9092

# להאזין עם קאפקה קונסומר
docker exec -it 123completed-kafka-1 kafka-console-consumer --topic events --bootstrap-server localhost:9092 --from-beginning

# לראות את רשימת הטופיקים
docker exec 123completed-kafka-1 /usr/bin/kafka-topics --list --bootstrap-server localhost:9092