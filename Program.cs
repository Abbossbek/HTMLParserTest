using HtmlAgilityPack;

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

namespace HTMLParserTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            foreach (var item in GetLinks("https://www.olx.uz/oz/nedvizhimost/kvartiry/prodazha/novostrojki/", "https://www.olx.uz/d/oz/obyavlenie"))
            {
                Console.WriteLine(item);
            }
        }
        public static List<string> GetLinks(string url, string contains = "")
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
            HtmlNode doc = html.DocumentNode;

            var list = new List<string>();
            foreach (HtmlNode link in doc.SelectNodes("//a[@href]"))
            {
                //HtmlAttribute att = link.Attributes["href"];
                string hrefValue = link.GetAttributeValue("href", string.Empty);
                if(contains == "")
                {
                        list.Add(hrefValue);
                }else
                if (hrefValue.Contains(contains))
                {
                        list.Add(hrefValue);
                }
            }

            return list;
        }
    }
}
