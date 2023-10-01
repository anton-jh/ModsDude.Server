FROM mcr.microsoft.com/dotnet/sdk:7.0

# Packages
RUN apt-get update && \
    apt-get install -y openssh-server

# Visual Studio debugging tools
WORKDIR /vsdbg
RUN curl -sSL https://aka.ms/getvsdbgsh | /bin/sh /dev/stdin -v latest -l /vsdbg

# SSH for remote debugging on 5003
RUN mkdir /var/run/sshd && \
    echo "root:debugroot" | chpasswd && \
    echo "PermitRootLogin yes" >> /etc/ssh/sshd_config

WORKDIR /app/ModsDude.Server.Api
ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=http://+:80
ENV TZ=Europe/Stockholm

ENTRYPOINT /usr/sbin/sshd && dotnet watch run --verbose --urls=http://+:80 --project=./ModsDude.Server.Api.csproj