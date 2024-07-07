docker pull redis

docker run --name redis-server -d -p 6379:6379 redis

docker exec -it redis-server redis-cli

SET key value

<!-- SET mykey "Hello, Redis!" -->

GET key

<!-- GET mykey -->

FLUSHALL

EXISTS mykey 
<!-- בדיקה אם מפתח קיים -->

EXPIRE mykey 60
<!-- הגדרת זמן למפתח במספר שניות -->

Keys * 
<!-- רשימת כל המפתחות -->

exit
