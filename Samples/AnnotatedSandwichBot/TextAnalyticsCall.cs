using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using System.Configuration;


namespace Microsoft.Bot.Sample.AnnotatedSandwichBot
{
    public class TextAnalyticsCall
    {
    }
    public class BatchInput
    {
        public List<DocumentInput> documents { get; set; }
    }
    public class DocumentInput
    {
        public double id { get; set; }
        public string text { get; set; }
    }
    public class BotMessage
    {
        public string OriginalMessage { get; set; }
        public float MessageScore { get; set; }

        public string MessageCategory { get; set; }
        public string ReturnMessage { get; set; }
    }
    // Classes to store the result from the sentiment analysis
    public class BatchResult
    {
        public List<DocumentResult> documents { get; set; }
    }
    public class DocumentResult
    {
        public double score { get; set; }
        public string id { get; set; }
    }

    public class CookieQuotes
    {
        private List<string> lstCookieQuotes;
        public CookieQuotes()
        {
            lstCookieQuotes = new List<string>();
            lstCookieQuotes.Add("He who throws dirt is losing ground.");
            lstCookieQuotes.Add("You can always find happiness at work on Friday.");
            lstCookieQuotes.Add("Let’s just say that the cookie is going to be the better part of the fortune cookie.");
            lstCookieQuotes.Add("Do not mistake temptation for opportunity.");
            lstCookieQuotes.Add("Man with hole in pocket feel cocky all day.");
            lstCookieQuotes.Add("A woman who seeks to be equal with men lacks ambition.");
            lstCookieQuotes.Add("The greatest danger could be your stupidity.");
            lstCookieQuotes.Add("The only normal people are the ones you don't know very well.");
            lstCookieQuotes.Add("Doesn't expecting the unexpected make the unexpected become the expected?");
            lstCookieQuotes.Add("Forgive your enemies...but REMEMBER THEIR NAMES!");
            lstCookieQuotes.Add("You may write in your own fortune on the line __________________________________________.");
            lstCookieQuotes.Add("He who laughs at himself never runs out of things to laugh at.");
            lstCookieQuotes.Add("He who laughs last is laughing at you.");
            lstCookieQuotes.Add("The Chinese food you just finished was the most American made thing you have bought since 1995.");
            lstCookieQuotes.Add("A closed mouth gathers no feet.");
            lstCookieQuotes.Add("A cynic is only a frustrated optimist.");
            lstCookieQuotes.Add("A fanatic is one who can't change his mind, and won't change the subject.");
            lstCookieQuotes.Add("You are feeling generous and will leave a substantial tip for the server at the Chinese restaurant.");
            lstCookieQuotes.Add("You seek to find meaning from a little slip of paper inside a little cookie. You are gullible.");
            lstCookieQuotes.Add("The world may be your oyster, but it doesn't mean you'll get its pearl.");
            lstCookieQuotes.Add("Pick another cookie");
            lstCookieQuotes.Add("You are surrounded by fortune hunters");
            lstCookieQuotes.Add("You will receive a fortune cookie.");
            lstCookieQuotes.Add("Flattery will go far tonight.");
            lstCookieQuotes.Add("Water is your friend. It will help you wash down your fortune cookie. ");
            lstCookieQuotes.Add("Your fortune cookie will taste suspiciously like an ice cream cone without the ice cream");
            lstCookieQuotes.Add("Opps… wrong cookie");
            lstCookieQuotes.Add("Never give up unless defeat arouses that girl in accounting");
            lstCookieQuotes.Add("Disregard this cookie");
            lstCookieQuotes.Add("Only listen to future cookies");
            lstCookieQuotes.Add("The road to riches is paved with homework");
            lstCookieQuotes.Add("Help!!! I am being held hostage in a Chinese bakery");
            lstCookieQuotes.Add("The fortune you seek is in another cookie");
            lstCookieQuotes.Add("Man who run behind car get exhausted.");
            lstCookieQuotes.Add("Some fortune cookies hold no fortune");
            lstCookieQuotes.Add("Marriage lets you annoy one special person for the rest of your life");
            lstCookieQuotes.Add("Today is probably a huge improvement over yesterday");
            lstCookieQuotes.Add("Never tease a midget with a high five");
            lstCookieQuotes.Add("Don’t play leap frog with a unicorn");
            lstCookieQuotes.Add("He who eats too many prunes, sits on toilet many moons. ");
            lstCookieQuotes.Add("If you think we are going to sum up your whole life on this little piece of paper your crazy");
            lstCookieQuotes.Add("You will receive a fortune (cookie)");
            lstCookieQuotes.Add("You will be hungry again in one hour");
            lstCookieQuotes.Add("Don’t eat Chinese food today");
            lstCookieQuotes.Add("That wasn’t chicken");
            lstCookieQuotes.Add("Fortune not found? Abort, Retry, Ignore ");
            lstCookieQuotes.Add("It’s about time I got out of that cookie");
            lstCookieQuotes.Add("Always keep your words soft and sweet, just in case you have to eat them. ");
            lstCookieQuotes.Add("A good time to keep your mouth shut is when you're in deep water.");
            lstCookieQuotes.Add("From the same people holding the prisoner?");
            lstCookieQuotes.Add("Always borrow money from a pessimist. He won't expect it back");
            lstCookieQuotes.Add("Your colon will self destruct in five seconds.");
            lstCookieQuotes.Add("A recent prison escapee that is sitting nearby wants to love you long time.");
            lstCookieQuotes.Add("Your dog Sparky... well, he's no longer missing");
            lstCookieQuotes.Add("Please don't eat me. ");
            lstCookieQuotes.Add("Only a fool would look to a cookie for words of wisdom.");
            lstCookieQuotes.Add("See the waiter about our new food poison life insurance policies.");
            lstCookieQuotes.Add("You will need good reading material in approximately 15 minutes.");
            lstCookieQuotes.Add("Don’t kiss an elephant on the lips today");
            lstCookieQuotes.Add("Never trouble trouble till trouble trouble’s you");
            lstCookieQuotes.Add("If you grow up to be famous you ow me $100 because i totally called it!");
            lstCookieQuotes.Add("WARNING! Do not eat your fortune.");
            lstCookieQuotes.Add("Come back later. I need a nap. Yes, fortune cookies sleep.");
            lstCookieQuotes.Add("No fortune for you.");
            lstCookieQuotes.Add("Better to Press Shirt than Press Luck ");
            lstCookieQuotes.Add("Confuscius say 'Man who fart in church sit in own pew'");
            lstCookieQuotes.Add("Confusis say, Tack in chair fun but screw in bed more fun. ");
            lstCookieQuotes.Add("Have you seen the cat?");
            lstCookieQuotes.Add("You are the crunchy noodle in the vegetarian salad of life");
            lstCookieQuotes.Add("Watch out for the brown bear and the gray donkey. One will claw you while the other will make an ass out of yourself.");
            lstCookieQuotes.Add("The beginning of our destruction started the day we began destroying ancient wisdom. ~Mike Dolan, @HawaiianLife");
            lstCookieQuotes.Add("Every man is a damn fool for at least five minutes every day; wisdom consists in not exceeding the limit. ~Elbert Hubbard");
            lstCookieQuotes.Add("We can be Knowledgeable with other men's knowledge, but we cannot be wise with other men's wisdom. ~Michel de Montaigne");
            lstCookieQuotes.Add("Wisdom begins at the end. ~Daniel Webster");
            lstCookieQuotes.Add("Wisdom is not what you know but how quickly you adjust when the opposite proves true. ~Robert Brault, rbrault.blogspot.com");
            lstCookieQuotes.Add("Patience is the companion of wisdom. ~St. Augustine");
            lstCookieQuotes.Add("Wisdom is the reward you get for a lifetime of listening when you'd have preferred to talk. ~Doug Larson");
            lstCookieQuotes.Add("Wisdom is the most beautiful ornament of the human race. ~Annæ Mariæ à Schurman, 1638  [Sapientia est generis humani ornamentum pulcherrimum. I've paraphrased this from her letter to Patri Andreas Rivetus. —tεᖇᖇ¡·g]");
            lstCookieQuotes.Add("Wisdom is knowing what to do next; virtue is doing it. ~David Star Jordan, The Philosophy of Despair");
            lstCookieQuotes.Add("He who devotes sixteen hours a day to hard study may become at sixty as wise as he thought himself at twenty. ~Mary Wilson Little");
            lstCookieQuotes.Add("Wisdom doesn't necessarily come with age. Sometimes age just shows up all by itself. ~Tom Wilson");
            lstCookieQuotes.Add("Some wise men would do well to exchange a portion of their weighty wisdom for the lighter burden of their neighbors' innocent folly. ~James Lendall Basford (1845–1915), Sparks from the Philosopher's Stone, 1882");
            lstCookieQuotes.Add("How can you be a sage if you're pretty? You can't get your wizard papers without wrinkles. ~Bill Veeck");
            lstCookieQuotes.Add("The years teach much which the days never knew. ~Ralph Waldo Emerson");
            lstCookieQuotes.Add("When I can look Life in the eyes, Grown calm and very coldly wise, Life will have given me the Truth, And taken in exchange — my youth. ~Sara Teasdale");
            lstCookieQuotes.Add("The saddest aspect of life right now is that science gathers knowledge faster than society gathers wisdom. ~Isaac Asimov");
            lstCookieQuotes.Add("Never does nature say one thing and wisdom another. ~Juvenal, Satires");
            lstCookieQuotes.Add("A wise man can see more from the bottom of a well than a fool can from a mountain top. ~Author Unknown");
            lstCookieQuotes.Add("I believe that all wisdom consists in caring immensely for a few right things, and not caring a straw about the rest. ~John Buchan");
            lstCookieQuotes.Add("It is easier to find a score of men wise enough to discover the truth than to find one intrepid enough, in the face of opposition, to stand up for it. ~A.A. Hodge");
            lstCookieQuotes.Add("A child can ask questions that a wise man cannot answer. ~Author Unknown");
            lstCookieQuotes.Add("A single conversation with a wise man during the eating of a meal, is better than ten years' mere study of books. ~Chinese Proverb  [Quoted in Justus Doolittle, A Vocabulary and Hand-Book of the Chinese Language, Romanized in the Mandarin Dialect, 'Metaphorical and Proverbial Sentences, ' 1872.");
            lstCookieQuotes.Add("Wisdom consists of the anticipation of consequences. ~Norman Cousins");
            lstCookieQuotes.Add("He dares to be a fool, and that is the first step in the direction of wisdom. ~James Gibbons Huneker");
            lstCookieQuotes.Add("For each generation must find the wisdom of the ages in the form of its own wisdom. ~Erik H. Erikson, 'Human Strength and the Cycle of Generations, ' Insight and Responsibility, 1964");
            lstCookieQuotes.Add("Wisdom comes by disillusionment. ~George Santayana");
            lstCookieQuotes.Add("Wisdom is the quality that keeps you from getting into situations where you need it. ~Doug Larson");
            lstCookieQuotes.Add("Some folks are wise and some are otherwise. ~Tobias Smollett");
            lstCookieQuotes.Add("Wisdom is digested experience... ~Janesville Daily Gazette (Janesville, Wisconsin), 1917 March 29th");
            lstCookieQuotes.Add("Wisdom is ofttimes nearer when we stoop than when we soar. ~William Wordsworth");
            lstCookieQuotes.Add("The young man knows the rules, but the old man knows the exceptions. ~Oliver Wendell Holmes Sr.");
            lstCookieQuotes.Add("Knowledge is proud that he has learn'd so much; Wisdom is humble that he knows no more. ~William Cowper, 'The Winter Walk at Noon'");
            lstCookieQuotes.Add("Knowledge comes, but wisdom lingers. ~Alfred Lord Tennyson");
            lstCookieQuotes.Add("Wisdom outweighs any wealth. ~Sophocles");
            lstCookieQuotes.Add("If wisdom and diamonds grew on the same tree we could soon tell how much men loved wisdom. ~Lemuel K. Washburn, Is The Bible Worth Reading And Other Essays, 1911");
            lstCookieQuotes.Add("The toughest test of good judgment is to know when to withhold your better judgment. ~Robert Brault, rbrault.blogspot.com");
            lstCookieQuotes.Add("Wisdom is like a baobab tree; no one individual can embrace it. ~East African Proverb");
            lstCookieQuotes.Add("Why, thou say'st well. I do now remember a saying: 'The fool doth think he is wise, but the wise man knows himself to be a fool.' ~William Shakespeare, As You Like It, [V, 1, Audrey]");
            lstCookieQuotes.Add("One must spend time in gathering knowledge to give it out richly. ~Edward C. Steadman");
            lstCookieQuotes.Add("The art of being wise is the art of knowing what to overlook. ~William James");
            lstCookieQuotes.Add("It is more easy to be wise for others than for ourselves. ~François VI de la Rochefoucault");
            lstCookieQuotes.Add("Mixing one's wines may be a mistake, but old and new wisdom mix admirably. ~Bertolt Brecht");
            lstCookieQuotes.Add("Mothers, teach your children this. Teach your children that Wisdom is everywhere. In pieces. Some of the Wisdom is in the trees, some of the Wisdom is with the animals. Some of the Wisdom is with the planets and the stars and the moons and the sun. Some of the Wisdom flows with the waters. Some of the Wisdom was with our ancestors. Some of the Wisdom is in our minds. All of the Wisdom is from the Spirit of God. ~Esther Davis-Thompson, MotherLove: Re-Inventing a Good and Blessed Future for Our Children, 'Spiritual Nourishment, ' 1999");
            lstCookieQuotes.Add("Knowledge is learning something every day. Wisdom is letting go of something every day. ~Zen Proverb");
            lstCookieQuotes.Add("We give advice, but we cannot give the wisdom to profit by it. ~François VI de la Rochefoucault");
            lstCookieQuotes.Add("Age is the price of wisdom. ~Proverb");
            lstCookieQuotes.Add("Learning sleeps and snores in libraries, but wisdom is everywhere, wide awake, on tiptoe. ~Josh Billings");
            lstCookieQuotes.Add("There are subjects in which I wish to become knowledgeable, and subjects in which I wish to remain wise. ~Robert Brault, rbrault.blogspot.com");
            lstCookieQuotes.Add("The prince who would know all, must ignore much. ~Domitius Afer, translated from Latin");
            lstCookieQuotes.Add("When the wisdom speaks, be silent. Do not waste your candle when the sun is there. ~Mehmet Murat ildan");
            lstCookieQuotes.Add("Silence is wisdom's sentinel. ~James Lendall Basford (1845–1915), Sparks from the Philosopher's Stone, 1882");
            lstCookieQuotes.Add("Every wise man lives in an observatory. ~Augustus William Hare and Julius Charles Hare, Guesses at Truth, by Two Brothers, 1827");
            lstCookieQuotes.Add("Knowledge is a process of piling up facts; wisdom lies in their simplification. ~Martin H. Fischer (1879–1962)");
            lstCookieQuotes.Add("A man begins cutting his wisdom teeth the first time he bites off more than he can chew. ~Herb Caen");
            lstCookieQuotes.Add("There is a wisdom of the head, and... a wisdom of the heart. ~Charles Dickens");
            lstCookieQuotes.Add("Common-sense in an uncommon degree is what the world calls wisdom. ~Samuel Taylor Coleridge");
            lstCookieQuotes.Add("No man was ever wise by chance. ~Seneca");
            lstCookieQuotes.Add("Life is one long experiment in learning. Who can be perfect all the time? Sometimes I feel half-wise, sometimes half-stupid. ~Terri Guillemets");
            lstCookieQuotes.Add("We are made wise not by the recollection of our past, but by the responsibility for our future. ~George Bernard Shaw");
        }
        public string getRandomQuote()
        {
            Random rnd = new Random();
            int idx = rnd.Next(0, lstCookieQuotes.Count-1);
            return lstCookieQuotes[idx];
        }
    }
    public class SessionList
    {
        private List<string> lstSessions;
        public SessionList()
        {
            lstSessions = new List<string>();
            lstSessions.Add("Introduction to Azure");
            lstSessions.Add("Manage your cloud infrastructure with Azure IaaS");
            lstSessions.Add("Store and process data with Azure Document DB");
            lstSessions.Add("Build Cloud Applications with Azure App Service");
        }
        public string getSessions()
        {
            string reVal = "";
            int cnt = 1;
            foreach (string str in lstSessions)
            {
                reVal += cnt.ToString() + ". ";
                reVal += str;
                reVal += "\r\n";
                cnt++;
            }
            return reVal;
        }
    }

