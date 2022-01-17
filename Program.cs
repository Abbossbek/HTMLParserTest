using HtmlAgilityPack;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;

namespace HTMLParserTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var links = GetLinks(GetHTMLDoc("https://www.olx.uz/oz/nedvizhimost/kvartiry/prodazha/novostrojki/tashkent/"), "https://www.olx.uz/d/oz/obyavlenie");
            var url = links.FirstOrDefault(x => !x.EndsWith("promoted"));
            OlxAd ad = GetOlxAd(GetHTMLDoc(url));
            ad.Link = url;
            var images = GetLinks(GetHTMLDoc(url), "https://apollo-olx.cdnvideo.ru", true);
            if (images.Count > 0)
            {
                new HttpClient().GetAsync("https://api.telegram.org/bot{{botToken}}/sendPhoto?chat_id={{channelId}}&photo=https://apollo-olx.cdnvideo.ru/v1/files/zedzhpj35s442-UZ/image;s=1000x700&caption=test");
            }
        }

        private static OlxAd GetOlxAd(HtmlNode doc)
        {
            var ad = new OlxAd() { Tags = new() };
            var images = GetLinks(doc, "https://apollo-olx.cdnvideo.ru", true);
            if (images.Count > 0)
                ad.ImageUrl = images.First();
            foreach (HtmlNode node in doc.SelectNodes("//p[@class='css-xl6fe0-Text eu5v0x0']"))
            {
                ad.Tags.Add(node.InnerText);
            }
            ad.Title = doc.SelectSingleNode("//h1[@data-cy='ad_title']").InnerText;
            ad.Price = doc.SelectSingleNode("//div[@data-testid='ad-price-container']").SelectSingleNode("//h3[@class='css-okktvh-Text eu5v0x0']").InnerText;
            ad.Content = doc.SelectSingleNode("//div[@data-cy='ad_description']").SelectSingleNode("//div[@class='css-g5mtbi-Text']").InnerText;
            return ad;
        }

        public static List<string> GetLinks(HtmlNode doc, string contains = "", bool imageLinks = false)
        {
            var list = new List<string>();
            foreach (HtmlNode link in doc.SelectNodes(imageLinks ? "//img[@src]" : "//a[@href]"))
            {
                //HtmlAttribute att = link.Attributes["href"];
                string value = link.GetAttributeValue(imageLinks ? "src" : "href", string.Empty);
                if (contains == "")
                {
                    list.Add(value);
                }
                else
                if (value.Contains(contains))
                {
                    list.Add(value);
                }
            }

            return list;
        }

        private static HtmlNode GetHTMLDoc(string url)
        {
            StringBuilder sb = new StringBuilder();
            byte[] ResultsBuffer = new byte[8192];
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            Stream resStream = response.GetResponseStream();
            string tempString = null;
            int count = 0;
            do
            {
                count = resStream.Read(ResultsBuffer, 0, ResultsBuffer.Length);
                if (count != 0)
                {
                    tempString = Encoding.UTF8.GetString(ResultsBuffer, 0, count);
                    sb.Append(tempString);
                }
            }

            while (count > 0);
            string sbb = sb.ToString();

            HtmlDocument html = new HtmlDocument();
            html.OptionOutputAsXml = true;
            html.LoadHtml(sbb);
            return html.DocumentNode;
        }
    }
}
