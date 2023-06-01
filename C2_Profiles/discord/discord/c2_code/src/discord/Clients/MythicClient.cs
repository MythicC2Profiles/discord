using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace C2Send.Clients
{
    public class MythicClient
    {
        private HttpClient mythicClient = new HttpClient();

        public MythicClient()
        {
            mythicClient.DefaultRequestHeaders.Add("mythic", "discord");
            ServicePointManager.DefaultConnectionLimit = 10;
        }

        public async Task<string> SendToMythic(string data)
        {
#if DEBUG
            string url = "http://192.168.4.201:17443/api/v1.4/agent_message";
#else
            string url = Environment.GetEnvironmentVariable("MYTHIC_ADDRESS");
#endif

            try //POST  Message
            {
                HttpContent postBody = new StringContent(data);
                var response = await mythicClient.PostAsync(url, postBody);
                return await response.Content.ReadAsStringAsync() ?? "";
            }
            catch (WebException ex)
            {
                Console.WriteLine($"[SendToMythic] WebException: {ex.Message} - {ex.Status}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"[SendToMythic] Exception: {e.Message}");
                Console.WriteLine(e.InnerException);
                Console.WriteLine(e.StackTrace);
            };

            return "";
        }
    }
}