
using System;

using System.Collections.Generic;

using System.Linq;

using System.Net;

using HtmlAgilityPack;



namespace SkelbiuScraper

{

    class Program

    {

        static List<string> nuorodos = new List<string>();

        static string link = "https://www.skelbiu.lt/skelbimai/?autocompleted=1&keywords=&cost_min=1000&cost_max=&type=0&condition=&cities=0&distance=0&mainCity=0&search=1&category_id=79&user_type=0&ad_since_min=0&ad_since_max=0&visited_page=1&orderBy=-1&detailsSearch=0";

        //static List<Skelbimas> skelbimai = new List<Skelbimas>();

        static void Main(string[] args)

        {

            var watch = System.Diagnostics.Stopwatch.StartNew();

            GetLinks();

            GautiSkelbimus();

            watch.Stop();

            var elapsedMs = watch.ElapsedMilliseconds;

            Console.WriteLine(elapsedMs.ToString());

            Console.ReadLine();

        }





        private static void GautiSkelbimus()
        {

            HtmlWeb web = new HtmlWeb();

            HtmlNode price;

            HtmlNode name;

            HtmlNodeCollection number;

            string numberValue;

            HtmlDocument document;

            foreach (string nuroda in nuorodos)

            {

                document = web.Load(nuroda);

                price = document.DocumentNode.SelectNodes("//p[@class='price']").FirstOrDefault();

                name = document.DocumentNode.SelectNodes("//h1[@itemprop='name']").FirstOrDefault();

                number = document.DocumentNode.SelectNodes("//div[@class='primary']");

                numberValue = number != null ? number.FirstOrDefault().InnerHtml : "";

                if (numberValue != "")

                {

                    Console.WriteLine($"Pavadinimas: { System.Text.RegularExpressions.Regex.Replace(WebUtility.HtmlDecode(name.InnerHtml).Replace("\n", "").Replace("\r", ""), @"\s+", " ")  }");

                    Console.WriteLine($"Kaina: {WebUtility.HtmlDecode(price.InnerHtml)}");

                    Console.WriteLine($"Telefono numeris: {System.Text.RegularExpressions.Regex.Replace(WebUtility.HtmlDecode(numberValue).Replace("\n", "").Replace("\r", ""), @"\s+", " ") }");

                    Console.WriteLine();

                }

            }

        }

        private static void GetLinks()

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

        }

    }

}