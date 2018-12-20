FROM microsoft/dotnet:2.2-runtime

WORKDIR /app
COPY ./bin/Release/netcoreapp2.1/linux/ ./
RUN chmod +x ./EvG

CMD ["./EvG", "--urls=http://.+:8080"]