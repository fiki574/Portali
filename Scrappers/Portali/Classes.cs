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

using Newtonsoft.Json;

namespace Portals
{
    public class Article
    {
        public string Link, ID, Title, Lead, Author, Time, Content;

        public override string ToString()
        {
            return $"[ID: {ID} | Title: {Title} | Author: {Author}]";
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
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
    }
}