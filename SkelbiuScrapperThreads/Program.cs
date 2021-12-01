using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Web.Script.Serialization;
using System.Diagnostics;

namespace SkelbiuScrapperThreads
{
    class Program
    {
        static BlockingCollection<string> nuorodos = new BlockingCollection<string>();

        static string link = "https://www.skelbiu.lt/skelbimai/?autocompleted=1&keywords=&cost_min=1000&cost_max=&type=0&condition=&cities=0&distance=0&mainCity=0&search=1&category_id=79&user_type=0&ad_since_min=0&ad_since_max=0&visited_page=1&orderBy=-1&detailsSearch=0";

        static List<Skelbimas> skelbimai = new List<Skelbimas>();

        static Thread[] consumerThreads = new Thread[4];

        static Stopwatch watch;
        static void Main(string[] args)
        {
            watch = Stopwatch.StartNew();

            Thread tid1 = new Thread(new ThreadStart(ProducerGetLinks));
            tid1.Start();

            for (int i = 0; i < 4; i++)
            {
                consumerThreads[i] = new Thread(() => ConsumerGautiSkelbimus());

                consumerThreads[i].Start();
            }

        }




        private static void ConsumerGautiSkelbimus()
        {
            HtmlWeb web = new HtmlWeb();
            HtmlNode price;
            HtmlNode name;
            HtmlNodeCollection number;
            string numberValue;
            HtmlDocument document;
            string nuoroda;
            Skelbimas skelbimas;
            while (true)
            {
                nuoroda = nuorodos.Take();
                if (nuoroda == null)
                {
                    break;
                }
                document = web.Load(nuoroda);
                price = document.DocumentNode.SelectNodes("//p[@class='price']").FirstOrDefault();
                name = document.DocumentNode.SelectNodes("//h1[@itemprop='name']").FirstOrDefault();
                number = document.DocumentNode.SelectNodes("//div[@class='primary']");
                numberValue = number != null ? number.FirstOrDefault().InnerHtml : "";
                if (numberValue != "")
                {
                    skelbimas = new Skelbimas()
                    {
                        phone = System.Text.RegularExpressions.Regex.Replace(WebUtility.HtmlDecode(numberValue).Replace("\n", "").Replace("\r", ""), @"\s+", " "),
                        price = WebUtility.HtmlDecode(price.InnerHtml),
                        name = System.Text.RegularExpressions.Regex.Replace(WebUtility.HtmlDecode(name.InnerHtml).Replace("\n", "").Replace("\r", ""), @"\s+", " ")
                    };
                    skelbimai.Add(skelbimas);
                    Console.WriteLine($"Pavadinimas: { skelbimas.name  }");
                    Console.WriteLine($"Kaina: {skelbimas.price}");
                    Console.WriteLine($"Telefono numeris: { skelbimas.phone}");
                    Console.WriteLine();
                }
            }
        }

        private static void ProducerGetLinks()

        {
            HtmlWeb web = new HtmlWeb();
            HtmlNodeCollection nextPage;
            HtmlDocument document;
            HtmlNodeCollection items;
            while (link != "")
            {
                document = web.Load(link);
                items = document.DocumentNode.SelectNodes("//a[@class=' js-cfuser-link']");
                foreach (HtmlNode item in items)
                {
                    nuorodos.Add(string.Concat("https://www.skelbiu.lt", item.GetAttributeValue("href", "")));
                }
                nextPage = document.DocumentNode.SelectNodes("//a[@class='pagination_link' and @rel='next']");
                link = nextPage != null ? string.Concat("https://www.skelbiu.lt", nextPage.FirstOrDefault().GetAttributeValue("href", "").Replace("amp;", "")) : "";
            }
            while (nuorodos.Count!=0)
            {
               ;
            }

            Thread.Sleep(1000);
            PrintData();
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine(elapsedMs.ToString());
            Console.ReadLine();

        }

        private static void PrintData()
        {
            string output = new JavaScriptSerializer().Serialize(skelbimai);
            File.WriteAllText(@"output.json", output);
        }
    }
}