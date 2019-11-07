/*
    Live feed of Croatian public news portals
    Copyright (C) 2019 Bruno Fištrek

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

namespace Portals
{
    public class Article
    {
        public string Link, ID, Title, Lead, Author, Time, Content;

        public string ToHtml(string portal)
        {
            if (portal == "24h")
                return _24h.ArticleHtml.Replace("@title@", Title).Replace("@lead@", Lead).Replace("@author@", Author).Replace("@time@", Time).Replace("@content@", Content).Replace("@link@", Link);
            else if (portal == "index")
                return Index.ArticleHtml;
            else if (portal == "jutarnji")
                return Jutarnji.ArticleHtml;
            else if (portal == "vecernji")
                return Vecernji.ArticleHtml;
            else if (portal == "dnevnik")
                return Dnevnik.ArticleHtml;
            else if (portal == "net")
                return Net.ArticleHtml;
            else
                return null; 
        }

        public bool IsValidArticle()
        {
            if (!Title.Equals("exception") && !Lead.Equals("exception") && !Author.Equals("exception") && !Time.Equals("exception") && !Content.Equals("exception"))
                return true;
            else
                return false;
        }

        public void ReplaceInvalidText()
        {
            if (IsValidArticle())
                return;

            if (Title.Equals("exception"))
                Title = "<i>nema naslova</i>";

            if (Lead.Equals("exception"))
                Lead = "<i>nema podnaslova</i>";

            if (Author.Equals("exception"))
                Author = "<i>nema autora</i>";

            if (Time.Equals("exception"))
                Time = "<i>nema vremena objave</i>";

            if (Content.Equals("exception"))
                Content = "<i>nema sadržaja</i>";
        }

        public bool ShouldBeDisplayed()
        {
            if (Title.ToLowerInvariant().Contains("igraj i osvoji") || Title.ToLowerInvariant().Contains("osvojite") || Title.ToLowerInvariant().Contains("kupon") || Title.ToLowerInvariant().Contains("prijavi se"))
                return false;

            if (Author.ToLowerInvariant().Contains("promo") || Author.ToLowerInvariant().Contains("sponzor") || Author.ToLowerInvariant().Contains("plaćeni") || Author.ToLowerInvariant().Contains("oglas") || Author.ToLowerInvariant().Contains("<i>nema autora</i>"))
                return false;

            if (Content.ToLowerInvariant().Contains("pravila korištenja osobnih podataka") || Content.ToLowerInvariant().Contains("pravila privatnosti") || Content.ToLowerInvariant().Contains("prijavi se"))
                return false;

            return true;
        }
    }
}