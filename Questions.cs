using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using StackExchange.StacMan;
using System.Threading.Tasks;

namespace stackoverazure
{
    public static class Questions
    {
        [FunctionName("Questions")]
        [StorageAccount("AzureWebJobsStorage")]
        public async static void Run([TimerTrigger("* */30 * * * *")]TimerInfo myTimer, [Queue("questions")] IAsyncCollector<StackCard> questionsOutput, TraceWriter log)
        {
            var questions = await GetQuestions(log);

            foreach (var question in questions.Data.Items)
            {
                var card = CreateCard(question);
                await questionsOutput.AddAsync(card);
            }
        }

        private static async Task<StacManResponse<Question>> GetQuestions(TraceWriter log)
        {
            var stackexchangekey = Util.GetSetting("StackExchangeKey");

            log.Info($"StackOverAzure function executed at: {DateTime.Now}");

            var client = new StacManClient(key: stackexchangekey, version: "2.1");
            var response = await client.Questions.GetAll("stackoverflow",
            tagged: Util.GetSetting("tags"),
            page: 1,
            pagesize: 50,
            sort: StackExchange.StacMan.Questions.AllSort.Creation,
            order: Order.Desc);

            return response;
        }

        private static StackCard CreateCard(Question q)
        {
            var sc = new StackCard(q.QuestionId.ToString());

            sc.Card.summary = $"New question posted on StackOverflow from {q.Owner.DisplayName}";
                     
            sc.Card.sections[0].startGroup = true;
            sc.Card.sections[0].title = $"**{q.Title}**";
            sc.Card.sections[0].activityImage = q.Owner.ProfileImage;
            sc.Card.sections[0].activityTitle = $"Posted by: [**{q.Owner.DisplayName}**]({q.Owner.Link})";
            sc.Card.sections[0].activitySubtitle = $"Created: {q.CreationDate.ToUniversalTime().ToShortDateString()} {q.CreationDate.ToUniversalTime().ToShortTimeString()}";

            sc.Card.sections[0].facts[0].name = "Tags";

            var tags = "";
            foreach (var t in q.Tags)
            {
                tags += $"[{t}](https://stackoverflow.com/questions/tagged/{t}), ";
            }
            tags.TrimEnd(", ".ToCharArray());

            sc.Card.sections[0].facts[0].value = tags;

            sc.Card.sections[0].facts[1].name = "Views:";
            sc.Card.sections[0].facts[1].value = q.ViewCount.ToString();

            sc.Card.sections[0].facts[2].name = "Up Vote:";
            sc.Card.sections[0].facts[2].value = q.UpVoteCount.ToString();

            sc.Card.sections[0].facts[3].name = "Down Vote:";
            sc.Card.sections[0].facts[3].value = q.DownVoteCount.ToString();

            sc.Card.sections[0].facts[4].name = "Favorited:";
            sc.Card.sections[0].facts[4].value = q.FavoriteCount.ToString();

            sc.Card.sections[0].potentialAction[0].type = "OpenUri";
            sc.Card.sections[0].potentialAction[0].name = "View on StackOverflow";
            
            sc.Card.sections[0].potentialAction[0].targets[0].os = "default";
            sc.Card.sections[0].potentialAction[0].targets[0].uri = q.Link;

            sc.Card.sections[0].potentialAction[0].targets[1].os = "iOS";
            sc.Card.sections[0].potentialAction[0].targets[1].uri = q.Link;

            sc.Card.sections[0].potentialAction[0].targets[2].os = "android";
            sc.Card.sections[0].potentialAction[0].targets[2].uri = q.Link;

            return sc;
        }
    }
}
