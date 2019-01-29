using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace stackoverazure
{
    public static class ProcessQuestions
    {
        [FunctionName("ProcessQuestions")]
        [Singleton]
        public static async Task RunQueue(
            [QueueTrigger("questions", Connection = "AzureWebJobsStorage")]StackCard question, 
            [DocumentDB("questionDatabase", "questions", ConnectionStringSetting = "stackoverazure_documentdb", Id = "{Id}", CreateIfNotExists = true)] dynamic inDocument, 
            [DocumentDB("questionDatabase", "questions", ConnectionStringSetting = "stackoverazure_documentdb")] IAsyncCollector<dynamic> outDocuments ,
            TraceWriter log)
        {

            if (question != null)
            {
                if (inDocument == null)
                {
                    await outDocuments.AddAsync(question);
                    await SendToTeams(question);
                }
            }
        }

        private static async Task<string> SendToTeams(StackCard question)
        {
            var webhook = Util.GetSetting("TeamWebhookUri");
            string jsonCard = JsonConvert.SerializeObject(question.Card, Formatting.Indented);

            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Post, webhook);

                request.Content = new StringContent(jsonCard, Encoding.UTF8, "application/json");

                var result = await client.SendAsync(request);

                return await result.Content.ReadAsStringAsync();
            }
        }
    }
}