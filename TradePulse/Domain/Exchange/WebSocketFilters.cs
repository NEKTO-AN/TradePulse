using Newtonsoft.Json;

namespace Domain.Exchange
{
    public class WebSocketFilters
	{
		[JsonProperty("op")]
		public string Op { get; set; }

		[JsonProperty("args")]
		public string[] Args { get; set; }

		public WebSocketFilters(string op, params string[] args)
		{
			Op = op;
			Args = args;
        }

        public override string ToString() => JsonConvert.SerializeObject(this, Formatting.None);
    }
}

