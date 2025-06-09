# Santander-Test

Run the project:
1. go to solution rootfolder /TalanTest
2. docker build -t santander-test -f Api/Dockerfile .
3. docker run -d -p 8080:8080 --name hackernews-api-proxy santander-test
4. open browser and open http://localhost:8080/swagger or open terminal and run curl "http://localhost:8080/api/v1/hackernews?top=2"

Endpoints takes time to finish, because of the comments count

Possible improvements:
- Introduce API response caching / HackerNews API response caching
- Introduce rate limiting, so endpoint is able to handle few concurrent calls. In order to avoid overloading HackerNews API
- Introduce retry with backoff on HackerNews API call failure to embrace resilience
