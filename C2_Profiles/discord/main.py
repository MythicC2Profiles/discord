import mythic_container
import subprocess
from discord.mythic.c2_functions.discord import *

p = subprocess.Popen(["dotnet", "publish", "-c", "Release", "-o", "/Mythic/discord/c2_code/"], cwd="/Mythic/discord/c2_code/src/discord")
p.wait()

mythic_container.mythic_service.start_and_run_forever()
