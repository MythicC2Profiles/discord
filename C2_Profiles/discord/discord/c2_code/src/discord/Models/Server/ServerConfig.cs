using System.Reflection;

namespace C2Send.Models.Server
{
    public class ServerConfig
    {
        public string BotToken { get; set; }
        public string ChannelID { get; set; }
        public bool IsAnyNullOrEmpty()
        {
            foreach (PropertyInfo pi in this.GetType().GetProperties())
            {
                if (pi.PropertyType == typeof(string))
                {
                    string value = (string)pi.GetValue(this);
                    if (string.IsNullOrEmpty(value))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
