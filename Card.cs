using Newtonsoft.Json;

namespace stackoverazure
{
    public class StackCard
    {
        public StackCard(string id)
        {
            this.id = id;
            Card = new Card() { type = "MessageCard", context = "http://schema.org/extensions", themeColor = "f48024" };

            Card.sections = new section[1];
            Card.sections.SetValue(new section(), 0);

            Card.sections[0].facts = new fact[5];
            for (int i = 0; i < 5; i++)
            {
                Card.sections[0].facts.SetValue(new fact(), i);
            }

            Card.sections[0].potentialAction = new potentialAction[1];
            Card.sections[0].potentialAction.SetValue(new potentialAction(), 0);

            Card.sections[0].potentialAction[0].targets = new target[3];

            for (int x = 0; x < 3; x++)
            {
                Card.sections[0].potentialAction[0].targets.SetValue(new target(), x);
            }
        }

        /// <summary>
        /// The question id or qid from StacMan
        /// </summary>
        public string id { get; set; }
        public Card Card { get; set; }
    }

    public class Card
    {         
        [JsonProperty(PropertyName = "@type")]
        public string type { get; set; }

        [JsonProperty(PropertyName = "@context")]
        public string context { get; set; }

        public string summary { get; set; }

        public string themeColor { get; set; }

        public section[] sections { get; set; }
    }

    public class section
    {
        public bool startGroup { get; set; }

        public string title { get; set; }

        public string activityImage { get; set; }

        public string activityTitle { get; set; }

        public string activitySubtitle { get; set; }

        public fact[] facts { get; set; }

        public potentialAction[] potentialAction { get; set; }
    }

    public class fact
    {
        public string name { get; set; }
        public string value { get; set; }
    }

    public class potentialAction
    {
        [JsonProperty(PropertyName = "@type")]
        public string type { get; set; }

        public string name { get; set; }

        public target[] targets { get; set; }

        public action[] actions { get; set; }
    }

    public class target
    {
        public string os { get; set; }

        public string uri { get; set; }
    }

    public class action
    {

        [JsonProperty(PropertyName = "@type")]
        public string type { get; set; }

        public string name { get; set; }

        public string target { get; set; }
    }
}