    public class BotMeta
    {
        public BotMeta() { }
        public string GetSurveyMetaJSON()
        {
            string metaUri = ConfigurationManager.AppSettings["MetaFunctionURL"];

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(metaUri);
            // Set the Method property of the request to POST.
            request.Method = "GET";
            // Get the response.
            WebResponse response = request.GetResponse();
            // Display the status.
            //Console.WriteLine(((HttpWebResponse)response).StatusDescription);
            // Get the stream containing content returned by the server.
            Stream dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.
            return reader.ReadToEnd();
        }
    }
    public class SentimentAnalysis
    {
        public SentimentAnalysis()
        {

        }
        public float GetSentiment(string comment)
        {
            
            string apiKey = ConfigurationManager.AppSettings["TextApiKey"];
            string queryUri = ConfigurationManager.AppSettings["TextApiUri"];

            // Create a request using a URL that can receive a post. 
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(queryUri);
            // Set the Method property of the request to POST.
            request.Method = "POST";
            request.Accept = "application/json";
            request.Headers.Add("Ocp-Apim-Subscription-Key", apiKey);

            var sentimentInput = new BatchInput
            {
                documents = new List<DocumentInput> {
                    new DocumentInput {
                            id = 1,
                            text = comment
                        }
                    }
            };
            string postData = JsonConvert.SerializeObject(sentimentInput);

            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            // Set the ContentType property of the WebRequest.
            //request.ContentType = "application/x-www-form-urlencoded";
            // Set the ContentLength property of the WebRequest.
            request.ContentLength = byteArray.Length;
            // Get the request stream.
            Stream dataStream = request.GetRequestStream();
            // Write the data to the request stream.
            dataStream.Write(byteArray, 0, byteArray.Length);
            // Close the Stream object.
            dataStream.Close();
            // Get the response.
            WebResponse response = request.GetResponse();
            // Display the status.
            // Console.WriteLine(((HttpWebResponse)response).StatusDescription);
            // Get the stream containing content returned by the server.
            dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.
            string responseFromServer = reader.ReadToEnd();
            // Display the content.

            var sentimentJsonResponse = JsonConvert.DeserializeObject<BatchResult>(responseFromServer);
            var sentimentScore = sentimentJsonResponse?.documents?.FirstOrDefault()?.score ?? 0;

            // Clean up the streams.
            reader.Close();
            dataStream.Close();
            response.Close();
            return (float)sentimentScore;
        }
    }

}