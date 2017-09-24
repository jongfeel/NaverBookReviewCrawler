using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NaverBookReviewCrawler
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // 네이버 북 네티즌 리뷰
            // 1. 블로그 url
            // 2. 게시 날짜
            // 3. 글자 수
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Result_Author.Text = "";
            Result_Review.Text = "";

            bool IsContinue = true;
            int page = 1;
            int.TryParse(Page.Text, out page);
            int pagebase = page;

            string authors = "";
            string contents = "";
            int authorCount = 1;
            int contentsCount = 1;

            while (IsContinue)
            {
                WebClient webClient = new WebClient();
                webClient.Encoding = Encoding.UTF8;
                string html = webClient.DownloadString(URL.Text + "&&page=" + page);
                page++;

                HtmlDocument htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(html);

                var htmlNodeCol = htmlDocument.DocumentNode.SelectNodes("//dd");

                bool IsAuthorExist = false;
                bool IsReviewTextExist = false;
                foreach (var item in htmlNodeCol)
                {
                    if (item.Attributes["class"] != null && item.Attributes["class"].Value == "txt_inline")
                    {
                        bool dateTimeParseResult = DateTime.TryParse(item.InnerText, out DateTime result);
                        if (dateTimeParseResult)
                        {
                            authors += result.ToShortDateString() + ", ";
                        }
                    }
                    if (item.Id.Contains("review_author"))
                    {
                        string author = item.InnerText.Replace("\t", "").Replace("\r", "").Replace("\n", "");
                        author = author.Insert(author.IndexOf("작성자명"), ", ");
                        int index = author.IndexOf("블로그명");
                        if (index != -1)
                        {
                            author = author.Insert(index, ", ");
                        }                        
                        authors += author + "\r\n";

                        IsAuthorExist = true;
                    }
                    
                    if (item.Id.Contains("review_text_"))
                    {
                        string replaced = item.InnerText.Replace('\n', ' ');
                        contents += "Review " + contentsCount++ + "----------------------------";
                        contents += replaced + "\r\n";

                        IsReviewTextExist = true;
                    }
                }

                if (IsAuthorExist == false && IsReviewTextExist == false)
                {
                    IsContinue = false;
                }

                if (pagebase + 10 == page)
                {
                    IsContinue = false;
                }
            }

            Result_Author.Text = "작성날짜, 블로그주소, 작성자명, 블로그명" + "\r\n";
            Result_Author.Text += authors + "\r\n\r\n\r\n";
            Result_Review.Text += contents;
        }
    }
}
