from mythic_container.C2ProfileBase import *
from pathlib import Path
import os

class Discord(C2Profile):
    name = "discord"
    description = "discord"
    author = "@tr41nwr3ck & @checkymander"
    is_p2p = False
    is_server_routed = False
    server_binary_path = Path(os.path.join(".", "discord", "c2_code","server"))
    server_folder_path = Path(os.path.join(".", "discord", "c2_code"))
    parameters = [
        C2ProfileParameter(
            name="discord_token",
            description="A Bot Token for sending messages",
            default_value="",
            #verifier_regex="",
            required=True,
        ),
        C2ProfileParameter(
            name="bot_channel",
            description="The channel ID for the messages",
            default_value="",
            required=True,
        ),
        C2ProfileParameter(
            name="message_checks",
            description="The number of times to attempt to send a message or check for a response from the server before assuming a failure",
            default_value="10",
            required=False,
        ),
        C2ProfileParameter(
            name="time_between_checks",
            description="The amount of time the agent should wait between checks in seconds",
            default_value="10",
            required=False,
        ),
        C2ProfileParameter(
            name="callback_interval",
            description="Callback Interval in seconds",
            default_value="60",
            verifier_regex="^[0-9]+$",
            required=False,
        ),
        C2ProfileParameter(
            name="callback_jitter",
            description="Callback Jitter in percent",
            default_value="10",
            verifier_regex="^[0-9]+$",
            required=False,
        ),
        C2ProfileParameter(
            name="encrypted_exchange_check",
            description="Perform Key Exchange",
            choices=["T", "F"],
            parameter_type=ParameterType.ChooseOne,
            required=False,
        ),
        C2ProfileParameter(
            name="AESPSK",
            description="Crypto type",
            default_value="aes256_hmac",
            parameter_type=ParameterType.ChooseOne,
            choices=["aes256_hmac", "none"],
            required=False,
            crypto_type=True
        ),
        C2ProfileParameter(
            name="user_agent",
            description="User Agent",
            default_value="Mozilla/5.0 (Windows NT 6.3; Trident/7.0; rv:11.0) like Gecko",
            required=False,
        ),
        C2ProfileParameter(
            name="proxy_host",
            description="Proxy Host",
            default_value="",
            required=False,
            verifier_regex="^$|^(http|https):\/\/[a-zA-Z0-9]+",
        ),
        C2ProfileParameter(
            name="proxy_port",
            description="Proxy Port",
            default_value="",
            verifier_regex="^$|^[0-9]+$",
            required=False,
        ),
        C2ProfileParameter(
            name="proxy_user",
            description="Proxy Username",
            default_value="",
            required=False,
        ),
        C2ProfileParameter(
            name="proxy_pass",
            description="Proxy Password",
            default_value="",
            required=False,
        ),
    ]
