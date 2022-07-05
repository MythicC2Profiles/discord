FROM itsafeaturemythic/python38_sanic_c2profile:0.0.5

RUN apt update && apt install wget nuget -y
RUN wget https://packages.microsoft.com/config/debian/10/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
RUN dpkg -i packages-microsoft-prod.deb
RUN rm packages-microsoft-prod.deb

RUN  apt-get update; \
  apt-get install -y apt-transport-https && \
  apt-get update && \
  apt-get install -y dotnet-sdk-6.0

COPY ["requirements.txt", "/requirements.txt"]
RUN pip install -r /requirements.txt

ENTRYPOINT ["/Mythic/mythic/c2_service.sh"]