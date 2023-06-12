# discord

A C2 profile that uses the Discord REST API for communication. 

## How to install an agent in this format within Mythic

When it's time for you to test out your install or for another user to install your c2 profile, it's pretty simple. Within Mythic you can run:

* `sudo ./mythic-cli install github https://github.com/MythicC2Profiles/discord` to install the main branch

## Configuring Proper Tokens

- Navigate to https://discord.com/developers/applications
- Click New Application, Enter a name for your bot and click Create.
- Next hit “Reset Token” and save your Token to use in Mythic
- Navigate to Settings > Oauth2 and grab your ClientID
- Replace the ClientID with yours and Navigate to the URL : https://discord.com/api/oauth2/authorize?client_id=<ClientID>&permissions=0&scope=bot
- Select Your Server from the Menu and Authorize. Your bot should now appear your Discord Server


## Getting Channel ID

- In Discord go to Settings -> Advanced -> and enable "Developer Mode"
- Go to your server and right click the channel you want your comms to happen in
- Right Click the Text Channel you wish to use "Copy ID" and the channel ID will be copied to your clipboard
  
## Configuring C2 Profile in Mythic
- Navigate to https://[ServerIP]:7443/new/payloadtypes
- Start profile > View/Edit Config 
- Enter your botToken And ChannelID
- Start profile 

## Troubleshooting
- If your bot  is offline run: sudo ./mythic-cli discord restart
