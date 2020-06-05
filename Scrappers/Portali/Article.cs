/*
    Live feed of Croatian public news portals
    Copyright (C) 2020/2021 Bruno Fištrek

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

        public string ToHtml(PortalType type)
        {
            if (type == PortalType.H24)
                return H24.ArticleHtml.Replace("@title@", Title).Replace("@lead@", Lead).Replace("@author@", Author).Replace("@time@", Time).Replace("@content@", Content).Replace("@link@", Link);
            else if (type == PortalType.Index)
                return Index.ArticleHtml.Replace("@title@", Title).Replace("@lead@", Lead).Replace("@author@", Author).Replace("@time@", Time).Replace("@content@", Content).Replace("@link@", Link);
            else if (type == PortalType.Jutarnji)
                return Jutarnji.ArticleHtml.Replace("@title@", Title).Replace("@lead@", Lead).Replace("@author@", Author).Replace("@time@", Time).Replace("@content@", Content).Replace("@link@", Link);
            else if (type == PortalType.Vecernji)
                return Vecernji.ArticleHtml.Replace("@title@", Title).Replace("@lead@", Lead).Replace("@author@", Author).Replace("@time@", Time).Replace("@content@", Content).Replace("@link@", Link);
            else if (type == PortalType.Net)
                return Net.ArticleHtml.Replace("@title@", Title).Replace("@lead@", Lead).Replace("@author@", Author).Replace("@time@", Time).Replace("@content@", Content).Replace("@link@", Link);
            else
                return null; 
        }

        public bool IsValidArticle()
        {
            if (Title != null && !Title.Equals("exception") && 
                Lead != null && !Lead.Equals("exception") && 
                Author != null && !Author.Equals("exception") && 
                Time != null && !Time.Equals("exception") && 
                Content != null && !Content.Equals("exception"))
                return true;
            else
                return false;
        }

        public void ReplaceInvalidText()
        {
            if (IsValidArticle())
                return;

            if (Title == null || Title.Equals("exception"))
                Title = "<i>nema naslova</i>";

            if (Lead == null || Lead.Equals("exception"))
                Lead = "<i>nema podnaslova</i>";

            if (Author == null || Author.Equals("exception"))
                Author = "<i>nema autora</i>";

            if (Time == null || Time.Equals("exception"))
                Time = "<i>nema vremena objave</i>";

            if (Content == null ||Content.Equals("exception"))
                Content = "<i>nema sadržaja</i>";
        }

        public bool ShouldBeDisplayed(PortalType type)
        {
            if (type == PortalType.H24)
            {
                if (Title.ToLowerInvariant().Contains("igraj i osvoji") || Title.ToLowerInvariant().Contains("osvojite") || Title.ToLowerInvariant().Contains("kupon") || Title.ToLowerInvariant().Contains("prijavi se"))
                    return false;

                if (Author.ToLowerInvariant().Contains("promo") || Author.ToLowerInvariant().Contains("sponzor") || Author.ToLowerInvariant().Contains("plaćeni") || Author.ToLowerInvariant().Contains("oglas"))
                    return false;

                if (Content.ToLowerInvariant().Contains("pravila korištenja osobnih podataka") || Content.ToLowerInvariant().Contains("pravila privatnosti") || Content.ToLowerInvariant().Contains("prijavi se"))
                    return false;
            }
            else if (type == PortalType.Jutarnji)
            {
                if (ID.ToLowerInvariant().Contains("https:--") || ID.ToLowerInvariant().Contains("-vijesti-zagreb"))
                    return false;
            }
            else if (type == PortalType.Vecernji)
            {
                if (Author.ToLowerInvariant().Contains("pr članak"))
                    return false;
            }
            else if (type == PortalType.Net)
            {
                if (ID.ToLowerInvariant().Contains("https:"))
                    return false;
            }
            return true;
        }

        public override string ToString()
        {
            return $"[{ID} | {Link}]";
        }
    }
}